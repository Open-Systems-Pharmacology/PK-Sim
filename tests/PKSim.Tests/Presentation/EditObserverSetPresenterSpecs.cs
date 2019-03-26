using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using PKSim.Assets;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Observers;
using PKSim.Presentation.Views.Observers;

namespace PKSim.Presentation
{
   public abstract class concern_for_EditObserverSetPresenter : ContextSpecification<IEditObserverSetPresenter>
   {
      protected IEditObserverSetView _view;
      private ISubPresenterItemManager<IObserverSetItemPresenter> _subPresenterManager;
      protected ObserverSet _observerSet;
      protected IImportObserverSetPresenter _importObserverSetSettings;

      protected override void Context()
      {
         _view = A.Fake<IEditObserverSetView>();
         _subPresenterManager = SubPresenterHelper.Create<IObserverSetItemPresenter>();
         _observerSet = new ObserverSet().WithName("TOTO");
         sut = new EditObserverSetPresenter(_view, _subPresenterManager);
         _importObserverSetSettings = _subPresenterManager.CreateFake(ObserverSetItems.ImportSettings);
         _importObserverSetSettings.ShowFilePath = true;
         sut.Edit(_observerSet);
      }
   }

   public class When_edititing_an_observer_building_block : concern_for_EditObserverSetPresenter
   {
      [Observation]
      public void should_edit_all_sub_presenter_with_the_observer_building_block()
      {
         A.CallTo(() => _importObserverSetSettings.Edit(_observerSet)).MustHaveHappened();
      }

      [Observation]
      public void should_add_the_file_path_since_they_are_not_persisted()
      {
         _importObserverSetSettings.ShowFilePath.ShouldBeFalse();
      }

      [Observation]
      public void should_have_updated_the_caption_using_the_name_of_the_building_block()
      {
         _view.Caption.ShouldBeEqualTo(PKSimConstants.UI.EditObserverSet(_observerSet.Name));
      }

      [Observation]
      public void should_display_the_view()
      {
         A.CallTo(() => _view.Display()).MustHaveHappened();
      }
   }

   public class When_the_edit_observer_building_block_presenter_is_notified_that_an_observer_was_added_to_the_edited_building_block : concern_for_EditObserverSetPresenter
   {
      protected override void Because()
      {
         sut.Handle(new AddObserverToObserverSetEvent {Container = _observerSet});
      }

      [Observation]
      public void should_edit_the_sub_presenters_to_reflect_the_change()
      {
         A.CallTo(() => _importObserverSetSettings.Edit(_observerSet)).MustHaveHappened(2, Times.Exactly);
      }
   }

   public class When_the_edit_observer_building_block_presenter_is_notified_that_an_observer_was_removed_from_the_edited_building_block : concern_for_EditObserverSetPresenter
   {
      protected override void Because()
      {
         sut.Handle(new RemoveObserverFromObserverSetEvent() {Container = _observerSet});
      }

      [Observation]
      public void should_edit_the_sub_presenters_to_reflect_the_change()
      {
         A.CallTo(() => _importObserverSetSettings.Edit(_observerSet)).MustHaveHappened(2, Times.Exactly);
      }
   }

   public class When_the_edit_observer_building_block_presenter_is_notified_that_an_observer_was_added_to_another_building_block : concern_for_EditObserverSetPresenter
   {
      protected override void Because()
      {
         sut.Handle(new AddObserverToObserverSetEvent {Container = new ObserverSet()});
      }

      [Observation]
      public void should_edit_the_sub_presenters_to_reflect_the_change()
      {
         A.CallTo(() => _importObserverSetSettings.Edit(_observerSet)).MustHaveHappened(1, Times.Exactly);
      }
   }

   public class When_the_edit_observer_building_block_presenter_is_notified_that_an_observer_was_removed_from_another_building_block : concern_for_EditObserverSetPresenter
   {
      protected override void Because()
      {
         sut.Handle(new RemoveObserverFromObserverSetEvent {Container = new ObserverSet()});
      }

      [Observation]
      public void should_edit_the_sub_presenters_to_reflect_the_change()
      {
         A.CallTo(() => _importObserverSetSettings.Edit(_observerSet)).MustHaveHappened(1, Times.Exactly);
      }
   }
}