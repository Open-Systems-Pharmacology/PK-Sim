using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Diagrams;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Diagrams
{
   public interface IReactionDiagramContainerPresenter : IDisposablePresenter, IPresenter<IReactionDiagramModalView>
   {
      void Show(Simulation simulation);
   }

   public class ReactionDiagramContainerPresenter : AbstractDisposableContainerPresenter<IReactionDiagramModalView, IReactionDiagramContainerPresenter>, IReactionDiagramContainerPresenter
   {
      private readonly IReactionDiagramPresenter _subPresenter;

      public ReactionDiagramContainerPresenter(IReactionDiagramModalView view, IReactionDiagramPresenter subPresenter) : base(view)
      {
         _subPresenter = subPresenter;
         AddSubPresenters(_subPresenter);
         _view.SetView(_subPresenter.BaseView);
      }

      public void Show(Simulation simulation)
      {
         _subPresenter.Edit(simulation);
         _view.Display();
      }
   }
}