using System.Collections.Generic;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.PopulationAnalyses
{
   public interface IValueMappingGroupingView : IView<IValueMappingGroupingPresenter>
   {
      void BindTo(IEnumerable<GroupingLabelDTO> groupingLabels);
   }
}