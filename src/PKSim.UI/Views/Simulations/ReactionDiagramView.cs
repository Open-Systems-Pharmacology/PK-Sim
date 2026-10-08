using PKSim.Assets;
using OSPSuite.UI.Services;
using OSPSuite.Assets;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Diagrams;
using OSPSuite.UI.Views.Diagram;

namespace PKSim.UI.Views.Simulations
{
   public partial class ReactionDiagramView : DiagramView, IReactionDiagramView
   {
      public ReactionDiagramView(IImageListRetriever imageListRetriever)
         : base(imageListRetriever)
      {
         InitializeComponent();
         SetReadOnly();
      }

      public override ApplicationIcon ApplicationIcon => ApplicationIcons.Reaction;

      public override string Caption => PKSimConstants.UI.ReactionDiagram;

      public void AttachPresenter(IReactionDiagramPresenter presenter)
      {
         _presenter = presenter;
      }
   }
}
