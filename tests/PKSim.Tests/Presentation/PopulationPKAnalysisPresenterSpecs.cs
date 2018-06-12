using System.Collections.Generic;
using System.Data;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Chart;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Mappers;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Mappers;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.PKAnalyses;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Services;
using TimeProfileCurveData = PKSim.Core.Chart.CurveData<PKSim.Core.Chart.TimeProfileXValue, PKSim.Core.Chart.TimeProfileYValue>;

namespace PKSim.Presentation
{
   public abstract class concern_for_PopulationPKAnalysisPresenter : ContextSpecification<IPopulationPKAnalysisPresenter>
   {
      protected IPKAnalysisExportTask _exportTask;
      protected IPKAnalysesTask _pkAnalysesTask;
      protected IPopulationPKAnalysisView _view;

      protected ChartData<TimeProfileXValue, TimeProfileYValue> _timeProfileChartData;
      protected IPopulationDataCollector _populationDataCollector;
      protected TimeProfileCurveData _curve1;
      protected TimeProfileCurveData _curve2;
      protected PKAnalysis _pk1;
      protected PKAnalysis _pk2;
      protected List<PopulationPKAnalysis> _allPKanalysis;
      protected IPopulationPKAnalysisToPKAnalysisDTOMapper _populationPKAnalysisToDTOMapper;
      protected DataTable _dataTable;
      protected IPopulationPKAnalysisToDataTableMapper _populationPKAnalysisToDataTableMapper;
      private IPKParameterRepository _pkParameterRepository;
      protected IPresentationSettingsTask _presenterSettingsTask;

      protected override void Context()
      {
         _exportTask = A.Fake<IPKAnalysisExportTask>();
         _pkAnalysesTask = A.Fake<IPKAnalysesTask>();
         _view = A.Fake<IPopulationPKAnalysisView>();
         _populationPKAnalysisToDTOMapper = A.Fake<IPopulationPKAnalysisToPKAnalysisDTOMapper>();
         _populationPKAnalysisToDataTableMapper = A.Fake<IPopulationPKAnalysisToDataTableMapper>();
         _pkParameterRepository = A.Fake<IPKParameterRepository>();

         _presenterSettingsTask = A.Fake<IPresentationSettingsTask>();
         sut = new PopulationPKAnalysisPresenter(_view, _pkAnalysesTask, _exportTask, _populationPKAnalysisToDTOMapper, _pkParameterRepository, _presenterSettingsTask);

         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         _timeProfileChartData = new ChartData<TimeProfileXValue, TimeProfileYValue>(null, null);
         var pane = new PaneData<TimeProfileXValue, TimeProfileYValue>(null) {Caption = "TOTO"};
         _curve1 = new TimeProfileCurveData {Caption = "A", Pane = pane};
         _curve2 = new TimeProfileCurveData { Caption = "B", Pane = pane };

         _pk1 = new PKAnalysis();
         _pk2 = new PKAnalysis();

         _allPKanalysis = new List<PopulationPKAnalysis>
         {
            new PopulationPKAnalysis(_curve1, _pk1),
            new PopulationPKAnalysis(_curve2, _pk2),
         };


         A.CallTo(() => _pkAnalysesTask.CalculateFor(_populationDataCollector, _timeProfileChartData)).Returns(_allPKanalysis);
         A.CallTo(() => _view.BindTo(A<PKAnalysisDTO>._)).Invokes(x => _dataTable = x.GetArgument<PKAnalysisDTO>(0).DataTable);
         A.CallTo(() => _populationPKAnalysisToDataTableMapper.MapFrom(A<IReadOnlyList<PopulationPKAnalysis>>._, true)).Returns(_dataTable);
      }
   }

   public class when_loading_preferred_units_for_pkanalysis : concern_for_PopulationPKAnalysisPresenter
   {
      private DefaultPresentationSettings _settings;
      private PKSimParameter _pkSimParameter;
      private Unit _newUnit;

      protected override void Context()
      {
         base.Context();
         var dimension = new Dimension(new BaseDimensionRepresentation(), "none", "l/f");

         _newUnit = new Unit("kl/f", 1000, 0);
         dimension.AddUnit(_newUnit);

         _pkSimParameter = new PKSimParameter {Dimension = dimension};

         _allPKanalysis.First().PKAnalysis.Add(_pkSimParameter);
         _settings = new DefaultPresentationSettings();
         A.CallTo(() => _presenterSettingsTask.PresentationSettingsFor<DefaultPresentationSettings>(A<IPresenterWithSettings>._, A<IWithId>._)).Returns(_settings);
         _settings.SetSetting(_pkSimParameter.Name, "kl/f");
         A.CallTo(() => _populationDataCollector.Name).Returns("TOTO");
         sut.LoadSettingsForSubject(A.Fake<IWithId>());
      }

      protected override void Because()
      {
         base.Because();
         sut.CalculatePKAnalysis(_populationDataCollector, _timeProfileChartData);
      }

      [Observation]
      public void should_adjust_the_units_for_the_pkparameters_whose_preferred_units_are_stored_in_the_settings()
      {
         _pkSimParameter.DisplayUnit.ShouldBeEqualTo(_newUnit);
      }
   }

