using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using FakeItEasy;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;


namespace PKSim.Core
{
   public abstract class concern_for_SetUsedBuildingBlockVersionCommand : ContextSpecification<SetUsedBuildingBlockVersionCommand>
   {
      protected Simulation _simulation;
      protected UsedBuildingBlock _usedBuildingBlock;
      protected int _version;
      protected IExecutionContext _context;
      protected IBuildingBlockInSimulationManager _buildingBlockInSimulationManager;

      protected override void Context()
      {
         _simulation = A.Fake<Simulation>();
         _usedBuildingBlock = A.Fake<UsedBuildingBlock>();
         _version = 3;
         _context = A.Fake<IExecutionContext>();
         _buildingBlockInSimulationManager = A.Fake<IBuildingBlockInSimulationManager>();
         A.CallTo(() => _context.Resolve<IBuildingBlockInSimulationManager>()).Returns(_buildingBlockInSimulationManager);
         sut = new SetUsedBuildingBlockVersionCommand(_simulation, _usedBuildingBlock, _version, _context);
      }
   }

   public class When_executing_the_set_version_command : concern_for_SetUsedBuildingBlockVersionCommand
   {
      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void should_update_the_version_of_the_given_simulation_for_the_building_block()
      {
         _usedBuildingBlock.Version.ShouldBeEqualTo(_version);
      }

      [Observation]
      public void should_update_the_building_block_names_used_in_the_simulation()
      {
         A.CallTo(() => _buildingBlockInSimulationManager.UpdateBuildingBlockNamesUsedIn(_simulation)).MustHaveHappened();
      }
   }

   public class The_inverse_of_the_set_version_command : concern_for_SetUsedBuildingBlockVersionCommand
   {
      private ICommand<IExecutionContext> _result;

      protected override void Because()
      {
         _result = sut.InverseCommand(_context);
      }

      [Observation]
      public void should_be_a_set_version_command()
      {
         _result.ShouldBeAnInstanceOf<SetUsedBuildingBlockVersionCommand>();
      }

      [Observation]
      public void should_have_been_marked_as_inverse_for_the_add_command()
      {
         _result.IsInverseFor(sut).ShouldBeTrue();
      }
   }
}