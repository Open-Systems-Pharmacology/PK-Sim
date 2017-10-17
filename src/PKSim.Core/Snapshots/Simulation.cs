using System.ComponentModel.DataAnnotations;

namespace PKSim.Core.Snapshots
{
   public class Simulation : SnapshotBase
   {
      [Required]
      public string Model { get; set; }

      public bool? AllowAging { get; set; }
      public string[] ObservedData { get; set; }
      public SolverSettings Solver { get; set; }
      public OutputSchema OutputSchema { get; set; }
      public LocalizedParameter[] Parameters { get; set; }
      public OutputSelections OutputSelections { get; set; }
      public string Individual { get; set; }
      public string Population { get; set; }
      public CompoundProperties[] Compounds { get; set; }
      public EventSelection[] Events { get; set; }
      public AdvancedParameter[] AdvancedParameters { get; set; }
      public bool HasResults { get; set; }

      //Individual charts (only set for individual simulation)
      public CurveChart[] IndividualAnalyses { get; set; }

      //Population analyses (only set for population simulation)
      public PopulationAnalysisChart[] PopulationAnalyses { get; set; }

      public CompoundProcessSelection[] Interactions { get; set; }
   }
}