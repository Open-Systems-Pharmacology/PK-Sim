using System.Data;
using PKSim.Presentation.Presenters.ProteinExpression;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.ProteinExpression
{
   public interface ITransferView : IView<ITransferPresenter>
   {
      void SetData(DataTable transferTable, string selectedUnit);
      DataTable GetData();
      bool HasData();
      string GetSelectedUnit();
   }
}