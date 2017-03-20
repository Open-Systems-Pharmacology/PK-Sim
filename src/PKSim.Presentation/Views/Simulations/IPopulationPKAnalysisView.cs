using System.Data;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Simulations
{
   public interface IPopulationPKAnalysisView : IView<IPopulationPKAnalysisPresenter>
   {
      void BindTo(PKAnalysisDTO dataTable);
      DataTable GetSummaryData();
   }
}