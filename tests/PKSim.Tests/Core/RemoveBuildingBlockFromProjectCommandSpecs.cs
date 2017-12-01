using OSPSuite.Core.Commands.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;

namespace PKSim.Core
{
   public abstract class concern_for_remove_building_block_from_project_command : ContextSpecification<RemoveBuildingBlockFromProjectCommand>
   {
      protected IExecutionContext _executionContext;
      protected IPKSimBuildingBlock _buildingBlock;
      protected PKSimProject _project;

      protected override void Context()
      {
         _executionContext =A.Fake<IExecutionContext>();
         _buildingBlock = A.Fake<IPKSimBuildingBlock>();
         _project = A.Fake<PKSimProject>();
         A.CallTo(() => _executionContext.CurrentProject).Returns(_project);
         sut = new RemoveBuildingBlockFromProjectCommand(_buildingBlock,_executionContext);
      }
   }

   
   public class When_removing_a_building_block_from_a_project : concern_for_remove_building_block_from_project_command
   {
      protected override void Because()
      {
         sut.Execute(_executionContext);
      }

      [Observation]
      public void should_have_removed_the_building_block()
      {
          A.CallTo(() => _project.RemoveBuildingBlock(_buildingBlock)).MustHaveHappened();
      }
   }

   
   public class The_inverse_of_a_remove_building_block_from_project_command : concern_for_remove_building_block_from_project_command
   {
      private IReversibleCommand<IExecutionContext> _result;

      protected override void Because()
      {
         _result = sut.InverseCommand(_executionContext);
      }

      [Observation]
      public void should_be_an_add_building_block_to_project_command()
      {
         _result.ShouldBeAnInstanceOf<AddBuildingBlockToProjectCommand>();
      }

      [Observation]
      public void should_have_beeen_marked_as_inverse_for_the_remove_command()
      {
         _result.IsInverseFor(sut).ShouldBeTrue();
      }
   }
}	