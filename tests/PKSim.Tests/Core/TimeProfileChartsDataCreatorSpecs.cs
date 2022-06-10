using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using FakeItEasy;
using PKSim.Core.Chart;
using PKSim.Core.Mappers;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Extensions;

namespace PKSim.Core
{
   public abstract class concern_for_TimeProfileChartsDataCreator : ContextSpecification<ITimeProfileChartDataCreator>
   {
      private IDimensionRepository _dimensionRepository;
      private IRepresentationInfoRepository _representationInfoRepository;
      protected PopulationAnalysisCovariateField _genderField;
      protected readonly PopulationAnalysisOutputField _outputFieldVenousBloodPlasma = new PopulationAnalysisOutputField {Name = "VenousBloodPlasma", QuantityPath = "VenousBloodPlasma", Color = Color.Blue};
      protected readonly PopulationAnalysisOutputField _outputFieldLiverCell = new PopulationAnalysisOutputField {Name = "LiverCell", QuantityPath = "LiverCell", Color = Color.Brown};
      protected PopulationStatisticalAnalysis _statisticalAnalysis;
      protected PredefinedStatisticalAggregation _predefinedStatisticalAggregation;
      protected PercentileStatisticalAggregation _percentileStatisticalAggregation;
      private IStatisticalDataCalculator _statisticalDataCalculator;
      private IPivotResultCreator _pivotResultCreator;
      private ILazyLoadTask _lazyLoadTask;
      protected IDataRepositoryToObservedCurveDataMapper _observedCurveDataMapper;
      protected IDimension _mergedDimensionVenousBloodPlasma;
      protected IDimension _mergedDimensionLiverCell;
      protected const string _singleCurveId = "SingleCurve";
      protected const string _percentileId = "Percentile";
      protected IPKAnalysesTask _pkAnalysesTask;

      protected override void Context()
      {
         _dimensionRepository = A.Fake<IDimensionRepository>();
         _representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         _lazyLoadTask = A.Fake<ILazyLoadTask>();
         _observedCurveDataMapper = A.Fake<IDataRepositoryToObservedCurveDataMapper>();
         _statisticalAnalysis = new PopulationStatisticalAnalysis();
         _genderField = ChartDataHelperForSpecs.CreateGenderField();
         _statisticalAnalysis.Add(_genderField);
         _statisticalAnalysis.Add(_outputFieldVenousBloodPlasma);
         _statisticalAnalysis.Add(_outputFieldLiverCell);
         _predefinedStatisticalAggregation = new PredefinedStatisticalAggregation {Selected = true};
         _percentileStatisticalAggregation = new PercentileStatisticalAggregation {Selected = false, Percentile = 50};
         _statisticalAnalysis.AddStatistic(_predefinedStatisticalAggregation);
         _statisticalAnalysis.AddStatistic(_percentileStatisticalAggregation);
         A.CallTo(() => _representationInfoRepository.DisplayNameFor(_predefinedStatisticalAggregation)).Returns(_singleCurveId);
         A.CallTo(() => _representationInfoRepository.DisplayNameFor(_percentileStatisticalAggregation)).Returns(_percentileId);
         _statisticalDataCalculator = new StatisticalDataCalculator();
         _pivotResultCreator = A.Fake<IPivotResultCreator>();

         _mergedDimensionVenousBloodPlasma = DomainHelperForSpecs.ConcentrationDimensionForSpecs();
         _mergedDimensionLiverCell = DomainHelperForSpecs.FractionDimensionForSpecs();

         A.CallTo(() => _dimensionRepository.MergedDimensionFor(A<IWithDimension>._))
            .WhenArgumentsMatch((IWithDimension context) => Equals((context as NumericFieldContext)?.NumericValueField, _outputFieldVenousBloodPlasma))
            .Returns(_mergedDimensionVenousBloodPlasma);

         A.CallTo(() => _dimensionRepository.MergedDimensionFor(A<IWithDimension>._))
            .WhenArgumentsMatch((IWithDimension context) => Equals((context as NumericFieldContext)?.NumericValueField, _outputFieldLiverCell))
            .Returns(_mergedDimensionLiverCell);

         _pkAnalysesTask = A.Fake<IPKAnalysesTask>();

         sut = new TimeProfileChartDataCreator(_dimensionRepository, _pivotResultCreator, _representationInfoRepository,
            _statisticalDataCalculator, _lazyLoadTask, _observedCurveDataMapper, _pkAnalysesTask);
      }
   }

