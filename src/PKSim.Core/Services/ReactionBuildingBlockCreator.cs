using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IReactionBuildingBlockCreator
   {
      /// <summary>
      ///    Creates a <see cref="ReactionBuildingBlock" /> based on the process settings defined in the
      ///    <paramref name="simulation" />
      /// </summary>
      ReactionBuildingBlock CreateFor(Simulation simulation);
   }

   public class ReactionBuildingBlockCreator : IReactionBuildingBlockCreator
   {
      private readonly IMoleculesAndReactionsCreator _moleculesAndReactionsCreator;
      private readonly IObjectBaseFactory _objectBaseFactory;

      public ReactionBuildingBlockCreator(IMoleculesAndReactionsCreator moleculesAndReactionsCreator, IObjectBaseFactory objectBaseFactory)
      {
         _moleculesAndReactionsCreator = moleculesAndReactionsCreator;
         _objectBaseFactory = objectBaseFactory;
      }

      public ReactionBuildingBlock CreateFor(Simulation simulation)
      {
         if (simulation.IsImported)
            return _objectBaseFactory.Create<ReactionBuildingBlock>();

         var simulationConfiguration = new SimulationConfiguration();
         var module = new Module
         {
            new PassiveTransportBuildingBlock()
         };

         var (molecules, reactions) = _moleculesAndReactionsCreator.CreateFor(module, simulation);
         module.Add(molecules);
         module.Add(reactions);

         simulationConfiguration.AddModuleConfiguration(new ModuleConfiguration(module));
         return module.Reactions;
      }
   }
}