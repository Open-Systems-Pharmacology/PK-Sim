using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Presenters;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.ExpressionProfiles;
using PKSim.Presentation.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_EntityTask : ContextSpecification<IEntityTask>
   {
      protected IApplicationController _applicationController;
      protected IExecutionContext _executionContext;
      protected IEntity _entity;
      protected IRenameObjectPresenter _renamePresenter;
      protected IRenameObjectDTOFactory _renameObjectFactory;
      protected RenameObjectDTO _renameDTO;

      protected override void Context()
      {
         _applicationController = A.Fake<IApplicationController>();
         _executionContext = A.Fake<IExecutionContext>();
         _entity = A.Fake<IEntity>();
         _renameObjectFactory = A.Fake<IRenameObjectDTOFactory>();
         _renamePresenter = A.Fake<IRenameObjectPresenter>();
         A.CallTo(() => _applicationController.Start<IRenameObjectPresenter>()).Returns(_renamePresenter);
         sut = new EntityTask(_applicationController, _executionContext, _renameObjectFactory);

         _renameDTO = new RenameObjectDTO(_entity.Name);
         _renameDTO.AddUsedNames(new[] {"A", "B"});
         A.CallTo(() => _renameObjectFactory.CreateFor(_entity)).Returns(_renameDTO);
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
         A.CallTo(() => _renamePresenter.NewNameFrom(_entity, _renameDTO.UsedNames, _renameDTO.ContainerType)).MustHaveHappened();
      }
   }

   public class When_the_rename_operation_was_canceled_by_the_user : concern_for_EntityTask
   {
      private ICommand _command;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _renamePresenter.NewNameFrom(_entity, A<IEnumerable<string>>._, A<string>._)).Returns("");
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
         A.CallTo(() => _renamePresenter.NewNameFrom(_entity, A<IEnumerable<string>>._, A<string>._)).Returns("NEW_NAME");
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

   public class When_starting_the_rename_expression_profile_action : concern_for_EntityTask
   {
      private IRenameExpressionProfilePresenter _renameExpressionProfilePresenter;
      private IEntity _expressionProfile;

      protected override void Context()
      {
         base.Context();

         _renameExpressionProfilePresenter = A.Fake<IRenameExpressionProfilePresenter>();
         A.CallTo(() => _applicationController.Start<IRenameExpressionProfilePresenter>()).Returns(_renameExpressionProfilePresenter);
         _expressionProfile = new ExpressionProfile();
         A.CallTo(() => _renameObjectFactory.CreateFor(_expressionProfile)).Returns(_renameDTO);
      }

      protected override void Because()
      {
         sut.Rename(_expressionProfile);
      }

      [Observation]
      public void should_use_the_rename_expression_profile_presenter_to_perform_the_rename()
      {
         A.CallTo(() => _renameExpressionProfilePresenter.NewNameFrom(_expressionProfile, _renameDTO.UsedNames, _renameDTO.ContainerType)).MustHaveHappened();
      }
   }
}