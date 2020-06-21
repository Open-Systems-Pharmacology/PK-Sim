using OSPSuite.DataBinding.DevExpress;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Services;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Format;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using OSPSuite.Presentation.DTO;
using OSPSuite.UI.Binders;
using PKSim.Assets;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.UI.Views.Compounds
{
   public partial class PermeabilityGroupView : CompoundParameterGroupWithCalculatedDefaultBaseView<PermeabilityAlternativeDTO>, IPermeabilityGroupView
   {
      private IGridViewColumn _colValue;

      public PermeabilityGroupView(IToolTipCreator toolTipCreator, IImageListRetriever imageListRetriever, ValueOriginBinder<PermeabilityAlternativeDTO> valueOriginBinder) : 
         base(toolTipCreator, imageListRetriever, valueOriginBinder)
      {
         InitializeComponent();
      }

      public override void InitializeBinding()
      {
         var colValue = _gridViewBinder.AutoBind(x => x.Permeability)
            .WithCaption(PKSimConstants.UI.Permeability);

         AddValueBinding(colValue);
         colValue.WithFormat(formatForAlternative);
         colValue.OnValueUpdating += (x, e) => permeabilityGroupPresenter.SetPermeabilityValue(x, e.NewValue);
         _colValue = colValue;
         _comboBoxUnit.ParameterUnitSet += (dto, unit) => OnEvent(() => permeabilityGroupPresenter.SetPermeabilityUnit(dto, unit));

         //to do at the end to respect order
         base.InitializeBinding();
      }

      private IFormatter<double> formatForAlternative(PermeabilityAlternativeDTO alternativeDTO)
      {
         return IsCalculatedAlternative(alternativeDTO) ? null : alternativeDTO.PermeabilityParameter.ParameterFormatter();
      }

      protected override void ConfigureValueRepository(BaseEdit baseEdit, ParameterAlternativeDTO alternativeDTO)
      {
         var permAlternative = alternativeDTO.DowncastTo<PermeabilityAlternativeDTO>();
         _comboBoxUnit.UpdateUnitsFor(baseEdit, permAlternative.PermeabilityParameter);
      }

      protected override bool ColumnIsValue(GridColumn gridColumn)
      {
         if (_colValue == null) return false;
         return _colValue.XtraColumn == gridColumn;
      }

      private IPermeabilityGroupPresenter permeabilityGroupPresenter => _presenter.DowncastTo<IPermeabilityGroupPresenter>();
   }
}