   public class When_creating_the_time_profile_chart_data_based_on_valid_data_containing_grouping_on_rows : concern_for_TimeProfileChartsDataCreator
   {
      private PivotResult _pivotResult;
      private ChartData<TimeProfileXValue, TimeProfileYValue> _chartData;

      protected override void Context()
      {
         base.Context();
         _statisticalAnalysis.SetPosition(_genderField, PivotArea.RowArea, 0);
         _statisticalAnalysis.SetPosition(_outputFieldVenousBloodPlasma, PivotArea.DataArea, 0);
         _statisticalAnalysis.SetPosition(_outputFieldLiverCell, PivotArea.DataArea, 1);
         _predefinedStatisticalAggregation.Method = StatisticalAggregationType.ArithmeticMean;
         _percentileStatisticalAggregation.Selected = true;
         _pivotResult = ChartDataHelperForSpecs.CreateOutputResults(_statisticalAnalysis, _genderField, _outputFieldVenousBloodPlasma, _outputFieldLiverCell);
      }

      protected override void Because()
      {
         _chartData = sut.CreateFor(_pivotResult);
      }

      [Observation]
      public void should_have_created_one_pane_for_each_row_fields()
      {
         _chartData.ShouldNotBeNull();
         //Male, Female
         _chartData.Panes.Count.ShouldBeEqualTo(2);
      }

      [Observation]
      public void should_have_added_the_expected_series()
      {
         foreach (var pane in _chartData.Panes)
         {
            //one serie for each output on each pane and statistical output 2x2
            pane.Curves.Count.ShouldBeEqualTo(4);
            foreach (var series in pane.Curves)
            {
               if (series.Id.StartsWith(_outputFieldLiverCell.Name))
               {
                  series.Color.ShouldBeEqualTo(_outputFieldLiverCell.Color);
                  series.YDimension.ShouldBeEqualTo(_mergedDimensionLiverCell);
               }
               if (series.Id.StartsWith(_outputFieldVenousBloodPlasma.Name))
               {
                  series.Color.ShouldBeEqualTo(_outputFieldVenousBloodPlasma.Color);
                  series.YDimension.ShouldBeEqualTo(_mergedDimensionVenousBloodPlasma);
               }
            }
         }
      }

      [Observation]
      public void should_have_calculated_the_expected_values_for_male()
      {
         var malePane = _chartData.Panes["Male"];
         foreach (var series in malePane.Curves)
         {
            //check time values
            series.XValues.Select(x => x.X).ShouldOnlyContainInOrder(1f, 2f, 3f, 4f);
         }

         var venousBloodPlasmaMaleMean = malePane.Curves[_outputFieldVenousBloodPlasma.Name + Constants.DISPLAY_PATH_SEPARATOR + _singleCurveId];

         //mean of 10, 20, 30, 40 and 1000, 2000, 3000, 4000 
         venousBloodPlasmaMaleMean.YValues.Select(y => y.Y).ShouldOnlyContainInOrder(
            new float[] {10, 1000}.ArithmeticMean(),
            new float[] {20, 2000}.ArithmeticMean(),
            new float[] {30, 3000}.ArithmeticMean(),
            new float[] {40, 4000}.ArithmeticMean());
         venousBloodPlasmaMaleMean.YValues.Select(y => y.LowerValue).ShouldOnlyContainInOrder(float.NaN, float.NaN, float.NaN, float.NaN);
         venousBloodPlasmaMaleMean.YValues.Select(y => y.UpperValue).ShouldOnlyContainInOrder(float.NaN, float.NaN, float.NaN, float.NaN);


         var liverCellMaleMean = malePane.Curves[_outputFieldLiverCell.Name + Constants.DISPLAY_PATH_SEPARATOR + _singleCurveId];

         //mean of  50, 60, 70, 80 and 5000, 6000, 7000, 8000
         liverCellMaleMean.YValues.Select(y => y.Y).ShouldOnlyContainInOrder(
            new float[] {50, 5000}.ArithmeticMean(),
            new float[] {60, 6000}.ArithmeticMean(),
            new float[] {70, 7000}.ArithmeticMean(),
            new float[] {80, 8000}.ArithmeticMean());
         liverCellMaleMean.YValues.Select(y => y.LowerValue).ShouldOnlyContainInOrder(float.NaN, float.NaN, float.NaN, float.NaN);
         liverCellMaleMean.YValues.Select(y => y.UpperValue).ShouldOnlyContainInOrder(float.NaN, float.NaN, float.NaN, float.NaN);
      }

