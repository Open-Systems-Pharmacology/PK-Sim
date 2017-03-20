using System.Collections.Generic;

using PKSim.Presentation.DTO.Compounds;

namespace PKSim.Presentation.Views.Compounds
{
   public interface IPermeabilityGroupView : ICompoundParameterGroupWithCalculatedDefaultView
   {
      void BindTo(IReadOnlyCollection<PermeabilityAlternativeDTO> permeabilityDTOs);
   }
}