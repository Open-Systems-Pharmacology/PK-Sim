using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Batch;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using Compound = PKSim.Core.Model.Compound;
using ILazyLoadTask = PKSim.Core.Services.ILazyLoadTask;
using Individual = PKSim.Core.Model.Individual;

namespace PKSim.BatchTool.Services
{
   public class ProjectComparisonRunner : IBatchRunner<ProjectComparisonOptions>
   {
      private readonly IWorkspacePersistor _workspacePersistor;
      private readonly IWorkspace _workspace;
      private readonly ILazyLoadTask _lazyLoadTask;
      private readonly IBatchLogger _logger;
      private readonly ISimulationExportTask _simulationExportTask;
      private readonly IMoBiExportTask _moBiExportTask;
      private readonly ISimulationFactory _simulationFactory;
      private readonly IExecutionContext _executionContext;
      private readonly ISimulationModelCreator _simulationModelCreator;

      private readonly ISimulationEngine<IndividualSimulation> _simulationEngine;

      public ProjectComparisonRunner(IWorkspacePersistor workspacePersistor, IWorkspace workspace,
         ILazyLoadTask lazyLoadTask, IBatchLogger logger, ISimulationEngineFactory simulationEngineFactory, ISimulationExportTask simulationExportTask,
         IMoBiExportTask moBiExportTask, ISimulationFactory simulationFactory, IExecutionContext executionContext, ISimulationModelCreator simulationModelCreator)
      {
         _workspacePersistor = workspacePersistor;
         _workspace = workspace;
         _lazyLoadTask = lazyLoadTask;
         _logger = logger;
         _simulationExportTask = simulationExportTask;
         _moBiExportTask = moBiExportTask;
         _simulationFactory = simulationFactory;
         _executionContext = executionContext;
         _simulationModelCreator = simulationModelCreator;
         _simulationEngine = simulationEngineFactory.Create<IndividualSimulation>();
      }

      public async Task RunBatchAsync(ProjectComparisonOptions projectComparisonOptions)
      {
         string inputFolder = projectComparisonOptions.InputFolder;
         string outputFolder = projectComparisonOptions.OutputFolder;

         clear();

         _logger.AddInSeparator($"Starting project comparison run: {DateTime.Now.ToIsoFormat()}");

         var inputDirectory = new DirectoryInfo(inputFolder);
         if (!inputDirectory.Exists)
            throw new ArgumentException($"Input folder '{inputFolder}' does not exist");

         var allProjectFiles = inputDirectory.GetFiles(CoreConstants.Filter.PROJECT_FILTER);
         if (allProjectFiles.Length == 0)
            throw new ArgumentException($"No project file found in '{inputFolder}'");

         var outputDirectory = new DirectoryInfo(outputFolder);
         if (outputDirectory.Exists)
         {
            outputDirectory.Delete(recursive: true);
            _logger.AddInfo($"Deleting folder '{outputFolder}'");
         }

         outputDirectory.Create();
         var afterConversionDirectory = outputDirectory.CreateSubdirectory("after_conversion_results");
         var originalDirectory = outputDirectory.CreateSubdirectory("original_results");
         var newDirectory = outputDirectory.CreateSubdirectory("new_simulation_based_on_old_building_blocks_results");
         var pkmlOldDirectory = outputDirectory.CreateSubdirectory("pkml_from_converted_simulations");
         var pkmlNewDirecotry = outputDirectory.CreateSubdirectory("pkml_from_new_simulations");

         var begin = DateTime.UtcNow;

         //this tasks cannot be run in // because of global stuff going on in deserialization. 
         foreach (var projectFile in allProjectFiles)
         {
            await exportProjectResults(projectFile, originalDirectory, afterConversionDirectory, newDirectory, pkmlOldDirectory, pkmlNewDirecotry);
         }

         var end = DateTime.UtcNow;
         var timeSpent = end - begin;

         _logger.AddInSeparator($"Finished project comparison run in {timeSpent.ToDisplay()}'");
      }

      private async Task exportProjectResults(FileInfo projectFile, DirectoryInfo originalDirectory, DirectoryInfo afterConversionDirectory, DirectoryInfo newDirectory, DirectoryInfo pkmlOldDirectory, DirectoryInfo pkmlNewDirectory)
      {
         int simulationCount = 0;
         var begin = DateTime.UtcNow;
         _logger.AddInSeparator($"Loading simulations in project file '{projectFile.FullName}'");

         _workspacePersistor.LoadSession(_workspace, projectFile.FullName);
         var project = _workspace.Project;
         foreach (var simulation in project.All<IndividualSimulation>())
         {
            try
            {
               var res = await exportSimulation(originalDirectory, afterConversionDirectory, newDirectory, pkmlOldDirectory, pkmlNewDirectory, simulation, project);
               if (res)
                  simulationCount++;
            }
            catch (Exception e)
            {
               _logger.AddError(e.FullMessage());
               _logger.AddError(e.FullStackTrace());
            }
         }

         var end = DateTime.UtcNow;
         var timeSpent = end - begin;

         _workspace.CloseProject();
         _logger.AddInfo($"{simulationCount} simulations exported from project '{projectFile.FullName}' in {timeSpent.ToDisplay()}");
      }

