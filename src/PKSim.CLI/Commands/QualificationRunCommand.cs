using System.Text;
using CommandLine;
using PKSim.CLI.Core.RunOptions;

namespace PKSim.CLI.Commands
{
   [Verb("qualification", HelpText = "Start qualification run workflow")]
   public class QualificationRunCommand : CLICommand<QualificationRunOptions>
   {
      public override string Name { get; } = "Qualification";
      public override bool LogCommandName { get; } = false;

      [Option('i', "input", Required = true, HelpText = "Json configuration file used to start the qualification workflow.")]
      public string ConfigurationFile { get; set; }

      [Option('v', "validate", Required = false, HelpText = "Specifies a validation run. Default is false")]
      public bool Validate { get; set; }

      [Option('r', "run", Required = false, HelpText = "Should the qualification runner also run the simulation or simply export the qualification report for further processing. Default is true")]
      public bool Run { get; set; } = true;

      public override QualificationRunOptions ToRunOptions()
      {
         return new QualificationRunOptions
         {
            ConfigurationFile = ConfigurationFile,
            Validate = Validate,
            Run = Run
         };
      }

      public override string ToString()
      {
         var sb = new StringBuilder();
         LogDefaultOptions(sb);
         sb.AppendLine($"Validate: {Validate}");
         sb.AppendLine($"Configuration file: {ConfigurationFile}");
         sb.AppendLine($"Run Simulations: {Run}");
         return sb.ToString();
      }
   }
}