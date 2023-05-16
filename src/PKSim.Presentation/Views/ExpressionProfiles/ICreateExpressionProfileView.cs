using OSPSuite.Presentation.Views;
using PKSim.Presentation.DTO.ExpressionProfiles;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Presenters.ExpressionProfiles;

namespace PKSim.Presentation.Views.ExpressionProfiles
{
   public interface ICreateExpressionProfileView : IModalView<ICloneExpressionProfilePresenter>, IModalView<ICreateExpressionProfilePresenter>
   {
      void BindTo(ExpressionProfileDTO expressionProfileDTO);
      void AddDiseaseStateView(IView view);
      void UpdateDiseaseStateVisibility(bool hasAtLeastTwo);
   }
}