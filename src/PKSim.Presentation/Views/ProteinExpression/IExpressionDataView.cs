using System.Data;
using PKSim.Presentation.Presenters.ProteinExpression;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.ProteinExpression
{
   public interface IExpressionDataView : IView<IExpressionDataPresenter>
   {
      void SetData(string proteinName, DataTable expressionDataTable, string selectedUnit);
      void SetLayoutSettings(string layoutSettings);
      string GetLayoutSettings();
      void ActualizeData(DataTable expressionDataTable);
      DataTable GetSelectedData();
      string GetSelectedUnit();
      void SetSelectedUnit(string unit);
      string GetFilterInformation();
      string GetMappingInformation();
   }
}