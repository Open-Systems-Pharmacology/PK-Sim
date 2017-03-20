using PKSim.Presentation.Presenters.Diagrams;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Diagrams
{
   public interface IReactionDiagramModalView : IModalView<IReactionDiagramContainerPresenter>
   {
      /// <summary>
      /// Add the underlying view to the modal view
      /// </summary>
      /// <param name="baseView">The view to add</param>
      void SetView(IView baseView);
   }
}