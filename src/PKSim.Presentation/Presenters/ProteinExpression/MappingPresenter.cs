using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using OSPSuite.Presentation.Presenters;
using PKSim.Presentation.Views.ProteinExpression;

namespace PKSim.Presentation.Presenters.ProteinExpression
{
   public interface IMappingPresenter : IPresenter<IMappingView>, IDisposablePresenter
   {
      void EditMapping(DataTable mappingTable, DataTable containerTable, IEnumerable<string> tissues);
      void SaveMapping(DataTable dataTable);
      void CancelChanged(DataTable dataTable);
      event Action MappingChanged;
   }

   public class MappingPresenter : AbstractDisposablePresenter<IMappingView, IMappingPresenter>, IMappingPresenter
   {
      public event Action MappingChanged = delegate { };

      public MappingPresenter(IMappingView view) : base(view)
      {
      }

      public void EditMapping(DataTable mappingTable, DataTable containerTable, IEnumerable<string> tissues)
      {
         View.SetData(mappingTable, containerTable, tissues.ToList());
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
