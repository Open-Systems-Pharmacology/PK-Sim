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
      protected IDiagramModelFactory _diagramModelFactory;

      protected override void Context()
      {
         _reactionBuildingBlockCreator = A.Fake<IReactionBuildingBlockCreator>();
         _diagramModelFactory = A.Fake<IDiagramModelFactory>();
         sut = new SimulationToSimulationReactionDiagramDTOMapper(_reactionBuildingBlockCreator, _diagramModelFactory);
         _reactionBuildingBlock = new ReactionBuildingBlock();

         _simulation = new IndividualSimulation();
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
         _reactionBuildingBlock.Add(_secondBuilder);
         A.CallTo(() => _reactionBuildingBlockCreator.CreateFor(_simulation)).Returns(_reactionBuildingBlock);

         _diagramModel = A.Fake<IDiagramModel>();
         _simulation.ReactionDiagramModel = _diagramModel;
         A.CallTo(() => _diagramModel.FindByName(_secondBuilder.Name)).Returns(_firstNode);
      }

      protected override void Because()
      {
         _dto = sut.MapFrom(_simulation, recreateDiagram: false);
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
         _dto.ReactionBuildingBlock.ShouldBeEqualTo(_reactionBuildingBlock);
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

      protected override void Because()
      {
         _dto = sut.MapFrom(_simulation, recreateDiagram: false);
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

   public class When_mapping_a_simulation_in_the_recreate_diagram_mode : concern_for_SimulationToSimulationReactionDiagramDTOMapper
   {
      private IDiagramModel _diagramModel;
      private ReactionBuildingBlock _simulationReactionBuildingBlock;

      protected override void Context()
      {
         base.Context();
         _diagramModel = A.Fake<IDiagramModel>();
         _simulationReactionBuildingBlock = new ReactionBuildingBlock();
         A.CallTo(() => _reactionBuildingBlockCreator.CreateFor(_simulation)).Returns(_reactionBuildingBlock);
         _simulation.AddReactions(_simulationReactionBuildingBlock);
         A.CallTo(() => _diagramModelFactory.Create()).Returns(_diagramModel);
      }

      protected override void Because()
      {
         _dto = sut.MapFrom(_simulation, recreateDiagram: true);
      }

      [Observation]
      public void should_create_a_new_reaction_building_block()
      {
         _dto.ReactionBuildingBlock.ShouldBeEqualTo(_reactionBuildingBlock);
      }

      [Observation]
      public void should_create_new_diagram_model()
      {
         _dto.DiagramModel.ShouldBeEqualTo(_diagramModel);
      }
   }

   public class When_mapping_a_simulation_in_the_recreate_diagram_mode_set_to_false : concern_for_SimulationToSimulationReactionDiagramDTOMapper
   {
      private IDiagramModel _diagramModel;
      private ReactionBuildingBlock _simulationReactionBuildingBlock;

      protected override void Context()
      {
         base.Context();
         _diagramModel = A.Fake<IDiagramModel>();
         _simulationReactionBuildingBlock = new ReactionBuildingBlock();
         A.CallTo(() => _reactionBuildingBlockCreator.CreateFor(_simulation)).Returns(_reactionBuildingBlock);
         _simulation.AddReactions(_simulationReactionBuildingBlock);
         _simulation.ReactionDiagramModel = A.Fake<IDiagramModel>();
         A.CallTo(() => _diagramModelFactory.Create()).Returns(_diagramModel);
      }

      protected override void Because()
      {
         _dto = sut.MapFrom(_simulation, recreateDiagram: false);
      }

      [Observation]
      public void should_use_simulation_reaction_building_block()
      {
         _dto.ReactionBuildingBlock.ShouldBeEqualTo(_simulationReactionBuildingBlock);
      }

      [Observation]
      public void should_use_simulation_diagram_model()
      {
         _dto.DiagramModel.ShouldBeEqualTo(_simulation.ReactionDiagramModel);
      }
   }
}