using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.Presentation.Core;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_PopulationTask : ContextSpecification<IPopulationTask>
   {
      protected ICreateRandomPopulationPresenter _randomPopulationPresenter;
      protected IBuildingBlockTask _buildingBlockTask;
      protected IExecutionContext _executionContext;
      protected RandomPopulation _population;
      protected PKSimProject _project;
      protected IApplicationController _applicationController;

      protected override void Context()
      {
         _buildingBlockTask = A.Fake<IBuildingBlockTask>();
         _executionContext = A.Fake<IExecutionContext>();
         _project = A.Fake<PKSimProject>();
         _population = A.Fake<RandomPopulation>();
         _randomPopulationPresenter = A.Fake<ICreateRandomPopulationPresenter>();
         _applicationController = A.Fake<IApplicationController>();
         A.CallTo(() => _applicationController.Start<ICreateRandomPopulationPresenter>()).Returns(_randomPopulationPresenter);
         A.CallTo(() => _randomPopulationPresenter.BuildingBlock).Returns(_population);
         A.CallTo(() => _buildingBlockTask.TypeFor(_population)).Returns("pop");
         A.CallTo(() => _executionContext.CurrentProject).Returns(_project);
         sut = new PopulationTask(_executionContext, _buildingBlockTask, _applicationController);
      }
   }

   public class When_asked_to_add_a_population_to_the_current_project : concern_for_PopulationTask
   {
      protected override void Because()
      {
         sut.AddToProject();
      }

      [Observation]
      public void should_start_the_create_population_presenter_and_allow_the_user_to_define_the_population_parameters()
      {
         A.CallTo(() => _randomPopulationPresenter.Create()).MustHaveHappened();
      }

      [Observation]
      public void should_release_the_presenter_when_the_use_case_is_over()
      {
         A.CallTo(() => _randomPopulationPresenter.Dispose()).MustHaveHappened();
      }
   }

   public class When_the_user_cancels_the_creation_of_the_population : concern_for_PopulationTask
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _randomPopulationPresenter.Create()).Returns(new PKSimEmptyCommand());
      }

      protected override void Because()
      {
         sut.AddToProject();
      }

      [Observation]
      public void should_not_add_any_command_to_the_history()
      {
         A.CallTo(() => _buildingBlockTask.AddCommandToHistory(A<IPKSimCommand>.Ignored)).MustNotHaveHappened();
      }
   }

   public class When_the_user_confirms_the_population_creation : concern_for_PopulationTask
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _randomPopulationPresenter.Create()).Returns(A.Fake<IPKSimCommand>());
      }

      protected override void Because()
      {
         sut.AddToProject();
      }

      [Observation]
      public void should_add_the_created_population_to_the_project()
      {
         A.CallTo(() => _buildingBlockTask.AddToProject((Population) _population, true, true)).MustHaveHappened();
      }
   }

   public class When_the_population_task_is_editing_a_random_population : concern_for_PopulationTask
   {
      private RandomPopulation _randomPop;

      protected override void Context()
      {
         base.Context();
         _randomPop = A.Fake<RandomPopulation>();
      }

      protected override void Because()
      {
         sut.Edit(_randomPop);
      }

      [Observation]
      public void should_start_the_edit_random_population_presenter()
      {
         A.CallTo(() => _buildingBlockTask.Edit(_randomPop)).MustHaveHappened();
      }
   }


   public class When_the_population_task_is_extracting_individuals : concern_for_PopulationTask
   {
      private List<int> _individualIds;
      private IExtractIndividualsFromPopulationPresenter _extractIndividualsPresenter;

      protected override void Context()
      {
         base.Context();
         _individualIds = new List<int>{1,2,3};
         _extractIndividualsPresenter= A.Fake<IExtractIndividualsFromPopulationPresenter>();
         A.CallTo(() => _applicationController.Start<IExtractIndividualsFromPopulationPresenter>()).Returns(_extractIndividualsPresenter);
      }

      protected override void Because()
      {
         sut.ExtractIndividuals(_population, _individualIds);
      }

      [Observation]
      public void should_start_the_extract_individuals_presenter()
      {
         A.CallTo(() => _extractIndividualsPresenter.ExctractIndividuals(_population, _individualIds)).MustHaveHappened();
      }
   }
}