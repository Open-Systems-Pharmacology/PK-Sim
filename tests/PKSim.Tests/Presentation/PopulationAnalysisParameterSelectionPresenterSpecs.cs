using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Presenters.Populations;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters.Nodes;

namespace PKSim.Presentation
{
   public abstract class concern_for_PopulationAnalysisParameterSelectionPresenter : ContextSpecification<IPopulationAnalysisParameterSelectionPresenter>
   {
      private IPopulationAnalysisParameterSelectionView _view;
      protected IPopulationParameterGroupsPresenter _allPopulationParametersPresenter;
      protected IPopulationAnalysisParameterFieldsPresenter _selectedParametersPresenter;
      protected IPopulationDataCollector _populationDataCollector;
      protected IEntityPathResolver _entityPathResolver;
      protected IPopulationAnalysisFieldDistributionPresenter _parameterDistributionPresenter;
      protected PopulationPivotAnalysis _populationPivotAnalysis;

      protected override void Context()
      {
         _view = A.Fake<IPopulationAnalysisParameterSelectionView>();
         _allPopulationParametersPresenter = A.Fake<IPopulationParameterGroupsPresenter>();
         _selectedParametersPresenter = A.Fake<IPopulationAnalysisParameterFieldsPresenter>();
         _parameterDistributionPresenter = A.Fake<IPopulationAnalysisFieldDistributionPresenter>();
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         _entityPathResolver = A.Fake<IEntityPathResolver>();
         _populationPivotAnalysis = A.Fake<PopulationPivotAnalysis>();


         sut = new PopulationAnalysisParameterSelectionPresenter(_view, _allPopulationParametersPresenter, _selectedParametersPresenter, _entityPathResolver, _parameterDistributionPresenter);
      }
   }

   public class When_starting_the_selection_presenter_for_a_given_population_simulation : concern_for_PopulationAnalysisParameterSelectionPresenter
   {
      private IParameter _p1;
      private IParameter _p2;
      private IParameter _p3NotVisible;
      protected IEnumerable<IParameter> _allParameters;

      protected override void Context()
      {
         base.Context();
         _p1 = A.Fake<IParameter>();
         _p1.Visible = true;
         _p2 = A.Fake<IParameter>();
         _p2.Visible = true;
         _p3NotVisible = A.Fake<IParameter>();
         _p3NotVisible.Visible = false;

         A.CallTo(() => _populationDataCollector.AllVectorialParameters(_entityPathResolver)).Returns(new List<IParameter> {_p1, _p2, _p3NotVisible});

         A.CallTo(() => _allPopulationParametersPresenter.AddParamtersAndCovariates(A<IEnumerable<IParameter>>._, A<IReadOnlyList<string>>._, A<bool>._))
            .Invokes(x => _allParameters = x.GetArgument<IEnumerable<IParameter>>(0));
      }

      protected override void Because()
      {
         sut.StartAnalysis(_populationDataCollector, _populationPivotAnalysis);
      }

      [Observation]
      public void should_add_all_the_visible_population_parameters_from_the_simulation()
      {
         _allParameters.ShouldOnlyContain(_p1, _p2);
      }
   }

   public class When_notified_that_a_parameter_was_selected : concern_for_PopulationAnalysisParameterSelectionPresenter
   {
      private IParameter _parameter;
      private PopulationAnalysisParameterField _field;

      protected override void Context()
      {
         base.Context();
         _parameter = A.Fake<IParameter>();
         _field = A.Fake<PopulationAnalysisParameterField>();
         sut.StartAnalysis(_populationDataCollector, _populationPivotAnalysis);
      }

      protected override void Because()
      {
         _selectedParametersPresenter.ParameterFieldSelected += Raise.With(new ParameterFieldSelectedEventArgs(_parameter, _field));
      }

      [Observation]
      public void should_update_the_distribution_for_the_selected_parameter()
      {
         A.CallTo(() => _parameterDistributionPresenter.Plot(_populationDataCollector, _parameter, _field)).MustHaveHappened();
      }
   }

