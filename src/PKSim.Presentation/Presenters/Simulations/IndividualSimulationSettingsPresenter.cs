using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface IIndividualSimulationSettingsPresenter : ISimulationOutputSelectionPresenter<IndividualSimulation>
   {
   }

   public class IndividualSimulationSettingsPresenter : SimulationOutputSelectionPresenter<IIndividualSimulationSettingsView, IIndividualSimulationSettingsPresenter, IndividualSimulation>, IIndividualSimulationSettingsPresenter
   {
      public IndividualSimulationSettingsPresenter(IIndividualSimulationSettingsView view, IQuantitySelectionPresenter quantitySelectionPresenter, ISimulationPersistableUpdater simulationPersistableUpdater, IPKSimProjectRetriever projectRetriever, IDialogCreator dialogCreator, IUserSettings userSettings)
         : base(view, quantitySelectionPresenter, simulationPersistableUpdater, projectRetriever, dialogCreator, userSettings)
      {
      }

      protected override void RefreshView()
      {
         base.RefreshView();
         int numberOfSelectedMolecules = _quantitySelectionPresenter.NumberOfSelectedQuantities;
         _quantitySelectionPresenter.Info = PKSimConstants.UI.NumberOfGeneratedCurves(numberOfSelectedMolecules);
      }
   }
}