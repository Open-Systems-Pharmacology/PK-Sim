using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using FakeItEasy;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_SetUsedBuildingBlockAlteredFlagCommand : ContextSpecification<SetUsedBuildingBlockAlteredFlagCommand>
   {
      protected Simulation _simulation;
      protected IPKSimBuildingBlock _buildingBlock;
      protected bool _altered;
      protected string _buildingBlockId;
      protected IExecutionContext _context;
      protected UsedBuildingBlock _usedBuildingBlock;
      protected IBuildingBlockInSimulationManager _buildingBlockInSimulationManager;

      protected override void Context()
      {
         _simulation = A.Fake<Simulation>();
         _buildingBlock = A.Fake<IPKSimBuildingBlock>();
         _context = A.Fake<IExecutionContext>();
         _usedBuildingBlock = A.Fake<UsedBuildingBlock>();
         _buildingBlockInSimulationManager = A.Fake<IBuildingBlockInSimulationManager>();
         _buildingBlockId = "toto";
         A.CallTo(() => _simulation.UsedBuildingBlockById(_buildingBlockId)).Returns(_usedBuildingBlock);
         _buildingBlock.Id = _buildingBlockId;
         _altered = false;
         A.CallTo(() => _simulation.GetAltered(_buildingBlockId)).Returns(true);
         A.CallTo(() => _context.Resolve<IBuildingBlockInSimulationManager>()).Returns(_buildingBlockInSimulationManager);
         sut = new SetUsedBuildingBlockAlteredFlagCommand(_simulation, _usedBuildingBlock, _altered, _context);
      }
   }

   public class When_executing_the_set_altered_flag_command : concern_for_SetUsedBuildingBlockAlteredFlagCommand
   {
      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void should_update_the_altered_flag_value_of_the_given_simulation_for_the_building_block()
      {
         _usedBuildingBlock.Altered.ShouldBeEqualTo(_altered);
      }

      [Observation]
      public void should_update_the_building_block_names_used_in_the_simulation()
      {
         A.CallTo(() => _buildingBlockInSimulationManager.UpdateBuildingBlockNamesUsedIn(_simulation)).MustHaveHappened();
      }
   }

   public class The_inverse_of_the_set_altered_flag_command : concern_for_SetUsedBuildingBlockAlteredFlagCommand
   {
      private IReversibleCommand<IExecutionContext> _result;

      protected override void Because()
      {
         _result = sut.InverseCommand(_context);
      }

      [Observation]
      public void should_be_a_set_altered_flag_command()
      {
         _result.ShouldBeAnInstanceOf<SetUsedBuildingBlockAlteredFlagCommand>();
      }

      [Observation]
      public void should_have_beeen_marked_as_inverse_for_the_add_command()
      {
         _result.IsInverseFor(sut).ShouldBeTrue();
      }
   }
}