using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using OSPSuite.Assets;
using OSPSuite.Core.Extensions;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using OSPSuite.UI.Services;
using PKSim.Assets;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using PKSim.UI.Views.Core;
using static OSPSuite.UI.UIConstants.Size;

namespace PKSim.UI.Views.Simulations
{
   public partial class SimulationObserversConfigurationView : BaseUserControlWithValueInGrid, ISimulationObserversConfigurationView
   {
      private readonly IToolTipCreator _toolTipCreator;
      private ISimulationObserversConfigurationPresenter _presenter;

      public override ApplicationIcon ApplicationIcon => ApplicationIcons.Observer;

      public override string Caption => PKSimConstants.UI.SimulationObserversConfiguration;
      private readonly GridViewBinder<ObserverSetMappingDTO> _gridViewBinder;
      private readonly UxRepositoryItemImageComboBox _observerSetRepository;

      private readonly RepositoryItemButtonEdit _removeButtonRepository = new UxRemoveButtonRepository();
      private readonly RepositoryItemButtonEdit _createButtonRepository = new UxRepositoryItemButtonImage(ApplicationIcons.Create, PKSimConstants.UI.CreateBuildingBlockHint(PKSimConstants.ObjectTypes.ObserverSet));
      private readonly RepositoryItemButtonEdit _loadButtonRepository = new UxRepositoryItemButtonImage(ApplicationIcons.LoadFromTemplate, PKSimConstants.UI.LoadItemFromTemplate(PKSimConstants.ObjectTypes.ObserverSet));
      private IGridViewColumn _colCreateObserverSet;
      private IGridViewColumn _colLoadObserverSet;
      private IGridViewColumn _colRemoveObserverSet;

      public SimulationObserversConfigurationView(IToolTipCreator toolTipCreator, IImageListRetriever imageListRetriever)
      {
         _toolTipCreator = toolTipCreator;
         InitializeComponent();
         _observerSetRepository = new UxRepositoryItemImageComboBox(gridView, imageListRetriever) {AllowHtmlDraw = DefaultBoolean.True};
         _gridViewBinder = new GridViewBinder<ObserverSetMappingDTO>(gridView);
         gridView.AllowsFiltering = false;
         InitializeWithGrid(gridView);

         var toolTipController = new ToolTipController();
         toolTipController.Initialize(imageListRetriever);
         toolTipController.GetActiveObjectInfo += (o, e) => OnEvent(onToolTipControllerGetActiveObjectInfo, o, e);
         gridControl.ToolTipController = toolTipController;
      }

      private void onToolTipControllerGetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
      {
         if (e.SelectedControl != gridControl)
            return;

         var observerSetMappingDTO = _gridViewBinder.ElementAt(e);
         if (observerSetMappingDTO == null) return;

         var superToolTip = _toolTipCreator.ToolTipFor(_presenter.ToolTipFor(observerSetMappingDTO));
         e.Info = _toolTipCreator.ToolTipControlInfoFor(observerSetMappingDTO, superToolTip);
      }

      public void AttachPresenter(ISimulationObserversConfigurationPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();

         _gridViewBinder.AutoBind(x => x.ObserverSet)
            .WithRepository(initObserverSetRepository)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithCaption(PKSimConstants.ObjectTypes.ObserverSet);

         _colCreateObserverSet = _gridViewBinder.AddUnboundColumn()
            .WithCaption(PKSimConstants.UI.EmptyColumn)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithFixedWidth(EMBEDDED_BUTTON_WIDTH)
            .WithRepository(dto => _createButtonRepository);

         _colLoadObserverSet = _gridViewBinder.AddUnboundColumn()
            .WithCaption(PKSimConstants.UI.EmptyColumn)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithFixedWidth(EMBEDDED_BUTTON_WIDTH)
            .WithRepository(dto => _loadButtonRepository);

         _colRemoveObserverSet = _gridViewBinder.AddUnboundColumn()
            .WithCaption(PKSimConstants.UI.EmptyColumn)
            .WithFixedWidth(EMBEDDED_BUTTON_WIDTH)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithRepository(dto => _removeButtonRepository);

         btnAddObserverSet.Click += (o, e) => OnEvent(_presenter.AddObserverSet);

         _removeButtonRepository.ButtonClick += (o, e) => OnEvent(() => _presenter.RemoveObserverSetMapping(_gridViewBinder.FocusedElement));

         _loadButtonRepository.ButtonClick += (o, e) => OnEvent(() => _presenter.LoadObserverSetForAsync(_gridViewBinder.FocusedElement));
         _createButtonRepository.ButtonClick += (o, e) => OnEvent(() => _presenter.CreateObserverFor(_gridViewBinder.FocusedElement));

         _gridViewBinder.Changed += NotifyViewChanged;
      }

      private UxRepositoryItemImageComboBox initObserverSetRepository(ObserverSetMappingDTO observerSetMappingDTO)
      {
         _observerSetRepository.FillImageComboBoxRepositoryWith(_presenter.AllUnmappedObserverSets(observerSetMappingDTO), x => ApplicationIcons.Observer.Index, x => _presenter.DisplayNameFor(x));
         return _observerSetRepository;
      }

      protected override bool ColumnIsButton(GridColumn column)
      {
         return column.IsOneOf(_colCreateObserverSet.XtraColumn, _colLoadObserverSet.XtraColumn, _colRemoveObserverSet.XtraColumn);
      }

      public void BindTo(IEnumerable<ObserverSetMappingDTO> allObserverSetMappingDTOs)
      {
         _gridViewBinder.BindToSource(allObserverSetMappingDTOs);
      }

      public void RefreshData()
      {
         gridView.RefreshData();
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         btnAddObserverSet.InitWithImage(ApplicationIcons.Add, PKSimConstants.UI.AddObserverSet);
         layoutItemAddObserverSet.AdjustButtonSize(layoutControl);
      }

      public override bool HasError => _gridViewBinder.HasError;
   }
}