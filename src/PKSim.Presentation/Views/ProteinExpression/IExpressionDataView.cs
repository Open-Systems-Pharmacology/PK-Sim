using System.Data;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.ProteinExpression;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.ProteinExpression
{
   public interface IExpressionDataView : IView<IExpressionDataPresenter>
   {
      void SetData(string proteinName, DataTable expressionDataTable);

      /// <summary>
      ///    Restores the layout of the view. Returns the criteria of the layout filter that the field filters do not express,
      ///    or an empty string. Such criteria are removed from the view.
      /// </summary>
      string SetLayoutSettings(string layoutSettings);

      string GetLayoutSettings();
      void ActualizeData(DataTable expressionDataTable);
      ExpressionDataFilter GetFilter();
      void SetFilter(ExpressionDataFilter filter);
   }
}
