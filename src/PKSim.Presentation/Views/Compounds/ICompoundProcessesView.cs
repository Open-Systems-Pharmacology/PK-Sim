using OSPSuite.Presentation.Views;
using PKSim.Presentation.Presenters.Compounds;

namespace PKSim.Presentation.Views.Compounds
{
   public interface ICompoundProcessesView : IView<ICompoundProcessesPresenter>, IExplorerView
   {
      void ActivateView(IView view);
      string GroupCaption { set; }
      void Clear();
   }
}