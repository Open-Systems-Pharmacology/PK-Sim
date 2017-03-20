using OSPSuite.UI.Services;
using DevExpress.Utils;
using DevExpress.XtraPivotGrid;
using PKSim.Assets;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;

namespace PKSim.UI.Views.Simulations
{
   public interface IPKAnalysisToolTipManager
   {
      void CreateForPivotGrid(PKAnalysisPivotGridControl pivotGrid);
   }

   public class PKAnalysisToolTipManager : IPKAnalysisToolTipManager
   {
      private PKAnalysisPivotGridControl _pivotGrid;
      private readonly IToolTipCreator _toolTipCreator;
      private readonly IImageListRetriever _imageListRetriever;

      public PKAnalysisToolTipManager(IImageListRetriever imageListRetriever, IToolTipCreator toolTipCreator)
      {
         _toolTipCreator = toolTipCreator;
         _imageListRetriever = imageListRetriever;
      }

      public void CreateForPivotGrid(PKAnalysisPivotGridControl pivotGrid)
      {
         _pivotGrid = pivotGrid;
         var toolTipController = new ToolTipController();
         toolTipController.Initialize(_imageListRetriever);
         toolTipController.GetActiveObjectInfo += onToolTipControllerGetActiveObjectInfo;
         _pivotGrid.ToolTipController = toolTipController;
      }

      private void onToolTipControllerGetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
      {
         if (e.SelectedControl != _pivotGrid)
            return;

         var hi = _pivotGrid.CalcHitInfo(e.ControlMousePosition);

         if (hi.HitTest == PivotGridHitTest.Value)
            showToolTipForParameterNameField(hi, e);

         else if (hi.HitTest == PivotGridHitTest.Cell)
            showToolTipForParameterValueField(hi, e);
      }

      private void showToolTipForParameterValueField(PivotGridHitInfo hi, ToolTipControllerGetActiveObjectInfoEventArgs e)
      {
         var field = hi.CellInfo.DataField;
         if (field != _pivotGrid.ValueField)
            return;

         var ds = hi.CellInfo.CreateDrillDownDataSource();
         var warning = ds.StringValue(PKSimConstants.PKAnalysis.Warning);
         if (string.IsNullOrEmpty(warning))
            return;

         var parameterDisplayName = ds.StringValue(PKSimConstants.PKAnalysis.ParameterDisplayName);
         var superToolTip = _toolTipCreator.WarningToolTip(warning);
         e.Info = _toolTipCreator.ToolTipControlInfoFor(parameterDisplayName, superToolTip);
      }

      private void showToolTipForParameterNameField(PivotGridHitInfo hi, ToolTipControllerGetActiveObjectInfoEventArgs e)
      {
         var field = hi.ValueInfo.Field;
         if (field != _pivotGrid.ParameterField)
            return;

         var ds = hi.ValueInfo.CreateDrillDownDataSource();
         var parameterDisplayName = ds.StringValue(PKSimConstants.PKAnalysis.ParameterDisplayName);
         var description = ds.StringValue(PKSimConstants.PKAnalysis.Description);
         var warning = ds.StringValue(PKSimConstants.PKAnalysis.Warning);
         var superToolTip = _toolTipCreator.ToolTipForPKAnalysis(parameterDisplayName, description, warning);
         e.Info = _toolTipCreator.ToolTipControlInfoFor(parameterDisplayName, superToolTip);
      }
   }
}