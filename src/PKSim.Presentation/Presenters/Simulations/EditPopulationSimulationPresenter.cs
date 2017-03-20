using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Assets;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Services;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface IEditPopulationSimulationPresenter : IEditSimulationPresenter<PopulationSimulation>,
      IListener<AddAdvancedParameterToContainerEvent>,
      IListener<RemoveAdvancedParameterFromContainerEvent>,
      IListener<AdvancedParameterDistributionChangedEvent>,
      IListener<AdvancedParameteSelectedEvent>
   {
   }

   public class EditPopulationSimulationPresenter : EditSimulationPresenter<IEditPopulationSimulationView, IEditPopulationSimulationPresenter, PopulationSimulation, IEditPopulationSimulationItemPresenter>,
      IEditPopulationSimulationPresenter
   {
      private ISimulationAdvancedParameterDistributionPresenter _distributionPresenter;

      public EditPopulationSimulationPresenter(IEditPopulationSimulationView view, ISubPresenterItemManager<IEditPopulationSimulationItemPresenter> subPresenterItemManager, 
         ISimulationAnalysisPresenterFactory simulationAnalysisPresenterFactory,  ISimulationAnalysisPresenterContextMenuFactory contextMenuFactory, IPresentationSettingsTask presentationSettingsTask, ISimulationAnalysisCreator simulationAnalysisCreator)
         : base(view, subPresenterItemManager, EditPopulationSimulationItems.All, simulationAnalysisPresenterFactory,  contextMenuFactory, presentationSettingsTask, simulationAnalysisCreator)
      {
      }

      public override void InitializeWith(ICommandCollector commandRegister)
      {
         base.InitializeWith(commandRegister);
         _distributionPresenter = PresenterAt(EditPopulationSimulationItems.ParameterDistribution);
      }

      protected override void UpdateCaption()
      {
         _view.Caption = PKSimConstants.UI.EditPopulationSimulation.FormatWith(Analyzable.Name);
         _view.ApplicationIcon = Analyzable.AllowAging ? ApplicationIcons.AgingPopulationSimulation : ApplicationIcons.PopulationSimulation;
      }

      public void Handle(AddAdvancedParameterToContainerEvent advancedParameterEvent)
      {
         if (!canHandle(advancedParameterEvent)) return;
         _distributionPresenter.AddAdvancedParameter(advancedParameterEvent.AdvancedParameter);
      }

      public void Handle(RemoveAdvancedParameterFromContainerEvent advancedParameterEvent)
      {
         if (!canHandle(advancedParameterEvent)) return;
         _distributionPresenter.RemoveAdvancedParameter(advancedParameterEvent.AdvancedParameter);
      }

      public void Handle(AdvancedParameterDistributionChangedEvent advancedParameterEvent)
      {
         refreshDistribution(advancedParameterEvent);
      }

      private bool canHandle(IAdvancedParameterEvent advancedParameterEvent)
      {
         return Equals(advancedParameterEvent.AdvancedParameterContainer, Analyzable);
      }

      public void Handle(AdvancedParameteSelectedEvent advancedParameterEvent)
      {
         refreshDistribution(advancedParameterEvent);
      }

      private void refreshDistribution(IAdvancedParameterEvent advancedParameterEvent)
      {
         if (!canHandle(advancedParameterEvent)) return;
         _distributionPresenter.Select(advancedParameterEvent.AdvancedParameter);
      }

      public override string PresentationKey => PresenterConstants.PresenterKeys.EditAnalyzablePresenter;
   }
}