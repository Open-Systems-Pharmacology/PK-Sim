using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;

namespace PKSim.Presentation
{
   public abstract class concern_for_TimeProfileFieldSelectionPresenter : ContextSpecification<ITimeProfileFieldSelectionPresenter>
   {
      private IPopulationAnalysisFieldSelectionView _view;
      protected IMultipleNumericFieldsPresenter _numericFieldPresenter;
      protected IPopulationAnalysisFieldsArrangementPresenter _fieldsArrangementPresenter;

      protected override void Context()
      {
         _view = A.Fake<IPopulationAnalysisFieldSelectionView>();
         _numericFieldPresenter = A.Fake<IMultipleNumericFieldsPresenter>();
         _fieldsArrangementPresenter = A.Fake<IPopulationAnalysisFieldsArrangementPresenter>();
         sut = new TimeProfileFieldSelectionPresenter(_view, _fieldsArrangementPresenter, _numericFieldPresenter);
      }
   }

   public class When_initializing_the_time_profile_field_selection_presenter : concern_for_TimeProfileFieldSelectionPresenter
   {
      [Observation]
      public void should_initialize_the_numeric_field_presenter_to_use_output_fields_only()
      {
         _numericFieldPresenter.AllowedType.ShouldBeEqualTo(typeof (PopulationAnalysisOutputField));
      }
   }
}