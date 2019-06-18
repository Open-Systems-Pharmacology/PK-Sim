using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PKSim.Assets;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;

namespace PKSim.Core.Services
{
   public interface ISimulationResultsImportTask
   {
      Task<SimulationResultsImport> ImportResults(PopulationSimulation populationSimulation, IReadOnlyCollection<string> files, CancellationToken cancellationToken);
   }

   public class SimulationResultsImportTask : ISimulationResultsImportTask
   {
      private readonly IEntitiesInContainerRetriever _quantitiesRetriever;
      private readonly IIndividualResultsImporter _individualResultsImporter;
      private readonly IProgressManager _progressManager;
      private PathCache<IQuantity> _allQuantities;

      public SimulationResultsImportTask(IEntitiesInContainerRetriever quantitiesRetriever, IIndividualResultsImporter individualResultsImporter, IProgressManager progressManager)
      {
         _quantitiesRetriever = quantitiesRetriever;
         _individualResultsImporter = individualResultsImporter;
         _progressManager = progressManager;
      }

      public async Task<SimulationResultsImport> ImportResults(PopulationSimulation populationSimulation, IReadOnlyCollection<string> files, CancellationToken cancellationToken)
      {
         try
         {
            using (var progressUpdater = _progressManager.Create())
            {
               progressUpdater.Initialize(files.Count, PKSimConstants.UI.ImportingResults);
               _allQuantities = _quantitiesRetriever.QuantitiesFrom(populationSimulation);

               // Use ToList to execute the query and start the import task.
               var tasks = files.Select(f => importFiles(f,populationSimulation, cancellationToken)).ToList();
               var allImportedResults = new List<IndividualResultsImport>();
               // Await the completion of all the running tasks. 
               // Add a loop to process the tasks one at a time until none remain. 
               while (tasks.Count > 0)
               {
                  cancellationToken.ThrowIfCancellationRequested();

                  // Identify the first task that completes.
                  var firstFinishedTask = await Task.WhenAny(tasks);

                  // Remove the selected task from the list so that you don't 
                  // process it more than once.
                  tasks.Remove(firstFinishedTask);

                  // Await the completed task. 
                  allImportedResults.Add(await firstFinishedTask);
                  progressUpdater.IncrementProgress();
               }

               //once all results have been imported, it is time to ensure that they are consistent
               var results = createSimulationResultsFrom(allImportedResults);

               //last but not least, check that the number of individuals in the simulation matches the simulation results
               var importedCount = results.SimulationResults.Count;
               var expectedCount = populationSimulation.NumberOfItems;

               if (expectedCount != importedCount)
                  results.AddError(PKSimConstants.Error.NumberOfIndividualsInResultsDoNotMatchPopulation(populationSimulation.Name, expectedCount, importedCount));

               addImportedQuantityToLogForSuccessfulImport(results);
               return results;
            }
         }
         finally
         {
            _allQuantities.Clear();
         }
      }

      private void addImportedQuantityToLogForSuccessfulImport(SimulationResultsImport simulationResultsImport)
      {
         if (simulationResultsImport.Status.Is(NotificationType.Error))
            return;

         simulationResultsImport.AddInfo(PKSimConstants.Information.FollowingOutputsWereSuccessfulyImported(simulationResultsImport.SimulationResults.Count));
         foreach (var quantityPath in simulationResultsImport.SimulationResults.AllQuantityPaths())
         {
            simulationResultsImport.AddInfo(quantityPath);
         }
      }

      private Task<IndividualResultsImport> importFiles(string fileFullPath, Simulation simulation, CancellationToken cancellationToken)
      {
         return Task.Run(() =>
         {
            var importResult = new IndividualResultsImport();
            var simulationResultsFile = new SimulationResultsImportFile {FilePath = fileFullPath};
            importResult.IndividualResults = _individualResultsImporter.ImportFrom(fileFullPath, simulation,simulationResultsFile).ToList();
            simulationResultsFile.NumberOfIndividuals = importResult.IndividualResults.Count;
            importResult.SimulationResultsFile = simulationResultsFile;

            if (importResult.IndividualResults.Any())
               importResult.SimulationResultsFile.NumberOfQuantities = importResult.IndividualResults.First().Count();

            return importResult;
         }, cancellationToken);
      }

