using System.Collections.Generic;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Services;
using OSPSuite.UI.RepositoryItems;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using PKSim.Assets;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Assets;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;

namespace PKSim.UI.Views.PopulationAnalyses
{
   public partial class PopulationAnalysisObservedDataSettingsView : BaseUserControl, IPopulationAnalysisObservedDataSettingsView
   {
      private readonly IToolTipCreator _toolTipCreator;
      private IPopulationAnalysisObservedDataSettingsPresenter _presenter;
      private readonly ScreenBinder<IPopulationAnalysisObservedDataSettingsPresenter> _screenBinder;
      private readonly GridViewBinder<ObservedDataCurveOptionsDTO> _gridViewBinder;
      private readonly RepositoryItem _lineStyleRepository;
      private readonly RepositoryItem _symbolsRepository;
      private readonly RepositoryItem _colorRepository;
      private readonly RepositoryItem _visibleRepository;
      private readonly ToolTipController _toolTipController;

      public PopulationAnalysisObservedDataSettingsView(IToolTipCreator toolTipCreator, IImageListRetriever imageListRetriever)
      {
         _toolTipCreator = toolTipCreator;
         InitializeComponent();
         _screenBinder = new ScreenBinder<IPopulationAnalysisObservedDataSettingsPresenter>();
         _gridViewBinder = new GridViewBinder<ObservedDataCurveOptionsDTO>(gridView);
         gridView.AllowsFiltering = false;
         _lineStyleRepository = new UxRepositoryItemLineStyles(gridView);
         _symbolsRepository = new UxRepositoryItemSymbols(gridView);
         _colorRepository = new UxRepositoryItemColorPickEditWithHistory(gridView);
         _visibleRepository = new UxRepositoryItemCheckEdit(gridView);
         _toolTipController = new ToolTipController();
         _toolTipController.Initialize(imageListRetriever);
         _toolTipController.GetActiveObjectInfo += (o, e) => OnEvent(onToolTipControllerGetActiveObjectInfo, e);
      }

      public void AttachPresenter(IPopulationAnalysisObservedDataSettingsPresenter presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(IEnumerable<ObservedDataCurveOptionsDTO> observedDataCurveOptions)
      {
         _gridViewBinder.BindToSource(observedDataCurveOptions);
         _screenBinder.BindToSource(_presenter);
      }

      public override void InitializeBinding()
      {
         _screenBinder.Bind(x => x.ApplyGroupingToObservedData)
            .To(chkApplyGroupingToObservedData)
            .WithCaption(PKSimConstants.UI.ApplyGroupingToObservedData);

         _gridViewBinder.AutoBind(x => x.Caption);

         _gridViewBinder.AutoBind(x => x.LineStyle)
            .WithRepository(x => _lineStyleRepository)
            .WithShowButton(ShowButtonModeEnum.ShowAlways);

         _gridViewBinder.AutoBind(x => x.Symbol)
            .WithRepository(x => _symbolsRepository)
            .WithShowButton(ShowButtonModeEnum.ShowAlways);

         _gridViewBinder.AutoBind(x => x.Color)
            .WithRepository(x => _colorRepository);

         _gridViewBinder.AutoBind(x => x.Visible)
            .WithRepository(x => _visibleRepository);

         RegisterValidationFor(_screenBinder, statusChangedNotify: NotifyViewChanged);
         _gridViewBinder.Changed += NotifyViewChanged;
      }

      private void onToolTipControllerGetActiveObjectInfo(ToolTipControllerGetActiveObjectInfoEventArgs e)
      {
         if (e.SelectedControl != chkApplyGroupingToObservedData)
            return;

         var superToolTip = _toolTipCreator.CreateToolTip(PKSimConstants.UI.ApplyGroupingToObservedDataToolTip, PKSimConstants.UI.ApplyGroupingToObservedData, ApplicationIcons.Info);
         e.Info = _toolTipCreator.ToolTipControlInfoFor(chkApplyGroupingToObservedData, superToolTip);
      }

      public override bool HasError
      {
         get { return _screenBinder.HasError || _gridViewBinder.HasError; }
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         chkApplyGroupingToObservedData.ToolTipController = _toolTipController;
      }
   }
}