using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Binders;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using OSPSuite.UI.Services;
using OSPSuite.UI.Views;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;
using PKSim.UI.Views.Core;
using static OSPSuite.UI.UIConstants.Size;

namespace PKSim.UI.Views.Compounds
{
   public partial class CompoundParameterWithDefaultAlternativeBaseView<TParameterAlternativeDTO> : BaseUserControlWithValueInGrid, ICompoundParameterGroupWithAlternativeView
      where TParameterAlternativeDTO : ParameterAlternativeDTO
   {
      private readonly IToolTipCreator _toolTipCreator;
      protected readonly IImageListRetriever _imageListRetriever;
      private readonly ValueOriginBinder<TParameterAlternativeDTO> _valueOriginBinder;
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
      private IGridViewColumn _colButtons;

      public CompoundParameterWithDefaultAlternativeBaseView()
      {
         InitializeComponent();
      }

      public CompoundParameterWithDefaultAlternativeBaseView(IToolTipCreator toolTipCreator, IImageListRetriever imageListRetriever, ValueOriginBinder<TParameterAlternativeDTO> valueOriginBinder) : this()
      {
         _toolTipCreator = toolTipCreator;
         _imageListRetriever = imageListRetriever;
         _valueOriginBinder = valueOriginBinder;
         _gridView.HiddenEditor += (o, e) => OnEvent(hideEditor);
         _gridView.AllowsFiltering = false;
         _gridView.ShowColumnChooser = false;
         _comboBoxUnit = new ComboBoxUnitParameter(_gridControl);
         _isDefaultRepository = new UxRepositoryItemCheckEdit(_gridView);
         _gridControl.ToolTipController = new ToolTipController().Initialize(imageListRetriever);
         _gridControl.ToolTipController.GetActiveObjectInfo += (o, e) => OnEvent(onToolTipControllerGetActiveObjectInfo, o, e);
         _nameRepository = new UxRepositoryItemButtonEdit(ButtonPredefines.Ellipsis) {TextEditStyle = TextEditStyles.DisableTextEditor};
         _nameRepository.Buttons[0].ToolTip = PKSimConstants.UI.Rename;
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

         _nameRepository.ButtonClick += (o, e) => OnEvent(() => nameButtonsClick(_gridViewBinder.FocusedElement));

         _valueOriginBinder.InitializeBinding(_gridViewBinder, updateValueOrigin);

         var colDefault = _gridViewBinder.Bind(x => x.IsDefault)
            .WithCaption(PKSimConstants.UI.IsDefault)
            .WithRepository(dto => _isDefaultRepository)
            .WithFixedWidth(EMBEDDED_CHECK_BOX_WIDTH);

         colDefault.OnValueUpdating += (o, e) => OnEvent(() => _presenter.SetIsDefaultFor(o, e.NewValue));
         _colDefault = colDefault;

         _colButtons = _gridViewBinder.AddUnboundColumn()
            .WithCaption(PKSimConstants.UI.EmptyColumn)
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithRepository(GetButtonRepository)
            .WithFixedWidth(EMBEDDED_BUTTON_WIDTH * 2);

         _addAndRemoveButtonRepository.ButtonClick += (o, e) => OnEvent(() => buttonRepositoryButtonClick(e, _gridViewBinder.FocusedElement));
         _addButtonRepository.ButtonClick += (o, e) => OnEvent(() => _presenter.AddAlternative());

         //last but not least: Set the column for name as the first column in the grid view
         _colName.XtraColumn.VisibleIndex = 0;
      }

      protected override bool ColumnIsButton(GridColumn column)
      {
         return Equals(_colButtons?.XtraColumn, column);
      }

      protected override bool ColumnIsCheckBox(GridColumn column)
      {
         return Equals(_colDefault?.XtraColumn, column);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         UpdateButtonImage(0, _addButtonRepository, ApplicationIcons.Add);
         UpdateButtonImage(1, _addButtonRepository, ApplicationIcons.Remove);
         UpdateButtonImage(0, _addAndRemoveButtonRepository, ApplicationIcons.Add);
         UpdateButtonImage(1, _addAndRemoveButtonRepository, ApplicationIcons.Remove);
      }

      protected void UpdateButtonImage(int buttonIndex, RepositoryItemButtonEdit repositoryItemButtonEdit, ApplicationIcon image)
      {
         repositoryItemButtonEdit.Buttons[buttonIndex].Kind = ButtonPredefines.Glyph;
         repositoryItemButtonEdit.Buttons[buttonIndex].Image = image;
      }

      private void updateValueOrigin(TParameterAlternativeDTO parameterAlternativeDTO, ValueOrigin newValueOrigin)
      {
         _presenter.UpdateValueOriginFor(parameterAlternativeDTO, newValueOrigin);
      }

      private void nameButtonsClick(TParameterAlternativeDTO parameterAlternativeDTO)
      {
         _presenter.RenameAlternative(parameterAlternativeDTO);
      }

      public override bool HasError => _gridViewBinder.HasError;

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

      private void buttonRepositoryButtonClick(ButtonPressedEventArgs e, ParameterAlternativeDTO parameterAlternativeDTO)
      {
         if (Equals(e.Button.Tag, ButtonType.Add))
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

      public virtual int OptimalHeight => _gridView.OptimalHeight + layoutItemGrid.Padding.Height;

      public virtual void Repaint()
      {
         _gridView.LayoutChanged();
      }
   }
}