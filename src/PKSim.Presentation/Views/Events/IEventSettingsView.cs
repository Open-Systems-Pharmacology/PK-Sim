
using PKSim.Presentation.DTO.Events;
using PKSim.Presentation.Presenters.Events;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Events
{
   public interface IEventSettingsView : IView<IEventSettingsPresenter>
   {
      void BindTo(EventDTO eventDTO);
      void AddParameterView(IView view);
      bool EventTemplateVisible { get; set; }
   }
}