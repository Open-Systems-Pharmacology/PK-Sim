using PKSim.Presentation.Presenters.Individuals;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Individuals
{
   public interface IMoleculesView : IView<IMoleculesPresenter>, IExplorerView
   {
      string GroupCaption { set; }
      string LinkedExpressionProfileCaption { set; }
      void ActivateView(IView expressionsView);
   }
}