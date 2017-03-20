using System.Linq;
using PKSim.Assets;
using OSPSuite.Utility.Events;
using PKSim.Core.Model;
using PKSim.Presentation.Core;
using PKSim.Presentation.Presenters.Diagrams;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Assets;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ISimulationCompoundProcessSummaryCollectorPresenter : ISimulationItemPresenter
   {
      void ShowDiagram();
   }

   public class SimulationCompoundProcessSummaryCollectorPresenter : SimulationCompoundCollectorPresenterBase<ISimulationCompoundProcessSummaryCollectorView, ISimulationCompoundProcessSummaryPresenter>, ISimulationCompoundProcessSummaryCollectorPresenter
   {
      private readonly ISimulationCompoundInteractionSelectionPresenter _interactionSelectionPresenter;
      private readonly IReactionDiagramContainerPresenter _reactionDiagramPresenter;

      public SimulationCompoundProcessSummaryCollectorPresenter(
         ISimulationCompoundProcessSummaryCollectorView view, 
         IApplicationController applicationController,
         IConfigurableLayoutPresenter configurableLayoutPresenter, 
         IEventPublisher eventPublisher, 
         ISimulationCompoundInteractionSelectionPresenter interactionSelectionPresenter,
         IReactionDiagramContainerPresenter reactionDiagramPresenter)
         : base(view, applicationController, configurableLayoutPresenter, eventPublisher)
      {
         view.ApplicationIcon = ApplicationIcons.Protein;
         view.Caption = PKSimConstants.UI.SimulationProcessDefinition;
         view.AddInteractionView(interactionSelectionPresenter.BaseView);
         _reactionDiagramPresenter = reactionDiagramPresenter;
         _interactionSelectionPresenter = interactionSelectionPresenter;
         AddSubPresenters(_interactionSelectionPresenter,_reactionDiagramPresenter);
      }

      public override void EditSimulation(Simulation simulation, CreationMode creationMode)
      {
         base.EditSimulation(simulation, creationMode);
         _interactionSelectionPresenter.EditSimulation(simulation, creationMode);
         _view.ShowInteractionView = inhibitionCanBeDefinedIn(simulation);
      }

      protected override bool ShouldBeDisplayed(ISimulationCompoundProcessSummaryPresenter presenter)
      {
         return presenter.HasProcessesDefined;
      }

      private bool inhibitionCanBeDefinedIn(Simulation simulation)
      {
         var individual = simulation.Individual;
         var compounds = simulation.Compounds;

         return individual.HasMolecules() && compounds.Any(x=>x.HasProcesses<InteractionProcess>());
      }

      public override void SaveConfiguration()
      {
         base.SaveConfiguration();
         _interactionSelectionPresenter.SaveConfiguration();
      }

      public void ShowDiagram()
      {
         SaveConfiguration();
         _reactionDiagramPresenter.Show(_simulation);
      }
   }
}