   public class When_notified_that_a_covariate_was_selected : concern_for_PopulationAnalysisParameterSelectionPresenter
   {
      private string _covariate;
      private PopulationAnalysisCovariateField _populationAnalysisCovariateField;

      protected override void Context()
      {
         base.Context();
         _covariate = "Covariate";
         sut.StartAnalysis(_populationDataCollector, _populationPivotAnalysis);
      }

      protected override void Because()
      {
         _populationAnalysisCovariateField = new PopulationAnalysisCovariateField();
         _selectedParametersPresenter.CovariateFieldSelected += Raise.With(new CovariateFieldSelectedEventArgs(_covariate, _populationAnalysisCovariateField));
      }

      [Observation]
      public void should_update_the_distribution_for_the_selected_covariate()
      {
         A.CallTo(() => _parameterDistributionPresenter.Plot(_populationDataCollector, _populationAnalysisCovariateField)).MustHaveHappened();
      }
   }

   public class When_notified_that_a_covariate_was_double_clicked : concern_for_PopulationAnalysisParameterSelectionPresenter
   {
      private string _covariate;
      private CovariateNode _covariateNode;

      protected override void Context()
      {
         base.Context();
         _covariate = "Covariate";
         _covariateNode = new CovariateNode(_covariate);
         sut.StartAnalysis(_populationDataCollector, _populationPivotAnalysis);
      }

      protected override void Because()
      {
         _allPopulationParametersPresenter.CovariateNodeDoubleClicked += Raise.With(new CovariateNodeSelectedEventArgs(_covariateNode));
      }

      [Observation]
      public void should_add_the_covariate_to_the_analyse()
      {
         A.CallTo(() => _selectedParametersPresenter.AddCovariate(_covariate)).MustHaveHappened();
      }
   }

   public class When_notified_that_a_parameter_was_double_clicked : concern_for_PopulationAnalysisParameterSelectionPresenter
   {
      private ITreeNode<IParameter> _parameterNode;
      private IParameter _parameter;

      protected override void Context()
      {
         base.Context();
         _parameter = A.Fake<IParameter>();
         _parameterNode = new ObjectWithIdAndNameNode<IParameter>(_parameter);
         sut.StartAnalysis(_populationDataCollector, _populationPivotAnalysis);
      }

      protected override void Because()
      {
         _allPopulationParametersPresenter.ParameterNodeDoubleClicked += Raise.With(new ParameterNodeSelectedEventArgs(_parameterNode));
      }

      [Observation]
      public void should_add_the_parameter_to_the_analyse()
      {
         A.CallTo(() => _selectedParametersPresenter.AddParameter(_parameter)).MustHaveHappened();
      }
   }

   public class When_notified_that_a_population_derived_field_was_selected : concern_for_PopulationAnalysisParameterSelectionPresenter
   {
      private PopulationAnalysisDerivedField _derivedField;

      protected override void Context()
      {
         base.Context();
         _derivedField = A.Fake<PopulationAnalysisDerivedField>().WithName("Toto");
         sut.StartAnalysis(_populationDataCollector, _populationPivotAnalysis);
      }

      protected override void Because()
      {
         _selectedParametersPresenter.DerivedFieldSelected += Raise.With(new DerivedFieldSelectedEventArgs(_derivedField));
      }

      [Observation]
      public void should_update_the_distribution_for_the_selected_derived_field()
      {
         A.CallTo(() => _parameterDistributionPresenter.Plot(_populationDataCollector, _derivedField, _populationPivotAnalysis)).MustHaveHappened();
      }
   }

   public class When_notified_that_a_parameter_unit_was_changed : concern_for_PopulationAnalysisParameterSelectionPresenter
   {
      private PopulationAnalysisParameterField _parameterField;
      private IParameter _parameter;

