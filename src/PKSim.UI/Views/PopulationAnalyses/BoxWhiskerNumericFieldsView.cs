using PKSim.Assets;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Assets;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;

namespace PKSim.UI.Views.PopulationAnalyses
{
   public partial class BoxWhiskerNumericFieldsView : BaseUserControl, IBoxWhiskerNumericFieldsView
   {
      private readonly IToolTipCreator _toolTipCreator;
      private IBoxWhiskerNumericFieldsPresenter _presenter;
      private readonly ScreenBinder<BoxWhiskerNumericFieldDTO> _screenBinder;

      public BoxWhiskerNumericFieldsView(IToolTipCreator toolTipCreator)
      {
         _toolTipCreator = toolTipCreator;
         InitializeComponent();
         _screenBinder = new ScreenBinder<BoxWhiskerNumericFieldDTO>();
      }

      public void AttachPresenter(IBoxWhiskerNumericFieldsPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeBinding()
      {
         _screenBinder.Bind(x => x.ShowOutliers)
            .To(chkShowOutliers)
            .WithCaption(PKSimConstants.UI.ShowOutliers)
            .Changed += () => OnEvent(_presenter.ShowOutliersChanged);
      }

      public void AddFieldSelectionView(IView view)
      {
         panelNumericFields.FillWith(view);
      }

      public void BindTo(BoxWhiskerNumericFieldDTO dto)
      {
         _screenBinder.BindToSource(dto);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemPanelNumericFields.TextVisible = false;
         chkShowOutliers.SuperTip = _toolTipCreator.CreateToolTip(PKSimConstants.UI.ShowOutliersToolTip, PKSimConstants.UI.ShowOutliers, ApplicationIcons.BoxWhiskerAnalysis);
      }
   }
}