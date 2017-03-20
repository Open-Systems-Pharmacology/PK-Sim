using System.Collections.Generic;

using PKSim.Presentation.DTO.Compounds;

namespace PKSim.Presentation.Views.Compounds
{
   public interface IFractionUnboundGroupView : ICompoundParameterGroupWithAlternativeView
   {
      void BindTo(IReadOnlyCollection<FractionUnboundAlternativeDTO> fractionUnboundDtos);
   }
}