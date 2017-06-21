using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.UI.Services;
using PKSim.Presentation.DTO;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Views.Simulations;
using PKSim.UI.Views.Core;
using OSPSuite.Core.Domain;

namespace PKSim.UI.Views.Compounds
{
   public partial class CalculationMethodSelectionViewForCompound : CalculationMethodSelectionViewBase<ICalculationMethodSelectionPresenterForCompound>, ICalculationMethodSelectionViewForCompound
   {
      public CalculationMethodSelectionViewForCompound(IToolTipCreator toolTipCreator, IImageListRetriever imageListRetriever)
         : base(toolTipCreator, imageListRetriever)
      {
         InitializeComponent();
      }

      public override void InitializeBinding()
      {
         base.InitializeBinding();
         _boundColumn.WithOnValueUpdating(newValueSetForCalculationMethod);
      }

  
      private void newValueSetForCalculationMethod(CategoryCalculationMethodDTO dto, PropertyValueSetEventArgs<CalculationMethod> propertySetEventArgs)
      {
         _presenter.SetCalculationMethodForCompound(dto.Category, propertySetEventArgs.NewValue, propertySetEventArgs.OldValue);
      }
   }
}
