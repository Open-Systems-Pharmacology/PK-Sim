using System.Threading;
using System.Threading.Tasks;
using PKSim.Assets;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;

namespace PKSim.Core.Services
{
   public interface ISimulationPKParametersImportTask
   {
      Task<SimulationPKParametersImport> ImportPKParameters(PopulationSimulation populationSimulation, string filefullPath, CancellationToken cancellationToken);
   }

   public class SimulationPKParametersImportTask : ISimulationPKParametersImportTask
   {
      private readonly ISimulationPKAnalysesImporter _pkAnalysesImporter;
      private readonly IEntitiesInContainerRetriever _quantityRetriever;

      public SimulationPKParametersImportTask(ISimulationPKAnalysesImporter pkAnalysesImporter, IEntitiesInContainerRetriever quantityRetriever)
      {
         _pkAnalysesImporter = pkAnalysesImporter;
         _quantityRetriever = quantityRetriever;
      }

      public async Task<SimulationPKParametersImport> ImportPKParameters(PopulationSimulation populationSimulation, string filefullPath, CancellationToken cancellationToken)
      {
         var importedPKAnalysis = await importPKAnalysesFromFile(filefullPath, cancellationToken);
         validateConsistencyWithSimulation(populationSimulation, importedPKAnalysis);
         addImportedPKToLogForSuccessfulImport(importedPKAnalysis);
         return importedPKAnalysis;
      }

      private void addImportedPKToLogForSuccessfulImport(SimulationPKParametersImport pkParameterImport)
      {
         if (pkParameterImport.Status.Is(NotificationType.Error))
            return;

         pkParameterImport.AddInfo(PKSimConstants.Information.FollowingPKParametersWereSuccessfulyImported);
         foreach (var quantityPKParameter in pkParameterImport.PKParameters)
         {
            pkParameterImport.AddInfo(quantityPKParameter.ToString());
         }
      }

      private void validateConsistencyWithSimulation(PopulationSimulation populationSimulation, SimulationPKParametersImport importedPKParameter)
      {
         var allQuantities = _quantityRetriever.OutputsFrom(populationSimulation);
         foreach (var pkParameter in importedPKParameter.PKParameters)
         {
            validateLength(populationSimulation, pkParameter, importedPKParameter);
            verifyThatQuantityExistsInSimulation(allQuantities, pkParameter, importedPKParameter);
            warnIfParameterAlreadyExists(populationSimulation.PKAnalyses, pkParameter, importedPKParameter);
         }
      }

      private void warnIfParameterAlreadyExists(PopulationSimulationPKAnalyses pkAnalyses, QuantityPKParameter pkParameter, SimulationPKParametersImport importedPKParameter)
      {
         if (pkAnalyses.IsNull())
            return;

         if (pkAnalyses.PKParameterBy(pkParameter.Id) == null)
            return;

         importedPKParameter.AddWarning(PKSimConstants.Warning.PKParameterAlreadyExistsAndWillBeOverwritten(pkParameter.Name, pkParameter.QuantityPath));

      }

      private void verifyThatQuantityExistsInSimulation(PathCache<IQuantity> allQuantities, QuantityPKParameter pkParameter, SimulationPKParametersImport importedPKParameter)
      {
         if (allQuantities.Contains(pkParameter.QuantityPath))
            return;

         importedPKParameter.AddError(PKSimConstants.Error.CouldNotFindQuantityWithPath(pkParameter.QuantityPath));
      }

      private void validateLength(PopulationSimulation populationSimulation, QuantityPKParameter pkParameter, SimulationPKParametersImport importedPKParameter)
      {
         if (pkParameter.Count == populationSimulation.NumberOfItems)
            return;

         importedPKParameter.AddError(PKSimConstants.Error.NotEnoughPKValuesForParameter(pkParameter.Name, pkParameter.QuantityPath, populationSimulation.NumberOfItems, pkParameter.Count));
      }

      private Task<SimulationPKParametersImport> importPKAnalysesFromFile(string filefullPath, CancellationToken cancellationToken)
      {
         return Task.Run(() =>
         {
            var pKAnalysesFile = new PKAnalysesFile { FilePath = filefullPath };
            var pkAnalyses = _pkAnalysesImporter.ImportPKParameters(filefullPath, pKAnalysesFile);
            return new SimulationPKParametersImport(pkAnalyses, pKAnalysesFile);
         }, cancellationToken);
      }
   }
}