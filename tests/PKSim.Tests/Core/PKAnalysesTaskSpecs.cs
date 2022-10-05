﻿using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.PKAnalyses;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Chart;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using ILazyLoadTask = PKSim.Core.Services.ILazyLoadTask;
using IPKCalculationOptionsFactory = PKSim.Core.Services.IPKCalculationOptionsFactory;
using PKAnalysesTask = PKSim.Core.Services.PKAnalysesTask;

namespace PKSim.Core
{
   public abstract class concern_for_PKAnalysesTask : ContextSpecification<PKAnalysesTask>
   {
      protected IPKValuesCalculator _pkCalculator;
      protected IPKValuesToPKAnalysisMapper _pkMapper;
      private IDimensionRepository _dimensionRepository;
      private IPKCalculationOptionsFactory _pkCalculationOptionsFactory;
      private IPKParameterRepository _pkParameterRepository;
      private ILazyLoadTask _lazyLoadTask;
      protected IStatisticalDataCalculator _statisticalDataCalculator;
      protected IRepresentationInfoRepository _representationInfoRepository;
      private IPopulationSimulationBodyWeightUpdater _populationSimulationBodyWeightUpdater;

      protected override void Context()
      {
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         _pkCalculator = A.Fake<IPKValuesCalculator>();
         _pkMapper = A.Fake<IPKValuesToPKAnalysisMapper>();
         _dimensionRepository = A.Fake<IDimensionRepository>();
         _pkCalculationOptionsFactory = A.Fake<IPKCalculationOptionsFactory>();
         _pkParameterRepository = A.Fake<IPKParameterRepository>();
         _statisticalDataCalculator = new StatisticalDataCalculator();
         _populationSimulationBodyWeightUpdater = A.Fake<IPopulationSimulationBodyWeightUpdater>();
         _representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         sut = new PKAnalysesTask(_lazyLoadTask, _pkCalculator, _pkParameterRepository, _pkCalculationOptionsFactory, _pkMapper, _dimensionRepository, _statisticalDataCalculator, _representationInfoRepository, _populationSimulationBodyWeightUpdater);
      }
   }

   public class When_creating_the_pk_analyses_for_a_given_population_simulation : concern_for_PKAnalysesTask
   {
      private ChartData<TimeProfileXValue, TimeProfileYValue> _chartData;
      private IPopulationDataCollector _populationDataCollector;
      private List<PopulationPKAnalysis> _result;
      private List<DataColumn> _dataColumn;

      protected override void Context()
      {
         base.Context();
         var dim = DomainHelperForSpecs.ConcentrationDimensionForSpecs();
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         _chartData = new ChartData<TimeProfileXValue, TimeProfileYValue>(new AxisData(dim, dim.DefaultUnit, Scalings.Linear), null);
         var pane = new PaneData<TimeProfileXValue, TimeProfileYValue>(new AxisData(dim, dim.DefaultUnit, Scalings.Linear));
         _chartData.AddPane(pane);
         var curve = new CurveData<TimeProfileXValue, TimeProfileYValue>()
         {
            Pane = pane,
            QuantityPath = "PATH",
         };
         curve.Add(new TimeProfileXValue(1), new TimeProfileYValue {Y = 10});
         curve.Add(new TimeProfileXValue(2), new TimeProfileYValue {Y = 20});
         pane.AddCurve(curve);

         var rangeCurve = new CurveData<TimeProfileXValue, TimeProfileYValue>()
         {
            Caption = "Compound-Peripheral Venous Blood-Plasma-Concentration-Range 2.5% to 97.5%",
            Pane = pane,
            QuantityPath = "RANGE_PATH",
         };

         rangeCurve.Add(new TimeProfileXValue(1), new TimeProfileYValue {Y = 10, LowerValue = 1, UpperValue = 2});
         pane.AddCurve(rangeCurve);

         A.CallTo(() => _populationDataCollector.MolWeightFor("PATH")).Returns(100);

         A.CallTo(() => _pkMapper.MapFrom(A<DataColumn>._, A<PKValues>._, A<PKParameterMode>._, A<string>._))
            .Invokes(x => _dataColumn.Add(x.GetArgument<DataColumn>(0)));

         _dataColumn = new List<DataColumn>();
      }

