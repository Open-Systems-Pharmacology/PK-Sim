using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Journal;
using OSPSuite.Presentation.Presenters.Journal;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Events;
using PKSim.Presentation.Core;

namespace PKSim.Presentation
{
   public abstract class concern_for_JournalPageEditorActivator : ContextSpecification<IJournalPageEditorActivator>
   {
      protected IContainer _container;
      protected IEventPublisher _eventPublisher;
      protected EditJournalPageStartedEvent _event;

      protected override void Context()
      {
         _container = A.Fake<IContainer>();
         _eventPublisher = A.Fake<IEventPublisher>();
         _event = new EditJournalPageStartedEvent(new JournalPage(), showEditor: true);
         sut = new JournalPageEditorActivator(_container, _eventPublisher);
      }
   }

   public class When_the_journal_page_editor_activator_handles_the_first_edit_journal_page_started_event : concern_for_JournalPageEditorActivator
   {
      protected override void Because()
      {
         sut.Handle(_event);
      }

      [Observation]
      public void should_create_the_journal_page_editor_form_presenter()
      {
         A.CallTo(() => _container.Resolve<IJournalPageEditorFormPresenter>()).MustHaveHappenedOnceExactly();
      }

      [Observation]
      public void should_republish_the_event_so_that_the_newly_subscribed_presenters_can_handle_it()
      {
         A.CallTo(() => _eventPublisher.PublishEvent(_event)).MustHaveHappenedOnceExactly();
      }

      [Observation]
      public void should_unsubscribe_itself_from_the_event_publisher()
      {
         A.CallTo(() => _eventPublisher.RemoveListener(sut)).MustHaveHappened();
      }
   }

   public class When_the_journal_page_editor_activator_handles_a_subsequent_edit_journal_page_started_event : concern_for_JournalPageEditorActivator
   {
      protected override void Context()
      {
         base.Context();
         sut.Handle(_event);
         Fake.ClearRecordedCalls(_container);
         Fake.ClearRecordedCalls(_eventPublisher);
      }

      protected override void Because()
      {
         sut.Handle(new EditJournalPageStartedEvent(new JournalPage(), showEditor: false));
      }

      [Observation]
      public void should_not_create_the_editor_form_presenter_again()
      {
         A.CallTo(() => _container.Resolve<IJournalPageEditorFormPresenter>()).MustNotHaveHappened();
      }

      [Observation]
      public void should_not_republish_the_event()
      {
         A.CallTo(() => _eventPublisher.PublishEvent(A<EditJournalPageStartedEvent>._)).MustNotHaveHappened();
      }
   }
}
