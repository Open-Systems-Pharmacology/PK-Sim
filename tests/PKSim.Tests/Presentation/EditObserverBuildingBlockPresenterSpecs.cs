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
   public abstract class concern_for_EditObserverBuildingBlockPresenter : ContextSpecification<IEditObserverBuildingBlockPresenter>
   {
      protected IEditObserverBuildingBlockView _view;
      private ISubPresenterItemManager<IObserverItemPresenter> _subPresenterManager;
      protected PKSimObserverBuildingBlock _observerBuildingBlock;
      protected IImportObserversPresenter _importObserverSettings;

      protected override void Context()
      {
         _view = A.Fake<IEditObserverBuildingBlockView>();
         _subPresenterManager = SubPresenterHelper.Create<IObserverItemPresenter>();
         _observerBuildingBlock = new PKSimObserverBuildingBlock().WithName("TOTO");
         sut = new EditObserverBuildingBlockPresenter(_view, _subPresenterManager);
         _importObserverSettings = _subPresenterManager.CreateFake(ObserverItems.ImportSettings);
         _importObserverSettings.ShowFilePath = true;
         sut.Edit(_observerBuildingBlock);
      }
   }

   public class When_edititing_an_observer_building_block : concern_for_EditObserverBuildingBlockPresenter
   {
      [Observation]
      public void should_edit_all_sub_presenter_with_the_observer_building_block()
      {
         A.CallTo(() => _importObserverSettings.Edit(_observerBuildingBlock)).MustHaveHappened();
      }

      [Observation]
      public void should_add_the_file_path_since_they_are_not_persisted()
      {
         _importObserverSettings.ShowFilePath.ShouldBeFalse();
      }

      [Observation]
      public void should_have_updated_the_caption_using_the_name_of_the_building_block()
      {
         _view.Caption.ShouldBeEqualTo(PKSimConstants.UI.EditObserverBuildingBlock(_observerBuildingBlock.Name));
      }

      [Observation]
      public void should_display_the_view()
      {
         A.CallTo(() => _view.Display()).MustHaveHappened();
      }
   }

   public class When_the_edit_observer_building_block_presenter_is_notified_that_an_observer_was_added_to_the_edited_building_block : concern_for_EditObserverBuildingBlockPresenter
   {
      protected override void Because()
      {
         sut.Handle(new AddObserverToObserverBuildingBlockEvent {Container = _observerBuildingBlock});
      }

      [Observation]
      public void should_edit_the_sub_presenters_to_reflect_the_change()
      {
         A.CallTo(() => _importObserverSettings.Edit(_observerBuildingBlock)).MustHaveHappened(2, Times.Exactly);
      }
   }

   public class When_the_edit_observer_building_block_presenter_is_notified_that_an_observer_was_removed_from_the_edited_building_block : concern_for_EditObserverBuildingBlockPresenter
   {
      protected override void Because()
      {
         sut.Handle(new RemoveObserverFromObserverBuildingBlockEvent() {Container = _observerBuildingBlock});
      }

      [Observation]
      public void should_edit_the_sub_presenters_to_reflect_the_change()
      {
         A.CallTo(() => _importObserverSettings.Edit(_observerBuildingBlock)).MustHaveHappened(2, Times.Exactly);
      }
   }

   public class When_the_edit_observer_building_block_presenter_is_notified_that_an_observer_was_added_to_another_building_block : concern_for_EditObserverBuildingBlockPresenter
   {
      protected override void Because()
      {
         sut.Handle(new AddObserverToObserverBuildingBlockEvent {Container = new PKSimObserverBuildingBlock()});
      }

      [Observation]
      public void should_edit_the_sub_presenters_to_reflect_the_change()
      {
         A.CallTo(() => _importObserverSettings.Edit(_observerBuildingBlock)).MustHaveHappened(1, Times.Exactly);
      }
   }

   public class When_the_edit_observer_building_block_presenter_is_notified_that_an_observer_was_removed_from_another_building_block : concern_for_EditObserverBuildingBlockPresenter
   {
      protected override void Because()
      {
         sut.Handle(new RemoveObserverFromObserverBuildingBlockEvent {Container = new PKSimObserverBuildingBlock()});
      }

      [Observation]
      public void should_edit_the_sub_presenters_to_reflect_the_change()
      {
         A.CallTo(() => _importObserverSettings.Edit(_observerBuildingBlock)).MustHaveHappened(1, Times.Exactly);
      }
   }
}