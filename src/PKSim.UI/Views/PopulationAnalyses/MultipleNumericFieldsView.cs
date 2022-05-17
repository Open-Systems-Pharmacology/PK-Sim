using System.Collections.Generic;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using DevExpress.XtraEditors;
using PKSim.Assets;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using PKSim.Presentation.Views.PopulationAnalyses;
using OSPSuite.Assets;
using OSPSuite.UI;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.PopulationAnalyses
{
   public partial class MultipleNumericFieldsView : BaseUserControl, IMultipleNumericFieldsView
   {
      private IMultipleNumericFieldsPresenter _presenter;
      private readonly GridViewBinder<FieldSelectionDTO> _gridViewBinder;
      private readonly UxRepositoryItemCheckEdit _selectionRepository;

      public MultipleNumericFieldsView()
      {
         InitializeComponent();
         _gridViewBinder = new GridViewBinder<FieldSelectionDTO>(gridView);
         _selectionRepository = new UxRepositoryItemCheckEdit(gridView);
         gridView.AllowsFiltering = false;
         gridView.ShowColumnHeaders = false;
         gridView.ShowRowIndicator = false;
         gridView.ShouldUseColorForDisabledCell = false;
         gridView.OptionsSelection.EnableAppearanceFocusedRow = true;
      }

      public void AttachPresenter(IMultipleNumericFieldsPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeBinding()
      {
         _gridViewBinder.Bind(x => x.Selected)
            .WithRepository(x => _selectionRepository)
            .WithFixedWidth(OSPSuite.UI.UIConstants.Size.EMBEDDED_BUTTON_WIDTH)
            .WithOnChanged(_presenter.SelectionChanged);

         _gridViewBinder.Bind(x => x.Name).AsReadOnly();

         btnMoveUp.Click += (o, e) => OnEvent(() => _presenter.MoveUp(SelectedItem));
         btnMoveDown.Click += (o, e) => OnEvent(() => _presenter.MoveDown(SelectedItem));
         gridView.FocusedRowChanged += (o, e) => OnEvent(() => _presenter.SelectedItemChanged());
      }

      public void BindTo(IEnumerable<FieldSelectionDTO> fieldSelectionDTOs)
      {
         _gridViewBinder.BindToSource(fieldSelectionDTOs);
      }

      public bool UpEnabled
      {
         get => btnMoveUp.Enabled;
         set => btnMoveUp.Enabled = value;
      }

      public bool DownEnabled
      {
         get => btnMoveDown.Enabled;
         set => btnMoveDown.Enabled = value;
      }

      public FieldSelectionDTO SelectedItem
      {
         get => _gridViewBinder.FocusedElement;
         set => gridView.FocusedRowHandle = _gridViewBinder.RowHandleFor(value);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         btnMoveUp.InitWithImage(ApplicationIcons.Up, imageLocation: ImageLocation.MiddleCenter, toolTip: PKSimConstants.UI.MoveUp);
         btnMoveDown.InitWithImage(ApplicationIcons.Down, imageLocation: ImageLocation.MiddleCenter, toolTip: PKSimConstants.UI.MoveDown);
         layoutItemButtonUp.AdjustButtonSizeWithImageOnly();
         layoutItemButtonDown.AdjustButtonSizeWithImageOnly();
         lblDescription.Text = PKSimConstants.UI.SelectedOutputs;
      }
   }
}