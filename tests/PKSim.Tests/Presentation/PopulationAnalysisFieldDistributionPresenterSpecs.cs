using System.Collections.Generic;
using System.Data;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Presenters.Populations;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Extensions;

namespace PKSim.Presentation
{
   public abstract class concern_for_PopulationAnalysisFieldDistributionPresenter : ContextSpecification<IPopulationAnalysisFieldDistributionPresenter>
   {
      protected IPopulationDistributionPresenter _populationDistributionPresenter;
      protected IPopulationAnalysisFlatTableCreator _flatTableCreator;
      protected IPopulationAnalysisFieldFactory _populationAnalysisFieldFactory;
      protected IDimensionRepository _dimensionRepository;
      protected IPopulationDataCollector _populationDataCollector;
      protected IDisplayUnitRetriever _displayUnitRetriever;

      protected override void Context()
      {
         _populationDistributionPresenter = A.Fake<IPopulationDistributionPresenter>();
         _flatTableCreator = A.Fake<IPopulationAnalysisFlatTableCreator>();
         _populationAnalysisFieldFactory = A.Fake<IPopulationAnalysisFieldFactory>();
         _dimensionRepository = A.Fake<IDimensionRepository>();
         _displayUnitRetriever = A.Fake<IDisplayUnitRetriever>();
         sut = new PopulationAnalysisFieldDistributionPresenter(_populationDistributionPresenter, _flatTableCreator, _populationAnalysisFieldFactory, _dimensionRepository, _displayUnitRetriever);

         _populationDataCollector = A.Fake<IPopulationDataCollector>();
      }
   }

   public class When_drwaing_the_distribution_for_a_derived_field : concern_for_PopulationAnalysisFieldDistributionPresenter
   {
      private PopulationAnalysisDerivedField _derivedField;
      private DataTable _dataTable;
      private PopulationAnalysis _populationAnalysis;

      protected override void Context()
      {
         base.Context();
         _populationAnalysis = A.Fake<PopulationAnalysis>();
         _derivedField = A.Fake<PopulationAnalysisDerivedField>().WithName("Toto");
         _dataTable = new DataTable();
         _dataTable.AddColumn<string>(_derivedField.Name);
         A.CallTo(_flatTableCreator).WithReturnType<DataTable>().Returns(_dataTable);
      }

      protected override void Because()
      {
         sut.Plot(_populationDataCollector, _derivedField, _populationAnalysis);
      }

      [Observation]
      public void should_retrieve_the_derived_data_for_the_derived_field()
      {
         A.CallTo(() => _flatTableCreator.Create(_populationDataCollector, A<IReadOnlyCollection<IPopulationAnalysisField>>._)).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_distribution_for_the_selected_parameter_using_the_ordering_definedin_the_derived_field()
      {
         A.CallTo(() => _populationDistributionPresenter.Plot(_populationDataCollector, A<IReadOnlyList<string>>._, _derivedField.Name, _derivedField, A<DistributionSettings>._)).MustHaveHappened();
      }
   }

   public class When_drowing_a_distribution_for_a_covariate_field_name : concern_for_PopulationAnalysisFieldDistributionPresenter
   {
      private DataTable _dataTable;
      private string _covriateName;
      private PopulationAnalysisCovariateField _covariateField;

      protected override void Context()
      {
         base.Context();
         _covriateName = "Gender";
         _covariateField = new PopulationAnalysisCovariateField().WithName(_covriateName);
         A.CallTo(() => _populationAnalysisFieldFactory.CreateFor(_covriateName, _populationDataCollector)).Returns(_covariateField);
         _dataTable = new DataTable();
         _dataTable.AddColumn<string>(_covriateName);
         A.CallTo(_flatTableCreator).WithReturnType<DataTable>().Returns(_dataTable);
      }

      protected override void Because()
      {
         sut.Plot(_populationDataCollector, _covriateName);
      }

      [Observation]
      public void should_create_a_coviariate_field_on_the_fly()
      {
         A.CallTo(() => _populationAnalysisFieldFactory.CreateFor(_covriateName, _populationDataCollector)).MustHaveHappened();
      }

      [Observation]
      public void should_update_the_distribution_for_the_selected_covariate_using_the_ordering_defined_in_the_covariate_field()
      {
         A.CallTo(() => _populationDistributionPresenter.Plot(_populationDataCollector, A<IReadOnlyList<string>>._, _covriateName, _covariateField, A<DistributionSettings>._)).MustHaveHappened();
      }
   }

   public class When_drawing_the_distribution_for_a_pk_parameter : concern_for_PopulationAnalysisFieldDistributionPresenter
   {
      private QuantityPKParameter _pkParameter;
      private PopulationAnalysisPKParameterField _pkParameterField;
      private NumericFieldContext _numericFieldContect;
      private IDimension _mergedDimension;
      private IDimension _pkParameterDimension;
      private Unit _displayUnit;

      protected override void Context()
      {
         base.Context();
         _displayUnit = A.Fake<Unit>();
         _mergedDimension = A.Fake<IDimension>();
         _pkParameterDimension = A.Fake<IDimension>();
         _pkParameter = new QuantityPKParameter().WithName("AUC").WithDimension(_pkParameterDimension);
         _pkParameterField = new PopulationAnalysisPKParameterField().WithDimension(_pkParameterDimension);
         _pkParameterField.DisplayUnit = _displayUnit;

         A.CallTo(() => _dimensionRepository.MergedDimensionFor(A<NumericFieldContext>._))
            .Invokes(x => _numericFieldContect = x.GetArgument<NumericFieldContext>(0))
            .Returns(_mergedDimension);
      }

      protected override void Because()
      {
         sut.Plot(_populationDataCollector, _pkParameter, _pkParameterField);
      }

      [Observation]
      public void should_retrieve_the_merge_dimension_for_the_pk_parameter()
      {
         _numericFieldContect.PopulationDataCollector.ShouldBeEqualTo(_populationDataCollector);
         _numericFieldContect.Dimension.ShouldBeEqualTo(_pkParameterDimension);
      }

      [Observation]
      public void should_plot_the_distribution_using_the_display_unit_of_the_pk_parameter_field_and_the_merged_dimension()
      {
         A.CallTo(() => _populationDistributionPresenter.Plot(_populationDataCollector, _pkParameter, A<DistributionSettings>._, _mergedDimension, _displayUnit)).MustHaveHappened();
      }
   }
}