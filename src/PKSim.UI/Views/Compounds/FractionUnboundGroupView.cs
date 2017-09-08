using System.Collections.Generic;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Services;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.RepositoryItems;
using OSPSuite.Utility.Extensions;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using PKSim.Assets;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.Presentation.Extensions;
using PKSim.Core.Model;
using UIConstants = OSPSuite.UI.UIConstants;

namespace PKSim.UI.Views.Compounds
{
   public partial class FractionUnboundGroupView : CompoundParameterWithDefaultAlternativeBaseView<FractionUnboundAlternativeDTO>, IFractionUnboundGroupView
   {
      private readonly UxRepositoryItemImageComboBox _speciesRepository;
      private IGridViewColumn _colValue;
      private LayoutControlItem _layoutItemBindingMode;
      private RadioGroup _rgPlasmaBindingPartner;

      public FractionUnboundGroupView()
      {
         InitializeComponent();
      }

      public FractionUnboundGroupView(IToolTipCreator toolTipCreator, IImageListRetriever imageListRetriever) : base(toolTipCreator, imageListRetriever)
      {
         InitializeComponent();
         _speciesRepository = new UxRepositoryItemImageComboBox(_gridView, imageListRetriever);
      }

      private IFractionUnboundGroupPresenter fractionUnboundGroupPresenter => _presenter.DowncastTo<IFractionUnboundGroupPresenter>();

      public override void InitializeBinding()
      {
         _colValue = _gridViewBinder.AutoBind(x => x.FractionUnbound)
            .WithCaption(PKSimConstants.UI.FractionUnbound)
            .WithFormat(dto => dto.FractionUnboundParameter.ParameterFormatter())
            .WithEditorConfiguration((editor, sol) => _comboBoxUnit.UpdateUnitsFor(editor, sol.FractionUnboundParameter))
            .WithOnValueUpdating((dto, e) => OnEvent(() => fractionUnboundGroupPresenter.SetFractionUnboundValue(dto, e.NewValue)));

         _comboBoxUnit.ParameterUnitSet += (dto, unit) => OnEvent(() => fractionUnboundGroupPresenter.SetFractionUnboundUnit(dto, unit));

         _gridViewBinder.Bind(x => x.Species)
            .WithRepository(dto => configureSpeciesRepository())
            .WithShowButton(ShowButtonModeEnum.ShowAlways)
            .WithOnValueUpdating((dto, e) => OnEvent(() => fractionUnboundGroupPresenter.SetSpeciesValue(dto, e.NewValue)));

         //to do at the end to respect order
         base.InitializeBinding();
      }

      public override void BindTo(IReadOnlyCollection<FractionUnboundAlternativeDTO> parameterAlternativeDtos)
      {
         base.BindTo(parameterAlternativeDtos);
         _rgPlasmaBindingPartner.EditValue = fractionUnboundGroupPresenter.PlasmaProteinBindingPartner;
      }

      protected override bool ColumnIsValue(GridColumn gridColumn)
      {
         if (_colValue == null) return false;
         return _colValue.XtraColumn == gridColumn;
      }

      private RepositoryItem configureSpeciesRepository()
      {
         _speciesRepository.FillImageComboBoxRepositoryWith(fractionUnboundGroupPresenter.AllSpecies(), sp => _imageListRetriever.ImageIndex(sp.Name));
         return _speciesRepository;
      }

      public override void InitializeResources()
      {
         base.InitializeResources();

         _rgPlasmaBindingPartner = new RadioGroup {Name = "rgPlasmaBindingPartner"};
         _rgPlasmaBindingPartner.Properties.Items.AddRange(new[]
         {
            new RadioGroupItem(PlasmaProteinBindingPartner.Albumin, PKSimConstants.UI.Albumin),
            new RadioGroupItem(PlasmaProteinBindingPartner.Glycoprotein, PKSimConstants.UI.Glycoprotein),
            new RadioGroupItem(PlasmaProteinBindingPartner.Unknown, PKSimConstants.UI.Unknown)
         });

         _layoutItemBindingMode = new LayoutControlItem
         {
            Parent = layoutControl.Root,
            Control = _rgPlasmaBindingPartner,
            Text = PKSimConstants.UI.FractionUnboundBindingType.FormatForLabel()
         };
         _layoutItemBindingMode.AdjustControlHeight(UIConstants.Size.RADIO_GROUP_HEIGHT);
         _layoutItemBindingMode.Move(layoutItemGrid, InsertType.Top);
         _rgPlasmaBindingPartner.SelectedIndexChanged += (o, e) => OnEvent(bindingPartnerChanged);
      }

      private void bindingPartnerChanged()
      {
         fractionUnboundGroupPresenter.PlasmaProteinBindingPartner = (PlasmaProteinBindingPartner) _rgPlasmaBindingPartner.Properties.Items[_rgPlasmaBindingPartner.SelectedIndex].Value;
      }

      public override int OptimalHeight => base.OptimalHeight + _layoutItemBindingMode.Height;
   }
}