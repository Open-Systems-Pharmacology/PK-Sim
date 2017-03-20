using OSPSuite.BDDHelper;
using FakeItEasy;
using PKSim.Presentation.DTO;
using OSPSuite.Core.Domain.Builder;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationReactionDiagramDTO : ContextSpecification<SimulationReactionDiagramDTO>
   {
      protected IReactionBuildingBlock _reactionBuildingBlock;

      protected override void Context()
      {
         _reactionBuildingBlock = A.Fake<IReactionBuildingBlock>();
         sut = new SimulationReactionDiagramDTO {ReactionBuildingBlock = _reactionBuildingBlock};
      }
   }

   public class iterating_builders_from_the_building_block_with_diagram : concern_for_SimulationReactionDiagramDTO
   {
      protected override void Because()
      {
         sut.GetEnumerator();
      }

      [Observation]
      public void Should_have_called_enumerator_from_base_building_block()
      {
         A.CallTo(() => _reactionBuildingBlock.GetEnumerator()).MustHaveHappened();
      }
   }
}
