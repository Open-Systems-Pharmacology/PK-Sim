using OSPSuite.Presentation.Views;
using OSPSuite.Assets;
using PKSim.Presentation.Presenters;

namespace PKSim.Presentation.Views
{
   public interface IBuildingBlockFromTemplateView : IModalView<ITemplatePresenter>, IExplorerView
   {
      void SetIcon(ApplicationIcon icon);
      string Description { set; }
   }
}