using System;
using System.Collections.Generic;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Data;
using FakeItEasy;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.PKAnalyses;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Services;

namespace PKSim.Core
{
   public static class ChartDataHelperForSpecs
   {
      static ChartDataHelperForSpecs()
      {
         DimensionTest.AddUnit(UnitTest1000);
         DimensionTest2.AddUnit(UnitTest2000);
      }

      public static ChartData<BoxWhiskerXValue, BoxWhiskerYValue> CreateBoxWhiskerChartData()
      {
         var chart1Data = new ChartData<BoxWhiskerXValue, BoxWhiskerYValue>(null, FieldValueComparersR1(), XValueNames, XValueComparer());

         chart1Data.AddPane(CreateBoxWhiskerPaneData12(chart1Data));
         chart1Data.AddPane(CreateBoxWhiskerPaneData11(chart1Data));

         return chart1Data;
      }

      public static CurveData<BoxWhiskerXValue, BoxWhiskerYValue> CreateBoxWhiskerCurveData111(PaneData<BoxWhiskerXValue, BoxWhiskerYValue> paneData)
      {
         IList<BoxWhiskerXYValue> curve111Values = new List<BoxWhiskerXYValue>()
         {
            new BoxWhiskerXYValue("Thick", "Young", 131.3F),
            new BoxWhiskerXYValue("Normal", "Young", 121.3F),
            new BoxWhiskerXYValue("Normal", "Old", 122.3F),
         };
         return CreateBoxWhiskerCurveData(paneData, "Male", curve111Values);
      }

      public static CurveData<BoxWhiskerXValue, BoxWhiskerYValue> CreateBoxWhiskerCurveData112(PaneData<BoxWhiskerXValue, BoxWhiskerYValue> paneData)
      {
         IList<BoxWhiskerXYValue> curve112Values = new List<BoxWhiskerXYValue>()
         {
            new BoxWhiskerXYValue("Normal", "Young", 221.3F),
            new BoxWhiskerXYValue("Thick", "Old", 232.3F),
         };
         return CreateBoxWhiskerCurveData(paneData, "Female", curve112Values);
      }

      public static CurveData<BoxWhiskerXValue, BoxWhiskerYValue> CreateBoxWhiskerCurveData121(PaneData<BoxWhiskerXValue, BoxWhiskerYValue> paneData)
      {
         IList<BoxWhiskerXYValue> curve121Values = new List<BoxWhiskerXYValue>()
         {
            new BoxWhiskerXYValue("Normal", "Old", 322.3F),
            new BoxWhiskerXYValue("Thin", "Young", 311.3F),
         };
         return CreateBoxWhiskerCurveData(paneData, "Male", curve121Values);
      }

      public static AxisData CreateAxisData(string caption)
      {
         return new AxisData(DimensionTest, UnitTest, Scalings.Log) {Caption = caption};
      }

      public static PaneData<BoxWhiskerXValue, BoxWhiskerYValue> CreateBoxWhiskerPaneData11(ChartData<BoxWhiskerXValue, BoxWhiskerYValue> chartData)
      {
         var name = "AUC";
         var pane11Data = new PaneData<BoxWhiskerXValue, BoxWhiskerYValue>(CreateAxisData(name),new Dictionary<string, string> {{name, name}}, FieldValueComparersR1());
         pane11Data.WithId(name);
         pane11Data.Caption = name;
         pane11Data.AddCurve(CreateBoxWhiskerCurveData112(pane11Data));
         pane11Data.AddCurve(CreateBoxWhiskerCurveData111(pane11Data));

         return pane11Data;
      }

      public static PaneData<BoxWhiskerXValue, BoxWhiskerYValue> CreateBoxWhiskerPaneData12(ChartData<BoxWhiskerXValue, BoxWhiskerYValue> chartData)
      {
         var name = "Cmax";
         var pane12Data = new PaneData<BoxWhiskerXValue, BoxWhiskerYValue>(new AxisData(DimensionTest2, UnitTest2000, Scalings.Log) {Caption=name}, new Dictionary<string, string> { { name, name } }, FieldValueComparersR1());
         pane12Data.WithId(name);
         pane12Data.Caption = name;
         pane12Data.AddCurve(CreateBoxWhiskerCurveData121(pane12Data));

         return pane12Data;
      }

