using System.Collections.Generic;
using System.Text;
using CommandLine;
using CommandLine.Text;
using PKSim.CLI.Core.RunOptions;

namespace PKSim.CLI.Commands
{
   [Verb("run", HelpText = "Start batch run by loading a set of batch json files, running all simulations and exporting the results")]
   public class JsonRunCommand : SimulationExportCommand<JsonRunOptions>, IWithInputAndOutputFolders
   {
      public override string Name { get; } = "Batch";

      [Option('i', "input", Required = true, HelpText = "Input folder containing all batch json files to run.")]
      public string InputFolder { get; set; }

      [Option('o', "output", Required = true, HelpText = "Output folder where calculated values will be exported.")]
      public string OutputFolder { get; set; }

      [Usage(ApplicationAlias = "PKSim.CLI")]
      public static IEnumerable<Example> Examples
      {
         get { yield return new Example("Running batch and export json", new JsonRunCommand {InputFolder = "<InputFolder>", OutputFolder = "<OutputFolder>", ExportJson = true}); }
      }

      public override string ToString()
      {
         var sb = new StringBuilder();
         this.LogOption(sb);
         sb.AppendLine($"Log file: {LogFileFullPath}");
         sb.AppendLine($"Log level: {LogLevel}");
         sb.AppendLine($"Export mode: {ExportMode}");
         return sb.ToString();
      }

      public override JsonRunOptions ToRunOptions()
      {
         return new JsonRunOptions
         {
            InputFolder = InputFolder,
            OutputFolder = OutputFolder,
            ExportMode = ExportMode,
         };
      }
   }
}