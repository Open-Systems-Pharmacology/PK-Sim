using CommandLine;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.DTO;
using OSPSuite.Utility.Validation;
using PKSim.BatchTool.Services;
using PKSim.Core;

namespace PKSim.BatchTool
{
   public interface IWithInputFolder
   {
      string InputFolder { get; set; }
   }

   public interface IWithOutputFolder
   {
      string OutputFolder { get; set; }
   }

   public interface IWithInputAndOutputFolders : IWithInputFolder, IWithOutputFolder
   {
   }

   [Verb("comparison", HelpText = "Start project comparison by loading a set of projects, running all simulations and exporting old and new results")]
   public class ProjectComparisonOptions : ValidatableDTO, IWithInputAndOutputFolders
   {
      private string _inputFolder;
      private string _outputFolder;

      [Option('i', "input", Required = true, HelpText = "Input folder with projects to be loaded")]
      public string InputFolder
      {
         get => _inputFolder;
         set => SetProperty(ref _inputFolder, value);
      }

      [Option('o', "output", Required = true, HelpText = "Output folder where value will be exported")]
      public string OutputFolder
      {
         get => _outputFolder;
         set => SetProperty(ref _outputFolder, value);
      }

      public ProjectComparisonOptions()
      {
         Rules.AddRange(new[]
         {
            OptionsRules.InputFolderDefined,
            OptionsRules.OutputFolderDefined,
         });
      }
   }

   [Verb("overview", HelpText = "Generate project overview by loading a set of projects and exporting a file containing a description of all building blocks and observed data used")]
   public class ProjectOverviewOptions : ValidatableDTO, IWithInputFolder
   {
      private string _inputFolder;

      [Option('i', "input", Required = true, HelpText = "Input folder with projects to be loaded")]
      public string InputFolder
      {
         get => _inputFolder;
         set => SetProperty(ref _inputFolder, value);
      }

      public ProjectOverviewOptions()
      {
         Rules.AddRange(new[]
         {
            OptionsRules.InputFolderDefined,
         });
      }
   }

   [Verb("run", HelpText = "Start batch run by loading a set of batch json files, running all simulations and exporting the results")]
   public class JsonRunOptions : ValidatableDTO, IWithInputAndOutputFolders
   {
      private string _outputFolder;
      private string _inputFolder;

      public BatchExportMode ExportMode { get; set; } 

      public NotificationType NotificationType { get; set; } = NotificationType.Info | NotificationType.Error;

      [Option('i', "input", Required = true, HelpText = "Input folder containing all batch json files to run")]
      public string InputFolder
      {
         get => _inputFolder;
         set => SetProperty(ref _inputFolder, value);
      }

      [Option('o', "output", Required = true, HelpText = "Output folder where calculated values will be exported")]
      public string OutputFolder
      {
         get => _outputFolder;
         set
         {
            SetProperty(ref _outputFolder, value);
            if (string.IsNullOrEmpty(LogFileFullPath))
            {
               LogFileFullPath = CoreConstants.DefaultBatchLogFullPath(OutputFolder);
            }
         }
      }

      [Option('l', "log", Required = false, HelpText = "Full path of log file where log output will be written. If not defined, it wil be exported in the output folder")]
      public string LogFileFullPath { get; set; }

      [Option('c', "csv", HelpText = "Export simulation outputs to csv")]
      public bool ExportCsv
      {
         set
         {
            if (value)
               ExportMode = ExportMode | BatchExportMode.Csv;
         }
      }

      [Option('x', "xml", HelpText = "Export the simulation model xml")]
      public bool ExportXml
      {
         set
         {
            if (value)
               ExportMode = ExportMode | BatchExportMode.Xml;
         }
      }

      [Option('j', "json", HelpText = "Export simulation outputs to json")]
      public bool ExportJson
      {
         set
         {
            if (value)
               ExportMode = ExportMode | BatchExportMode.Json;
         }
      }

      [Option('d', "debug", HelpText = "Show debug notifications in log file")]
      public bool Debug
      {
         set
         {
            if (value)
               NotificationType = NotificationType | NotificationType.Debug;
         }
      }

      [Option('w', "warning", HelpText = "Show warning notifications in log file")]
      public bool Warning
      {
         set
         {
            if (value)
               NotificationType = NotificationType | NotificationType.Warning;
         }
      }

      public JsonRunOptions()
      {
         Rules.AddRange(new[]
         {
            OptionsRules.InputFolderDefined,
            OptionsRules.OutputFolderDefined,
            GenericRules.NonEmptyRule<JsonRunOptions>(x => x.LogFileFullPath)
         });
      }
   }

   [Verb("training", HelpText = "Generate training materials")]
   public class TrainingMaterialsOptions : ValidatableDTO, IWithOutputFolder
   {
      private string _outputFolder;

      [Option('o', "output", Required = true, HelpText = "Output folder where training materials will be generated")]
      public string OutputFolder
      {
         get => _outputFolder;
         set => SetProperty(ref _outputFolder, value);
      }

   }

   public static class OptionsRules
   {
      public static IBusinessRule OutputFolderDefined { get; } = GenericRules.NonEmptyRule<IWithOutputFolder>(x => x.OutputFolder);
      public static IBusinessRule InputFolderDefined { get; } = GenericRules.NonEmptyRule<IWithInputFolder>(x => x.InputFolder);
   }
}