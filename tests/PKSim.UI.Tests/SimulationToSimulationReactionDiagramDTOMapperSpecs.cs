using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Diagram;
using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO;
using PKSim.Presentation.DTO.Mappers;
using PKSim.UI.Mappers;

namespace PKSim.UI
{
   public abstract class concern_for_SimulationToSimulationReactionDiagramDTOMapper : ContextSpecification<ISimulationToSimulationReactionDiagramDTOMapper>
   {
      protected IReactionBuildingBlockCreator _reactionBuildingBlockCreator;
      protected Simulation _simulation;
      protected SimulationReactionDiagramDTO _dto;
      protected ReactionBuildingBlock _reactionBuildingBlock;

      protected override void Context()
      {
         _reactionBuildingBlockCreator = A.Fake<IReactionBuildingBlockCreator>();
         sut = new SimulationToSimulationReactionDiagramDTOMapper(_reactionBuildingBlockCreator);
         _reactionBuildingBlock = new ReactionBuildingBlock();

         _simulation = new IndividualSimulation();
      }

      protected override void Because()
      {
         _dto = sut.MapFrom(_simulation);
      }
   }

   public class When_mapping_a_simulation_with_new_reaction_building_block_that_contains_the_same_reactions_as_the_existing_model : concern_for_SimulationToSimulationReactionDiagramDTOMapper
   {
      private IDiagramModel _diagramModel;
      private ReactionBuilder _secondBuilder;
      private IBaseNode _firstNode;

      protected override void Context()
      {
         base.Context();
         _firstNode = A.Fake<IBaseNode>();
         A.CallTo(() => _firstNode.Id).Returns("Id1");
         A.CallTo(() => _firstNode.Name).Returns("Name");
         _secondBuilder = new ReactionBuilder {Name = "Name", Id = "Id2"};
         _simulation.AddReactions(_reactionBuildingBlock);
         _reactionBuildingBlock.Add(_secondBuilder);

         _diagramModel = A.Fake<IDiagramModel>();
         _simulation.ReactionDiagramModel = _diagramModel;

         A.CallTo(() => _diagramModel.FindByName(_secondBuilder.Name)).Returns(_firstNode);
      }

      [Observation]
      public void should_re_identify_the_existing_node_to_match_the_new_builder_id()
      {
         A.CallTo(() => _diagramModel.ReplaceNodeIds(A<IDictionary<string, string>>.That.Matches(dictionary =>
            dictionary.Keys.First().Equals("Id1") && dictionary.Values.First().Equals("Id2")
         ))).MustHaveHappened();
      }

      [Observation]
      public void should_use_the_reaction_building_block_from_the_simulation()
      {
         _dto.ReactionBuildingBlock.ShouldBeEqualTo(_simulation.Reactions.First());
      }

      [Observation]
      public void should_use_the_reaction_diagram_model_from_the_simulation()
      {
         _dto.DiagramModel.ShouldBeEqualTo(_simulation.ReactionDiagramModel);
      }
   }

   public class When_mapping_a_simulation_whose_reaction_building_block_is_not_defined_to_a_simulation_reaction_diagram_dto : concern_for_SimulationToSimulationReactionDiagramDTOMapper
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _reactionBuildingBlockCreator.CreateFor(_simulation)).Returns(_reactionBuildingBlock);
      }

      [Observation]
      public void should_create_a_new_reaction_building_block()
      {
         _dto.ReactionBuildingBlock.ShouldBeEqualTo(_reactionBuildingBlock);
      }

      [Observation]
      public void should_use_the_reaction_diagram_model_from_the_simulation()
      {
         _dto.DiagramModel.ShouldBeEqualTo(_simulation.ReactionDiagramModel);
      }
   }
}