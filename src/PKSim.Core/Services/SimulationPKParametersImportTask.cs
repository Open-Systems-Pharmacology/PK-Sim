using System.Threading;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Infrastructure.Import.Services;
using PKSim.Assets;
using PKSim.Core.Model;
using ICoreSimulationPKParametersImportTask = OSPSuite.Infrastructure.Import.Services.ISimulationPKParametersImportTask;

namespace PKSim.Core.Services
{
   public interface ISimulationPKParametersImportTask
   {
      Task<SimulationPKParametersImport> ImportPKParameters(PopulationSimulation populationSimulation, string fileFullPath, CancellationToken cancellationToken);
   }

   public class SimulationPKParametersImportTask : ISimulationPKParametersImportTask
   {
      private readonly ICoreSimulationPKParametersImportTask _coreSimulationPKParametersImportTask;

      public SimulationPKParametersImportTask(ICoreSimulationPKParametersImportTask coreSimulationPKParametersImportTask)
      {
         _coreSimulationPKParametersImportTask = coreSimulationPKParametersImportTask;
      }

      public async Task<SimulationPKParametersImport> ImportPKParameters(PopulationSimulation populationSimulation, string fileFullPath, CancellationToken cancellationToken)
      {
         var importedPKAnalysis = await _coreSimulationPKParametersImportTask.ImportPKParameters(fileFullPath, populationSimulation, cancellationToken);
         cancellationToken.ThrowIfCancellationRequested();
         validateConsistencyWithSimulation(populationSimulation, importedPKAnalysis);
         return importedPKAnalysis;
      }

      private void validateConsistencyWithSimulation(PopulationSimulation populationSimulation, SimulationPKParametersImport importedPKParameter)
      {
         foreach (var pkParameter in importedPKParameter.PKParameters)
         {
            validateLength(populationSimulation, pkParameter, importedPKParameter);
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

      private void validateLength(PopulationSimulation populationSimulation, QuantityPKParameter pkParameter, SimulationPKParametersImport importedPKParameter)
      {
         if (pkParameter.Count == populationSimulation.NumberOfItems)
            return;

         importedPKParameter.AddError(PKSimConstants.Error.NotEnoughPKValuesForParameter(pkParameter.Name, pkParameter.QuantityPath, populationSimulation.NumberOfItems, pkParameter.Count));
      }
   }
}