using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Views;
using PKSim.Presentation.Presenters.Observers;

namespace PKSim.Presentation.Views.Observers
{
   public interface ICreateObserverBuildingBlockView : IModalView<ICreateObserverBuildingBlockPresenter>
   {
      void BindToProperties(ObjectBaseDTO observerBuildingBlockDTO);
   }
}