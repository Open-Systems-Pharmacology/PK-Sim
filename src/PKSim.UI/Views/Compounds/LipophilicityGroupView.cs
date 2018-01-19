using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Services;
using OSPSuite.Utility.Extensions;
using DevExpress.XtraGrid.Columns;
using OSPSuite.UI.Binders;
using PKSim.Assets;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.UI.Extensions;

namespace PKSim.UI.Views.Compounds
{
   public partial class LipophilicityGroupView : CompoundParameterWithDefaultAlternativeBaseView<LipophilictyAlternativeDTO>, ILipophilicityGroupView
   {
      private IGridViewColumn _colValue;

      public LipophilicityGroupView(IToolTipCreator toolTipCreator, IImageListRetriever imageListRetriever, ValueOriginBinder<LipophilictyAlternativeDTO> valueOriginBinder) : base(toolTipCreator, imageListRetriever, valueOriginBinder)
      {
         InitializeComponent();
      }

      public override void InitializeBinding()
      {
         _colValue = _gridViewBinder.AutoBind(x => x.Lipophilicty)
            .WithCaption(PKSimConstants.UI.Lipophilicity)
            .WithFormat(dto => dto.LipophilictyParameter.ParameterFormatter())
            .WithEditorConfiguration((editor, dto) => _comboBoxUnit.UpdateUnitsFor(editor, dto.LipophilictyParameter))
            .WithOnValueUpdating((dto, e) => OnEvent(() => lipophilicityGroupPresenter.SetLipophilicityValue(dto, e.NewValue)));

         //to do at the end to respect order
         base.InitializeBinding();
      }

      protected override bool ColumnIsValue(GridColumn gridColumn)
      {
         if (_colValue == null) return false;
         return _colValue.XtraColumn == gridColumn;
      }

      private ILipophilicityGroupPresenter lipophilicityGroupPresenter => _presenter.DowncastTo<ILipophilicityGroupPresenter>();
   }
}