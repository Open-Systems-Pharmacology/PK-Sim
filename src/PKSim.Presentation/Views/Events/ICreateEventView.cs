using PKSim.Presentation.Presenters.Events;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Events
{
   public interface ICreateEventView : IModalView<ICreateEventPresenter>, IContainerView
   {
      void BindToProperties(ObjectBaseDTO eventDTO);
   }
}