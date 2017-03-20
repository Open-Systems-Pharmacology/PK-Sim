using BTS.UI.Resources;
using BTS.UI.Services;
using PKSim.Core.Diagram;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.Mappers;
using PKSim.Presentation.Views.Diagrams;
using SBSuite.Core.Diagram;
using SBSuite.Presentation.Diagram.Diagram;
using SBSuite.Presentation.Diagram.Presenters;

namespace PKSim.Presentation.Presenters.Diagrams
{
   public class PKSimReactionDiagramPresenter : BaseDiagramPresenter<IPKSimReactionDiagramView, IPKSimReactionDiagramPresenter, IReactionBuildingBlockWithDiagram>, IPKSimReactionDiagramPresenter
   {
      private readonly IReactionBuildingBlockCreator _reactionBuildingBlockCreator;
      private readonly IUserSettings _userSettings;
      private readonly IReactionBuildingBlockToReactionBuildingBlockWithDiagramMapper _buildingBlockToWithDiagramMapper;
      private readonly IDiagramLayoutTask _layoutTask;
      private readonly ISimulationToSimulationReactionDiagramDTOMapper _simulationReactionDiagramMapper;

      public PKSimReactionDiagramPresenter(IPKSimReactionDiagramView view, IContainerBaseLayouter layouter, IDialogCreator dialogCreator,
         IReactionBuildingBlockCreator reactionBuildingBlockCreator,
         IUserSettings userSettings,
         IReactionBuildingBlockToReactionBuildingBlockWithDiagramMapper buildingBlockToWithDiagramMapper,
         IDiagramLayoutTask layoutTask, ISimulationToSimulationReactionDiagramDTOMapper simulationReactionDiagramMapper)
         : base(view, layouter, dialogCreator)
      {
         _reactionBuildingBlockCreator = reactionBuildingBlockCreator;
         _userSettings = userSettings;
         _buildingBlockToWithDiagramMapper = buildingBlockToWithDiagramMapper;
         _layoutTask = layoutTask;
         _simulationReactionDiagramMapper = simulationReactionDiagramMapper;
      }

      public void EditSimulation(IndividualSimulation simulation)
      {
         Edit(simulation);
      }

      public void EditSimulation(PopulationSimulation simulation)
      {
         Edit(simulation);
      }

      public ApplicationIcon Icon
      {
         get { return _view.ApplicationIcon; }
      }

      protected override IDiagramOptions GetDiagramOptions()
      {
         return _userSettings.DiagramOptions;
      }

      public override void Edit(IReactionBuildingBlockWithDiagram pkModel)
      {
         base.Edit(pkModel);

         if (DiagramModel.IsLayouted) return;

         _layoutTask.LayoutReactionDiagram(DiagramModel);

         Refresh();
      }

      public void Edit(Simulation simulation)
      {
         Edit( _simulationReactionDiagramMapper.MapFrom(simulation));
      }

      public bool ShouldCancel
      {
         get { return true; }
      }
   }
}