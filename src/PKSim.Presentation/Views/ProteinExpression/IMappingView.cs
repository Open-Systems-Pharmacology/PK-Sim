using System.Collections;
using System.Data;
using PKSim.Presentation.Presenters.ProteinExpression;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.ProteinExpression
{
   public interface IMappingView : IModalView<IMappingPresenter>
   {
      void SetData(DataTable mapping, DataTable containerTable, ICollection tissueLov);
      void Hide();
   }
}