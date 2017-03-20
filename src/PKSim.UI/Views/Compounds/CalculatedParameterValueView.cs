using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using OSPSuite.UI.Extensions;

namespace PKSim.UI.Views.Compounds
{
   public partial class CalculatedParameterValueView : BaseUserControl, ICalculatedParameterValueView
   {
      public CalculatedParameterValueView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(ICalculatedParameterValuePresenter presenter)
      {
      }

      public void SetParameterView(IView view)
      {
         panelParameters.FillWith(view);
      }

      public string Description
      {
         set { layoutItemParameter.Text = value; }
      }
   }
}
