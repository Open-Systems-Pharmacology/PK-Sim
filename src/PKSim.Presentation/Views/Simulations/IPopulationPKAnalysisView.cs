using System.Data;
using OSPSuite.Presentation.Views;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;

namespace PKSim.Presentation.Views.Simulations
{
   public interface IPopulationPKAnalysisView : IView<IPopulationPKAnalysisPresenter>
   {
      void BindTo(IntegratedPKAnalysisDTO pkAnalysisDTO);
      DataTable GetSummaryData();
      void AddGlobalPKAnalysisView(IGlobalPKAnalysisView view);
      bool IsAggregatedPKValuesSelected { get; }
      bool GlobalPKVisible { set; }
      void ShowPKAnalysisIndividualPKValues(bool visible);
   }
}