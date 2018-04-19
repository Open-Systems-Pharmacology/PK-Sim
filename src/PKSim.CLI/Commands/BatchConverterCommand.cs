using System.Text;
using CommandLine;
using PKSim.CLI.Core.RunOptions;

namespace PKSim.CLI.Commands
{
   [Verb("convert", HelpText = "Convert batch files from old format to new format (DEV ONLY. Will be removed)")]
   public class BatchConverterCommand : CLICommand<BatchConverterRunOptions>, IWithInputAndOutputFolders
   {
      public override string Name { get; } = "Convert";

      [Option('i', "input", Required = true, HelpText = "Input folder containing all batch json files to convert.")]
      public string InputFolder { get; set; }

      [Option('o', "output", Required = true, HelpText = "Output folder where new batch files will be exported.")]
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