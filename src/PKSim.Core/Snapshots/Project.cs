using System.Collections.Generic;

namespace PKSim.Core.Snapshots
{
   public class Project : SnapshotBase
   {
      public List<Individual> Individuals { get; set; } = new List<Individual>();
      public List<Population> Populations { get; set; } = new List<Population>();
      public List<Compound> Compounds { get; set; } = new List<Compound>();
      public List<Formulation> Formulations { get; set; } = new List<Formulation>();
      public List<Protocol> Protocols { get; set; } = new List<Protocol>();
      public List<Event> Events { get; set; } = new List<Event>();
   }
}