   public class When_the_population_pk_analysis_presenter_is_exporting_the_analysis_to_excel : concern_for_PopulationPKAnalysisPresenter
   {
      protected override void Context()
      {
         base.Context();

         A.CallTo(() => _populationDataCollector.Name).Returns("TOTO");
         sut.CalculatePKAnalysis(_populationDataCollector, _timeProfileChartData);
      }

      protected override void Because()
      {
         sut.ExportToExcel();
      }

      [Observation]
      public void should_export_all_visible_analysis_to_excel()
      {
         A.CallTo(() => _exportTask.ExportToExcel(A<DataTable>._, _populationDataCollector.Name)).MustHaveHappened();
      }
   }

   public class When_the_population_pk_analysis_presenter_is_notified_that_a_unit_was_changed_for_a_pk_parameter : concern_for_PopulationPKAnalysisPresenter
   {
      private Unit _newDisplayUnit;

      protected override void Context()
      {
         base.Context();
         _newDisplayUnit = A.Fake<Unit>();
         _pk1.Add(A.Fake<IParameter>().WithName(Constants.PKParameters.AUC));
         sut.CalculatePKAnalysis(_populationDataCollector, _timeProfileChartData);
      }

      protected override void Because()
      {
         sut.ChangeUnit(Constants.PKParameters.AUC, _newDisplayUnit);
      }

      [Observation]
      public void should_update_the_display_unit_in_all_pk_parameters()
      {
         _pk1.Parameter(Constants.PKParameters.AUC).DisplayUnit.ShouldBeEqualTo(_newDisplayUnit);
      }

      [Observation]
      public void should_update_the_view()
      {
         A.CallTo(() => _view.BindTo(A<PKAnalysisDTO>._)).MustHaveHappened(Repeated.Exactly.Twice);
      }
   }

   public class When_the_population_pk_analysis_presenter_is_notified_that_a_unit_was_changed_for_a_pk_parameter_that_does_not_exist : concern_for_PopulationPKAnalysisPresenter
   {
      private Unit _newDisplayUnit;

      protected override void Context()
      {
         base.Context();
         _newDisplayUnit = A.Fake<Unit>();
         _pk1.Add(A.Fake<IParameter>().WithName(Constants.PKParameters.AUC));
         sut.CalculatePKAnalysis(_populationDataCollector, _timeProfileChartData);
      }

      [Observation]
      public void should_not_crash_when_setting_the_value_for_a_parameter_that_does_not_exist()
      {
         sut.ChangeUnit(Constants.PKParameters.AUC_inf, _newDisplayUnit);
      }
   }
   
   public class When_the_population_pk_analysis_presenter_is_retrieving_the_available_units : concern_for_PopulationPKAnalysisPresenter
   {
      private IDimension _dimension;

      protected override void Context()
      {
         base.Context();
         _dimension = DomainHelperForSpecs.ConcentrationDimensionForSpecs();
         _pk1.Add(A.Fake<IParameter>()
            .WithName(Constants.PKParameters.AUC_inf)
            .WithDimension(A.Fake<IDimension>()));

         _pk2.Add(A.Fake<IParameter>()
           .WithName(Constants.PKParameters.AUC)
           .WithDimension(_dimension));

         sut.CalculatePKAnalysis(_populationDataCollector, _timeProfileChartData);
      }

      [Observation]
      public void should_return_the_units_defined_in_the_pk_parameter_of_the_first_analysis_where_the_parameter_is_defined()
      {
         sut.AvailableUnitsFor(Constants.PKParameters.AUC).ShouldBeEqualTo(_dimension.Units);
      }

      [Observation]
      public void should_return_an_empty_enumeration_otherwise()
      {
         sut.AvailableUnitsFor("TOTO").ShouldBeEmpty();
      }
   }

   public class When_the_population_pk_analysis_presenter_is_retrieving_the_current_display_unit_for_a_pk_parameter : concern_for_PopulationPKAnalysisPresenter
   {
      private IDimension _dimension;

      protected override void Context()
      {
         base.Context();
         _dimension = DomainHelperForSpecs.ConcentrationDimensionForSpecs();
         _pk1.Add(A.Fake<IParameter>()
            .WithName(Constants.PKParameters.AUC)
            .WithDimension(_dimension));
         _pk1.Parameter(Constants.PKParameters.AUC).DisplayUnit = A.Fake<Unit>();
         sut.CalculatePKAnalysis(_populationDataCollector, _timeProfileChartData);
      }

      [Observation]
      public void should_return_the_display_unit_of_the_pk_parameter_of_the_first_analysis_for_an_existing_parameter()
      {
         sut.DisplayUnitFor(Constants.PKParameters.AUC).ShouldBeEqualTo(_pk1.Parameter(Constants.PKParameters.AUC).DisplayUnit);
      }

      [Observation]
      public void should_return_the_default_display_unit_of_the_no_dimension()
      {
         sut.DisplayUnitFor("TOTO").ShouldBeEqualTo(Constants.Dimension.NO_DIMENSION.DefaultUnit);
      }
   }
}