using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Services;
using OSPSuite.UI.RepositoryItems;
using OSPSuite.Assets;
using OSPSuite.Utility.Extensions;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using PKSim.Assets;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;
using PKSim.UI.Extensions;
using PKSim.UI.Views.Core;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;
using UIConstants = OSPSuite.UI.UIConstants;

namespace PKSim.UI.Views.Compounds
{
   public partial class CompoundParameterWithDefaultAlternativeBaseView<TParameterAlternativeDTO> : BaseUserControlWithValueInGrid, ICompoundParameterGroupWithAlternativeView
      where TParameterAlternativeDTO : ParameterAlternativeDTO
   {
      private readonly IToolTipCreator _toolTipCreator;
      protected readonly IImageListRetriever _imageListRetriever;
      protected ICompoundParameterGroupWithAlternativePresenter _presenter;
      private IGridViewBoundColumn _colName;
      private readonly UxRepositoryItemCheckEdit _isDefaultRepository;
      private IGridViewColumn _colDefault;

      protected readonly RepositoryItemButtonEdit _addAndRemoveButtonRepository = new UxAddAndRemoveButtonRepository();
      protected readonly RepositoryItemButtonEdit _addButtonRepository = new UxAddAndDisabledRemoveButtonRepository();
      private readonly UxRepositoryItemButtonEdit _nameRepository;
      public event EventHandler<ViewResizedEventArgs> HeightChanged = delegate { };
      protected readonly ComboBoxUnitParameter _comboBoxUnit;
      protected readonly GridViewBinder<TParameterAlternativeDTO> _gridViewBinder;

      public CompoundParameterWithDefaultAlternativeBaseView()
      {
         InitializeComponent();
      }

      public CompoundParameterWithDefaultAlternativeBaseView(IToolTipCreator toolTipCreator, IImageListRetriever imageListRetriever) : this()
      {
         _toolTipCreator = toolTipCreator;
         _imageListRetriever = imageListRetriever;
         _gridView.HiddenEditor += (o, e) => OnEvent(hideEditor);
         _gridView.AllowsFiltering = false;
         _gridView.ShowColumnChooser = false;
         _comboBoxUnit = new ComboBoxUnitParameter(_gridControl);
         _isDefaultRepository = new UxRepositoryItemCheckEdit(_gridView);
         _gridControl.ToolTipController = new ToolTipController().Initialize(imageListRetriever);
         _gridControl.ToolTipController.GetActiveObjectInfo += (o, e) => OnEvent(onToolTipControllerGetActiveObjectInfo, o, e);
         _nameRepository = new UxRepositoryItemButtonEdit(ButtonPredefines.Ellipsis) {TextEditStyle = TextEditStyles.DisableTextEditor};
         _nameRepository.Buttons[0].ToolTip = PKSimConstants.UI.Rename;
         _nameRepository.AddButton(ButtonPredefines.Glyph);
         _nameRepository.Buttons[1].ToolTip = PKSimConstants.UI.EditValueDescription;
         _nameRepository.Buttons[1].Image = ApplicationIcons.Description.ToImage(IconSizes.Size16x16);
         _gridViewBinder = new GridViewBinder<TParameterAlternativeDTO>(_gridView) {BindingMode = BindingMode.OneWay};
         InitializeWithGrid(_gridView);
      }

      private void hideEditor()
      {
         _comboBoxUnit.Hide();
      }

      public override void InitializeBinding()
      {
         _colName = _gridViewBinder.Bind(x => x.Name)
            .WithCaption(PKSimConstants.UI.Experiment)
            .WithRepository(dto => _nameRepository)
            .WithShowButton(ShowButtonModeEnum.ShowAlways);

         _nameRepository.ButtonClick += (o, e) => OnEvent(() => nameButtonsClick(e.Button, _gridViewBinder.FocusedElement));

         var colDefault = _gridViewBinder.Bind(x => x.IsDefault)
            .WithCaption(PKSimConstants.UI.IsDefault)
            .WithRepository(dto => _isDefaultRepository)
            .WithFixedWidth(UIConstants.Size.EMBEDDED_CHECK_BOX_WIDTH);

         colDefault.OnValueUpdating += (o, e) => OnEvent(() => _presenter.SetIsDefaultFor(o, e.NewValue));
         _colDefault = colDefault;

         _gridViewBinder.AddUnboundColumn()
            .WithCaption(PKSimConstants.UI.EmptyColumn)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithRepository(GetButtonRepository)
            .WithFixedWidth(UIConstants.Size.EMBEDDED_BUTTON_WIDTH * 2);

         _addAndRemoveButtonRepository.ButtonClick += (o, e) => OnEvent(() => buttonRepositoryButtonClick(o, e, _gridViewBinder.FocusedElement));
         _addButtonRepository.ButtonClick += (o, e) => OnEvent(() => _presenter.AddAlternative());

         //last but not least: Set the column for name as the first column in the grid view
         _colName.XtraColumn.VisibleIndex = 0;
      }

      private void nameButtonsClick(EditorButton button, TParameterAlternativeDTO parameterAlternativeDTO)
      {
         if (button.Index == 0)
            _presenter.RenameAlternative(parameterAlternativeDTO);
         else
            _presenter.EditValueDescriptionFor(parameterAlternativeDTO);
      }

      public override bool HasError
      {
         get { return _gridViewBinder.HasError; }
      }

      public virtual void BindTo(IReadOnlyCollection<TParameterAlternativeDTO> parameterAlternativeDtos)
      {
         _gridViewBinder.BindToSource(parameterAlternativeDtos.ToBindingList());
         AdjustLayout(parameterAlternativeDtos.Count());
      }

      private void onToolTipControllerGetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
      {
         var column = _gridView.ColumnAt(e);
         if (!Equals(_colName.XtraColumn, column)) return;

         var alternativeDTO = _gridViewBinder.ElementAt(e);
         if (alternativeDTO == null) return;

         var superToolTip = _toolTipCreator.ToolTipFor(alternativeDTO);

         //An object that uniquely identifies a row cell
         e.Info = _toolTipCreator.ToolTipControlInfoFor(alternativeDTO, superToolTip);
      }

      public void AttachPresenter(ICompoundParameterGroupWithAlternativePresenter presenter)
      {
         _presenter = presenter;
      }

      protected virtual RepositoryItem GetButtonRepository(ParameterAlternativeDTO alternativeDTO)
      {
         if (alternativeDTO.IsDefault)
            return _addButtonRepository;
         return _addAndRemoveButtonRepository;
      }

      private void buttonRepositoryButtonClick(object sender, ButtonPressedEventArgs e, ParameterAlternativeDTO parameterAlternativeDTO)
      {
         var editor = (ButtonEdit) sender;
         var buttonIndex = editor.Properties.Buttons.IndexOf(e.Button);
         if (buttonIndex == 0)
            _presenter.AddAlternative();
         else
            _presenter.RemoveAlternative(parameterAlternativeDTO);
      }

      protected void AdjustLayout(int numberOfBoundItems)
      {
         _colDefault.UpdateVisibility(numberOfBoundItems > 1);
         AdjustHeight();
      }

      public virtual void AdjustHeight()
      {
         HeightChanged(this, new ViewResizedEventArgs(OptimalHeight));
      }

      public virtual int OptimalHeight
      {
         get { return _gridView.OptimalHeight + layoutItemGrid.Padding.Height; }
      }

      public virtual void Repaint()
      {
         _gridView.LayoutChanged();
      }
   }
}