using System;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationSubjectTask : ContextSpecification<ISimulationSubjectTask>
   {
      protected IIndividualTask _individualTask;
      protected IApplicationController _applicationController;
      protected IPopulationTask _populationTask;
      protected ISimulationSubjectSelectionPresenter _presenter;
      protected IBuildingBlockTask _buildingBlockTask;

      protected override void Context()
      {
         _individualTask = A.Fake<IIndividualTask>();
         _applicationController = A.Fake<IApplicationController>();
         _populationTask = A.Fake<IPopulationTask>();
         _presenter = A.Fake<ISimulationSubjectSelectionPresenter>();
         _buildingBlockTask = A.Fake<IBuildingBlockTask>();
         A.CallTo(() => _applicationController.Start<ISimulationSubjectSelectionPresenter>()).Returns(_presenter);
         sut = new SimulationSubjectTask(_individualTask, _populationTask, _applicationController, _buildingBlockTask);
      }
   }

   public class When_the_simulation_subject_task_is_editing_a_subject : concern_for_SimulationSubjectTask
   {
      [Observation]
      public void should_throw_an_exception_since_the_method_should_never_be_called_direclty()
      {
         The.Action(() => sut.Edit(A.Fake<ISimulationSubject>())).ShouldThrowAn<NotSupportedException>();
      }
   }

   public class When_the_simulation_subject_task_is_told_to_add_a_simulation_subject_to_a_project_and_the_user_decides_to_add_an_individual : concern_for_SimulationSubjectTask
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _presenter.ChooseSimulationSubject()).Returns(true);
         A.CallTo(() => _presenter.SimulationSubjetType).Returns(typeof(Individual));
      }

      protected override void Because()
      {
         sut.AddToProject();
      }

      [Observation]
      public void should_add_an_individual_to_the_project()
      {
         A.CallTo(() => _individualTask.AddToProject()).MustHaveHappened();
      }
   }

   public class When_the_simulation_subject_task_is_told_to_add_a_simulation_subject_to_a_project_and_the_user_decides_to_add_a_population : concern_for_SimulationSubjectTask
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _presenter.ChooseSimulationSubject()).Returns(true);
         A.CallTo(() => _presenter.SimulationSubjetType).Returns(typeof(Population));
      }

      protected override void Because()
      {
         sut.AddToProject();
      }

      [Observation]
      public void should_add_an_individual_to_the_project()
      {
         A.CallTo(() => _populationTask.AddToProject()).MustHaveHappened();
      }
   }

   public class When_the_simulation_subject_task_is_told_to_add_a_simulation_subject_to_a_project_and_the_user_cancels_the_action : concern_for_SimulationSubjectTask
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _presenter.ChooseSimulationSubject()).Returns(false);
      }

      protected override void Because()
      {
         sut.AddToProject();
      }

      [Observation]
      public void should_neither_add_a_population_nor_an_individual()
      {
         A.CallTo(() => _populationTask.AddToProject()).MustNotHaveHappened();
         A.CallTo(() => _individualTask.AddToProject()).MustNotHaveHappened();
      }
   }

   public class When_told_to_load_a_building_block_from_template : concern_for_SimulationSubjectTask
   {
      protected override void Because()
      {
         sut.LoadSingleFromTemplateAsync();
      }

      [Observation]
      public void should_load_the_available_individual_and_population_from_the_database()
      {
         A.CallTo(() => _buildingBlockTask.LoadSingleFromTemplateAsync<ISimulationSubject>(PKSimBuildingBlockType.Individual | PKSimBuildingBlockType.Population)).MustHaveHappened();
      }
   }
}