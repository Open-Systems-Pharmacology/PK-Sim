using PKSim.Assets;
using OSPSuite.Assets;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Presentation;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Controls;
using ISimulationParametersView = PKSim.Presentation.Views.Simulations.ISimulationParametersView;

namespace PKSim.UI.Views.Simulations
{
   public partial class SimulationParametersView : BaseUserControl, ISimulationParametersView
   {
      private ISimulationParametersPresenter _presenter;

      public SimulationParametersView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(ISimulationParametersPresenter presenter)
      {
         _presenter = presenter;
      }

      public override ApplicationIcon ApplicationIcon => ApplicationIcons.Parameters;

      public void AddParametersView(IView view)
      {
         this.FillWith(view);
      }

      public override string Caption => PKSimConstants.UI.Parameters;

   }
}