using System.Data;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Simulations
{
   public interface IIndividualPKAnalysisView : IView<IIndividualPKAnalysisPresenter>
   {
      bool ShowControls { set; }
      void AddGlobalPKAnalysisView(IGlobalPKAnalysisView view);
      void BindTo(PKAnalysisDTO pkAnalysisDTO);
      DataTable GetSummaryData();
      bool GlobalPKVisible { set; }
   }
}