using OSPSuite.BDDHelper;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Diagrams;
using PKSim.Presentation.Presenters.Simulations;
using PKSim.Presentation.Views.Diagrams;

namespace PKSim.Presentation
{
   public abstract class concern_for_ReactionDiagramModalPresenter : ContextSpecification<ReactionDiagramContainerPresenter>
   {
      protected IReactionDiagramModalView _reactionDiagramModalView;
      protected IReactionDiagramPresenter _reactionDiagramPresenter;

      protected override void Context()
      {
         _reactionDiagramModalView = A.Fake<IReactionDiagramModalView>();
         _reactionDiagramPresenter = A.Fake<IReactionDiagramPresenter>();

         sut = new ReactionDiagramContainerPresenter(_reactionDiagramModalView, _reactionDiagramPresenter);
      }
   }

   public class When_editing_simulation_in_modal_presenter : concern_for_ReactionDiagramModalPresenter
   {
      private Simulation _simulation;

      protected override void Because()
      {
         _simulation = A.Fake<Simulation>();
         sut.Show(_simulation);
      }

      [Observation]
      public void Underlying_presenter_should_have_been_initialized_in_recreate_mode()
      {
         A.CallTo(() => _reactionDiagramPresenter.Edit(_simulation, true)).MustHaveHappened();
      }

      [Observation]
      public void The_diagram_view_must_have_been_displayed()
      {
         A.CallTo(() => _reactionDiagramModalView.Display()).MustHaveHappened();
      }

      [Observation]
      public void The_main_modal_view_must_have_been_initialized_with_the_view_of_the_base_presenter()
      {
         A.CallTo(() => _reactionDiagramModalView.SetView(_reactionDiagramPresenter.BaseView)).MustHaveHappened();
      }
   }
}
