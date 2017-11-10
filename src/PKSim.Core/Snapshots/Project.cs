using System.ComponentModel.DataAnnotations;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Snapshots
{
   public class Project : IWithDescription
   {
      [Required]
      public int Version { get; set; }

      public string Description { get; set; }

      public Individual[] Individuals { get; set; }
      public Population[] Populations { get; set; }
      public Compound[] Compounds { get; set; }
      public Formulation[] Formulations { get; set; }
      public Protocol[] Protocols { get; set; }
      public Event[] Events { get; set; }
      public Simulation[] Simulations { get; set; }
      public DataRepository[] ObservedData { get; set; }
      public SimulationComparison[] SimulationComparisons { get; set; }
      public Classification[] ObservedDataClassifications { get; set; }
      public Classification[] SimulationComparisonClassifications { get; set; }
      public Classification[] SimulationClassifications { get; set; }
   }
}