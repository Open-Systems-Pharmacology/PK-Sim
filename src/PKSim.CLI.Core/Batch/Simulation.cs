using System.Collections.Generic;
using PKSim.Core.Model;

namespace PKSim.Core.Batch
{
   internal class Simulation
   {
      public static string Name = "Sim";

      public Simulation()
      {
         ParameterVariationSets = new List<ParameterVariationSet>();
         ApplicationProtocols = new List<ApplicationProtocol>();
         Interactions = new List<InteractionSelection>();
         Compounds = new List<Compound>();
         Configuration = new SimulationConfiguration();
      }

      public Formulation Formulation { get; set; }
      public Individual Individual { get; set; }
      public List<Compound> Compounds { get; set; }
      public List<ApplicationProtocol> ApplicationProtocols { get; set; }
      public List<Event> Events { get; set; }
      public SimulationConfiguration Configuration { get; set; }
      public List<ParameterVariationSet> ParameterVariationSets { get; set; }
      public List<InteractionSelection> Interactions { get; set; }
   }
}