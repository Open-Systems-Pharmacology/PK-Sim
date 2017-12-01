using CommandLine;
using PKSim.CLI.Core.Services;

namespace PKSim.CLI.Commands
{
   public abstract class SimulationExportCommand<TRunOptions> : CLICommand<TRunOptions>
   {
      public SimulationExportMode ExportMode { get; set; }

      [Option('c', "csv", HelpText = "Export simulation outputs to csv.")]
      public bool ExportCsv
      {
         set
         {
            if (value)
               ExportMode = ExportMode | SimulationExportMode.Csv;
         }
         get => ExportMode.HasFlag(SimulationExportMode.Csv);
      }

      [Option('x', "xml", HelpText = "Export simulation model xml.")]
      public bool ExportXml
      {
         set
         {
            if (value)
               ExportMode = ExportMode | SimulationExportMode.Xml;
         }
         get => ExportMode.HasFlag(SimulationExportMode.Xml);
      }

      [Option('j', "json", HelpText = "Export simulation outputs to json. Available for individual simulation only.")]
      public bool ExportJson
      {
         set
         {
            if (value)
               ExportMode = ExportMode | SimulationExportMode.Json;
         }
         get => ExportMode.HasFlag(SimulationExportMode.Json);
      }

      [Option('k', "pkml", HelpText = "Export simulation to pkml format.")]
      public bool ExportPkml
      {
         set
         {
            if (value)
               ExportMode = ExportMode | SimulationExportMode.Pkml;
         }
         get => ExportMode.HasFlag(SimulationExportMode.Pkml);
      }
   }
}