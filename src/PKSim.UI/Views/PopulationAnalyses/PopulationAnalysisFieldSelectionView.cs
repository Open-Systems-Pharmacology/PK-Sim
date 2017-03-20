using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;

namespace PKSim.UI.Views.PopulationAnalyses
{
   public partial class PopulationAnalysisFieldSelectionView : BaseUserControl, IPopulationAnalysisFieldSelectionView
   {

      public PopulationAnalysisFieldSelectionView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(IPopulationAnalysisFieldSelectionPresenter presenter)
      {
         /*nothing to do*/
      }

      public void SetArrangementFieldView(IView view)
      {
         panelArrangement.FillWith(view);
      }

      public void SetDataFieldView(IView view)
      {
         panelData.FillWith(view);
      }
   }
}