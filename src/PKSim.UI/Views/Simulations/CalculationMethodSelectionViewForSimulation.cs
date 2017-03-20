using OSPSuite.UI.Services;
using PKSim.Presentation.Presenters;
using PKSim.Presentation.Views.Simulations;
using PKSim.UI.Views.Core;

namespace PKSim.UI.Views.Simulations
{
   public partial class CalculationMethodSelectionViewForSimulation : CalculationMethodSelectionViewBase<ICalculationMethodSelectionPresenterForSimulation>, ICalculationMethodSelectionViewForSimulation
   {
      public CalculationMethodSelectionViewForSimulation(IToolTipCreator toolTipCreator, IImageListRetriever imageListRetriever)
         : base(toolTipCreator, imageListRetriever)
      {
         InitializeComponent();
      }

      public void SetReadOnly(bool readOnly)
      {
         _boundColumn.ReadOnly = readOnly;
      }
   }
}  