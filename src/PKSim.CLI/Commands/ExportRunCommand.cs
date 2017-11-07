using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommandLine;
using CommandLine.Text;
using OSPSuite.Utility.Extensions;
using PKSim.CLI.Core.RunOptions;

namespace PKSim.CLI.Commands
{
   [Verb("export", HelpText = "Start project export by loading the given project file and exported all or selected simulations to the output folder.")]
   public class ExportRunCommand : SimulationExportCommand<ExportRunOptions>, IWithOutputFolder
   {
      public override string Name { get; } = "Export";

      [Option('p', "project", Required = true, HelpText = "Full path of project file whose simulations should be exported.")]
      public string ProjectFile { get; set; }

      [Option('o', "output", Required = true, HelpText = "Output folder where simulations artifacts will be exported.")]
      public string OutputFolder { get; set; }

      [Option('s', "simulations", Required = false, HelpText = "Optional. List of simulations to export using space as a delimiter. If not set, all simulations will be exported. Note: Simulation name containing space should be between quotes like so \"my simulation\"" )]
      public IEnumerable<string> Simulations { get; set; } = new List<string>();

      [Option('r', "run", Required = false, HelpText = "Optional. Flag indicating whether simulations should be run before exporting? Default is false.")]
      public bool RunSimulation { get; set; }

      [Usage(ApplicationAlias = "PKSim.CLI")]
      public static IEnumerable<Example> Examples
      {
         get
         {
            var exportProjectToCsvAndXml  = new ExportRunCommand {ProjectFile = "<ProjectFile>.pksim5", OutputFolder = "<OutputFolder>", ExportXml = true, ExportCsv = true};
            yield return new Example("Export all simulations from a project to csv and xml using short notation", UnParserSettings.WithGroupSwitchesOnly(), exportProjectToCsvAndXml);
            yield return new Example("Export all simulations from a project to csv and xml using long notation", new UnParserSettings(), exportProjectToCsvAndXml);

            var runAndExportSelectedSimulationToCsv = new ExportRunCommand {RunSimulation = true, ProjectFile = "<ProjectFile>.pksim5", OutputFolder = "<OutputFolder>", ExportCsv = true, Simulations = new[] {"My 1st simulation", "My 2nd simulation", "S3"}};
            yield return new Example("Run and export specified simulations from a project to csv using short notation", UnParserSettings.WithGroupSwitchesOnly(), runAndExportSelectedSimulationToCsv);
            yield return new Example("Run and export specified simulations from a project to csv using long notation", new UnParserSettings(), runAndExportSelectedSimulationToCsv);
         }
      }

      public override string ToString()
      {
         var sb = new StringBuilder();
         sb.AppendLine($"Project file: {ProjectFile}");
         this.LogOption(sb);
         sb.AppendLine($"Export mode: {ExportMode}");

         if (Simulations.Any())
            sb.AppendLine($"Exporting simulation: {Simulations.ToString(", ")}");
         else
            sb.AppendLine($"Exporting all simulations");

         sb.AppendLine($"Run simulation: {RunSimulation}");
         sb.AppendLine($"Log level: {LogLevel}");
         sb.AppendLine($"Log file: {LogFileFullPath}");
         return sb.ToString();
      }

      public override ExportRunOptions ToRunOptions()
      {
         return new ExportRunOptions
         {
            ProjectFile = ProjectFile,
            ExportMode = ExportMode,
            OutputFolder = OutputFolder,
            Simulations = Simulations,
            RunSimulation = RunSimulation,
         };
      }
   }
}