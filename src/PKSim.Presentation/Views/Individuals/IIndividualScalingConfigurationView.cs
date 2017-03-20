using System.Collections.Generic;

using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Presenters.Individuals;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Individuals
{
   public interface IIndividualScalingConfigurationView : IView<IIndividualScalingConfigurationPresenter>
   {
      void BindTo(IEnumerable<ParameterScalingDTO> parameterScalingsDTO);
      void BindToWeight();
      bool WeightVisible { get; set; }
   }
}