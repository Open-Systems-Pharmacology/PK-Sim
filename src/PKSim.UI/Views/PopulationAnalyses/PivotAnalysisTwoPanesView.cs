using OSPSuite.Assets;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.PopulationAnalyses
{
   public partial class PivotAnalysisTwoPanesView : BaseUserControl, IPopulationAnalysisResultsView
   {
      protected IPopulationAnalysisResultsPresenter _presenter;
      private readonly ApplicationIcon _icon;

      public PivotAnalysisTwoPanesView(string caption, ApplicationIcon icon)
      {
         InitializeComponent();
         Caption = caption;
         _icon = icon;
      }

      public override ApplicationIcon ApplicationIcon
      {
         get { return _icon; }
      }

      public void SetChartView(IView view)
      {
         splitContainerControl.Panel2.FillWith(view);
      }

      public void SetFieldSelectionView(IView view)
      {
         splitContainerControl.Panel1.FillWith(view);
      }
   }
}