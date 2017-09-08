using System;
using System.Collections.Generic;
using System.ComponentModel;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using OSPSuite.Utility.Extensions;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Compounds;
using PKSim.UI.Extensions;
using PKSim.UI.Views.Core;
using OSPSuite.Presentation.Views;
using OSPSuite.UI;

namespace PKSim.UI.Views.Compounds
{
   public partial class CompoundTypeGroupView : BaseUserControlWithValueInGrid, ICompoundTypeGroupView
   {
      private ICompoundParameterGroupPresenter _presenter;
      private readonly GridViewBinder<TypePKaDTO> _gridViewBinder;
      private IGridViewBoundColumn _colPKa;
      private readonly UxRepositoryItemComboBox _compoundTypeRepository;
      private readonly RepositoryItemTextEdit _parameterEditRepository = new RepositoryItemTextEdit();
      private readonly UxRepositoryItemCheckEdit _favoriteRepository;
      private IGridViewColumn _colFavorites;
      public event EventHandler<ViewResizedEventArgs> HeightChanged = delegate { };

      public CompoundTypeGroupView()
      {
         InitializeComponent();

         _gridViewBinder = new GridViewBinder<TypePKaDTO>(gridView)
         {
            BindingMode = BindingMode.OneWay,
         };

         _compoundTypeRepository = new UxRepositoryItemComboBox(gridView);
         _parameterEditRepository.ConfigureWith(typeof (double));
         _parameterEditRepository.Appearance.TextOptions.HAlignment = HorzAlignment.Far;

         _favoriteRepository = new UxRepositoryItemCheckEdit(gridView);
         gridView.ShowColumnHeaders = false;
         gridView.RowCellStyle += (o, e) => OnEvent(updateRowCellStyle, e);
         gridView.AllowsFiltering = false;
         gridView.ShowingEditor += (o, e) => OnEvent(onShowingEditor, e);
         gridView.ShowRowIndicator = false;
         InitializeWithGrid(gridView);
      }

      private void onShowingEditor(CancelEventArgs e)
      {
         var typePkaDTO = _gridViewBinder.FocusedElement;
         if (typePkaDTO == null) return;
         if (gridView.FocusedColumn != _colPKa.XtraColumn) return;
         e.Cancel = (typePkaDTO.CompoundType == CompoundType.Neutral);
      }

      public void AttachPresenter(ICompoundParameterGroupPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeBinding()
      {
         _gridViewBinder.Bind(dto => dto.CompoundType)
            .WithRepository(dto => compoundTypeRepository())
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .OnValueUpdating += (dto, e) => compoundTypeGroupPresenter.SetCompoundType(dto, e.NewValue);

         _colPKa = _gridViewBinder.Bind(x => x.PKa)
            .WithEditRepository(x => _parameterEditRepository)
            .WithFormat(dto => new PKaFormatter(dto))
            .WithOnValueUpdating((dto, e) => compoundTypeGroupPresenter.SetPKa(dto, e.NewValue));

         _colFavorites = _gridViewBinder.Bind(x => x.IsFavorite)
            .WithCaption(PKSimConstants.UI.Favorites)
            .WithFixedWidth(UIConstants.Size.EMBEDDED_CHECK_BOX_WIDTH)
            .WithRepository(x => _favoriteRepository)
            .WithToolTip(PKSimConstants.UI.FavoritesToolTip)
            .WithOnValueUpdating((o, e) => OnEvent(() => compoundTypeGroupPresenter.SetFavorite(o, e.NewValue)));
      }

      protected override bool ColumnIsValue(GridColumn column)
      {
         return _colPKa.XtraColumn == column;
      }

      private RepositoryItem compoundTypeRepository()
      {
         _compoundTypeRepository.FillComboBoxRepositoryWith(compoundTypeGroupPresenter.AllCompoundTypes());
         return _compoundTypeRepository;
      }

      private void updateRowCellStyle(RowCellStyleEventArgs e)
      {
         //Row style only for pka columns
         if (e.Column != _colPKa.XtraColumn) return;
         var typePkaDTO = _gridViewBinder.ElementAt(e.RowHandle);
         if (typePkaDTO == null) return;
         gridView.AdjustAppearance(e, typePkaDTO.CompoundType != CompoundType.Neutral);
      }

      public void BindTo(IEnumerable<TypePKaDTO> allTypePKas)
      {
         _gridViewBinder.BindToSource(allTypePKas.ToBindingList());
         AdjustHeight();
      }

      public bool ShowFavorites
      {
         set => _colFavorites.UpdateVisibility(value);
      }

      public override bool HasError => _gridViewBinder.HasError;

      private ICompoundTypeGroupPresenter compoundTypeGroupPresenter => _presenter.DowncastTo<ICompoundTypeGroupPresenter>();

      public void AdjustHeight()
      {
         HeightChanged(this, new ViewResizedEventArgs(OptimalHeight));
      }

      public void Repaint()
      {
         gridView.LayoutChanged();
      }

      public int OptimalHeight => gridView.OptimalHeight;
   }
}