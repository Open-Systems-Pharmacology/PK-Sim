using PKSim.Assets;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Assets;

namespace PKSim.UI.Views.PopulationAnalyses
{
   public partial class BoxWhiskerAnalysisResultsView : PivotAnalysisTwoPanesView, IBoxWhiskerAnalysisResultsView
   {
      public BoxWhiskerAnalysisResultsView() : base(PKSimConstants.UI.BoxWhiskerAnalysis, ApplicationIcons.BoxWhiskerAnalysis)
      {
         InitializeComponent();
      }

      public void AttachPresenter(IBoxWhiskerAnalysisResultsPresenter presenter)
      {
         _presenter = presenter;
      }
   }
}