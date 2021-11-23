using OSPSuite.Presentation.Views;
using PKSim.Presentation.DTO.ExpressionProfiles;
using PKSim.Presentation.Presenters;

namespace PKSim.Presentation.Views.ExpressionProfiles
{
   public interface ICloneExpressionProfileView : IModalView<ICloneExpressionProfilePresenter>
   {
      void BindTo(ExpressionProfileDTO expressionProfileDTO);
   }
}