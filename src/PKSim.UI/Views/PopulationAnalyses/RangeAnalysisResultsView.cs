using PKSim.Assets;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Assets;

namespace PKSim.UI.Views.PopulationAnalyses
{
   public partial class RangeAnalysisResultsView : PivotAnalysisTwoPanesView, IRangeAnalysisResultsView
   {
      public RangeAnalysisResultsView() : base(PKSimConstants.UI.RangeAnalysis, ApplicationIcons.RangeAnalysis)
      {
         InitializeComponent();
      }

      public void AttachPresenter(IRangeAnalysisResultsPresenter presenter)
      {
         _presenter = presenter;
      }
   }
}