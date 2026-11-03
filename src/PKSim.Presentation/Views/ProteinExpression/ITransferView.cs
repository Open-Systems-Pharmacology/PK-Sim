using System.Collections.Generic;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.ProteinExpression;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.ProteinExpression
{
   public interface ITransferView : IView<ITransferPresenter>
   {
      void SetData(IReadOnlyList<UnitExpression> unitExpressions, string selectedUnit);
      bool HasData();
      string GetSelectedUnit();
   }
}