      protected override void Because()
      {
         _result = sut.CalculateFor(_populationDataCollector, _chartData).ToList();
      }

      [Observation]
      public void should_return_curve_data_with_the_mol_weight_set()
      {
         _dataColumn.First(x => x.DataInfo.MolWeight != null).DataInfo.MolWeight.ShouldBeEqualTo(100);
      }

      [Observation]
      public void should_not_generate_two_curves_for_each_range_plot()
      {
         _result.Count.ShouldBeEqualTo(3);
      }

      [Observation]
      public void should_name_the_generated_curves_by_the_correct_range_names()
      {
         _result.Select(x => x.ExtraDescription).ShouldContain("Compound-Peripheral Venous Blood-Plasma-Concentration-2.5%");
         _result.Select(x => x.ExtraDescription).ShouldContain("Compound-Peripheral Venous Blood-Plasma-Concentration-97.5%");
      }

      [Observation]
      public void should_have_calculated_the_pk_analysis_with_the_expected_value()
      {
         _dataColumn.First(x => x.Values.Count == 2).Values.ShouldOnlyContainInOrder(10f, 20f);
      }
   }

   public class When_creating_the_pk_analyses_for_a_given_population_simulation_with_axis_not_in_concentration_unit : concern_for_PKAnalysesTask
   {
      private ChartData<TimeProfileXValue, TimeProfileYValue> _chartData;
      private IPopulationDataCollector _populationDataCollector;
      private List<PopulationPKAnalysis> _result;
      private DataColumn _dataColumn;

      protected override void Context()
      {
         base.Context();
         var dim = DomainHelperForSpecs.LengthDimensionForSpecs();
         _populationDataCollector = A.Fake<IPopulationDataCollector>();
         _chartData = new ChartData<TimeProfileXValue, TimeProfileYValue>(new AxisData(dim, dim.DefaultUnit, Scalings.Linear), null);
         var pane = new PaneData<TimeProfileXValue, TimeProfileYValue>(new AxisData(dim, dim.DefaultUnit, Scalings.Linear));
         _chartData.AddPane(pane);
         var curve = new CurveData<TimeProfileXValue, TimeProfileYValue>()
         {
            Pane = pane,
            QuantityPath = "PATH",
         };
         pane.AddCurve(curve);

         A.CallTo(() => _pkMapper.MapFrom(A<DataColumn>._, A<PKValues>._, A<PKParameterMode>._, A<string>._))
            .Invokes(x => _dataColumn = x.GetArgument<DataColumn>(0));
      }

      protected override void Because()
      {
         _result = sut.CalculateFor(_populationDataCollector, _chartData).ToList();
      }

      [Observation]
      public void should_not_calculate_the_pk_analysis()
      {
         _dataColumn.ShouldBeNull();
      }
   }

   public class When_calculating_the_pk_analyses_for_a_set_of_selected_data_and_simulations : concern_for_PKAnalysesTask
   {
      private IEnumerable<IndividualPKAnalysis> _results;
      private IndividualSimulation _simulation;
      private DataColumn _dataColumn1, _dataColumn2;
      private GlobalPKAnalysis _globalPKAnalysis;
      private BaseGrid _baseGrid;
      private PKAnalysis _pkC1;
      private PKAnalysis _pkC2;
      private int _defaultNumberOfRules;

