using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;
using PKSim.Presentation.Core;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation
{
   public abstract class concern_for_PopulationSimulationAnalysisStarter : ContextSpecification<IPopulationSimulationAnalysisStarter>
   {
      protected IApplicationController _applicationController;
      protected ICloner _cloner;

      protected override void Context()
      {
         _applicationController = A.Fake<IApplicationController>();
         _cloner = A.Fake<ICloner>();
         sut = new PopulationSimulationAnalysisStarter(_applicationController, _cloner);
      }
   }

   public class When_creating_a_time_profile_chart_for_a_population_simulation : concern_for_PopulationSimulationAnalysisStarter
   {
      private IPopulationDataCollector _populationDataCollector;
      private ICreateTimeProfileAnalysisPresenter _timeProfileAnalysePresenter;

      protected override void Context()
      {
         base.Context();
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         _timeProfileAnalysePresenter = A.Fake<ICreateTimeProfileAnalysisPresenter>();
         A.CallTo(() => _applicationController.Start<ICreateTimeProfileAnalysisPresenter>()).Returns(_timeProfileAnalysePresenter);
      }

      protected override void Because()
      {
         sut.CreateAnalysisForPopulationSimulation(_populationDataCollector, PopulationAnalysisType.TimeProfile);
      }

      [Observation]
      public void should_start_the_time_profile_analysis_presenter()
      {
         A.CallTo(() => _timeProfileAnalysePresenter.Create(_populationDataCollector)).MustHaveHappened();
      }
   }

   public class When_creating_a_box_whisker_analysis_for_a_population_simulation : concern_for_PopulationSimulationAnalysisStarter
   {
      private IPopulationDataCollector _populationDataCollector;
      private ISimulationAnalysis _createdChart;
      private ICreateBoxWhiskerAnalysisPresenter _boxWhiskerAnalysisPresenter;

      protected override void Context()
      {
         base.Context();
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         _boxWhiskerAnalysisPresenter = A.Fake<ICreateBoxWhiskerAnalysisPresenter>();
         A.CallTo(() => _applicationController.Start<ICreateBoxWhiskerAnalysisPresenter>()).Returns(_boxWhiskerAnalysisPresenter);
      }

      protected override void Because()
      {
         _createdChart = sut.CreateAnalysisForPopulationSimulation(_populationDataCollector, PopulationAnalysisType.BoxWhisker);
      }

      [Observation]
      public void should_start_the_box_whisker_analysis()
      {
         A.CallTo(() => _boxWhiskerAnalysisPresenter.Create(_populationDataCollector)).MustHaveHappened();
      }
   }

   public class When_editing_a_population_analysis_and_the_user_cancels_the_edit_action : concern_for_PopulationSimulationAnalysisStarter
   {
      private IPopulationDataCollector _populationDataCollector;
      private PopulationAnalysisChart _analysisToEdit;
      private PopulationAnalysisChart _editedAnalysis;
      private PopulationAnalysisChart _clonedAnalysis;
      private ICreateScatterAnalysisPresenter _scatterAnalysisPresenter;

      protected override void Context()
      {
         base.Context();
         _populationDataCollector = new PopulationSimulation();
         _clonedAnalysis = A.Fake<PopulationAnalysisChart>();
         _analysisToEdit = new ScatterAnalysisChart {PopulationAnalysis = new PopulationPivotAnalysis()};
         _populationDataCollector.AddAnalysis(_analysisToEdit);
         A.CallTo(() => _cloner.Clone(_analysisToEdit)).Returns(_clonedAnalysis);
         _scatterAnalysisPresenter = A.Fake<ICreateScatterAnalysisPresenter>();
         A.CallTo(() => _applicationController.Start<ICreateScatterAnalysisPresenter>()).Returns(_scatterAnalysisPresenter);
         A.CallTo(() => _scatterAnalysisPresenter.Edit(_populationDataCollector, _clonedAnalysis)).Returns(false);
      }

      protected override void Because()
      {
         _editedAnalysis = sut.EditAnalysisForPopulationSimulation(_populationDataCollector, _analysisToEdit);
      }

      [Observation]
      public void should_edit_a_clone_of_the_source_analysis()
      {
         A.CallTo(() => _scatterAnalysisPresenter.Edit(_populationDataCollector, _clonedAnalysis)).MustHaveHappened();
      }

      [Observation]
      public void should_not_change_the_edited_analysis()
      {
         _populationDataCollector.Analyses.ShouldContain(_analysisToEdit);
      }

      [Observation]
      public void should_return_the_edited_analysis()
      {
         _editedAnalysis.ShouldBeEqualTo(_analysisToEdit);
      }
   }

   public class When_editing_a_population_analysis_and_the_user_confirmes_the_edit_action : concern_for_PopulationSimulationAnalysisStarter
   {
      private IPopulationDataCollector _populationDataCollector;
      private PopulationAnalysisChart _analysisToEdit;
      private PopulationAnalysisChart _editedAnalysis;
      private PopulationAnalysisChart _clonedAnalysis;
      private ICreateScatterAnalysisPresenter _scatterAnalysisPresenter;

      protected override void Context()
      {
         base.Context();
         _populationDataCollector =new  PopulationSimulation();
         _clonedAnalysis = A.Fake<PopulationAnalysisChart>();
         _analysisToEdit = new ScatterAnalysisChart();
         _populationDataCollector.AddAnalysis(_analysisToEdit);
         A.CallTo(() => _cloner.Clone(_analysisToEdit)).Returns(_clonedAnalysis);
         _scatterAnalysisPresenter = A.Fake<ICreateScatterAnalysisPresenter>();
         A.CallTo(() => _applicationController.Start<ICreateScatterAnalysisPresenter>()).Returns(_scatterAnalysisPresenter);
         A.CallTo(() => _scatterAnalysisPresenter.Edit(_populationDataCollector, _clonedAnalysis)).Returns(true);
      }

      protected override void Because()
      {
         _editedAnalysis = sut.EditAnalysisForPopulationSimulation(_populationDataCollector, _analysisToEdit);
      }

      [Observation]
      public void should_edit_a_clone_of_the_source_analysis()
      {
         A.CallTo(() => _scatterAnalysisPresenter.Edit(_populationDataCollector, _clonedAnalysis)).MustHaveHappened();
      }

      [Observation]
      public void should_have_removed_the_edited_analysis_in_favor_of_the_new_analysis()
      {
         _populationDataCollector.Analyses.ShouldOnlyContain(_editedAnalysis);
      }

      [Observation]
      public void should_return_the_edited_analysis_as_clone_of_the_first_analysis()
      {
         _editedAnalysis.ShouldBeEqualTo(_clonedAnalysis);
      }
   }
}