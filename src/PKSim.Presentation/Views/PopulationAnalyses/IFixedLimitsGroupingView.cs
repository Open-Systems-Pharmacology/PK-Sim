using System.Collections.Generic;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.PopulationAnalyses
{
   public interface IFixedLimitsGroupingView : IView<IFixedLimitsGroupingPresenter>
   {
      void BindTo(IEnumerable<FixedLimitGroupingDTO> fixedLimitDTOs, Unit fieldUnit);
   }
}