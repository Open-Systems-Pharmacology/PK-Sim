using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Services;
using OSPSuite.Assets;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface IEditIndividualSimulationPresenter : IEditSimulationPresenter<IndividualSimulation>
   {
   }

   public class EditIndividualSimulationPresenter : EditSimulationPresenter<IEditIndividualSimulationView, IEditIndividualSimulationPresenter, IndividualSimulation, IEditIndividualSimulationItemPresenter>,
      IEditIndividualSimulationPresenter
   {
      public EditIndividualSimulationPresenter(IEditIndividualSimulationView view, ISubPresenterItemManager<IEditIndividualSimulationItemPresenter> subPresenterItemManager, 
         ISimulationAnalysisPresenterFactory simulationAnalysisPresenterFactory, ISimulationAnalysisPresenterContextMenuFactory contextMenuFactory, 
         IPresentationSettingsTask presentationSettingsTask, ISimulationAnalysisCreator simulationAnalysisCreator)
         : base(view, subPresenterItemManager, EditIndividualSimulationItems.All, simulationAnalysisPresenterFactory, contextMenuFactory, presentationSettingsTask, simulationAnalysisCreator)
      {
      }

      protected override void UpdateCaption()
      {
         _view.Caption = PKSimConstants.UI.EditIndividualSimulation.FormatWith(Analyzable.Name);
         _view.ApplicationIcon = Analyzable.AllowAging ? ApplicationIcons.AgingSimulation : ApplicationIcons.Simulation;
      }

      public override string PresentationKey => PresenterConstants.PresenterKeys.EditAnalyzablePresenter;
   }
}