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
         var ind = new Individual();
         ind.Species = CoreConstants.Species.Human;
         ind.Population = CoreConstants.Population.ICRP;
         ind.Age = 30;
         ind.Weight = 80;
         ind.Height = 178;
         return ind;
      }

      public static Compound Compound()
      {
         var comp = new Compound();
         comp.Lipophilicity = 3;
         comp.FractionUnbound  = 0.8;
         comp.MolWeight  = 400;
         comp.F  = 1;
         comp.SolubilityAtRefpH = 100;
         comp.PkaTypes.Add(new PkaType{Type = CompoundType.Acid.ToString(),Value =8 });
         return comp;
      }

      public static ApplicationProtocol ApplicationProtocol()
      {
         var protocol = new ApplicationProtocol();
         protocol.ApplicationType = CoreConstants.Application.Name.IntravenousBolus;
         protocol.DosingInterval = DosingIntervalId.DI_6_6_6_6.ToString();
         protocol.EndTime = 1440;
         return protocol;
      }

      public static SimulationConfiguration Configuration()
      {
         var config = new SimulationConfiguration();
         config.Model = CoreConstants.Model.FourComp;
         return config;
      }
      public static Simulation DefaultSimulation()
      {
         return SimulationFrom(Individual(), Compound(), ApplicationProtocol(), Configuration(), Enumerable.Empty<ParameterVariationSet>());
      }

      public static Simulation SimulationFrom(Individual individual, Compound compound, 
         ApplicationProtocol protocol, SimulationConfiguration configuration, IEnumerable<ParameterVariationSet> parameterVariationSets )
      {
         var simulation = new Simulation();
         simulation.Individual =individual;
         simulation.Compounds.Add(compound);
         simulation.ApplicationProtocols.Add(protocol);
         simulation.Configuration = configuration;
         simulation.ParameterVariationSets = new List<ParameterVariationSet>(parameterVariationSets);
         return simulation;
      }
   }
}