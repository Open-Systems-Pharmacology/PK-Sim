using System.Data;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Simulations
{
   public interface IPopulationPKAnalysisView : IView<IPopulationPKAnalysisPresenter>
   {
      void BindTo(PKAnalysisDTO pkAnalysisOnCurveDTO, PKAnalysisDTO pkAnalysisOnIndividualsDTO);
      DataTable GetSummaryData();
      void AddGlobalPKAnalysisView(IGlobalPKAnalysisView view);
      bool IsOnCurvesSelected { get; }
   }
}