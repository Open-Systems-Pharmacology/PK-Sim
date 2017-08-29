using System.Text;
using CommandLine;
using PKSim.CLI.Core.RunOptions;

namespace PKSim.CLI.Commands
{
   [Verb("convert", HelpText = "Start batch run by loading a set of batch json files, running all simulations and exporting the results")]
   public class BatchConverterCommand : CLICommand<BatchConverterRunOptions>, IWithInputAndOutputFolders
   {
      public override string Name { get; } = "Convert";

      [Option('i', "input", Required = true, HelpText = "Input folder containing all batch json files to run.")]
      public string InputFolder { get; set; }

      [Option('o', "output", Required = true, HelpText = "Output folder where calculated values will be exported.")]
      public string OutputFolder { get; set; }

      public override string ToString()
      {
         var sb = new StringBuilder();
         this.LogOption(sb);
         sb.AppendLine($"Log file: {LogFileFullPath}");
         sb.AppendLine($"Log level: {LogLevel}");
         return sb.ToString();
      }

      public override BatchConverterRunOptions ToRunOptions()
      {
         return new BatchConverterRunOptions
         {
            InputFolder = InputFolder,
            OutputFolder = OutputFolder,
         };
      }
   }
}