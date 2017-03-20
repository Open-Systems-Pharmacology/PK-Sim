using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Chart;

namespace PKSim.Core
{
   public abstract class concern_for_PaneData : ContextSpecification<PaneData<BoxWhiskerXValue, BoxWhiskerYValue>>
   {
      protected ChartData<BoxWhiskerXValue, BoxWhiskerYValue> _chartData;
      protected AxisData _axisData;

      protected override void Context()
      {
         _chartData = ChartDataHelperForSpecs.CreateBoxWhiskerChartData();
         var name = "TMax";
         _axisData = ChartDataHelperForSpecs.CreateAxisData(name);
         sut = new PaneData<BoxWhiskerXValue, BoxWhiskerYValue>(_axisData, new Dictionary<string, string> {{name, name}}, ChartDataHelperForSpecs.FieldValueComparersR1());
         sut.Caption = name;
         sut.Id = name;
         sut.Chart=_chartData;
      }
   }

   public class When_calling_PaneData_constructor : concern_for_PaneData
   {
      [Observation]
      public void should_Name_be_set()
      {
         sut.Id.ShouldBeEqualTo("TMax");
      }

      [Observation]
      public void should_Chart_be_set()
      {
         sut.Chart.ShouldBeEqualTo(_chartData);
      }

      [Observation]
      public void should_count_be_0()
      {
         sut.Curves.Count.ShouldBeEqualTo(0);
      }
   }

   public class When_adding_CurveData : concern_for_PaneData
   {
      [Observation]
      public void should_contain_data()
      {
         var curveData1 = ChartDataHelperForSpecs.CreateBoxWhiskerCurveData111(sut);
         var curveData2 = ChartDataHelperForSpecs.CreateBoxWhiskerCurveData112(sut);
         sut.AddCurve(curveData1);
         sut.AddCurve(curveData2);
         sut.Curves.Count.ShouldBeEqualTo(2);
         sut.Curves["Male"].ShouldBeEqualTo(curveData1);
         sut.Curves["Female"].ShouldBeEqualTo(curveData2);
      }
   }
}