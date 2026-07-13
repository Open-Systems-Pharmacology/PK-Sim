using OSPSuite.Core.Journal;
using OSPSuite.Presentation.Presenters.Journal;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Events;

namespace PKSim.Presentation.Core
{
   public interface IJournalPageEditorActivator : IListener<EditJournalPageStartedEvent>
   {
   }

   /// <summary>
   ///    Creates the journal page editor on first use because it constructs expensive DevExpress components.
   /// </summary>
   public class JournalPageEditorActivator : IJournalPageEditorActivator
   {
      private readonly IContainer _container;
      private readonly IEventPublisher _eventPublisher;
      private bool _activated;

      public JournalPageEditorActivator(IContainer container, IEventPublisher eventPublisher)
      {
         _container = container;
         _eventPublisher = eventPublisher;
      }

      public void Handle(EditJournalPageStartedEvent eventToHandle)
      {
         if (_activated) return;

         _container.Resolve<IJournalPageEditorFormPresenter>();
         //set only after a successful resolve (so a failed resolve retries on the next edit), but before
         //re-publishing so the re-dispatched event does not re-enter this activator
         _activated = true;
         //re-publish because the presenters created above were not yet subscribed when this event was published
         _eventPublisher.PublishEvent(eventToHandle);
         _eventPublisher.RemoveListener(this);
      }
   }
}