      public static CurveData<BoxWhiskerXValue, BoxWhiskerYValue> CreateBoxWhiskerCurveData(PaneData<BoxWhiskerXValue, BoxWhiskerYValue> paneData, string name, IList<BoxWhiskerXYValue> bwValues)
      {
         var curveData = new CurveData<BoxWhiskerXValue, BoxWhiskerYValue>(new Dictionary<string, string> { { name, name } });
         curveData.Id = name;
         curveData.Caption = name;
         foreach (var v in bwValues)
         {
            var X = new BoxWhiskerXValue(new List<string>() {v.X1, v.X2});
            var Y = new BoxWhiskerYValue() {LowerWhisker = v.LW, LowerBox = v.LW, Median = v.M, UpperBox = v.LW, UpperWhisker = v.LW};
            curveData.Add(X, Y);
         }

         return curveData;
      }

      public static IReadOnlyList<string> XValueNames = new List<string>() {"BMIClass", "AgeClass"};
      public static List<string> XValues1 = new List<string> {"Thin", "Normal", "Thick"};
      public static List<string> XValues2 = new List<string> {"Young", "Old"};
      public static Unit UnitTest = new Unit("UnitTest", 1, 0);
      public static Unit UnitTest1000 = new Unit("UnitTest1000", 1000, 0);
      public static Unit UnitTest2000 = new Unit("UnitTest2000", 1000, 0);
      public static IDimension DimensionTest = new Dimension(new BaseDimensionRepresentation(new double[] {1, 0, 0, 0, 0, 0, 0}), "DimensionTest", "UnitTest");
      public static IDimension DimensionTest2 = new Dimension(new BaseDimensionRepresentation(new double[] {0, 2, 0, 0, 0, 0, 0}), "DimensionTest2", "UnitTest2");

      public static IComparer<BoxWhiskerXValue> XValueComparer()
      {
         return new BoxWhiskerXValueComparer(new[] {new StringPopulationAnalysisFieldForSpecs(XValues1), new StringPopulationAnalysisFieldForSpecs(XValues2)});
      }

      public static void ShouldBeEqual(BoxWhiskerXValue value, string xValue1, string xValue2, float x)
      {
         value[0].ShouldBeEqualTo(xValue1);
         value[1].ShouldBeEqualTo(xValue2);
         value.X.ShouldBeEqualTo(x);
      }

      public static int CompareStringReverse(object x, object y)
      {
         return - string.Compare(x.ToString(), y.ToString(), StringComparison.InvariantCulture);
      }

      public static List<IComparer<object>> FieldValueComparersR1()
      {
         return new List<IComparer<object>> {new StringReverseComparer()};
      }

      public static List<IComparer<object>> FieldValueComparersR2()
      {
         return new List<IComparer<object>> {new StringReverseComparer(), new StringReverseComparer()};
      }

      public class StringPopulationAnalysisFieldForSpecs : PopulationAnalysisFieldBase
      {
         private readonly List<string> _values;

         public StringPopulationAnalysisFieldForSpecs(List<string> values) : base(typeof (string))
         {
            _values = values;
         }

         public override string Id
         {
            get { return string.Empty; }
         }

         //string alphabetical descending
         public override int Compare(object x, object y)
         {
            return _values.IndexOf(x.ToString()) - _values.IndexOf(y.ToString());
         }
      }

      public static PivotResult CreatePivotResult(PopulationPivotAnalysis pivotAnalysis, Aggregate aggregate,
         PopulationAnalysisCovariateField genderFielder,
         PopulationAnalysisCovariateField raceField,
         PopulationAnalysisParameterField bmiField,
         PopulationAnalysisPKParameterField cmaxField)
      {
         var pivotResultCreator = new PivotResultCreator(new Pivoter(), new PopulationAnalysisFlatTableCreator());

         var populationSimulation = A.Fake<IPopulationDataCollector>();
         A.CallTo(() => populationSimulation.NumberOfItems).Returns(3);

         //thin, thin,  big
         A.CallTo(() => populationSimulation.AllValuesFor(bmiField.ParameterPath)).Returns(new List<double> {10, 20, 30});
         A.CallTo(() => populationSimulation.AllCovariateValuesFor(genderFielder.Covariate)).Returns(new List<string> {"Male", "Female", "Male"});
         A.CallTo(() => populationSimulation.AllCovariateValuesFor(raceField.Covariate)).Returns(new List<string> {"US", "EU", "EU"});
         A.CallTo(() => populationSimulation.AllPKParameterValuesFor(cmaxField.QuantityPath, cmaxField.PKParameter)).Returns(new List<double> {900, 600, 1000});
         A.CallTo(() => populationSimulation.AllSimulationNames()).Returns(new List<string> {"Sim", "Sim", "Sim"});

         return pivotResultCreator.Create(pivotAnalysis, populationSimulation, new ObservedDataCollection(), aggregate);
      }

