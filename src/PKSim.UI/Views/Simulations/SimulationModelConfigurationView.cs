using OSPSuite.Assets;
using DevExpress.XtraLayout;
using PKSim.Assets;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Simulations;
using PKSim.UI.Views.Core;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Views;

namespace PKSim.UI.Views.Simulations
{
   public partial class SimulationModelConfigurationView : BaseContainerUserControl, ISimulationModelConfigurationView
   {
      public SimulationModelConfigurationView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(ISimulationModelConfigurationPresenter presenter)
      {
      }

      public void AddSubView(ISubPresenterItem subPresenterItem, IView view)
      {
         fillIf(subPresenterItem, view, SimulationModelConfigurationItems.Subject, layoutItemSubjectSelectionView);
         fillIf(subPresenterItem, view, SimulationModelConfigurationItems.ModelSelection, layoutItemModelSelectionView);
         fillIf(subPresenterItem, view, SimulationModelConfigurationItems.CompoundsSelection, layoutItemCompoundListView);
      }

      private void fillIf(ISubPresenterItem current, IView view, ISubPresenterItem target, LayoutControlItem layoutControlItem)
      {
         if (current == target)
            AddViewTo(layoutControlItem,view);
      }

      public override string Caption => PKSimConstants.UI.ModelStructure;

      public override ApplicationIcon ApplicationIcon => ApplicationIcons.ModelStructure;

      public override void InitializeResources()
      {
         base.InitializeResources();
         layoutItemCompoundListView.TextVisible = false;
         layoutItemModelSelectionView.TextVisible = false;
         layoutItemSubjectSelectionView.TextVisible = false;

         layoutGroupCompoundsSelection.Text = PKSimConstants.UI.CompoundsSelection;
         layoutGroupSubjectSelection.Text = PKSimConstants.ObjectTypes.IndividualOrPopulation;
      }
   }
}