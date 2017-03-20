using System;
using System.Data;
using PKSim.Presentation.Views.ProteinExpression;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.ProteinExpression
{
   public interface IMappingPresenter : IPresenter<IMappingView>, IDisposablePresenter
   {
      void EditMapping(DataTable mappingTable, DataTable containerTable, DataTable expressionDataTable);
      void SaveMapping(DataTable dataTable);
      void CancelChanged(DataTable dataTable);
      event Action MappingChanged;
   }

   public class MappingPresenter : AbstractDisposablePresenter<IMappingView, IMappingPresenter>, IMappingPresenter
   {
      private readonly IProteinExpressionDataHelper _dataHelper;
      public event Action MappingChanged = delegate { };

      public MappingPresenter(IMappingView view, IProteinExpressionDataHelper dataHelper) : base(view)
      {
         _dataHelper = dataHelper;
      }

      public void EditMapping(DataTable mappingTable, DataTable containerTable, DataTable expressionDataTable)
      {
         var tissueLov = _dataHelper.GetDistinctLoV(expressionDataTable.Columns[DatabaseConfiguration.ExpressionDataColumns.COL_TISSUE]);
         View.SetData(mappingTable, containerTable, tissueLov);
         View.Display();
      }

      public void SaveMapping(DataTable dataTable)
      {
         if (dataTable == null) return;
         dataTable.AcceptChanges();
         MappingChanged();
         View.Hide();
      }

      public void CancelChanged(DataTable dataTable)
      {
         if (dataTable == null) return;
         dataTable.RejectChanges();
         View.Hide();
      }

   }
}