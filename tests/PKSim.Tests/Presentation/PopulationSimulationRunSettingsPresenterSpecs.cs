using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using FakeItEasy;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;

using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;
using PKSim.Core.Services;
using ISimulationPersistableUpdater = PKSim.Core.Services.ISimulationPersistableUpdater;

namespace PKSim.Presentation
{
   public abstract class concern_for_PopulationSimulationRunSettingsPresenter : ContextSpecification<IPopulationSimulationSettingsPresenter>
   {
      protected IPopulationSimulationSettingsView _view;
      protected List<IObserver> _observers;
      protected PopulationSimulation _populationSimulation;
      protected OutputSelections _originalSettings;
      protected OutputSelections _editedSettings;
      protected List<QuantitySelection> _selectedQuantities;
      protected IQuantitySelectionPresenter _quantitySelectionPresenter;
      protected ISimulationPersistableUpdater _simulationPersistableUpdater;
      protected IPKSimProjectRetriever _projectRetriever;
      protected IDialogCreator _dialogCreator;
      private IUserSettings _userSettings;

      protected override void Context()
      {
         _view = A.Fake<IPopulationSimulationSettingsView>();
         _quantitySelectionPresenter = A.Fake<IQuantitySelectionPresenter>();
         _simulationPersistableUpdater = A.Fake<ISimulationPersistableUpdater>();
         _projectRetriever = A.Fake<IPKSimProjectRetriever>();
         _dialogCreator = A.Fake<IDialogCreator>();
         _userSettings = A.Fake<IUserSettings>();

         _populationSimulation = A.Fake<PopulationSimulation>();
         A.CallTo(() => _populationSimulation.NumberOfItems).Returns(10);
         _populationSimulation.Model = new Model {Root = new Container()};
         _originalSettings = A.Fake<OutputSelections>();
         _editedSettings = A.Fake<OutputSelections>();
         A.CallTo(() => _originalSettings.Clone()).Returns(_editedSettings);
         A.CallTo(() => _populationSimulation.OutputSelections).Returns(_originalSettings);
         _selectedQuantities = new List<QuantitySelection>();
         A.CallTo(() => _quantitySelectionPresenter.SelectedQuantities()).Returns(_selectedQuantities);
         sut = new PopulationSimulationSettingsPresenter(_view, _quantitySelectionPresenter, _simulationPersistableUpdater, _projectRetriever, _dialogCreator, _userSettings);
      }
   }

   public class When_creating_the_settings_for_a_simulation : concern_for_PopulationSimulationRunSettingsPresenter
   {
      protected override void Because()
      {
         sut.CreateSettings(_populationSimulation);
      }

      [Observation]
      public void should_edit_a_clone_of_the_default_settigns()
      {
         A.CallTo(() => _quantitySelectionPresenter.Edit(_populationSimulation.Model.Root, _editedSettings.AllOutputs)).MustHaveHappened();
      }

      [Observation]
      public void should_reset_the_default_persitable_data()
      {
         A.CallTo(() => _simulationPersistableUpdater.ResetPersistable(_populationSimulation)).MustHaveHappened();
      }

      [Observation]
      public void should_not_expand_the_grouping_nodes()
      {
         _quantitySelectionPresenter.ExpandAllGroups.ShouldBeFalse();
      }
   }

   public class When_the_user_cancels_the_action_of_editing_the_settings_ : concern_for_PopulationSimulationRunSettingsPresenter
   {
      protected override void Because()
      {
         A.CallTo(() => _view.Canceled).Returns(true);
      }

      [Observation]
      public void the_create_settings_should_return_false()
      {
         sut.CreateSettings(_populationSimulation).ShouldBeNull();
      }
   }

   public class When_the_user_has_edited_the_settings_ : concern_for_PopulationSimulationRunSettingsPresenter
   {
      protected override void Because()
      {
         base.Because();
         A.CallTo(() => _view.Canceled).Returns(false);
      }

      [Observation]
      public void the_create_settings_should_return_true()
      {
         sut.CreateSettings(_populationSimulation).ShouldNotBeNull();
      }
   }

   public class When_the_population_settings_presenter_is_being_notifed_that_the_selection_has_changed : concern_for_PopulationSimulationRunSettingsPresenter
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _quantitySelectionPresenter.NumberOfSelectedQuantities).Returns(2);
         A.CallTo(() => _quantitySelectionPresenter.HasSelection).Returns(true);
         sut.CreateSettings(_populationSimulation);
      }

      protected override void Because()
      {
         _quantitySelectionPresenter.StatusChanged += Raise.WithEmpty();
      }

      [Observation]
      public void it_should_update_the_view_with_the_number_of_curves_that_will_be_generated()
      {
         _quantitySelectionPresenter.Info.ShouldBeEqualTo(PKSimConstants.UI.NumberOfGeneratedCurves(2 * 10));
      }

      [Observation]
      public void it_should_enable_the_ok_button()
      {
         _view.OkEnabled.ShouldBeEqualTo(true);
      }
   }

   public class When_the_user_is_saving_the_current_edited_settings_to_the_project : concern_for_PopulationSimulationRunSettingsPresenter
   {
      private PKSimProject _project;
      private OutputSelections _templateSettings;

      protected override void Context()
      {
         base.Context();
         _project = A.Fake<PKSimProject>();
         _templateSettings = A.Fake<OutputSelections>();
         A.CallTo(() => _editedSettings.Clone()).Returns(_templateSettings);

         sut.CreateSettings(_populationSimulation);
         A.CallTo(() => _projectRetriever.Current).Returns(_project);
      }

      protected override void Because()
      {
         sut.SaveSettingsToProject();
      }

      [Observation]
      public void should_have_added_a_clone_of_the_edited_settings_to_the_project()
      {
         _project.OutputSelections.ShouldBeEqualTo(_templateSettings);
      }

      [Observation]
      public void should_display_a_message_to_the_user_informing_that_saving_was_successful()
      {
         A.CallTo(() => _dialogCreator.MessageBoxInfo(PKSimConstants.UI.SimulationSettingsSavedFor(PKSimConstants.UI.SaveSimulationSettingsToProject))).MustHaveHappened();
      }
   }
}