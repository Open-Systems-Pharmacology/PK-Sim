using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;

namespace PKSim.Core
{
   public abstract class concern_for_ReactionBuildingBlockCreator : ContextSpecification<IReactionBuildingBlockCreator>
   {
      protected IMoleculesAndReactionsCreator _moleculeAndReactionCreator;
      protected IObjectBaseFactory _objectBaseFactory;

      protected override void Context()
      {
         _moleculeAndReactionCreator = A.Fake<IMoleculesAndReactionsCreator>();
         _objectBaseFactory = A.Fake<IObjectBaseFactory>();
         sut = new ReactionBuildingBlockCreator(_moleculeAndReactionCreator, _objectBaseFactory);
      }
   }

   public class When_creating_the_reaction_building_block_based_on_the_settings_of_a_given_simulation : concern_for_ReactionBuildingBlockCreator
   {
      private Simulation _simulation;
      private IReactionBuildingBlock _reactionBuildingBlock;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<Simulation>();
         _reactionBuildingBlock = A.Fake<IReactionBuildingBlock>();
         A.CallTo(() => _moleculeAndReactionCreator.CreateFor(A<SimulationConfiguration>._, _simulation))
            .Invokes(x => x.GetArgument<SimulationConfiguration>(0).Module.Reaction = _reactionBuildingBlock);
      }

      [Observation]
      public void should_leverage_the_molecule_and_reaction_creator_to_create_a_reaction_building_block()
      {
         sut.CreateFor(_simulation).ShouldBeEqualTo(_reactionBuildingBlock);
      }
   }

   public class When_creating_a_rection_building_block_for_an_imported_simulation : concern_for_ReactionBuildingBlockCreator
   {
      private Simulation _simulation;
      private IReactionBuildingBlock _reactionBuildingBlock;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<Simulation>();
         A.CallTo(() => _simulation.IsImported).Returns(true);
         _reactionBuildingBlock = A.Fake<IReactionBuildingBlock>();
         A.CallTo(() => _objectBaseFactory.Create<IReactionBuildingBlock>()).Returns(_reactionBuildingBlock);
      }

      [Observation]
      public void should_return_an_empty_reaction_building_block()
      {
         sut.CreateFor(_simulation).ShouldBeEqualTo(_reactionBuildingBlock);
      }
   }
}