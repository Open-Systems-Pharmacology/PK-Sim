using System;
using System.Data;
using PKSim.Presentation.Views.ProteinExpression;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.ProteinExpression
{
   public interface IExpressionDataPresenter : IExpressionItemPresenter
   {
      void EditMapping();
      event Action OnEditMapping;

      void SetData(string symbol, DataTable expressionData, string selectedUnit);
      void SetLayoutSetting(string layoutSettings);
      string GetLayoutSetting();

      string GetFilterInformation();
      DataTable GetSelectedData();
      void SetSelectedUnit(string unit);
      string GetSelectedUnit();
      void ActualizeData(DataTable newData);
   }

   public class ExpressionDataPresenter : AbstractSubPresenter<IExpressionDataView, IExpressionDataPresenter>, IExpressionDataPresenter
   {
      public event Action OnEditMapping = delegate { };

      public ExpressionDataPresenter(IExpressionDataView view) : base(view)
      {
      }

      public void EditMapping()
      {
         OnEditMapping();
      }

      public void SetData(string symbol, DataTable expressionData, string selectedUnit)
      {
         View.SetData(symbol, expressionData, selectedUnit);
      }

      public void SetLayoutSetting(string layoutSettings)
      {
         View.SetLayoutSettings(layoutSettings);
      }

      public string GetLayoutSetting()
      {
         return View.GetLayoutSettings();
      }

      public string GetFilterInformation()
      {
         return View.GetFilterInformation();
      }

      public DataTable GetSelectedData()
      {
         return View.GetSelectedData();
      }

      public void SetSelectedUnit(string unit)
      {
         View.SetSelectedUnit(unit);
      }

      public string GetSelectedUnit()
      {
         return View.GetSelectedUnit();
      }

      public void ActualizeData(DataTable newData)
      {
         View.ActualizeData(newData);
      }
   }
}