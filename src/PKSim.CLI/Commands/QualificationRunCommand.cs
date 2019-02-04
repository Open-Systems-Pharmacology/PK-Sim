using System.Text;
using CommandLine;
using PKSim.CLI.Core.RunOptions;

namespace PKSim.CLI.Commands
{
   [Verb("qualification", HelpText = "Start qualification run workflow")]
   public class QualificationRunCommand : CLICommand<QualificationRunOptions>
   {
      public override string Name { get; } = "Qualification";

      [Option('c', "config", Required = false, HelpText = "Json configuration string used to start the qualification workflow.")]
      public string Configuration { get; set; }

      [Option('f', "file", Required = false, HelpText = "Json configuration file used to start the qualification workflow.")]
      public string ConfigurationFile { get; set; }

      [Option('v', "validate", Required = false, HelpText = "Specifies a validation run. Default is false")]
      public bool Validate { get; set; }

      public override QualificationRunOptions ToRunOptions()
      {
         return new QualificationRunOptions
         {
            Configuration = Configuration,
            ConfigurationFile = ConfigurationFile,
            Validate = Validate
         };
      }

      public override string ToString()
      {
         var sb = new StringBuilder();
         LogDefaultOptions(sb);
         sb.AppendLine($"Validate: {Validate}");

         if (!string.IsNullOrEmpty(Configuration))
            sb.AppendLine($"Configuration string: {Configuration}");

         if (!string.IsNullOrEmpty(ConfigurationFile))
            sb.AppendLine($"Configuration file: {ConfigurationFile}");
         
         return sb.ToString();
      }
   }
}