      private SimulationResultsImport createSimulationResultsFrom(IEnumerable<IndividualResultsImport> importedResults)
      {
         var simulationResultsImport = new SimulationResultsImport();

         //First add all available results
         importedResults.Each(import => addIndividualResultsFromSingleFile(simulationResultsImport, import));

         //now check that the defined outputs are actually available in the population simulation
         validateImportedQuantities(simulationResultsImport);


         return simulationResultsImport;
      }

      private void addIndividualResultsFromSingleFile(SimulationResultsImport simulationResultsImport, IndividualResultsImport individualResultsImport)
      {
         var simulationResults = simulationResultsImport.SimulationResults;
         simulationResultsImport.SimulationResultsFiles.Add(individualResultsImport.SimulationResultsFile);

         foreach (var individualResult in individualResultsImport.IndividualResults)
         {
            validateResults(simulationResultsImport, individualResult);

            if (simulationResults.HasResultsFor(individualResult.IndividualId))
               simulationResultsImport.AddError(PKSimConstants.Error.DuplicatedIndividualResultsForId(individualResult.IndividualId));
            else
               simulationResults.Add(individualResult);
         }
      }

      private void validateImportedQuantities(SimulationResultsImport simulationResultsImport)
      {
         foreach (var quantityPath in simulationResultsImport.SimulationResults.AllQuantityPaths())
         {
            var quantity = _allQuantities[quantityPath];
            if (quantity != null) continue;

            simulationResultsImport.AddError(PKSimConstants.Error.CouldNotFindQuantityWithPath(quantityPath));
         }
      }

      private class IndividualResultsImport
      {
         public IList<IndividualResults> IndividualResults { get; set; }
         public SimulationResultsImportFile SimulationResultsFile { get; set; }
      }

      private static void validateResults(SimulationResultsImport simulationResultsImport, IndividualResults individualResults)
      {
         var simulationResults = simulationResultsImport.SimulationResults;

         //No entry yet? Set this indiviudal results as base for the import
         if (!simulationResults.Any())
         {
            simulationResults.Time = individualResults.Time;
            individualResults.UpdateQuantityTimeReference();
            return;
         }

         validateTimeResults(simulationResultsImport, individualResults);
         var availableQuantityPaths = simulationResults.AllQuantityPaths();
         var currentQuantityPaths = individualResults.Select(x => x.QuantityPath).ToList();

         if (availableQuantityPaths.Count != currentQuantityPaths.Count)
         {
            simulationResultsImport.AddError(PKSimConstants.Error.IndividualResultsDoesNotHaveTheExpectedQuantity(individualResults.IndividualId, availableQuantityPaths, currentQuantityPaths));
            return;
         }

         for (int i = 0; i < availableQuantityPaths.Count; i++)
         {
            if (!string.Equals(availableQuantityPaths[i], currentQuantityPaths[i]))
            {
               simulationResultsImport.AddError(PKSimConstants.Error.IndividualResultsDoesNotHaveTheExpectedQuantity(individualResults.IndividualId, availableQuantityPaths, currentQuantityPaths));
               return;
            }
         }
      }

      private static void validateTimeResults(SimulationResultsImport simulationResultsImport, IndividualResults individualResults)
      {
         var time = simulationResultsImport.SimulationResults.Time;
         int expectedLength = time.Values.Length;
         int currentLength = individualResults.Time.Values.Length;

         if (time.Values.Length != individualResults.Time.Values.Length)
         {
            simulationResultsImport.AddError(PKSimConstants.Error.TimeArrayLengthDoesNotMatchFirstIndividual(individualResults.IndividualId, expectedLength, currentLength));
            return;
         }

         for (int i = 0; i < currentLength; i++)
         {
            if (!ValueComparer.AreValuesEqual(time[i], individualResults.Time[i]))
               simulationResultsImport.AddError(PKSimConstants.Error.TimeArrayValuesDoesNotMatchFirstIndividual(individualResults.IndividualId, i, time[i], individualResults.Time[i]));
         }

         //update reference time to ensure that all results are using the same time
         individualResults.Time = time;
         individualResults.UpdateQuantityTimeReference();
      }
   }
}