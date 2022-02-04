using System.Collections.Generic;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ISimulationWizardPresenter : IWizardPresenter
   {
      void ModelConfigurationDone();
      Simulation Simulation { get; }

      /// <summary>
      ///    Creates the <see cref="Simulation" />  for the given workflow and close the view in case of success
      /// </summary>
      void CreateSimulation();
   }

   public abstract class SimulationWizardPresenter<TView> : PKSimWizardPresenter<TView, ISimulationWizardPresenter, ISimulationItemPresenter>, ISimulationWizardPresenter
      where TView : ISimulationWizardView
   {
      protected readonly ISimulationModelCreator _simulationModelCreator;
      protected readonly IHeavyWorkManager _heavyWorkManager;

      protected SimulationWizardPresenter(TView view, ISubPresenterItemManager<ISimulationItemPresenter> subPresenterItemManager,
         ISimulationModelCreator simulationModelCreator, IHeavyWorkManager heavyWorkManager, IDialogCreator dialogCreator)
         : base(view, subPresenterItemManager, SimulationItems.All, dialogCreator)
      {
         _simulationModelCreator = simulationModelCreator;
         _heavyWorkManager = heavyWorkManager;
      }

      public abstract void ModelConfigurationDone();
      protected abstract string HeavyWorkCaption { get; }

      public override void WizardCurrent(int previousIndex, int newIndex)
      {
         if (previousIndex == SimulationItems.Model.Index)
            ModelConfigurationDone();

         base.WizardCurrent(previousIndex, newIndex);
      }

      public virtual Simulation Simulation => PresenterAt(SimulationItems.Model).Simulation;

      protected override void UpdateControls(int indexThatWillHaveFocus)
      {
         UpdateViewStatus();

         //we are on the subject selection. Next only enable if the model /subject selection ok
         if (indexThatWillHaveFocus == SimulationItems.Model.Index)
            _view.NextEnabled = PresenterAt(SimulationItems.Model).CanClose;

         //we are on the compound selection. Next only enable if the compound selection ok
         else if (indexThatWillHaveFocus == SimulationItems.Compounds.Index)
            _view.NextEnabled = PresenterAt(SimulationItems.Compounds).CanClose;

         else
            _view.NextEnabled = true;

         var simulationCreated = PresenterAt(SimulationItems.Model).SimulationCreated;
         AllSimulationItemsAfterModel.Each(x => _view.SetControlEnabled(x, simulationCreated));

         _view.OkEnabled = CanClose && simulationCreated;
      }

      protected abstract void UpdateSimulationProperties();

      public virtual void CreateSimulation()
      {
         var success = _heavyWorkManager.Start(UpdateSimulationProperties, HeavyWorkCaption);
         if (success)
            _view.CloseView();
      }

      protected override bool HasData()
      {
         return PresenterAt(SimulationItems.Model).SimulationCreated;
      }

      protected IEnumerable<ISubPresenterItem<ISimulationItemPresenter>> AllSimulationItemsAfterModel => new ISubPresenterItem<ISimulationItemPresenter>[]
      {
         SimulationItems.Compounds, SimulationItems.CompoundsProcesses, SimulationItems.CompoundProtocols, SimulationItems.Events, SimulationItems.Observers
      };

      /// <summary>
      ///    Save the configuration defined in all SimulationItem presenters
      /// </summary>
      protected void SaveBuildingBlocksConfiguration()
      {
         //Simulation was already created once, we can save the existing configuration
         if (Simulation == null) return;
         AllSimulationItemsAfterModel.Each(x => PresenterAt(x).SaveConfiguration());
      }
   }
}