using System.Collections.Generic;
using OSPSuite.Core.Domain;
using PKSim.Core.Model.PopulationAnalyses;

namespace PKSim.Core.Snapshots
{
   public class SimulationComparison : IWithName, IWithDescription
   {
      public string Name { get; set; }
      public string Description { get; set; }
      public string[] Simulations { get; set; }
      public CurveChart IndividualComparison { get; set; }
      public PopulationAnalysisChart[] PopulationComparisons { get; set; }
      public GroupingItem ReferenceGroupingItem { get; set; }
      public string ReferenceSimulation { get; set; }
   }
}