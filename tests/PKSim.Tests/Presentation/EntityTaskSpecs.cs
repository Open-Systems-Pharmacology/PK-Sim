using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Services;
using PKSim.Presentation.Services;
using FakeItEasy;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation
{
   public abstract class concern_for_EntityTask : ContextSpecification<IEntityTask>
   {
      protected IApplicationController _applicationController;
      protected IExecutionContext _executionContext;
      protected IEntity _entity;
      protected IRenameObjectPresenter _renamePresenter;

      protected override void Context()
      {
         _applicationController = A.Fake<IApplicationController>();
         _executionContext = A.Fake<IExecutionContext>();
         _entity = A.Fake<IEntity>();
         _renamePresenter = A.Fake<IRenameObjectPresenter>();
         A.CallTo(() => _applicationController.Start<IRenameObjectPresenter>()).Returns(_renamePresenter);
         sut = new EntityTask(_applicationController, _executionContext);
      }
   }

   
   public class When_renaming_an_entity : concern_for_EntityTask
   {
      protected override void Because()
      {
         sut.Rename(_entity);
      }

      [Observation]
      public void should_retrieve_the_rename_presenter_and_start_it()
      {
         A.CallTo(() => _renamePresenter.Edit(_entity)).MustHaveHappened();
      }
   }

   
   public class When_the_rename_operation_was_canceled_by_the_user : concern_for_EntityTask
   {
      private ICommand _command;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _renamePresenter.Edit(_entity)).Returns(false);
      }

      protected override void Because()
      {
         _command = sut.Rename(_entity);
      }

      [Observation]
      public void should_return_an_empty_command()
      {
         _command.ShouldBeAnInstanceOf<PKSimEmptyCommand>();
      }
   }

   
   public class When_the_rename_operation_was_confirmed_by_the_user : concern_for_EntityTask
   {
      private ICommand _command;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _renamePresenter.Edit(_entity)).Returns(true);
      }

      protected override void Because()
      {
         _command = sut.Rename(_entity);
      }

      [Observation]
      public void should_return_a_rename_entity_command()
      {
         _command.ShouldBeAnInstanceOf<RenameEntityCommand>();
      }
   }
}