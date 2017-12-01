using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Events;
using FakeItEasy;
using PKSim.Core.Events;
using PKSim.Core.Model;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Events;

namespace PKSim.Core
{
   public abstract class concern_for_ProjectChangedNotifier : ContextSpecification<IProjectChangedNotifier>
   {
      protected IProjectRetriever _workspace;
      protected IEventPublisher _eventPublisher;

      protected override void Context()
      {
         _workspace = A.Fake<IProjectRetriever>();
         _eventPublisher = A.Fake<IEventPublisher>();
         sut = new ProjectChangedNotifier(_workspace, _eventPublisher);
      }
   }

   public class When_the_project_changed_notifier_is_told_to_notify_a_change : concern_for_ProjectChangedNotifier
   {
      protected PKSimProject _project;

      protected override void Context()
      {
         base.Context();
         _project = A.Fake<PKSimProject>();
         A.CallTo(() => _workspace.CurrentProject).Returns(_project);
      }

      protected override void Because()
      {
         sut.Changed();
      }

      [Observation]
      public void the_project_changed_should_be_set_to_true()
      {
         _project.HasChanged.ShouldBeTrue();
      }

      [Observation]
      public void should_notify_to_all_listened_that_the_project_has_changed()
      {
         A.CallTo(() => _eventPublisher.PublishEvent(A<ProjectChangedEvent>.Ignored)).MustHaveHappened();
      }
   }

   public class When_the_project_changed_notifier_is_told_to_notify_a_change_for_a_non_existing_project : concern_for_ProjectChangedNotifier
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _workspace.CurrentProject).Returns(null);
      }

      protected override void Because()
      {
         sut.Changed();
      }

      [Observation]
      public void should_not_notify_anything()
      {
         A.CallTo(() => _eventPublisher.PublishEvent(A<ProjectChangedEvent>.Ignored)).MustNotHaveHappened();
      }
   }

   public class When_notifying_an_object_change_for_a_building_block : concern_for_ProjectChangedNotifier
   {
      private Individual _individual;

      protected override void Context()
      {
         base.Context();
         _individual= A.Fake<Individual>();
      }

      protected override void Because()
      {
         sut.NotifyChangedFor(_individual);
      }

      [Observation]
      public void should_notify_the_project_changed()
      {
         A.CallTo(() => _eventPublisher.PublishEvent(A<ProjectChangedEvent>.Ignored)).MustHaveHappened();
      }

      [Observation]
      public void should_set_the_has_changed_flag_to_true()
      {
         _individual.HasChanged.ShouldBeTrue();
      }
   }
}