      private async Task<bool> exportSimulation(DirectoryInfo originalDirectory, DirectoryInfo afterConversionDirectory, DirectoryInfo newDirectory, DirectoryInfo pkmlOldDirectory, DirectoryInfo pkmlNewDirectory, IndividualSimulation simulation, PKSimProject project)
      {
         _lazyLoadTask.Load(simulation);

         if (!simulation.HasResults)
            return warn($"No results found for simulation '{simulation.Name}'");

         if (!simulation.HasUpToDateResults)
            return warn($"Results not up to date in simulation '{simulation.Name}'");

         if (simulation.IsImported)
            return warn($"Imported simulation '{simulation.Name}' cannot be exported");

         await exportSimulationToPkml(pkmlOldDirectory, project.Name, simulation);

         await exportSimulationResultsToCSV(originalDirectory, project.Name, simulation, "old");

         await runSimulation(simulation);

         await exportSimulationResultsToCSV(afterConversionDirectory, project.Name, simulation, "new");

         IndividualSimulation otherSimulation = null;
         try
         {
            _logger.AddDebug($"Creating simulation based on old building blocks for '{simulation.Name}'");
            otherSimulation = await createNewSimulationBasedOn(simulation, project);

            await runSimulation(simulation);

            await exportSimulationResultsToCSV(newDirectory, project.Name, otherSimulation, "new from old bb");

            await exportSimulationToPkml(pkmlNewDirectory, project.Name, simulation);
         }
         finally
         {
            _executionContext.Unregister(otherSimulation);
         }

         return true;
      }

      private Task runSimulation(IndividualSimulation simulation)
      {
         _logger.AddDebug($"Running simulation '{simulation.Name}'");
         return _simulationEngine.RunAsync(simulation);
      }

      private Task<IndividualSimulation> createNewSimulationBasedOn(IndividualSimulation individualSimulation, PKSimProject project)
      {
         return Task.Run(() =>
         {
            var otherSimulation = _executionContext.Clone(individualSimulation);
            var individual = allTemplateBuildingBlocksFor<Individual>(otherSimulation, project).First();
            var compounds = allTemplateBuildingBlocksFor<Compound>(otherSimulation, project);
            otherSimulation = _simulationFactory.CreateFrom(individual, compounds, otherSimulation.ModelProperties, otherSimulation)
               .DowncastTo<IndividualSimulation>();

            _simulationModelCreator.CreateModelFor(otherSimulation);
            _executionContext.Register(otherSimulation);

            return otherSimulation;
         });
      }

      private bool warn(string warning)
      {
         _logger.AddWarning(warning);
         return false;
      }

      private IReadOnlyList<T> allTemplateBuildingBlocksFor<T>(IndividualSimulation simulation, PKSimProject project) where T : class, IPKSimBuildingBlock
      {
         var buildingBlocks = simulation.UsedBuildingBlocksInSimulation<T>().Select(x => project.All<T>().FindById(x.TemplateId)).ToList();
         buildingBlocks.Each(_lazyLoadTask.Load);
         return buildingBlocks;
      }

      private Task exportSimulationToPkml(DirectoryInfo directoryInfo, string projectName, IndividualSimulation simulation)
      {
         _logger.AddDebug($"Exporting simulation to pkml format '{simulation.Name}'");
         var fileName = createFileName(directoryInfo, projectName, simulation, Constants.Filter.PKML_EXTENSION);
         return _moBiExportTask.SaveSimulationToFileAsync(simulation, fileName);
      }

      private Task exportSimulationResultsToCSV(DirectoryInfo directoryInfo, string projectName, IndividualSimulation simulation, string type)
      {
         _logger.AddDebug($"Exporting {type} results for simulation '{simulation.Name}'");

         var fileName = createFileName(directoryInfo, projectName, simulation, Constants.Filter.CSV_EXTENSION);
         return _simulationExportTask.ExportResultsToCSVAsync(simulation, fileName);
      }

      private string createFileName(DirectoryInfo directoryInfo, string projectName, IndividualSimulation simulation, string extension)
      {
         return Path.Combine(directoryInfo.FullName, $"{projectName}.{FileHelper.RemoveIllegalCharactersFrom(simulation.Name)}{extension}");
      }

      private void clear()
      {
         _logger.Clear();
      }
   }
}