using System.Collections.Generic;
using OSPSuite.Presentation.Views;
using PKSim.Presentation.DTO.Individuals;
using PKSim.Presentation.Presenters.Individuals;

namespace PKSim.Presentation.Views.Individuals
{
   public interface IExpressionProfileSelectionView : IModalView<IExpressionProfileSelectionPresenter>
   {
      void BindTo(ExpressionProfileSelectionDTO expressionProfileSelectionDTO);
      void RefreshList();
   }
}