      [Observation]
      public void should_have_calculated_the_expected_values_for_female()
      {
         var femalePane = _chartData.Panes["Female"];
         foreach (var series in femalePane.Curves)
         {
            //check time values
            series.XValues.Select(x => x.X).ShouldOnlyContainInOrder(1f, 2f, 3f, 4f);
         }

         var venousBloodPlasmaFemaleMean = femalePane.Curves[_outputFieldVenousBloodPlasma.Name + Constants.DISPLAY_PATH_SEPARATOR + _singleCurveId];

         //only one value 100,200,300,400
         venousBloodPlasmaFemaleMean.YValues.Select(y => y.Y).ShouldOnlyContainInOrder(100, 200, 300, 400);
         venousBloodPlasmaFemaleMean.YValues.Select(y => y.LowerValue).ShouldOnlyContainInOrder(float.NaN, float.NaN, float.NaN, float.NaN);
         venousBloodPlasmaFemaleMean.YValues.Select(y => y.UpperValue).ShouldOnlyContainInOrder(float.NaN, float.NaN, float.NaN, float.NaN);

         var liverCellFemaleMean = femalePane.Curves[_outputFieldLiverCell.Name + Constants.DISPLAY_PATH_SEPARATOR + _singleCurveId];

         //only one value 500,600,700,800
         liverCellFemaleMean.YValues.Select(y => y.Y).ShouldOnlyContainInOrder(500, 600, 700, 800);
         liverCellFemaleMean.YValues.Select(y => y.LowerValue).ShouldOnlyContainInOrder(float.NaN, float.NaN, float.NaN, float.NaN);
         liverCellFemaleMean.YValues.Select(y => y.UpperValue).ShouldOnlyContainInOrder(float.NaN, float.NaN, float.NaN, float.NaN);
      }
   }

   public class When_creating_the_time_profile_chart_data_based_on_valid_data_containing_grouping_on_rows_with_range_series : concern_for_TimeProfileChartsDataCreator
   {
      private PivotResult _pivotResult;
      private ChartData<TimeProfileXValue, TimeProfileYValue> _chartData;

      protected override void Context()
      {
         base.Context();
         _statisticalAnalysis.SetPosition(_genderField, PivotArea.RowArea, 0);
         _statisticalAnalysis.SetPosition(_outputFieldVenousBloodPlasma, PivotArea.DataArea, 0);
         _statisticalAnalysis.SetPosition(_outputFieldLiverCell, PivotArea.DataArea, 1);
         _predefinedStatisticalAggregation.Method = StatisticalAggregationType.Range95;
         _pivotResult = ChartDataHelperForSpecs.CreateOutputResults(_statisticalAnalysis, _genderField, _outputFieldVenousBloodPlasma, _outputFieldLiverCell);
      }

      protected override void Because()
      {
         _chartData = sut.CreateFor(_pivotResult);
      }

