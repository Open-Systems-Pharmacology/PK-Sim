using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Batch;
using PKSim.Core.Commands;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;

namespace PKSim.BatchTool.Services
{
   public class JsonSimulationRunner : IBatchRunner
   {
      private readonly ISimulationLoader _simulationLoader;
      private readonly ISimulationExporter _simulationExporter;
      private readonly IBatchLogger _logger;
      private readonly IEntitiesInContainerRetriever _entitiesInContainerRetriever;
      private readonly IExecutionContext _executionContext;
      private readonly ICommandTask _commandTask;
      private readonly IList<string> _allSimulationNames = new List<string>();

      public JsonSimulationRunner(ISimulationLoader simulationLoader, ISimulationExporter simulationExporter, IBatchLogger logger,
         IEntitiesInContainerRetriever entitiesInContainerRetriever, IExecutionContext executionContext, ICommandTask commandTask)
      {
         _simulationLoader = simulationLoader;
         _simulationExporter = simulationExporter;
         _logger = logger;
         _entitiesInContainerRetriever = entitiesInContainerRetriever;
         _executionContext = executionContext;
         _commandTask = commandTask;
      }

      public Task RunBatch(dynamic parameters)
      {
         string inputFolder = parameters.inputFolder;
         string outputFolder = parameters.outputFolder;
         clear();

         return Task.Run(() =>
         {

            _logger.AddInSeparator("Starting batch run: {0}".FormatWith(DateTime.Now.ToIsoFormat()));

            var inputDirectory = new DirectoryInfo(inputFolder);
            if (!inputDirectory.Exists)
               throw new ArgumentException($"Input folder '{inputFolder}' does not exist");

            var allSimulationFiles = inputDirectory.GetFiles(Constants.Filter.JSON_FILE_FILTER);
            if (allSimulationFiles.Length == 0)
               throw new ArgumentException($"No simulation json file found in '{inputFolder}'");

            var outputDirectory = new DirectoryInfo(outputFolder);
            if (!outputDirectory.Exists)
            {
               _logger.AddInfo("Creating folder '{0}'".FormatWith(outputFolder));
               outputDirectory.Create();
            }

            var allExistingFiles = outputDirectory.GetFiles("*.csv");
            allExistingFiles.Each(outputFile =>
            {
               FileHelper.DeleteFile(outputFile.FullName);
               _logger.AddDebug("Deleting file '{0}'".FormatWith(outputFile.FullName));
            });

            var begin = DateTime.UtcNow;
            allSimulationFiles.Each(s => exportSimulationTo(s, outputFolder));
            var end = DateTime.UtcNow;
            var timeSpent = end - begin;
            _logger.AddInSeparator("{0} simulations calculated and exported in '{1}'".FormatWith(_allSimulationNames.Count, timeSpent.ToDisplay()));

            createSummaryFilesIn(outputDirectory);

            _logger.AddInSeparator("Batch run finished: {0}".FormatWith(DateTime.Now.ToIsoFormat()));

            exportLogFile(outputDirectory);
         });
      }

      private void clear()
      {
         _logger.Clear();
         _allSimulationNames.Clear();
      }

      private void exportLogFile(DirectoryInfo outputDirectory)
      {
         using (var file = new StreamWriter(Path.Combine(outputDirectory.FullName, "log.txt")))
         {
            _logger.Entries.Each(file.WriteLine);
         }
      }

      private void createSummaryFilesIn(DirectoryInfo outputDirectory)
      {
         var dataTable = new DataTable(outputDirectory.Name);
         dataTable.Columns.Add("Sim_id", typeof (string));
         foreach (var simulationName in _allSimulationNames)
         {
            var row = dataTable.NewRow();
            row[0] = simulationName;
            dataTable.Rows.Add(row);
         }
         var fileName = Path.Combine(outputDirectory.FullName, $"{outputDirectory.Name}.csv");
         if (FileHelper.FileExists(fileName))
         {
            var tmp = FileHelper.FileNameFromFileFullPath(FileHelper.GenerateTemporaryFileName());
            tmp = Path.Combine(outputDirectory.FullName, $"{tmp}.csv");
            _logger.AddWarning("File '{0}' already exists and will be saved under '{1}'".FormatWith(fileName, tmp));
            fileName = tmp;
         }

         _logger.AddDebug("Exporting simulation file names to '{0}'".FormatWith(fileName));
         dataTable.ExportToCSV(fileName);
      }

      private void exportSimulationTo(FileInfo simulationFile, string outputFolder)
      {
         _logger.AddInSeparator("Starting batch simulation for file '{0}'".FormatWith(simulationFile));
         try
         {
            var simForBatch = _simulationLoader.LoadSimulationFrom(simulationFile.FullName);
            string simText = simForBatch.NumberOfSimulations > 1 ? "simulations" : "simulation";
            _logger.AddInfo("{0} {1} found in file '{2}'".FormatWith(simForBatch.NumberOfSimulations, simText, simulationFile));

            var simulation = simForBatch.Simulation;

            var defaultSimulationName = FileHelper.FileNameFromFileFullPath(simulationFile.FullName);
            var allParameters = _entitiesInContainerRetriever.ParametersFrom(simulation);
            _simulationExporter.RunAndExport(simulation, outputFolder, defaultSimulationName, simForBatch.Configuration);
            _allSimulationNames.Add(defaultSimulationName);
            foreach (var parameterValueSet in simForBatch.ParameterVariationSets)
            {
               string currentName = $"{defaultSimulationName}_{parameterValueSet.Name}";
               var command = updateParameters(allParameters, parameterValueSet);
               _simulationExporter.RunAndExport(simulation, outputFolder, currentName, simForBatch.Configuration);
               _allSimulationNames.Add(currentName);
               _commandTask.ResetChanges(command, _executionContext);
            }

            //when simulation is not used anymore, unregister simulation
            _executionContext.Unregister(simulation);
         }
         catch (Exception e)
         {
            _logger.AddError($"{e.FullMessage()}\n{e.FullStackTrace()}");
         }
      }

      private IPKSimMacroCommand updateParameters(PathCache<IParameter> allParameters, ParameterVariationSet parameterVariationSet)
      {
         var macroCommand = new PKSimMacroCommand();

         foreach (var parameterValue in parameterVariationSet.ParameterValues)
         {
            var parameterPath = parameterValue.ParameterPath;

            var parameter = allParameters[parameterPath];
            if (parameter == null)
            {
               //try with adding the name of the simulation at first
               parameterPath = "{0}{1}{2}".FormatWith(Simulation.Name, ObjectPath.PATH_DELIMITER, parameterPath);
               parameter = allParameters[parameterPath];
               if (parameter == null)
               {
                  _logger.AddWarning("Parameter with path '{0}' not found!".FormatWith(parameterValue.ParameterPath));
                  continue;
               }
            }

            _logger.AddDebug("Parameter '{0}' value set from '{1} to '{2}'.".FormatWith(parameterValue.ParameterPath, parameter.Value, parameterValue.Value));
            macroCommand.Add(new SetParameterValueCommand(parameter, parameterValue.Value));
         }

         return macroCommand.Run(_executionContext);
      }
   }
}