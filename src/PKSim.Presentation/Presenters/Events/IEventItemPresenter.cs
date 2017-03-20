using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Events
{
   public interface IEventItemPresenter : ISubPresenter
   {
      void EditEvent( PKSim.Core.Model.PKSimEvent pkSimEvent);
   }
}