using System;
using PKSim.Assets;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.Extensions;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Presenters;
using PKSim.Core.Services;
using ISimulationPersistableUpdater = PKSim.Core.Services.ISimulationPersistableUpdater;

namespace PKSim.Presentation.Presenters.Simulations
{
   public interface ISimulationOutputSelectionPresenter<TSimulation> : IDisposablePresenter where TSimulation : Simulation
   {
      /// <summary>
      ///    Starts the presenter and display the settings used to run the simulation population.
      ///    Returns the edited settings if the user confirms the action otherwise null
      /// </summary>
      /// <param name="simulation"> Simulation for which the setting should be edited </param>
      OutputSelections CreateSettings(TSimulation simulation);

      void SaveSettingsToProject();
      void SaveSettingsToUserSettings();
   }

   public abstract class SimulationOutputSelectionPresenter<TView, TPresenter, TSimulation> : AbstractDisposablePresenter<TView, TPresenter>, ISimulationOutputSelectionPresenter<TSimulation>
      where TView : ISimulationOutputSelectionView<TPresenter>
      where TPresenter : ISimulationOutputSelectionPresenter<TSimulation> where TSimulation : Simulation

   {
      protected readonly IQuantitySelectionPresenter _quantitySelectionPresenter;
      private readonly ISimulationPersistableUpdater _simulationPersistableUpdater;
      private readonly IPKSimProjectRetriever _projectRetriever;
      private readonly IDialogCreator _dialogCreator;
      private readonly ICoreUserSettings _userSettings;

      private OutputSelections _editedOutputSelections;
      protected TSimulation _simulation;

      protected SimulationOutputSelectionPresenter(TView view, IQuantitySelectionPresenter quantitySelectionPresenter,
         ISimulationPersistableUpdater simulationPersistableUpdater, IPKSimProjectRetriever projectRetriever, IDialogCreator dialogCreator, ICoreUserSettings userSettings)
         : base(view)
      {
         _quantitySelectionPresenter = quantitySelectionPresenter;
         _simulationPersistableUpdater = simulationPersistableUpdater;
         _projectRetriever = projectRetriever;
         _dialogCreator = dialogCreator;
         _userSettings = userSettings;
         _quantitySelectionPresenter.StatusChanged += quantitySelectionChanged;
         _view.AddSettingsView(_quantitySelectionPresenter.BaseView);
         _quantitySelectionPresenter.ExpandAllGroups = false;
      }

      protected OutputSelections DefaultSettingsFrom(Simulation simulation)
      {
         return simulation.OutputSelections.Clone();
      }

      public OutputSelections CreateSettings(TSimulation simulation)
      {
         _simulation = simulation;
         setupSelectionPresenter();
         _editedOutputSelections = DefaultSettingsFrom(simulation);
         _simulationPersistableUpdater.ResetPersistable(_simulation);
         _quantitySelectionPresenter.Edit(_simulation.Model.Root, _editedOutputSelections.AllOutputs);

         RefreshView();
         _view.Display();
         if (_view.Canceled)
            return null;

         updateSettingsFromSelection();
         return _editedOutputSelections;
      }

      private void updateSettingsFromSelection()
      {
         _editedOutputSelections.Clear();
         foreach (var quantitySelection in _quantitySelectionPresenter.SelectedQuantities())
         {
            _editedOutputSelections.AddOutput(quantitySelection);
         }
      }

      public virtual void SaveSettingsToProject()
      {
         saveSettings(x => { _projectRetriever.Current.OutputSelections = x; }, PKSimConstants.UI.SaveSimulationSettingsToProject);
      }

      public void SaveSettingsToUserSettings()
      {
         saveSettings(x => { _userSettings.OutputSelections = x; }, PKSimConstants.UI.SaveSimulationSettingsToUserSettings);
      }

      private void saveSettings(Action<OutputSelections> saveAction, string saveType)
      {
         updateSettingsFromSelection();
         saveAction(_editedOutputSelections.Clone());
         _dialogCreator.MessageBoxInfo(PKSimConstants.UI.SimulationSettingsSavedFor(saveType));
      }

      private void setupSelectionPresenter()
      {
         _quantitySelectionPresenter.Description = PKSimConstants.UI.SimulationSettingsDescription.FormatForDescription();
         _quantitySelectionPresenter.UpdateColumnSettings(_simulation);
      }

      protected override void Cleanup()
      {
         try
         {
            _quantitySelectionPresenter.StatusChanged -= quantitySelectionChanged;
         }
         finally
         {
            base.Cleanup();
         }
      }

      private void quantitySelectionChanged(object sender, EventArgs e)
      {
         RefreshView();
      }

      protected virtual void RefreshView()
      {
         View.OkEnabled = _quantitySelectionPresenter.HasSelection;
         View.ExtraEnabled = _quantitySelectionPresenter.HasSelection;
      }
   }
}