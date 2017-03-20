
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Presenters.Parameters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Parameters
{
   public interface IScaleParametersView : IView<IScaleParametersPresenter>
   {
      void BindTo(ParameterScaleWithFactorDTO parameterScaleWithFactorDTO);
      bool ReadOnly {  set; }
   }
}