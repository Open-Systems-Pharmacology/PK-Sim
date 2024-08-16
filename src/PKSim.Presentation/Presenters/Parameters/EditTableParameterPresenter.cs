using System;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.Charts;
using PKSim.Presentation.Views.Parameters;

namespace PKSim.Presentation.Presenters.Parameters
{
   public interface IEditTableParameterPresenter : IDisposablePresenter, IPresenter<IEditTableParameterView>
   {
      bool Edit(IParameter tableParameter);
      TableFormula EditedFormula { get; }
   }

   public abstract class EditTableParameterPresenter<T> :
      AbstractDisposablePresenter<IEditTableParameterView, IEditTableParameterPresenter>, IEditTableParameterPresenter where T : ITableParameterPresenter
   {
      private readonly T _tableParameterPresenter;
      private readonly IFullPathDisplayResolver _fullPathDisplayResolver;
      private readonly ISimpleChartPresenter _chartPresenter;

      protected EditTableParameterPresenter(
         IEditTableParameterView view,
         T tableParameterPresenter,
         IFullPathDisplayResolver fullPathDisplayResolver,
         ISimpleChartPresenter chartPresenter) : base(view)
      {
         _tableParameterPresenter = tableParameterPresenter;
         _fullPathDisplayResolver = fullPathDisplayResolver;
         _chartPresenter = chartPresenter;
         _view.AddView(tableParameterPresenter.BaseView);
         _view.AddChart(_chartPresenter.BaseView);
         AddSubPresenters(chartPresenter, tableParameterPresenter);
         _tableParameterPresenter.StatusChanged += tableFormulaChanged;
      }

      public bool Edit(IParameter tableParameter)
      {
         _tableParameterPresenter.Edit(tableParameter);
         _view.Caption = Captions.EditTableParameter(_fullPathDisplayResolver.FullPathFor(tableParameter), tableParameter.Editable);
         _view.CancelVisible = tableParameter.Editable;
         plotTable();
         _view.Display();

         return tableParameter.Editable && !_view.Canceled;
      }

      private void tableFormulaChanged(object sender, EventArgs eventArgs)
      {
         plotTable();
         View.OkEnabled = CanClose;
      }

      private void plotTable() => _chartPresenter.Plot(EditedFormula);

      public TableFormula EditedFormula => _tableParameterPresenter.EditedFormula;
   }

   public class EditTableParameterPresenter : EditTableParameterPresenter<ITableParameterPresenter>
   {
      public EditTableParameterPresenter(IEditTableParameterView view, ITableParameterPresenter tableParameterPresenter, IFullPathDisplayResolver fullPathDisplayResolver, ISimpleChartPresenter chartPresenter) : base(view, tableParameterPresenter, fullPathDisplayResolver, chartPresenter)
      {
      }
   }
}
