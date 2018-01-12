using System.Drawing;
using OSPSuite.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.DTO;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Views.Diagrams;
using OSPSuite.Core.Diagram;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Diagram.Elements;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.Diagram;
using OSPSuite.Presentation.Services;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface IReactionDiagramPresenter : IBaseDiagramPresenter, IDisposablePresenter, IEditIndividualSimulationItemPresenter, IEditPopulationSimulationItemPresenter
   {
      /// <summary>
      ///    Sets the subject of this reaction diagram to the simulation
      /// </summary>
      /// <param name="simulation">The simulation containing the reaction being displayed</param>
      void Edit(Simulation simulation);

      void DiagramHasChanged();
   }

   public class ReactionDiagramPresenter : BaseDiagramPresenter<IReactionDiagramView, IReactionDiagramPresenter, SimulationReactionDiagramDTO>, IReactionDiagramPresenter
   {
      private readonly IUserSettings _userSettings;
      private readonly IDiagramLayoutTask _layoutTask;
      private readonly ISimulationToSimulationReactionDiagramDTOMapper _simulationReactionDiagramDTOMapper;
      private Simulation _simulation;

      public ReactionDiagramPresenter(IReactionDiagramView view, 
         IContainerBaseLayouter layouter, 
         IDialogCreator dialogCreator, 
         IDiagramModelFactory diagramModelFactory,
         IUserSettings userSettings,
         IDiagramLayoutTask layoutTask, 
         ISimulationToSimulationReactionDiagramDTOMapper simulationReactionDiagramDTOMapper)
         : base(view, layouter, dialogCreator,diagramModelFactory)
      {
         _userSettings = userSettings;
         _layoutTask = layoutTask;
         _simulationReactionDiagramDTOMapper = simulationReactionDiagramDTOMapper;
         NodeMoved += (sender, args) => DiagramHasChanged();
      }

      public void EditSimulation(IndividualSimulation simulation)
      {
         Edit(simulation);
      }

      public void EditSimulation(PopulationSimulation simulation)
      {
         Edit(simulation);
      }

      public ApplicationIcon Icon => _view.ApplicationIcon;

      protected override IDiagramOptions GetDiagramOptions()
      {
         return _userSettings.DiagramOptions;
      }

      public override void ShowContextMenu(IBaseNode baseNode, Point popupLocation, PointF locationInDiagramView)
      {
         // no context menu
      }

      public override void Edit(SimulationReactionDiagramDTO simulationReactionDiagramDTO)
      {
         base.Edit(simulationReactionDiagramDTO);

         if (DiagramModel.IsLayouted) return;

         _layoutTask.LayoutReactionDiagram(DiagramModel);

         Refresh();
      }

      public void Edit(Simulation simulation)
      {
         _simulation = simulation;

         var dto = _simulationReactionDiagramDTOMapper.MapFrom(simulation);
         Edit(dto);
      }

      public void DiagramHasChanged()
      {
         _simulation.HasChanged = true;
      }

      public bool ShouldClose => true;
   }
}