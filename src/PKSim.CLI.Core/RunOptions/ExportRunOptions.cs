using System.Collections.Generic;
using PKSim.CLI.Core.Services;

namespace PKSim.CLI.Core.RunOptions
{
   public class ExportRunOptions : IWithOutputFolder
   {
      public string ProjectFile { get; set; }
      public SimulationExportMode ExportMode { get; set; }
      public string OutputFolder { get; set; }
      public IEnumerable<string> Simulations { get; set; }
      public bool RunSimulation { get; set; }
   }
}