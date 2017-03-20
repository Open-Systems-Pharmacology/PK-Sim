using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.PopulationAnalyses
{
   public interface INumberOfBinsGroupingView : IView<INumberOfBinsGroupingPresenter>
   {
      void BindTo(BinSizeGroupingDTO binSizeGroupingDTO);
      void RefreshLabels();

   }
}