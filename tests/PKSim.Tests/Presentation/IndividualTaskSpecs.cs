using OSPSuite.BDDHelper;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Individuals;
using PKSim.Presentation.Services;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation
{
   public abstract class concern_for_IndividualTask : ContextSpecification<IIndividualTask>
   {
      protected ICreateIndividualPresenter _createIndividualPresenter;
      protected IScaleIndividualPresenter _scaleIndividualPresenter;
      protected Individual _individual;
      protected IExecutionContext _executionContext;
      protected IBuildingBlockTask _buildingBlockTask;
      protected IApplicationController _applicationController;

      protected override void Context()
      {
         _createIndividualPresenter = A.Fake<ICreateIndividualPresenter>();
         _buildingBlockTask = A.Fake<IBuildingBlockTask>();
         _scaleIndividualPresenter = A.Fake<IScaleIndividualPresenter>();
         _executionContext = A.Fake<IExecutionContext>();
         _applicationController = A.Fake<IApplicationController>();
         A.CallTo(() => _executionContext.CurrentProject).Returns(A.Fake<PKSimProject>());
         _individual = A.Fake<Individual>();
         A.CallTo(() => _applicationController.Start<ICreateIndividualPresenter>()).Returns(_createIndividualPresenter);
         A.CallTo(() => _applicationController.Start<IScaleIndividualPresenter>()).Returns(_scaleIndividualPresenter);
         A.CallTo(() => _createIndividualPresenter.BuildingBlock).Returns(_individual);
         A.CallTo(() => _buildingBlockTask.TypeFor(_individual)).Returns("Individual");
         sut = new IndividualTask(_executionContext, _buildingBlockTask, _applicationController);
      }
   }

   public class When_creating_an_individual : concern_for_IndividualTask
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _createIndividualPresenter.Create()).Returns(new PKSimEmptyCommand());
      }

      protected override void Because()
      {
         sut.AddToProject();
      }

      [Observation]
      public void should_initialize_the_individual_presenter()
      {
         A.CallTo(() => _applicationController.Start<ICreateIndividualPresenter>()).MustHaveHappened();
      }

      [Observation]
      public void should_leverage_the_presenter_to_display_the_create_individual_form()
      {
         A.CallTo(() => _createIndividualPresenter.Create()).MustHaveHappened();
      }
   }

   public class When_the_user_cancels_the_action_of_creating_an_individual : concern_for_IndividualTask
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _createIndividualPresenter.Create()).Returns(new PKSimEmptyCommand());
      }

      protected override void Because()
      {
         sut.AddToProject();
      }

      [Observation]
      public void should_not_add_anything_to_the_history()
      {
         A.CallTo(() => _buildingBlockTask.AddCommandToHistory(A<IPKSimCommand>.Ignored)).MustNotHaveHappened();
      }
   }

   public class When_the_individual_creation_was_successfull : concern_for_IndividualTask
   {
      private IPKSimCommand _createIndividualCommand;

      protected override void Context()
      {
         base.Context();
         _createIndividualCommand = A.Fake<IPKSimCommand>();
         A.CallTo(() => _createIndividualPresenter.Create()).Returns(_createIndividualCommand);
         A.CallTo(() => _createIndividualPresenter.BuildingBlock).Returns(_individual);
      }

      protected override void Because()
      {
         sut.AddToProject();
      }

      [Observation]
      public void should_add_a_command_to_the_history()
      {
         A.CallTo(() => _buildingBlockTask.AddToProject(_individual, true, true)).MustHaveHappened();
      }
   }

   public class When_editing_an_individual : concern_for_IndividualTask
   {
      protected override void Because()
      {
         sut.Edit(_individual);
      }

      [Observation]
      public void should_leverage_the_presenter_to_edit_the_individual()
      {
         A.CallTo(() => _buildingBlockTask.Edit(_individual)).MustHaveHappened();
      }
   }

   public class When_scaling_an_indiviual : concern_for_IndividualTask
   {
      protected override void Because()
      {
         sut.ScaleIndividual(_individual);
      }

      [Observation]
      public void should_initialize_the_scale_individual_presenter()
      {
         A.CallTo(() => _applicationController.Start<IScaleIndividualPresenter>()).MustHaveHappened();
      }

      [Observation]
      public void should_leverage_the_presenter_to_display_the_scale_individual_form()
      {
         A.CallTo(() => _scaleIndividualPresenter.ScaleIndividual(_individual)).MustHaveHappened();
      }

      [Observation]
      public void should_load_the_individual()
      {
         A.CallTo(() => _buildingBlockTask.Load(_individual)).MustHaveHappened();
      }
   }

   public class When_the_user_cancels_the_action_of_scaling_an_individual : concern_for_IndividualTask
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _scaleIndividualPresenter.ScaleIndividual(_individual)).Returns(new PKSimEmptyCommand());
      }

      protected override void Because()
      {
         sut.ScaleIndividual(_individual);
      }

      [Observation]
      public void should_not_add_anything_to_the_history()
      {
         A.CallTo(() => _buildingBlockTask.AddCommandToHistory(A<IPKSimCommand>.Ignored)).MustNotHaveHappened();
      }
   }
}