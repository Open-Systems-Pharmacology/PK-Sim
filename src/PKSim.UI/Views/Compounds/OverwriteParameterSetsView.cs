using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using OSPSuite.Assets;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Controls;
using PKSim.Assets;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;
using static OSPSuite.UI.UIConstants.Size;

namespace PKSim.UI.Views.Compounds;

public partial class OverwriteParameterSetsView : BaseUserControl, IOverwriteParameterSetsView
{
   private IOverwriteParameterSetsPresenter _presenter;
   private readonly GridViewBinder<OverwriteParameterSetDTO> _gridViewBinderSets;
   private readonly GridViewBinder<OverwriteParameterValueDTO> _gridViewBinderParameterValues;
   private readonly RepositoryItemButtonEdit _removeButtonRepository = new UxRemoveButtonRepository();

   public OverwriteParameterSetsView()
   {
      InitializeComponent();

      _gridViewBinderSets = new GridViewBinder<OverwriteParameterSetDTO>(gridViewSets)
      {
         BindingMode = BindingMode.OneWay
      };

      _gridViewBinderParameterValues = new GridViewBinder<OverwriteParameterValueDTO>(gridViewParameterValues)
      {
         BindingMode = BindingMode.OneWay
      };

      gridViewSets.OptionsSelection.EnableAppearanceFocusedCell = false;
      gridViewParameterValues.OptionsSelection.EnableAppearanceFocusedCell = false;
      gridViewParameterValues.EditorShowMode = EditorShowMode.MouseDown;

      gridViewSets.FocusedRowChanged += (o, e) => OnEvent(selectedSetChanged);
   }

   public void AttachPresenter(IOverwriteParameterSetsPresenter presenter)
   {
      _presenter = presenter;
   }

   public override void InitializeBinding()
   {
      _gridViewBinderSets.Bind(x => x.Name)
         .WithCaption(PKSimConstants.UI.Name)
         .AsReadOnly();

      _gridViewBinderSets.Bind(x => x.IsDefault)
         .WithCaption(PKSimConstants.UI.IsDefault)
         .AsReadOnly();

      _gridViewBinderSets.Bind(x => x.Species)
         .WithCaption(PKSimConstants.UI.Species)
         .AsReadOnly();

      _gridViewBinderSets.Bind(x => x.DiseaseState)
         .WithCaption(PKSimConstants.UI.DiseaseState)
         .AsReadOnly();

      _gridViewBinderParameterValues.Bind(x => x.Path)
         .WithCaption(Captions.Diff.ObjectPath)
         .AsReadOnly();

      _gridViewBinderParameterValues.Bind(x => x.Value)
         .WithCaption(Captions.Value)
         .WithOnValueUpdating((dto, e) => OnEvent(() => onParameterValueUpdating(dto, e.NewValue)));

      _gridViewBinderParameterValues.Bind(x => x.Unit)
         .WithCaption(Captions.Unit)
         .AsReadOnly();

      _gridViewBinderParameterValues.Bind(x => x.ValueOrigin)
         .WithCaption(Captions.ValueOrigin)
         .AsReadOnly();

      _gridViewBinderParameterValues.AddUnboundColumn()
         .WithCaption(Captions.EmptyColumn)
         .WithFixedWidth(EMBEDDED_BUTTON_WIDTH)
         .WithShowButton(ShowButtonModeEnum.ShowAlways)
         .WithRepository(_ => _removeButtonRepository);

      _removeButtonRepository.ButtonClick += (_, _) => OnEvent(() => _presenter.RemoveParameterValue(_gridViewBinderSets.FocusedElement, _gridViewBinderParameterValues.FocusedElement));
   }

   public override void InitializeResources()
   {
      base.InitializeResources();
      Caption = PKSimConstants.UI.OverwriteParameterSetsTabCaption;
      ApplicationIcon = ApplicationIcons.ParameterValues;
   }

   public void BindTo(IReadOnlyList<OverwriteParameterSetDTO> overwriteParameterSets)
   {
      _gridViewBinderSets.BindToSource(overwriteParameterSets);
      bindToSelectedParameterValues();
   }

   private void selectedSetChanged() => bindToSelectedParameterValues();

   private void bindToSelectedParameterValues()
   {
      var selectedSet = _gridViewBinderSets.FocusedElement;
      if (selectedSet == null)
      {
         _gridViewBinderParameterValues.BindToSource([]);
         return;
      }

      _gridViewBinderParameterValues.BindToSource(selectedSet.ParameterValues);
   }

   private void onParameterValueUpdating(OverwriteParameterValueDTO dto, double? newValue)
   {
      if (!newValue.HasValue)
         return;

      var selectedSet = _gridViewBinderSets.FocusedElement;
      if (selectedSet == null)
         return;

      _presenter.UpdateParameterValue(selectedSet, dto, newValue.Value);
      gridViewParameterValues.CloseEditor();
   }
}