      [Observation]
      public void should_have_calculated_the_expected_values_for_male()
      {
         var malePane = _chartData.Panes.FindById("Male");
         foreach (var series in malePane.Curves)
         {
            //check time values
            series.XValues.Select(x => x.X).ShouldOnlyContainInOrder(1f, 2f, 3f, 4f);
         }

         var venousBloodPlasmaMaleMean = malePane.Curves[_outputFieldVenousBloodPlasma.Name + Constants.DISPLAY_PATH_SEPARATOR + _singleCurveId];
         venousBloodPlasmaMaleMean.YValues.Select(y => y.Y).ShouldOnlyContainInOrder(float.NaN, float.NaN, float.NaN, float.NaN);

         //mean of 10, 20, 30, 40 and 1000, 2000, 3000, 4000 
         venousBloodPlasmaMaleMean.YValues.Select(y => y.LowerValue).ShouldOnlyContainInOrder(
            new float[] {10, 1000}.Percentile(2.5),
            new float[] {20, 2000}.Percentile(2.5),
            new float[] {30, 3000}.Percentile(2.5),
            new float[] {40, 4000}.Percentile(2.5));

         venousBloodPlasmaMaleMean.YValues.Select(y => y.UpperValue).ShouldOnlyContainInOrder(
            new float[] {10, 1000}.Percentile(97.5),
            new float[] {20, 2000}.Percentile(97.5),
            new float[] {30, 3000}.Percentile(97.5),
            new float[] {40, 4000}.Percentile(97.5));
      }

      [Observation]
      public void should_have_calculated_the_expected_values_for_female()
      {
         var femalePane = _chartData.Panes.FindById("Female");
         foreach (var series in femalePane.Curves)
         {
            //check time values
            series.XValues.Select(x => x.X).ShouldOnlyContainInOrder(1f, 2f, 3f, 4f);
         }

         var venousBloodPlasmaFemaleMean = femalePane.Curves[_outputFieldVenousBloodPlasma.Name + Constants.DISPLAY_PATH_SEPARATOR + _singleCurveId];

         //only one value 100,200,300,400
         venousBloodPlasmaFemaleMean.YValues.Select(y => y.Y).ShouldOnlyContainInOrder(float.NaN, float.NaN, float.NaN, float.NaN);
         venousBloodPlasmaFemaleMean.YValues.Select(y => y.LowerValue).ShouldOnlyContainInOrder(100, 200, 300, 400);
         venousBloodPlasmaFemaleMean.YValues.Select(y => y.UpperValue).ShouldOnlyContainInOrder(100, 200, 300, 400);
      }
   }

   public class When_creating_the_time_profile_chart_data_based_on_valid_data_containing_grouping_on_columns : concern_for_TimeProfileChartsDataCreator
   {
      private PivotResult _pivotResult;
      private ChartData<TimeProfileXValue, TimeProfileYValue> _chartData;

      protected override void Context()
      {
         base.Context();
         _statisticalAnalysis.SetPosition(_genderField, PivotArea.ColumnArea, 0);
         _statisticalAnalysis.ColorField = _genderField;
         _statisticalAnalysis.SetPosition(_outputFieldVenousBloodPlasma, PivotArea.DataArea, 0);
         _statisticalAnalysis.SetPosition(_outputFieldLiverCell, PivotArea.DataArea, 1);
         _predefinedStatisticalAggregation.Method = StatisticalAggregationType.ArithmeticMean;
         _percentileStatisticalAggregation.Selected = true;
         _pivotResult = ChartDataHelperForSpecs.CreateOutputResults(_statisticalAnalysis, _genderField, _outputFieldVenousBloodPlasma, _outputFieldLiverCell);
      }

      protected override void Because()
      {
         _chartData = sut.CreateFor(_pivotResult);
      }

