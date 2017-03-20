using System.Data;
using DevExpress.Utils;
using PKSim.Presentation.Views.ProteinExpression;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.ProteinExpression
{
   public interface ITransferPresenter : IExpressionItemPresenter
   {
      DataTable GetData();
      bool HasData();
      string GetSelectedUnit();
      void SetData(DataTable transferData, string selectedUnit);
      bool ShowOldValues { get; set; }
   }

   public class TransferPresenter : AbstractSubPresenter<ITransferView, ITransferPresenter>, ITransferPresenter
   {
      public TransferPresenter(ITransferView view) : base(view)
      {
      }

      public DataTable GetData()
      {
         return View.GetData();
      }

      public bool HasData()
      {
         return View.HasData();
      }

      public string GetSelectedUnit()
      {
         return View.GetSelectedUnit();
      }

      public void SetData(DataTable transferData, string selectedUnit)
      {
         View.SetData(transferData, selectedUnit);
      }

      public bool ShowOldValues { get; set; }


   }
}