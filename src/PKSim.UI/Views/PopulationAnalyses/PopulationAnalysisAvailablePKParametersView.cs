using System.Collections.Generic;
using System.Linq;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Services;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Base;
using PKSim.Assets;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;

namespace PKSim.UI.Views.PopulationAnalyses
{
   public partial class PopulationAnalysisAvailablePKParametersView : BaseUserControl, IPopulationAnalysisAvailablePKParametersView
   {
      private readonly IToolTipCreator _toolTipCreator;
      private IPopulationAnalysisAvailablePKParametersPresenter _presenter;
      private readonly GridViewBinder<QuantityPKParameterDTO> _gridViewBinder;
      private IGridViewBoundColumn _colPKParameter;
      private readonly ToolTipController _toolTipController;

      public PopulationAnalysisAvailablePKParametersView(IToolTipCreator toolTipCreator, IImageListRetriever imageListRetriever)
      {
         _toolTipCreator = toolTipCreator;
         _toolTipController = new ToolTipController();
         _toolTipController.Initialize(imageListRetriever);
         InitializeComponent();
         _gridViewBinder = new GridViewBinder<QuantityPKParameterDTO>(gridView);
         gridView.AllowsFiltering = true;
         gridView.MultiSelect = true;
         gridView.ShouldUseColorForDisabledCell = false;
         gridView.GroupFormat = "[#image]{1}";
         gridControl.ToolTipController = _toolTipController;
         gridView.DoubleClick += (o, e) => OnEvent(gridViewDoubleClicked);
         gridView.FocusedRowChanged += (o, e) => OnEvent(gridViewRowChanged, e);
      }

      private void gridViewRowChanged(FocusedRowChangedEventArgs e)
      {
         var selectedItem = _gridViewBinder.ElementAt(e.FocusedRowHandle);
         if (selectedItem == null) return;
         _presenter.QuantityPKParameterSelected(selectedItem);
      }

      public override void InitializeBinding()
      {
         _gridViewBinder.Bind(x => x.QuantityDisplayPath)
            .WithCaption(PKSimConstants.UI.Output)
            .AsReadOnly();

         _colPKParameter = _gridViewBinder.Bind(x => x.DisplayName)
            .WithCaption(PKSimConstants.UI.PKParameter)
            .AsReadOnly();

         _toolTipController.GetActiveObjectInfo += (o, e) => OnEvent(onToolTipControllerGetActiveObjectInfo, e);
      }

      private void gridViewDoubleClicked()
      {
         var pt = gridView.GridControl.PointToClient(MousePosition);
         _presenter.QuantityPKParameterDTODoubleClicked(_gridViewBinder.ElementAt(pt));
      }

      private void onToolTipControllerGetActiveObjectInfo(ToolTipControllerGetActiveObjectInfoEventArgs e)
      {
         if (e.SelectedControl != gridControl) return;
         var dto = _gridViewBinder.ElementAt(e);
         if (dto == null) return;

         var superToolTip = _toolTipCreator.ToolTipFor(dto);
         e.Info = _toolTipCreator.ToolTipControlInfoFor(dto, superToolTip);
      }

      public void AttachPresenter(IPopulationAnalysisAvailablePKParametersPresenter presenter)
      {
         _presenter = presenter;
      }

      public void BindTo(IReadOnlyCollection<QuantityPKParameterDTO> allPKParameters)
      {
         _gridViewBinder.BindToSource(allPKParameters);

         //Group by display parameter path
         _colPKParameter.XtraColumn.GroupIndex = 0;
      }

      public IEnumerable<QuantityPKParameterDTO> SelectedPKParameters
      {
         get
         {
            return gridView.GetSelectedRows()
               .Select(rowHandle => _gridViewBinder.ElementAt(rowHandle));
         }
      }
   }
}