      [Observation]
      public void should_have_created_only_one_pane()
      {
         _chartData.ShouldNotBeNull();
         _chartData.Panes.Count().ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_have_added_the_expected_series()
      {
         foreach (var pane in _chartData.Panes)
         {
            //one series for each output on each pane and statistical output 2x2
            pane.Curves.Count.ShouldBeEqualTo(8);
            foreach (var series in pane.Curves)
            {
               if (series.Id.EndsWith("Male"))
               {
                  series.Color.ShouldBeEqualTo(PKSimColors.Male);
               }
               if (series.Id.EndsWith("Female"))
               {
                  series.Color.ShouldBeEqualTo(PKSimColors.Female);
               }
            }
         }
      }

      [Observation]
      public void should_have_calculated_the_expected_values_for_male()
      {
         var pane = _chartData.Panes.First();
         foreach (var series in pane.Curves)
         {
            //check time values
            series.XValues.Select(x => x.X).ShouldOnlyContainInOrder(1f, 2f, 3f, 4f);
         }

         var venousBloodPlasmaMaleMean = pane.Curves[_outputFieldVenousBloodPlasma.Name + Constants.DISPLAY_PATH_SEPARATOR + _singleCurveId + Constants.DISPLAY_PATH_SEPARATOR + "Male"];

         //mean of 10, 20, 30, 40 and 1000, 2000, 3000, 4000 
         venousBloodPlasmaMaleMean.YValues.Select(y => y.Y).ShouldOnlyContainInOrder(
            new float[] {10, 1000}.ArithmeticMean(),
            new float[] {20, 2000}.ArithmeticMean(),
            new float[] {30, 3000}.ArithmeticMean(),
            new float[] {40, 4000}.ArithmeticMean());
         venousBloodPlasmaMaleMean.YValues.Select(y => y.LowerValue).ShouldOnlyContainInOrder(float.NaN, float.NaN, float.NaN, float.NaN);
         venousBloodPlasmaMaleMean.YValues.Select(y => y.UpperValue).ShouldOnlyContainInOrder(float.NaN, float.NaN, float.NaN, float.NaN);


         var liverCellMaleMean = pane.Curves[_outputFieldLiverCell.Name + Constants.DISPLAY_PATH_SEPARATOR + _singleCurveId + Constants.DISPLAY_PATH_SEPARATOR + "Male"];

         //mean of  50, 60, 70, 80 and 5000, 6000, 7000, 8000
         liverCellMaleMean.YValues.Select(y => y.Y).ShouldOnlyContainInOrder(
            new float[] {50, 5000}.ArithmeticMean(),
            new float[] {60, 6000}.ArithmeticMean(),
            new float[] {70, 7000}.ArithmeticMean(),
            new float[] {80, 8000}.ArithmeticMean());
         liverCellMaleMean.YValues.Select(y => y.LowerValue).ShouldOnlyContainInOrder(float.NaN, float.NaN, float.NaN, float.NaN);
         liverCellMaleMean.YValues.Select(y => y.UpperValue).ShouldOnlyContainInOrder(float.NaN, float.NaN, float.NaN, float.NaN);
      }

      [Observation]
      public void should_have_calculated_the_expected_values_for_female()
      {
         var pane = _chartData.Panes.First();
         foreach (var series in pane.Curves)
         {
            //check time values
            series.XValues.Select(x => x.X).ShouldOnlyContainInOrder(1f, 2f, 3f, 4f);
         }

         var venousBloodPlasmaFemaleMean = pane.Curves[_outputFieldVenousBloodPlasma.Name + Constants.DISPLAY_PATH_SEPARATOR + _singleCurveId + Constants.DISPLAY_PATH_SEPARATOR + "Female"];

         //only one value 100,200,300,400
         venousBloodPlasmaFemaleMean.YValues.Select(y => y.Y).ShouldOnlyContainInOrder(100, 200, 300, 400);
         venousBloodPlasmaFemaleMean.YValues.Select(y => y.LowerValue).ShouldOnlyContainInOrder(float.NaN, float.NaN, float.NaN, float.NaN);
         venousBloodPlasmaFemaleMean.YValues.Select(y => y.UpperValue).ShouldOnlyContainInOrder(float.NaN, float.NaN, float.NaN, float.NaN);

         var liverCellFemaleMean = pane.Curves[_outputFieldLiverCell.Name + Constants.DISPLAY_PATH_SEPARATOR + _singleCurveId + Constants.DISPLAY_PATH_SEPARATOR + "Female"];

         //only one value 500,600,700,800
         liverCellFemaleMean.YValues.Select(y => y.Y).ShouldOnlyContainInOrder(500, 600, 700, 800);
         liverCellFemaleMean.YValues.Select(y => y.LowerValue).ShouldOnlyContainInOrder(float.NaN, float.NaN, float.NaN, float.NaN);
         liverCellFemaleMean.YValues.Select(y => y.UpperValue).ShouldOnlyContainInOrder(float.NaN, float.NaN, float.NaN, float.NaN);
      }
   }

