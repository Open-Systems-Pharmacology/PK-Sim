using System.Collections.Generic;
using PKSim.Core.Model;

namespace PKSim.Core.Batch
{
   internal class SimulationForBatch
   {
      public IndividualSimulation Simulation { get; set; }
      public SimulationConfiguration Configuration { get; set; }
      public List<ParameterVariationSet> ParameterVariationSets { get; private set; }

      public int NumberOfSimulations
      {
         //+1 for the actual default simulation
         get { return ParameterVariationSets.Count + 1; }
      }


      public SimulationForBatch()
      {
         ParameterVariationSets = new List<ParameterVariationSet>();
      }
   }
}