      public static PivotResult CreateOutputResults(PopulationPivotAnalysis analysis, PopulationAnalysisCovariateField genderField, PopulationAnalysisOutputField outputField1,
         PopulationAnalysisOutputField outputField2,
         ObservedDataCollection observedDataCollection = null)
      {
         var populationSimulation = A.Fake<IPopulationDataCollector>();

         var pivotResultCreator = new PivotResultCreator(new Pivoter(), new PopulationAnalysisFlatTableCreator());
         //simulation with 4 time points
         var time = new QuantityValues {Values = new float[] {1, 2, 3, 4}};
         var output11 = createValues(time, 10, 20, 30, 40);
         var output12 = createValues(time, 100, 200, 300, 400);
         var output13 = createValues(time, 1000, 2000, 3000, 4000);
         var output21 = createValues(time, 50, 60, 70, 80);
         var output22 = createValues(time, 500, 600, 700, 800);
         var output23 = createValues(time, 5000, 6000, 7000, 8000);

         A.CallTo(() => populationSimulation.NumberOfItems).Returns(3);
         A.CallTo(() => populationSimulation.AllCovariateValuesFor(CoreConstants.Covariates.GENDER)).Returns(new List<string> {"Male", "Female", "Male"});
         A.CallTo(() => populationSimulation.AllOutputValuesFor(outputField1.QuantityPath)).Returns(new List<QuantityValues> {output11, output12, output13});
         A.CallTo(() => populationSimulation.AllOutputValuesFor(outputField2.QuantityPath)).Returns(new List<QuantityValues> {output21, output22, output23});
         A.CallTo(() => populationSimulation.AllSimulationNames()).Returns(new List<string> {"Sim", "Sim", "Sim"});


         if (observedDataCollection == null)
            observedDataCollection = new ObservedDataCollection();

         return pivotResultCreator.Create(analysis, populationSimulation, observedDataCollection, AggregationFunctions.QuantityAggregation);
      }

      public static PopulationAnalysisCovariateField CreateGenderField()
      {
         var populationSimulation = A.Fake<IPopulationDataCollector>();
         A.CallTo(() => populationSimulation.NumberOfItems).Returns(3);
         A.CallTo(() => populationSimulation.AllCovariateValuesFor(CoreConstants.Covariates.GENDER)).Returns(new List<string> {"Male", "Female", "Male"});

         var entityPathResolver = A.Fake<IEntityPathResolver>();
         var fullPathDisplayResolver = A.Fake<IFullPathDisplayResolver>();
         var genderRepository = A.Fake<IGenderRepository>();
         var pkParameterRepository = A.Fake<IPKParameterRepository>(); 
         var male = new Gender {DisplayName = "Male"};
         var female = new Gender {DisplayName = "Female"};
         A.CallTo(() => genderRepository.Male).Returns(male);
         A.CallTo(() => genderRepository.Female).Returns(female);
         var colorGenerator = A.Fake<IColorGenerator>();
         var populationAnalysisFieldFactory = new PopulationAnalysisFieldFactory(entityPathResolver, fullPathDisplayResolver, genderRepository, colorGenerator,pkParameterRepository);

         return populationAnalysisFieldFactory.CreateFor(CoreConstants.Covariates.GENDER, populationSimulation);
      }

      private static QuantityValues createValues(QuantityValues time, params float[] values)
      {
         return new QuantityValues {Values = values, Time = time};
      }
   }

   public class StringReverseComparer : IComparer<object>
   {
      public int Compare(object x, object y)
      {
         return -string.Compare(x.ToString(), y.ToString(), StringComparison.InvariantCulture);
      }
   }

   public class BoxWhiskerXYValue
   {
      public string X1 { get; private set; }
      public string X2 { get; private set; }
      public float LW { get; private set; }
      public float LB { get; private set; }
      public float M { get; private set; }
      public float UB { get; private set; }
      public float UW { get; private set; }

      public BoxWhiskerXYValue(string xValue1, string xValue2, float median)
      {
         X1 = xValue1;
         X2 = xValue2;
         LW = median - 0.2F;
         LB = median - 0.1F;
         M = median;
         UB = median + 0.1F;
         UW = median + 0.2F;
      }
   }
}