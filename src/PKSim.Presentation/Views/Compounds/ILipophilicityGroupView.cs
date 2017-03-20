using System.Collections.Generic;

using PKSim.Presentation.DTO.Compounds;

namespace PKSim.Presentation.Views.Compounds
{
   public interface ILipophilicityGroupView : ICompoundParameterGroupWithAlternativeView
   {
      void BindTo(IReadOnlyCollection<LipophilictyAlternativeDTO> lipophilicityDTOs);
   }
}