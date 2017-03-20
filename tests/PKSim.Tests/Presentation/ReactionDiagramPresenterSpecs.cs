using OSPSuite.BDDHelper;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Presentation.DTO;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Diagrams;
using OSPSuite.Core.Diagram;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Diagram.Elements;
using OSPSuite.Presentation.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_ReactionDiagramPresenter : ContextSpecification<ReactionDiagramPresenter>
   {
      protected IDiagramLayoutTask _diagramLayoutTask;
      protected ISimulationToSimulationReactionDiagramDTOMapper _mapper;
      protected SimulationReactionDiagramDTO _simulationReactionDiagramDTO;
      private IDiagramModelFactory _diagramModelFactory;

      protected override void Context()
      {
         _diagramLayoutTask = A.Fake<IDiagramLayoutTask>();
         _mapper= A.Fake<ISimulationToSimulationReactionDiagramDTOMapper>();
         _diagramModelFactory= A.Fake<IDiagramModelFactory>();
         _simulationReactionDiagramDTO = new SimulationReactionDiagramDTO
         {
            DiagramModel = A.Fake<IDiagramModel>(),
            DiagramManager = A.Fake<IDiagramManager<SimulationReactionDiagramDTO>>()
         };

         sut = new ReactionDiagramPresenter(A.Fake<IReactionDiagramView>(), A.Fake<IContainerBaseLayouter>(), A.Fake<IDialogCreator>(), _diagramModelFactory,
            A.Fake<IUserSettings>(), _diagramLayoutTask,_mapper);

         A.CallTo(_mapper).WithReturnType<SimulationReactionDiagramDTO>().Returns(_simulationReactionDiagramDTO);
      }
   }

   public class When_editing_a_simulation_with_a_diagram_presenter_ : concern_for_ReactionDiagramPresenter
   {
      private Simulation _simulation;

      protected override void Context()
      {
         base.Context();
         _simulation = A.Fake<Simulation>();
      }

      protected override void Because()
      {
         sut.Edit(_simulation);
      }

      [Observation]
      public void should_create_a_simulation_reaction_diagram_object_based_on_the_given_simulation()
      {
         A.CallTo(() => _mapper.MapFrom(_simulation)).MustHaveHappened();
      }

   }

   public class When_editing_a_building_block_dto_with_a_diagram_presenter : concern_for_ReactionDiagramPresenter
   {
      protected override void Because()
      {
         sut.Edit(_simulationReactionDiagramDTO);
      }

      [Observation]
      public void layout_task_must_have_performed_a_layout_for_the_diagram()
      {
         A.CallTo(() => _diagramLayoutTask.LayoutReactionDiagram(A<IDiagramModel>._)).MustHaveHappened();
      }
   }
}
