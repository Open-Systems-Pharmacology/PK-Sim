using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;


namespace PKSim.Presentation
{
   public abstract class concern_for_BoxWhiskerFieldSelectionPresenter : ContextSpecification<IBoxWhiskerFieldSelectionPresenter>
   {
      private IPopulationAnalysisFieldSelectionView _view;
      protected IBoxWhiskerNumericFieldsPresenter _numericFieldPresenter;
      protected IPopulationAnalysisFieldsArrangementPresenter _fieldsArrangementPresenter;

      protected override void Context()
      {
         _view = A.Fake<IPopulationAnalysisFieldSelectionView>();
         _numericFieldPresenter = A.Fake<IBoxWhiskerNumericFieldsPresenter>();
         _fieldsArrangementPresenter = A.Fake<IPopulationAnalysisFieldsArrangementPresenter>();
         sut = new BoxWhiskerFieldSelectionPresenter(_view, _fieldsArrangementPresenter, _numericFieldPresenter);
      }
   }

   public class When_initializing_the_box_whisker_field_selection_presenter : concern_for_BoxWhiskerFieldSelectionPresenter
   {
      [Observation]
      public void should_change_the_caption_of_the_row_area_to_reflect_the_box_whisker_logic()
      {
         A.CallTo(() => _fieldsArrangementPresenter.UpdateDescription(PivotArea.RowArea, PKSimConstants.UI.XGrouping)).MustHaveHappened();
      }
    
   }

   public class When_starting_the_analysis_in_a_box_whisker_selection_presenter : concern_for_BoxWhiskerFieldSelectionPresenter
   {
      private IPopulationDataCollector _populationDataCollector;
      private PopulationPivotAnalysis _populationAnalysis;

      protected override void Context()
      {
         base.Context();
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         _populationAnalysis = A.Fake<PopulationPivotAnalysis>();
      }

      protected override void Because()
      {
         sut.StartAnalysis(_populationDataCollector, _populationAnalysis);
      }

      [Observation]
      public void should_also_start_the_analysis_in_the_the_arrangement_presenter_and_numeric_field_presenters()
      {
         A.CallTo(() => _fieldsArrangementPresenter.StartAnalysis(_populationDataCollector, _populationAnalysis)).MustHaveHappened();
         A.CallTo(() => _numericFieldPresenter.StartAnalysis(_populationDataCollector, _populationAnalysis)).MustHaveHappened();
      }
   }

   public class When_refreshing_the_analysis_in_a_box_whisker_selection_presenter : concern_for_BoxWhiskerFieldSelectionPresenter
   {
      protected override void Because()
      {
         sut.RefreshAnalysis();
      }

      [Observation]
      public void should_also_refresh_the_analysis_in_the_the_arrangement_presenter_and_numeric_field_presenters()
      {
         A.CallTo(() => _fieldsArrangementPresenter.RefreshAnalysis()).MustHaveHappened();
         A.CallTo(() => _numericFieldPresenter.RefreshAnalysis()).MustHaveHappened();
      }
   }
}