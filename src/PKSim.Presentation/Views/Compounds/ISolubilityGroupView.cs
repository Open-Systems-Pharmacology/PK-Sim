using System.Collections.Generic;

using PKSim.Presentation.DTO.Compounds;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Compounds
{
   public interface ISolubilityGroupView : ICompoundParameterGroupWithAlternativeView
   {
      void BindTo(IReadOnlyCollection<SolubilityAlternativeDTO> solubilityAlternativeDTOs);

      void SetChartView(IView view);
   }
}