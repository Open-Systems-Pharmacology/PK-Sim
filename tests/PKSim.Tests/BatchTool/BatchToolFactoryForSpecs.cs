using System.Collections.Generic;
using System.Linq;
using PKSim.Core;
using PKSim.Core.Batch;
using PKSim.Core.Model;
using Compound = PKSim.Core.Batch.Compound;
using Individual = PKSim.Core.Batch.Individual;
using Simulation = PKSim.Core.Batch.Simulation;

namespace PKSim.BatchTool
{
   public static class BatchToolFactoryForSpecs
   {
      public static Individual Individual()
      {
         return new Individual
         {
            Species = CoreConstants.Species.Human,
            Population = CoreConstants.Population.ICRP,
            Age = 30,
            Weight = 80,
            Height = 17.8
         };
      }

      public static Compound Compound()
      {
         var comp = new Compound
         {
            Lipophilicity = 3,
            FractionUnbound = 0.8,
            MolWeight = 4E-7,
            F = 1,
            SolubilityAtRefpH = 100
         };
         comp.PkaTypes.Add(new PkaType{Type = CompoundType.Acid.ToString(),Value =8 });
         return comp;
      }

      public static ApplicationProtocol ApplicationProtocol()
      {
         return new ApplicationProtocol
         {
            ApplicationType = CoreConstants.Application.Name.IntravenousBolus,
            DosingInterval = DosingIntervalId.DI_6_6_6_6.ToString(),
            EndTime = 1440
         };
      }

      public static SimulationConfiguration Configuration()
      {
         return new SimulationConfiguration {Model = CoreConstants.Model.FourComp};
      }
      public static Simulation DefaultSimulation()
      {
         return SimulationFrom(Individual(), Compound(), ApplicationProtocol(), Configuration(), Enumerable.Empty<ParameterVariationSet>());
      }

      public static Simulation SimulationFrom(Individual individual, Compound compound, 
         ApplicationProtocol protocol, SimulationConfiguration configuration, IEnumerable<ParameterVariationSet> parameterVariationSets )
      {
         var simulation = new Simulation {Individual = individual};
         simulation.Compounds.Add(compound);
         simulation.ApplicationProtocols.Add(protocol);
         simulation.Configuration = configuration;
         simulation.ParameterVariationSets = new List<ParameterVariationSet>(parameterVariationSets);
         return simulation;
      }
   }
}