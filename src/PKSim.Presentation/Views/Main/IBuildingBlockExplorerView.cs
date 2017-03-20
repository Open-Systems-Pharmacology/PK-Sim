using PKSim.Presentation.Presenters.Main;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Main
{
   public interface IBuildingBlockExplorerView : IView<IBuildingBlockExplorerPresenter>, IExplorerView, IBatchUpdatable
   {
   }
}