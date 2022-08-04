using OSPSuite.DataBinding;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using PKSim.Assets;
using PKSim.Presentation.DTO.Simulations;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;

namespace PKSim.UI.Views.Simulations
{
   public partial class SimulationExpressionsView : BaseContainerUserControl, ISimulationExpressionsView
   {
      private ISimulationExpressionsPresenter _presenter;
      private readonly ScreenBinder<SimulationExpressionsDTO> _screenBinder;

      public SimulationExpressionsView()
      {
         InitializeComponent();
         _screenBinder = new ScreenBinder<SimulationExpressionsDTO>();
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutGroupMoleculeParameters.Text = PKSimConstants.UI.Properties;
         layoutGroupMoleculeParameters.ExpandButtonVisible = true;
         layoutItemMoleculeParameters.TextVisible = false;
         layoutItemExpressionParameters.TextVisible = false;
      }

      public void AttachPresenter(ISimulationExpressionsPresenter presenter)
      {
         _presenter = presenter;
      }

      public void AddMoleculeParametersView(IView view) => AddViewTo(layoutItemMoleculeParameters, mainLayout, view);

      public void AddExpressionParametersView(IView view) => AddViewTo(layoutItemExpressionParameters, mainLayout, view);
   }
}