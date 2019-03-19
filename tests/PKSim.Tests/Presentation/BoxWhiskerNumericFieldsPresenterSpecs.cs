using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Events;
using FakeItEasy;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;


namespace PKSim.Presentation
{
   public abstract class concern_for_BoxWhiskerNumericFieldsPresenter : ContextSpecification<IBoxWhiskerNumericFieldsPresenter>
   {
      protected IMultipleNumericFieldsPresenter _numericFieldPresenter;
      protected IBoxWhiskerNumericFieldsView _view;
      protected IPopulationDataCollector _populationDataCollector;
      protected PopulationBoxWhiskerAnalysis _boxWhiskerAnalysis;
      protected IEventPublisher _eventPublisher;

      protected override void Context()
      {
         _numericFieldPresenter = A.Fake<IMultipleNumericFieldsPresenter>();
         _view = A.Fake<IBoxWhiskerNumericFieldsView>();
         _eventPublisher = A.Fake<IEventPublisher>();
         sut = new BoxWhiskerNumericFieldsPresenter(_view, _numericFieldPresenter, _eventPublisher);

         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         _boxWhiskerAnalysis = new PopulationBoxWhiskerAnalysis();
      }
   }

   public class When_initializing_the_box_whisker_numeric_field_selection_presenter : concern_for_BoxWhiskerNumericFieldsPresenter
   {
      [Observation]
      public void should_initialize_the_numeric_field_presenter_to_use_numeric_fields_only()
      {
         _numericFieldPresenter.AllowedType.ShouldBeEqualTo(typeof (PopulationAnalysisNumericField));
      }
   }

   public class When_the_user_changes_the_visibility_status_of_the_outliers : concern_for_BoxWhiskerNumericFieldsPresenter
   {
      private BoxWhiskerNumericFieldDTO _dto;
      private PopulationAnalysisDataSelectionChangedEvent _event;

      protected override void Context()
      {
         base.Context();

         A.CallTo(() => _view.BindTo(A<BoxWhiskerNumericFieldDTO>._))
            .Invokes(x => _dto = x.GetArgument<BoxWhiskerNumericFieldDTO>(0));

         A.CallTo(() => _eventPublisher.PublishEvent(A<PopulationAnalysisDataSelectionChangedEvent>._))
            .Invokes(x => _event = x.GetArgument<PopulationAnalysisDataSelectionChangedEvent>(0));

         _boxWhiskerAnalysis.ShowOutliers = false;
         sut.StartAnalysis(_populationDataCollector, _boxWhiskerAnalysis);
      }

      protected override void Because()
      {
         _dto.ShowOutliers = true;
         sut.ShowOutliersChanged();
      }

      [Observation]
      public void should_notify_a_data_selection_changed()
      {
         _event.ShouldNotBeNull();
         _event.PopulationAnalysis.ShouldBeEqualTo(_boxWhiskerAnalysis);
      }

      [Observation]
      public void should_update_the_value_in_the_underlying_box_whisker_analysis()
      {
         _boxWhiskerAnalysis.ShowOutliers.ShouldBeTrue();
      }
   }

   public class When_editing_a_box_whisker_analysis_showing_the_outliers : concern_for_BoxWhiskerNumericFieldsPresenter
   {
      private BoxWhiskerNumericFieldDTO _dto;

      protected override void Context()
      {
         base.Context();

         A.CallTo(() => _view.BindTo(A<BoxWhiskerNumericFieldDTO>._))
            .Invokes(x => _dto = x.GetArgument<BoxWhiskerNumericFieldDTO>(0));


         _boxWhiskerAnalysis.ShowOutliers = true;
         sut.StartAnalysis(_populationDataCollector, _boxWhiskerAnalysis);
      }

      [Observation]
      public void should_set_the_expected_value_in_the_view ()
      {
         _dto.ShowOutliers.ShouldBeTrue();
      }
   }
}