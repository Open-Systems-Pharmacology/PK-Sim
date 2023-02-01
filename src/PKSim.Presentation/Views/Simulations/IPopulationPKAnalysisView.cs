using System.Data;
using OSPSuite.Presentation.Views;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;

namespace PKSim.Presentation.Views.Simulations
{
   public interface IPKAnalysisView
   {
      void AddGlobalPKAnalysisView(IGlobalPKAnalysisView view);
      bool GlobalPKVisible { set; }
      DataTable GetSummaryData();
   }

   public interface IPopulationPKAnalysisView : IView<IPopulationPKAnalysisPresenter>, IPKAnalysisView
   {
      void BindTo(IntegratedPKAnalysisDTO pkAnalysisDTO);

      bool IsAggregatedPKValuesSelected { get; }
      void ShowPKAnalysisIndividualPKValues(bool visible);
   }
}