using OSPSuite.Presentation.Nodes;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Main;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Compounds
{
   public interface ICompoundProcessesView : IView<ICompoundProcessesPresenter>, IExplorerView
   {
      void ActivateView(IView view);
      string GroupCaption { set; }
      void Clear();
   }
}