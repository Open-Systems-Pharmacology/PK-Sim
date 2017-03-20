using PKSim.Assets;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Assets;

namespace PKSim.UI.Views.PopulationAnalyses
{
   public partial class ScatterAnalysisResultsView : PivotAnalysisTwoPanesView, IScatterAnalysisResultsView
   {
      public ScatterAnalysisResultsView() : base(PKSimConstants.UI.ScatterAnalysis, ApplicationIcons.ScatterAnalysis)
      {
         InitializeComponent();
      }

      public void AttachPresenter(IScatterAnalysisResultsPresenter presenter)
      {
         _presenter = presenter;
      }
   }
}