   public class When_creating_the_time_profile_chart_data_based_on_valid_data_containing_color_grouping_with_observed_data : concern_for_TimeProfileChartsDataCreator
   {
      private PivotResult _pivotResult;
      private ChartData<TimeProfileXValue, TimeProfileYValue> _chartData;
      private ObservedDataCollection _observedDataCollection;
      private List<ObservedCurveData> _observedCurveDataList;
      private ObservedCurveData _observedCurveData;

      protected override void Context()
      {
         base.Context();
         _observedCurveDataList = new List<ObservedCurveData>();
         _observedCurveData = new ObservedCurveData(AuxiliaryType.Undefined) {Caption = "Male"};

         _observedCurveDataList.Add(_observedCurveData);
         _observedCurveData.Color = Color.Aqua;

         A.CallTo(_observedCurveDataMapper).WithReturnType<IReadOnlyList<ObservedCurveData>>().Returns(_observedCurveDataList);

         _statisticalAnalysis.SetPosition(_genderField, PivotArea.ColumnArea, 0);
         _statisticalAnalysis.ColorField = _genderField;
         _statisticalAnalysis.SetPosition(_outputFieldVenousBloodPlasma, PivotArea.DataArea, 0);
         _statisticalAnalysis.SetPosition(_outputFieldLiverCell, PivotArea.DataArea, 1);
         _predefinedStatisticalAggregation.Method = StatisticalAggregationType.ArithmeticMean;
         _percentileStatisticalAggregation.Selected = true;

         _observedDataCollection = new ObservedDataCollection {ApplyGroupingToObservedData = true};
         var observedData = DomainHelperForSpecs.ObservedData();
         observedData.ExtendedProperties.Add(new ExtendedProperty<string> {Name = _genderField.Name, Value = "Male"});
         _observedDataCollection.AddObservedData(observedData);

         _pivotResult = ChartDataHelperForSpecs.CreateOutputResults(_statisticalAnalysis, _genderField, _outputFieldVenousBloodPlasma, _outputFieldLiverCell, _observedDataCollection);
      }

      protected override void Because()
      {
         _chartData = sut.CreateFor(_pivotResult);
      }

      [Observation]
      public void should_have_created_only_one_pane()
      {
         _chartData.ShouldNotBeNull();
         _chartData.Panes.Count().ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_have_added_the_expected_series()
      {
         foreach (var pane in _chartData.Panes)
         {
            //one serie for each output on each pane and statistical output 2x2
            pane.Curves.Count.ShouldBeEqualTo(8);
            foreach (var series in pane.Curves)
            {
               if (series.Id.EndsWith("Male"))
               {
                  series.Color.ShouldBeEqualTo(PKSimColors.Male);
               }
               if (series.Id.EndsWith("Female"))
               {
                  series.Color.ShouldBeEqualTo(PKSimColors.Female);
               }
            }
            pane.ObservedCurveData.Count.ShouldBeEqualTo(1);
            foreach (var series in pane.ObservedCurveData)
            {
               series.Color.ShouldBeEqualTo(PKSimColors.Male);
            }
         }
      }
   }

   public class When_creating_the_time_profile_chart_data_based_on_valid_data_containing_pane_grouping_with_observed_data : concern_for_TimeProfileChartsDataCreator
   {
      private PivotResult _pivotResult;
      private ChartData<TimeProfileXValue, TimeProfileYValue> _chartData;
      private ObservedDataCollection _observedDataCollection;
      private List<ObservedCurveData> _observedCurveDataList;
      private ObservedCurveData _observedCurveData;

