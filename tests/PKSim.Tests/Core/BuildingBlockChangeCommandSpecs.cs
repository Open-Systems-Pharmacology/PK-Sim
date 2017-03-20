using OSPSuite.BDDHelper;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Commands;
using FakeItEasy;

namespace PKSim.Core

{
   public abstract class concern_for_BuildingBlockChangeCommand : ContextSpecification<IBuildingBlockChangeCommand>
   {
      protected IExecutionContext _context;

      protected override void Context()
      {
         _context = A.Fake<IExecutionContext>();
         sut = new BuildingBlockChangeCommandForTest();
      }
   }

   
   public class When_executing_a_building_block_change_command : concern_for_BuildingBlockChangeCommand
   {
      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void should_update_the_version_of_the_used_building_block()
      {
         A.CallTo(() => _context.UpdateBuildingBlockVersion(sut)).MustHaveHappened();
      }
   }

   internal class BuildingBlockChangeCommandForTest : BuildingBlockChangeCommand
   {
      protected override void ClearReferences()
      {
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return null;
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
      }
   }
}