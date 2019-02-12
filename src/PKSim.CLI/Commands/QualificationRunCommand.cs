using System.Text;
using CommandLine;
using PKSim.CLI.Core.RunOptions;

namespace PKSim.CLI.Commands
{
   [Verb("qualification", HelpText = "Start qualification run workflow")]
   public class QualificationRunCommand : CLICommand<QualificationRunOptions>
   {
      public override string Name { get; } = "Qualification";

      [Option('i', "input", Required = true, HelpText = "Json configuration file used to start the qualification workflow.")]
      public string ConfigurationFile { get; set; }

      [Option('v', "validate", Required = false, HelpText = "Specifies a validation run. Default is false")]
      public bool Validate { get; set; }

      public override QualificationRunOptions ToRunOptions()
      {
         return new QualificationRunOptions
         {
            ConfigurationFile = ConfigurationFile,
            Validate = Validate
         };
      }

      public override string ToString()
      {
         var sb = new StringBuilder();
         LogDefaultOptions(sb);
         sb.AppendLine($"Validate: {Validate}");
         sb.AppendLine($"Configuration file: {ConfigurationFile}");
         return sb.ToString();
      }
   }
}