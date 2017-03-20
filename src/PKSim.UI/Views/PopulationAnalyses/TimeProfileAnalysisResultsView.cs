using PKSim.Assets;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Assets;

namespace PKSim.UI.Views.PopulationAnalyses
{
   public partial class TimeProfileAnalysisResultsView : PivotAnalysisTwoPanesView, ITimeProfileAnalysisResultsView
   {
      public TimeProfileAnalysisResultsView() : base(PKSimConstants.UI.TimeProfileAnalysis, ApplicationIcons.TimeProfileAnalysis)
      {
         InitializeComponent();
      }

      public void AttachPresenter(ITimeProfileAnalysisResultsPresenter presenter)
      {
         _presenter = presenter;
      }
   }
}