using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using NUnit.Framework;
using PKSim.Core.Chart;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_BoxWhiskerCurveData : ContextSpecification<CurveData<BoxWhiskerXValue, BoxWhiskerYValue>>
   {
      protected PaneData<BoxWhiskerXValue, BoxWhiskerYValue> _paneData;

      protected override void Context()
      {
         var chartData = ChartDataHelperForSpecs.CreateBoxWhiskerChartData();
         _paneData = ChartDataHelperForSpecs.CreateBoxWhiskerPaneData11(chartData);
         var name = "Male";
         sut = new CurveData<BoxWhiskerXValue, BoxWhiskerYValue>(new Dictionary<string, string>{{name,name}});
         sut.Id = name;
         sut.Caption = name;
         sut.Pane = _paneData;
      }
   }

   public class When_calling_CurveData_constructor : concern_for_BoxWhiskerCurveData
   {
      [Observation]
      public void should_XValues_count_be_0()
      {
         sut.XValues.Count.ShouldBeEqualTo(0);
      }

      [Observation]
      public void should_YValues_count_be_0()
      {
         sut.YValues.Count.ShouldBeEqualTo(0);
      }

      [Observation]
      public void should_Name_be_set()
      {
         sut.Id.ShouldBeEqualTo("Male");
      }

      [Observation]
      public void should_Pane_be_set()
      {
         sut.Pane.ShouldBeEqualTo(_paneData);
      }
   }

   public class When_calling_add : concern_for_BoxWhiskerCurveData
   {
      private BoxWhiskerXValue _xValue;
      private BoxWhiskerYValue _yValue;

      protected override void Because()
      {
         _xValue = new BoxWhiskerXValue(new string[] {"Normal", "Young"});
         _yValue = new BoxWhiskerYValue() {LowerWhisker = 1.1F, LowerBox = 1.2F, Median = 1.3F, UpperBox = 1.4F, UpperWhisker = 1.5F};
         sut.Add(_xValue, _yValue);
      }

      [Observation]
      public void should_XValues_count_be_1()
      {
         sut.XValues.Count.ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_YValues_count_be_1()
      {
         sut.YValues.Count.ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_XValues0_be_set()
      {
         sut.XValues[0].ShouldBeEqualTo(_xValue);
      }

      [Observation]
      public void should_YValues0_be_set()
      {
         sut.YValues[0].ShouldBeEqualTo(_yValue);
      }
   }

   public abstract class concern_for_ScatterCurveData : ContextSpecification<CurveData<ScatterXValue, ScatterYValue>>
   {
      protected override void Context()
      {
         var paneData = A.Fake<PaneData<ScatterXValue, ScatterYValue>>();
         var dimension = Constants.Dimension.NO_DIMENSION;
         var xAxis = new AxisData(dimension, dimension.DefaultUnit, Scalings.Linear);
         var yAxis = new AxisData(dimension, dimension.DefaultUnit, Scalings.Linear);
         A.CallTo(() => paneData.ChartAxis).Returns(xAxis);
         A.CallTo(() => paneData.Axis).Returns(yAxis);
         sut = new CurveData<ScatterXValue, ScatterYValue>();
         sut.Pane = paneData;
      }
   }

   public class When_retrieving_the_index_of_the_nearest_point_to_given_coordinates : concern_for_ScatterCurveData
   {
      protected override void Context()
      {
         base.Context();
         sut.Add(new ScatterXValue(1), new ScatterYValue(10));
         sut.Add(new ScatterXValue(2), new ScatterYValue(20));
         sut.Add(new ScatterXValue(2), new ScatterYValue(10));
         sut.Add(new ScatterXValue(2), new ScatterYValue(30));
         sut.Add(new ScatterXValue(2), new ScatterYValue(200));
      }

      [Observation]
      [TestCase(1, 10, 0)]
      [TestCase(2, 10, 2)]
      [TestCase(3, 10, 2)]
      [TestCase(3, 20, 1)]
      [TestCase(2, 50, 3)]
      [TestCase(2, 400, 4)]
      public void should_return_the_expected_index(double x, double y, int index)
      {
         sut.GetPointIndexForDisplayValues(x, y).ShouldBeEqualTo(index);
      }
   }

   public abstract class concern_for_BoxWiskerCurveData : ContextSpecification<CurveData<BoxWhiskerXValue, BoxWhiskerYValue>>
   {
      protected override void Context()
      {
         var paneData = A.Fake<PaneData<BoxWhiskerXValue, BoxWhiskerYValue>>();
         var dimension = Constants.Dimension.NO_DIMENSION;
         var xAxis = new AxisData(dimension, dimension.DefaultUnit, Scalings.Linear);
         var yAxis = new AxisData(dimension, dimension.DefaultUnit, Scalings.Linear);
         A.CallTo(() => paneData.ChartAxis).Returns(xAxis);
         A.CallTo(() => paneData.Axis).Returns(yAxis);
         sut = new CurveData<BoxWhiskerXValue, BoxWhiskerYValue>();
         sut.Pane = paneData;
      }
   }

   public class When_retrieving_the_index_of_the_nearest_point_to_given_coordinates_for_a_box_whisker_plot : concern_for_BoxWiskerCurveData
   {
      protected override void Context()
      {
         base.Context();
         sut.Add(new BoxWhiskerXValue(new[] {"A"}) {X = 1}, new BoxWhiskerYValue {LowerWhisker = 10});
         sut.Add(new BoxWhiskerXValue(new[] {"B"}) {X = 2}, new BoxWhiskerYValue {LowerWhisker = 20});
      }

      [Observation]
      [TestCase(1, 10, 0)]
      [TestCase(2, 20, 1)]
      [TestCase(1, 20, 0)]
      [TestCase(2, 10, 1)]
      public void should_return_the_expected_index(double x, double y, int index)
      {
         sut.GetPointIndexForDisplayValues(x, y).ShouldBeEqualTo(index);
      }
   }
}