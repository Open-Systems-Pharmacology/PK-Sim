using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Builder;
using PKSim.Presentation.DTO;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationReactionDiagramDTO : ContextSpecification<SimulationReactionDiagramDTO>
   {
      protected ReactionBuildingBlock _reactionBuildingBlock;
      protected ReactionBuilder _reaction1;

      protected override void Context()
      {
         _reaction1 = new ReactionBuilder();
         _reactionBuildingBlock = new ReactionBuildingBlock
         {
            _reaction1
         };

         sut = new SimulationReactionDiagramDTO {ReactionBuildingBlock = _reactionBuildingBlock};
      }
   }

   public class iterating_builders_from_the_building_block_with_diagram : concern_for_SimulationReactionDiagramDTO
   {
      [Observation]
      public void should_return_the_reaction_from_the_underlying_building_block()
      {
         sut.ShouldOnlyContain(_reaction1);
      }
   }
}