      protected override void Context()
      {
         base.Context();
         _simulation = new IndividualSimulation {DataRepository = new DataRepository()};
         _baseGrid = new BaseGrid("Time", DomainHelperForSpecs.TimeDimensionForSpecs()) {Values = new[] {10f, 20f, 30f}};
         _dataColumn1 = new DataColumn("C1", DomainHelperForSpecs.ConcentrationDimensionForSpecs(), _baseGrid)
         {
            DataInfo = {Origin = ColumnOrigins.Calculation},
            QuantityInfo = {Path = new[] {"C1", "Concentration"}},
            Values = new[] {11f, 21f, 31f}
         };

         _dataColumn2 = new DataColumn("C2", DomainHelperForSpecs.ConcentrationDimensionForSpecs(), _baseGrid)
         {
            DataInfo = {Origin = ColumnOrigins.Calculation},
            QuantityInfo = {Path = new[] {"C2", "Concentration"}},
            Values = new[] {12f, 22f, 32f}
         };

         _simulation.DataRepository.Add(_dataColumn1);
         _simulation.DataRepository.Add(_dataColumn2);

         //Setup global PK analysis
         _globalPKAnalysis = new GlobalPKAnalysis();
         var c1Container = new Container().WithName("C1");
         c1Container.Add(DomainHelperForSpecs.ConstantParameterWithValue(0.5).WithName(CoreConstants.PKAnalysis.FractionAbsorbed));
         var c2Container = new Container().WithName("C2");
         c2Container.Add(DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(CoreConstants.PKAnalysis.FractionAbsorbed));
         _globalPKAnalysis.Add(c1Container);
         _globalPKAnalysis.Add(c2Container);

         _pkC1 = new PKAnalysis
         {
            DomainHelperForSpecs.ConstantParameterWithValue(1).WithName(Constants.PKParameters.MRT),
            DomainHelperForSpecs.ConstantParameterWithValue(2).WithName(Constants.PKParameters.Tmax)
         };

         _pkC2 = new PKAnalysis
         {
            DomainHelperForSpecs.ConstantParameterWithValue(3).WithName(Constants.PKParameters.MRT),
            DomainHelperForSpecs.ConstantParameterWithValue(4).WithName(Constants.PKParameters.Tmax)
         };

         A.CallTo(() => _pkMapper.MapFrom(_dataColumn1, A<PKValues>._, A<PKParameterMode>._, "C1")).Returns(_pkC1);
         A.CallTo(() => _pkMapper.MapFrom(_dataColumn2, A<PKValues>._, A<PKParameterMode>._, "C2")).Returns(_pkC2);

         _defaultNumberOfRules = DomainHelperForSpecs.ConstantParameterWithValue(3).Rules.Count;
      }

      protected override void Because()
      {
         _results = sut.CalculateFor(new[] {_simulation}, new[] {_dataColumn1, _dataColumn2}, _globalPKAnalysis);
      }

      [Observation]
      public void should_have_calculated_the_pk_analysis_with_the_expected_value()
      {
         _results.Count().ShouldBeEqualTo(2);
         _results.ElementAt(0).PKAnalysis.ShouldBeEqualTo(_pkC1);
         _results.ElementAt(1).PKAnalysis.ShouldBeEqualTo(_pkC2);
      }

      [Observation]
      public void should_have_set_the_warnings_for_the_pk_parameters_belonging_to_a_compound_for_which_fraction_absorbed_is_smaller_than_1()
      {
         _pkC1.Parameter(Constants.PKParameters.MRT).Rules.Count.ShouldBeEqualTo(_defaultNumberOfRules + 1);
         _pkC1.Parameter(Constants.PKParameters.Tmax).Rules.Count.ShouldBeEqualTo(_defaultNumberOfRules);
      }

      [Observation]
      public void should_have_not_set_the_warnings_for_the_pk_parameters_belonging_to_a_compound_for_which_fraction_absorbed_is_equal_to_1()
      {
         _pkC2.Parameter(Constants.PKParameters.MRT).Rules.Count.ShouldBeEqualTo(_defaultNumberOfRules);
         _pkC2.Parameter(Constants.PKParameters.Tmax).Rules.Count.ShouldBeEqualTo(_defaultNumberOfRules);
      }
   }
}