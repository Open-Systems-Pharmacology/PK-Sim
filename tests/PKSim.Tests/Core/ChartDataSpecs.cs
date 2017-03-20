using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Chart;
using OSPSuite.Core.Domain;
using Helper = PKSim.Core.ChartDataHelperForSpecs;

namespace PKSim.Core
{
   public abstract class concern_for_ChartData : ContextSpecification<ChartData<BoxWhiskerXValue, BoxWhiskerYValue>>
   {
      protected override void Context()
      {
         sut = new ChartData<BoxWhiskerXValue, BoxWhiskerYValue>(null, ChartDataHelperForSpecs.FieldValueComparersR1(), Helper.XValueNames, Helper.XValueComparer());
      }
   }

   public class When_calling_ChartData_constructor : concern_for_ChartData
   {
      [Observation]
      public void should_XValueNames_be_set()
      {
         sut.XFieldNames.ShouldBeEqualTo(Helper.XValueNames);
      }

      [Observation]
      public void should_count_be_0()
      {
         sut.Panes.Count().ShouldBeEqualTo(0);
      }
   }

   public class When_adding_PaneData : concern_for_ChartData
   {
      [Observation]
      public void should_contain_data()
      {
         var paneData1 = Helper.CreateBoxWhiskerPaneData11(sut);
         var paneData2 = Helper.CreateBoxWhiskerPaneData12(sut);
         sut.AddPane(paneData2);
         sut.AddPane(paneData1);
         sut.Panes.Count().ShouldBeEqualTo(2);
         sut.Panes.FindById("AUC").ShouldBeEqualTo(paneData1);
         sut.Panes.FindById("Cmax").ShouldBeEqualTo(paneData2);
      }
   }

   public abstract class concern_for_filled_ChartData : concern_for_ChartData
   {
      protected PaneData<BoxWhiskerXValue, BoxWhiskerYValue> _paneData1;
      protected PaneData<BoxWhiskerXValue, BoxWhiskerYValue> _paneData2;

      protected override void Context()
      {
         base.Context();
         _paneData1 = Helper.CreateBoxWhiskerPaneData11(sut);
         _paneData2 = Helper.CreateBoxWhiskerPaneData12(sut);
         sut.AddPane(_paneData2);
         sut.AddPane(_paneData1);
      }
   }

   public class When_creating_an_x_order_for_box_whisker_values : concern_for_filled_ChartData
   {
      [Observation]
      public void XValues_should_have_the_expected_order()
      {
         sut.CreateXOrder();

         var curve_AUC_Male = sut.Panes["AUC"].Curves["Male"];
         Helper.ShouldBeEqual(curve_AUC_Male.XValues[0], "Thick", "Young", 3);
         Helper.ShouldBeEqual(curve_AUC_Male.XValues[1], "Normal", "Young", 1);
         Helper.ShouldBeEqual(curve_AUC_Male.XValues[2], "Normal", "Old", 2);
         curve_AUC_Male.YValues[1].Median.ShouldBeEqualTo(121.3F);

         var curve_AUC_Female = sut.Panes["AUC"].Curves["Female"];
         Helper.ShouldBeEqual(curve_AUC_Female.XValues[0], "Normal", "Young", 1);
         Helper.ShouldBeEqual(curve_AUC_Female.XValues[1], "Thick", "Old", 4);

         var curve_Cmax_Male = sut.Panes["Cmax"].Curves["Male"];
         Helper.ShouldBeEqual(curve_Cmax_Male.XValues[0], "Normal", "Old", 2);
         Helper.ShouldBeEqual(curve_Cmax_Male.XValues[1], "Thin", "Young", 0);
      }
   }

   public class When_creating_an_pane_order_for_box_whisker_values : concern_for_filled_ChartData
   {
      [Observation]
      public void panes_should_have_the_expected_order()
      {
         sut.CreatePaneOrder(); //string alphabetical descending

         sut.Panes.FirstOrDefault().ShouldBeEqualTo(_paneData2); //paneId = Cmax
         sut.Panes.LastOrDefault().ShouldBeEqualTo(_paneData1); //paneId = AUC
      }
   }
}