      protected override void Context()
      {
         base.Context();
         _parameterField = A.Fake<PopulationAnalysisParameterField>();
         _parameter = A.Fake<IParameter>();
         A.CallTo(() => _selectedParametersPresenter.SelectedParameter).Returns(_parameter);
         sut.StartAnalysis(_populationDataCollector, _populationPivotAnalysis);
      }

      protected override void Because()
      {
         sut.Handle(new FieldUnitChangedInPopulationAnalysisEvent(_populationPivotAnalysis, _parameterField));
      }

      [Observation]
      public void should_redraw_the_distribution()
      {
         A.CallTo(() => _parameterDistributionPresenter.Plot(_populationDataCollector, _parameter, _parameterField)).MustHaveHappened();
      }
   }

   public class When_notified_that_a_parameter_name_was_changed : concern_for_PopulationAnalysisParameterSelectionPresenter
   {
      private PopulationAnalysisParameterField _parameterField;
      private IParameter _parameter;

      protected override void Context()
      {
         base.Context();
         _parameterField = A.Fake<PopulationAnalysisParameterField>();
         _parameter = A.Fake<IParameter>();
         A.CallTo(() => _selectedParametersPresenter.SelectedParameter).Returns(_parameter);
         sut.StartAnalysis(_populationDataCollector, _populationPivotAnalysis);
      }

      protected override void Because()
      {
         sut.Handle(new FieldRenamedInPopulationAnalysisEvent(_populationPivotAnalysis, _parameterField));
      }

      [Observation]
      public void should_redraw_the_distribution()
      {
         A.CallTo(() => _parameterDistributionPresenter.Plot(_populationDataCollector, _parameter, _parameterField)).MustHaveHappened();
      }
   }

   public class When_notified_that_a_covariate_name_was_changed : concern_for_PopulationAnalysisParameterSelectionPresenter
   {
      private PopulationAnalysisCovariateField _covariateField;

      protected override void Context()
      {
         base.Context();
         _covariateField = A.Fake<PopulationAnalysisCovariateField>();
         sut.StartAnalysis(_populationDataCollector, _populationPivotAnalysis);
      }

      protected override void Because()
      {
         sut.Handle(new FieldRenamedInPopulationAnalysisEvent(_populationPivotAnalysis, _covariateField));
      }

      [Observation]
      public void should_redraw_the_distribution()
      {
         A.CallTo(() => _parameterDistributionPresenter.Plot(_populationDataCollector, _covariateField)).MustHaveHappened();
      }
   }

   public class When_notified_that_a_covariate_name_was_changed_for_another_analysis : concern_for_PopulationAnalysisParameterSelectionPresenter
   {
      private PopulationAnalysisCovariateField _covariateField;

      protected override void Context()
      {
         base.Context();
         _covariateField = A.Fake<PopulationAnalysisCovariateField>();
         sut.StartAnalysis(_populationDataCollector, _populationPivotAnalysis);
      }

      protected override void Because()
      {
         sut.Handle(new FieldRenamedInPopulationAnalysisEvent(A.Fake<PopulationAnalysis>(), _covariateField));
      }

      [Observation]
      public void should_not_redraw_the_distribution()
      {
         A.CallTo(() => _parameterDistributionPresenter.Plot(_populationDataCollector, _covariateField)).MustNotHaveHappened();
      }
   }

   public class When_notified_that_a_derived_field_name_was_changed : concern_for_PopulationAnalysisParameterSelectionPresenter
   {
      private PopulationAnalysisDerivedField _derivedField;

      protected override void Context()
      {
         base.Context();
         _derivedField = A.Fake<PopulationAnalysisDerivedField>();
         sut.StartAnalysis(_populationDataCollector, _populationPivotAnalysis);
      }

      protected override void Because()
      {
         sut.Handle(new FieldRenamedInPopulationAnalysisEvent(_populationPivotAnalysis, _derivedField));
      }

      [Observation]
      public void should_redraw_the_distribution()
      {
         A.CallTo(() => _parameterDistributionPresenter.Plot(_populationDataCollector, _derivedField, _populationPivotAnalysis)).MustHaveHappened();
      }
   }
}