using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OSPSuite.Core.Services;
using OSPSuite.Infrastructure.Import.Services;
using PKSim.Assets;
using PKSim.Core.Model;
using ICoreSimulationResultsImportTask = OSPSuite.Infrastructure.Import.Services.ISimulationResultsImportTask;

namespace PKSim.Core.Services
{
   public interface ISimulationResultsImportTask
   {
      Task<SimulationResultsImport> ImportResults(PopulationSimulation populationSimulation, IReadOnlyCollection<string> files, CancellationToken cancellationToken);
   }

   public class SimulationResultsImportTask : ISimulationResultsImportTask
   {
      private readonly ICoreSimulationResultsImportTask _coreSimulationResultsImportTask;

      public SimulationResultsImportTask(ICoreSimulationResultsImportTask coreSimulationResultsImportTask)
      {
         _coreSimulationResultsImportTask = coreSimulationResultsImportTask;
      }

      public async Task<SimulationResultsImport> ImportResults(PopulationSimulation populationSimulation, IReadOnlyCollection<string> files, CancellationToken cancellationToken)
      {
         var results = await _coreSimulationResultsImportTask.ImportResults(populationSimulation, files, cancellationToken);
         //last but not least, check that the number of individuals in the simulation matches the simulation results
         var importedCount = results.SimulationResults.Count;
         var expectedCount = populationSimulation.NumberOfItems;

         if (expectedCount != importedCount)
            results.AddError(PKSimConstants.Error.NumberOfIndividualsInResultsDoNotMatchPopulation(populationSimulation.Name, expectedCount, importedCount));

         return results;
      }
   }
}