using System.Data;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Simulations
{
   public interface IPopulationPKAnalysisView : IView<IPopulationPKAnalysisPresenter>
   {
      void BindTo(IntegratedPKAnalysisDTO pkAnalysisDTO);
      DataTable GetSummaryData();
      void AddGlobalPKAnalysisView(IGlobalPKAnalysisView view);
      bool IsAggregatedPKValuesSelected { get; }
      void ShowPKAnalysisIndividualPKValues(bool visible);
   }
}