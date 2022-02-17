using System.Collections.Generic;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Services;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using PKSim.Presentation.DTO;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Views.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.Core
{
   public partial class CalculationMethodSelectionViewBase<TPresenter> : BaseGridViewOnlyUserControl, ICalculationMethodSelectionView<TPresenter> where TPresenter : ICalculationMethodSelectionPresenter
   {
      private readonly UxRepositoryItemComboBox _repositoryForCalculationMethods;
      private readonly IToolTipCreator _toolTipCreator;
      private readonly ToolTipController _toolTipController;
      private readonly GridViewBinder<CategoryCalculationMethodDTO> _gridViewBinder;
      protected TPresenter _presenter;
      protected IGridViewBoundColumn<CategoryCalculationMethodDTO, CalculationMethod> _boundColumn;

      public CalculationMethodSelectionViewBase(IToolTipCreator toolTipCreator, IImageListRetriever imageListRetriever)
      {
         InitializeComponent();
         _repositoryForCalculationMethods = new UxRepositoryItemComboBox(gridView);
         _toolTipCreator = toolTipCreator;
         _toolTipController = new ToolTipController();
         _toolTipController.Initialize(imageListRetriever);

         gridView.CustomRowFilter += customizedRowVisibility;
         gridControl.ToolTipController = _toolTipController;
         gridView.ShowColumnHeaders = false;
         gridView.HorzScrollVisibility = ScrollVisibility.Never;
         _gridViewBinder = new GridViewBinder<CategoryCalculationMethodDTO>(gridView);
      }

      public void AttachPresenter(TPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeBinding()
      {
         _gridViewBinder.Bind(pvv => pvv.DisplayName)
            .AsReadOnly();

         _boundColumn = _gridViewBinder.Bind(pvv => pvv.CalculationMethod)
            .WithRepository(pvv => _repositoryForCalculationMethods)
            .WithEditorConfiguration(updateCalculationMethodsForCategory)
            .WithShowButton(ShowButtonModeEnum.ShowAlways);

         _gridViewBinder.Changed += NotifyViewChanged;

         _toolTipController.GetActiveObjectInfo += onToolTipControllerGetActiveObjectInfo;
      }

      public void BindTo(IEnumerable<CategoryCalculationMethodDTO> calculationMethodDtos)
      {
         _gridViewBinder.BindToSource(calculationMethodDtos);
         // AdjustHeight();
      }

      private void onToolTipControllerGetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
      {
         var categoryCategoryItemDTO = _gridViewBinder.ElementAt(e);
         if (categoryCategoryItemDTO == null)
            return;

         var superToolTip = _toolTipCreator.ToolTipFor(categoryCategoryItemDTO);
         e.Info = _toolTipCreator.ToolTipControlInfoFor(categoryCategoryItemDTO, superToolTip);
      }

      private void customizedRowVisibility(object sender, RowFilterEventArgs e)
      {
         var calculationMethodDTO = _gridViewBinder.SourceElementAt(e.ListSourceRow);
         e.Visible = _presenter.ShouldDisplayCategory(calculationMethodDTO.Category);
         e.Handled = true;
      }

      private void updateCalculationMethodsForCategory(BaseEdit activeEditor, CategoryCalculationMethodDTO cm)
      {
         activeEditor.FillComboBoxEditorWith(_presenter.AllCalculationMethodsFor(cm.Category));
      }
   }
}