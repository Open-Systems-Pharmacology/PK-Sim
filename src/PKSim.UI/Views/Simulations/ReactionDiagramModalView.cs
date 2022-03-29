using PKSim.Assets;
using PKSim.Presentation.Presenters.Diagrams;
using PKSim.Presentation.Views.Diagrams;
using OSPSuite.Assets;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;

namespace PKSim.UI.Views.Simulations
{
   public partial class ReactionDiagramModalView : BaseModalContainerView, IReactionDiagramModalView
   {

      public ReactionDiagramModalView(IShell shell):base(shell)
      {
         InitializeComponent();
      }
      
      public override void InitializeResources()
      {
         base.InitializeResources();
         ApplicationIcon = ApplicationIcons.Reaction;
         Text = PKSimConstants.UI.ReactionDiagram;
         CancelVisible = false;
      }

      public void AttachPresenter(IReactionDiagramContainerPresenter presenter)
      {
      }

      public void SetView(IView baseView)
      {
         viewPanel.FillWith(baseView);
      }
   }
}
