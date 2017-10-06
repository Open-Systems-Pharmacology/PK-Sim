using System.Text;
using CommandLine;
using OSPSuite.Core.Domain;
using PKSim.CLI.Core;
using PKSim.CLI.Core.RunOptions;
using PKSim.CLI.Core.Services;

namespace PKSim.CLI.Commands
{
   [Verb("run", HelpText = "Start batch run by loading a set of batch json files, running all simulations and exporting the results")]
   public class JsonRunCommand : IWithInputAndOutputFolders
   {
      public BatchExportMode ExportMode { get; set; }

      public NotificationType NotificationType { get; set; } = NotificationType.Info | NotificationType.Error;

      [Option('i', "input", Required = true, HelpText = "Input folder containing all batch json files to run")]
      public string InputFolder { get; set; }

      [Option('o', "output", Required = true, HelpText = "Output folder where calculated values will be exported")]
      public string OutputFolder { get; set; }

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

      public override string ToString()
      {
         var sb = new StringBuilder();
         this.LogOption(sb);
         sb.AppendLine($"Log file: {LogFileFullPath}");
         sb.AppendLine($"Notification type: {NotificationType}");
         sb.AppendLine($"Export mode: {ExportMode}");
         return sb.ToString();
      }

      public JsonRunOptions ToRunOptions()
      {
         return new JsonRunOptions
         {
            LogFileFullPath = LogFileFullPath,
            InputFolder = InputFolder,
            OutputFolder = OutputFolder,
            ExportMode = ExportMode,
            NotificationType = NotificationType,
         };
      }
   }
}