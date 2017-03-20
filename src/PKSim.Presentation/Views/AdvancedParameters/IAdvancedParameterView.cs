using PKSim.Presentation.DTO.Populations;
using OSPSuite.Presentation.Views;

using PKSim.Presentation.Presenters.AdvancedParameters;

namespace PKSim.Presentation.Views.AdvancedParameters
{
   public interface IAdvancedParameterView : IView<IAdvancedParameterPresenter>
   {
      void BindTo(AdvancedParameterDTO advancedParameterDTO);
      void AddParameterView(IView view);
      void DeleteBinding();
   }
}