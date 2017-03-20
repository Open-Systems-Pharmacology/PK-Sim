using FakeItEasy;
using OSPSuite.BDDHelper;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;

using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Presentation
{
   public abstract class concern_for_PopulationAnalysisPKParameterSelectionPresenter : ContextSpecification<IPopulationAnalysisPKParameterSelectionPresenter>
   {
      protected IPopulationAnalysisPKParameterSelectionView _view;
      protected IPopulationAnalysisAvailablePKParametersPresenter _allPKParametersPresenter;
      protected IPopulationAnalysisPKParameterFieldsPresenter _selectedPKParameterFieldsPresenter;
      protected IPopulationAnalysisFieldDistributionPresenter _fieldDistributionPresenter;

      protected override void Context()
      {
         _view= A.Fake<IPopulationAnalysisPKParameterSelectionView>();
         _allPKParametersPresenter= A.Fake<IPopulationAnalysisAvailablePKParametersPresenter>();
         _selectedPKParameterFieldsPresenter= A.Fake<IPopulationAnalysisPKParameterFieldsPresenter>();
         _fieldDistributionPresenter= A.Fake<IPopulationAnalysisFieldDistributionPresenter>();
         sut = new PopulationAnalysisPKParameterSelectionPresenter(_view,_allPKParametersPresenter,_selectedPKParameterFieldsPresenter,_fieldDistributionPresenter);
      }
   }

   public class When_starting_the_pk_parameter_selection_for_a_given_population_simulation : concern_for_PopulationAnalysisPKParameterSelectionPresenter
   {
      private PopulationPivotAnalysis _populationPivotAnalysis;
      private IPopulationDataCollector _populationDataCollector;

      protected override void Context()
      {
         base.Context();
         _populationPivotAnalysis = new PopulationPivotAnalysis();
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
      }

      protected override void Because()
      {
         sut.StartAnalysis(_populationDataCollector,_populationPivotAnalysis);
      }

      [Observation]
      public void should_display_the_list_of_all_available_pk_parameters()
      {
         A.CallTo(() => _allPKParametersPresenter.InitializeWith(_populationDataCollector)).MustHaveHappened();
      }

      [Observation]
      public void should_display_the_list_of_all_available_parameters_fields()
      {
         A.CallTo(() => _selectedPKParameterFieldsPresenter.StartAnalysis(_populationDataCollector,_populationPivotAnalysis)).MustHaveHappened();
      }

      [Observation]
      public void should_set_the_sub_presenter_views_in_the_main_view()
      {
         A.CallTo(() => _view.AddAllPKParametersView(_allPKParametersPresenter.View)).MustHaveHappened();
         A.CallTo(() => _view.AddSelectedPKParametersView(_selectedPKParameterFieldsPresenter.BaseView)).MustHaveHappened();
      }
   }

   public class When_notified_that_a_pk_parameter_was_selected : concern_for_PopulationAnalysisPKParameterSelectionPresenter
   {
      private QuantityPKParameter _pkParameter;
      private IPopulationDataCollector _populationDataCollector;
      private PopulationPivotAnalysis _populationPivotAnalysis;
      private PopulationAnalysisPKParameterField _field;

      protected override void Context()
      {
         base.Context();
         _pkParameter = new QuantityPKParameter();
         _populationPivotAnalysis = new PopulationPivotAnalysis();
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         _field = A.Fake<PopulationAnalysisPKParameterField>();
         sut.StartAnalysis(_populationDataCollector, _populationPivotAnalysis);
      }

      protected override void Because()
      {
         _selectedPKParameterFieldsPresenter.PKParameterSelected += Raise.With(new PKParameterFieldSelectedEventArgs(_pkParameter,_field));
      }

      [Observation]
      public void should_draw_the_distribution_for_the_selected_parameter()
      {
         A.CallTo(() => _fieldDistributionPresenter.Plot(_populationDataCollector, _pkParameter, _field)).MustHaveHappened();
      }
   }

   public class When_notified_that_a_population_derived_field_for_pk_parameter_was_selected : concern_for_PopulationAnalysisPKParameterSelectionPresenter
   {
      private PopulationAnalysisDerivedField _derivedField;
      private PopulationAnalysis _populationPivotAnalysis;
      private IPopulationDataCollector _populationDataCollector;

      protected override void Context()
      {
         base.Context();
         _populationPivotAnalysis = new PopulationPivotAnalysis();
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         _derivedField = A.Fake<PopulationAnalysisDerivedField>().WithName("Toto");
         sut.StartAnalysis(_populationDataCollector, _populationPivotAnalysis);
      }

      protected override void Because()
      {
         _selectedPKParameterFieldsPresenter.DerivedFieldSelected += Raise.With(new DerivedFieldSelectedEventArgs(_derivedField));
      }

      [Observation]
      public void should_update_the_distribution_for_the_selected_derived_field()
      {
         A.CallTo(() => _fieldDistributionPresenter.Plot(_populationDataCollector, _derivedField, _populationPivotAnalysis)).MustHaveHappened();
      }
   }
}	