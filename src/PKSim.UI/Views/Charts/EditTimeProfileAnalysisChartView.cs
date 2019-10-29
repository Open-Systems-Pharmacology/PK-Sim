using OSPSuite.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Presentation.Presenters.Charts;
using PKSim.Presentation.Views.Charts;
using OSPSuite.Presentation;

namespace PKSim.UI.Views.Charts
{
   public partial class EditTimeProfileAnalysisChartView : BasePKAnalysisWithChartView, IEditTimeProfileAnalysisChartView
   {
      private ApplicationIcon _icon;

      public EditTimeProfileAnalysisChartView()
      {
         InitializeComponent();
         _icon = ApplicationIcons.TimeProfileAnalysis;
      }

      public override ApplicationIcon ApplicationIcon
      {
         get => _icon;
         set => _icon = value;
      }

      public void UpdateIcon(ApplicationIcon icon)
      {
         _icon = icon;
      }

      public void AttachPresenter(IEditPopulationAnalysisChartPresenter presenter)
      {
         AttachPresenter(presenter.DowncastTo<IPKAnalysisWithChartPresenter>());
      }

      public void AttachPresenter(IEditTimeProfileAnalysisChartPresenter presenter)
      {
         AttachPresenter(presenter.DowncastTo<IEditPopulationAnalysisChartPresenter>());
      }
   }
}