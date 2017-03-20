using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Events;
using FakeItEasy;
using PKSim.Core.Services;
using PKSim.Presentation.Services;
using PKSim.Presentation.UICommands;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Events;
using OSPSuite.Core.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_RenameObjectUICommand : ContextSpecification<RenameObjectUICommand>
   {
      protected IEventPublisher _eventPublisher;
      protected IEntityTask _entityTask;
      protected IWithName _anObject;

      protected override void Context()
      {
         _entityTask = A.Fake<IEntityTask>();
         _eventPublisher = A.Fake<IEventPublisher>();
         _anObject = A.Fake<IWithName>();
         
         sut = new RenameObjectUICommand(_entityTask, _eventPublisher) {Subject = _anObject};
      }
   }

   public class When_renaming_an_object_and_the_entity_task_returns_a_name : concern_for_RenameObjectUICommand
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _entityTask.NewNameFor(_anObject, A<IEnumerable<string>>.That.IsEmpty(), A<string>._)).Returns("aname");
      }

      protected override void Because()
      {
         sut.Execute();
      }

      [Observation]
      public void the_object_should_be_renamed()
      {
         _anObject.Name.ShouldBeEqualTo("aname");
      }

      [Observation]
      public void when_a_new_name_is_not_returned_the_command_should_publish_a_renamed_event()
      {
         A.CallTo(() => _eventPublisher.PublishEvent(A<RenamedEvent>._)).MustHaveHappened();
      }

      [Observation]
      public void the_entity_task_should_be_used_to_create_a_unique_name_with_no_forbidden_names()
      {
         A.CallTo(() => _entityTask.NewNameFor(_anObject, A<IEnumerable<string>>.That.IsEmpty(), A<string>._)).MustHaveHappened();
      }
   }

   public class When_renaming_an_object_and_the_entity_task_returns_null : concern_for_RenameObjectUICommand
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _entityTask.NewNameFor(_anObject, A<IEnumerable<string>>.That.IsEmpty(), A<string>._)).Returns(null);
      }

      protected override void Because()
      {
         sut.Execute();
      }

      [Observation]
      public void when_a_new_name_is_not_returned_the_command_should_not_publish_a_renamed_event()
      {
         A.CallTo(() => _eventPublisher.PublishEvent(A<RenamedEvent>._)).MustNotHaveHappened();
      }

      [Observation]
      public void the_entity_task_should_be_used_to_create_a_unique_name_with_no_forbidden_names()
      {
        A.CallTo(() => _entityTask.NewNameFor(_anObject, A<IEnumerable<string>>.That.IsEmpty(), A<string>._)).MustHaveHappened();
      }
   }
}
