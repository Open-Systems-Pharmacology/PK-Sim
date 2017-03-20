using System.Data;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Simulations
{
   public interface IPKAnalysisPivotView : IView
   {
      void BindTo(DataTable dataTable);
      DataTable GetSummaryData();
      void BindUnitsMenuToPresenter(IPKAnalysisPresenter presenter);
   }
}