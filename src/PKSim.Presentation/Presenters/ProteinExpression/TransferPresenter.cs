using System.Collections.Generic;
using OSPSuite.Presentation.Presenters;
using PKSim.Core.Model;
using PKSim.Presentation.Views.ProteinExpression;

namespace PKSim.Presentation.Presenters.ProteinExpression
{
   public interface ITransferPresenter : IExpressionItemPresenter
   {
      bool HasData();
      string GetSelectedUnit();
      void SetData(IReadOnlyList<UnitExpression> unitExpressions, string selectedUnit);
      bool ShowOldValues { get; set; }
   }

   public class TransferPresenter : AbstractSubPresenter<ITransferView, ITransferPresenter>, ITransferPresenter
   {
      public TransferPresenter(ITransferView view) : base(view)
      {
      }

      public bool HasData()
      {
         return View.HasData();
      }

      public string GetSelectedUnit()
      {
         return View.GetSelectedUnit();
      }

      public void SetData(IReadOnlyList<UnitExpression> unitExpressions, string selectedUnit) => View.SetData(unitExpressions, selectedUnit);

      public bool ShowOldValues { get; set; }


   }
}
