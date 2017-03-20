using PKSim.Presentation.Presenters.Individuals;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Individuals
{
   public interface IMoleculesView : IView<IMoleculesPresenter>, IExplorerView
   {
      string GroupCaption { set; }
      void ActivateView(IView expressionsView);
   }
}