using System;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Views.Parameters;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Presenters.Charts;

namespace PKSim.Presentation.Presenters.Parameters
{
   public interface IEditTableParameterPresenter : IDisposablePresenter, IPresenter<IEditTableParameterView>
   {
      bool Edit(IParameter tableParameter);
      TableFormula EditedFormula { get; }
   }

   public class EditTableParameterPresenter : AbstractDisposablePresenter<IEditTableParameterView, IEditTableParameterPresenter>, IEditTableParameterPresenter
   {
      private readonly ITableParameterPresenter _tableParameterPresenter;
      private readonly IFullPathDisplayResolver _fullPathDisplayResolver;
      private readonly ISimpleChartPresenter _chartPresenter;

      public EditTableParameterPresenter(IEditTableParameterView view, ITableParameterPresenter tableParameterPresenter,
         IFullPathDisplayResolver fullPathDisplayResolver, ISimpleChartPresenter chartPresenter) : base(view)
      {
         _tableParameterPresenter = tableParameterPresenter;
         _fullPathDisplayResolver = fullPathDisplayResolver;
         _chartPresenter = chartPresenter;
         _view.AddView(tableParameterPresenter.BaseView);
         _view.AddChart(_chartPresenter.BaseView);
         _tableParameterPresenter.StatusChanged += tableFormulaChanged;
      }

      public bool Edit(IParameter tableParameter)
      {
         _tableParameterPresenter.Edit(tableParameter);
         _view.Caption = PKSimConstants.UI.EditTableParameter(_fullPathDisplayResolver.FullPathFor(tableParameter), tableParameter.Editable);
         _view.CancelVisible = tableParameter.Editable;
         plotTable();
         _view.Display();

         return tableParameter.Editable && !_view.Canceled;
      }

      private void tableFormulaChanged(object sender, EventArgs eventArgs)
      {
         plotTable();
      }

      private void plotTable()
      {
         _chartPresenter.Plot(EditedFormula);
      }

      public TableFormula EditedFormula
      {
         get { return _tableParameterPresenter.EditedFormula; }
      }
   }
}