      protected override void Context()
      {
         base.Context();

         _observedCurveDataList = new List<ObservedCurveData>();
         _observedCurveData = new ObservedCurveData(AuxiliaryType.Undefined) {Caption = "Male"};

         _observedCurveDataList.Add(_observedCurveData);
         _observedCurveData.Color = Color.Aqua;

         A.CallTo(_observedCurveDataMapper).WithReturnType<IReadOnlyList<ObservedCurveData>>().Returns(_observedCurveDataList);

         _statisticalAnalysis.SetPosition(_genderField, PivotArea.RowArea, 0);
         _statisticalAnalysis.SetPosition(_outputFieldVenousBloodPlasma, PivotArea.DataArea, 0);
         _statisticalAnalysis.SetPosition(_outputFieldLiverCell, PivotArea.DataArea, 1);
         _predefinedStatisticalAggregation.Method = StatisticalAggregationType.ArithmeticMean;
         _percentileStatisticalAggregation.Selected = true;

         _observedDataCollection = new ObservedDataCollection {ApplyGroupingToObservedData = true};
         var observedData = DomainHelperForSpecs.ObservedData();
         observedData.ExtendedProperties.Add(new ExtendedProperty<string> {Name = _genderField.Name, Value = "Male"});
         _observedDataCollection.AddObservedData(observedData);

         _pivotResult = ChartDataHelperForSpecs.CreateOutputResults(_statisticalAnalysis, _genderField, _outputFieldVenousBloodPlasma, _outputFieldLiverCell, _observedDataCollection);
      }

      protected override void Because()
      {
         _chartData = sut.CreateFor(_pivotResult);
      }

      [Observation]
      public void should_have_created_two_panes()
      {
         _chartData.ShouldNotBeNull();
         _chartData.Panes.Count().ShouldBeEqualTo(2);
      }

      [Observation]
      public void should_have_added_the_expected_series()
      {
         foreach (var pane in _chartData.Panes)
         {
            //one serie for each output on each pane and statistical output 2x2
            pane.Curves.Count.ShouldBeEqualTo(4);

            if (pane.Id.EndsWith("Male"))
            {
               pane.ObservedCurveData.Count.ShouldBeEqualTo(1);
               foreach (var series in pane.ObservedCurveData)
               {
                  series.Color.ShouldBeEqualTo(Color.Aqua);
               }
            }
            else
            {
               pane.ObservedCurveData.Count.ShouldBeEqualTo(0);
            }
         }
      }
   }

   public class When_creating_a_time_profile_analysis_for_an_analysis_with_output_fields_but_without_statistical_selection : concern_for_TimeProfileChartsDataCreator
   {
      private PivotResult _pivotResult;

      protected override void Context()
      {
         base.Context();
         _statisticalAnalysis.SetPosition(_outputFieldVenousBloodPlasma, PivotArea.DataArea, 0);
         _statisticalAnalysis.Statistics.Each(s => s.Selected = false);
         _pivotResult = ChartDataHelperForSpecs.CreateOutputResults(_statisticalAnalysis, _genderField, _outputFieldVenousBloodPlasma, _outputFieldLiverCell);
      }

      [Observation]
      public void should_return_null()
      {
         sut.CreateFor(_pivotResult).ShouldBeNull();
      }
   }

   public class When_creating_the_time_profile_chart_data_based_on_data_that_contain_null_values : concern_for_TimeProfileChartsDataCreator
   {
      private PivotResult _pivotResult;
      private ChartData<TimeProfileXValue, TimeProfileYValue> _chartData;

      protected override void Context()
      {
         base.Context();
         _statisticalAnalysis.SetPosition(_outputFieldVenousBloodPlasma, PivotArea.DataArea, 0);
         _pivotResult = ChartDataHelperForSpecs.CreateOutputResults(_statisticalAnalysis, _genderField, _outputFieldVenousBloodPlasma, _outputFieldLiverCell);
         foreach (DataRow dataRow in _pivotResult.PivotedData.Rows)
         {
            dataRow[_pivotResult.AggregationName] = DBNull.Value;
         }
      }

      protected override void Because()
      {
         _chartData = sut.CreateFor(_pivotResult);
      }

      [Observation]
      public void should_not_crash()
      {
         _chartData.ShouldNotBeNull();
      }
   }
}