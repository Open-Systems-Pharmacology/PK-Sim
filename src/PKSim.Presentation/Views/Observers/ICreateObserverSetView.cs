using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Views;
using PKSim.Presentation.Presenters.Observers;

namespace PKSim.Presentation.Views.Observers
{
   public interface ICreateObserverSetView : IModalView<ICreateObserverSetPresenter>, IContainerView
   {
      void BindToProperties(ObjectBaseDTO observerBuildingBlockDTO);
   }
}