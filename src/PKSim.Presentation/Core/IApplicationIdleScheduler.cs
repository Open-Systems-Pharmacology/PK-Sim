using System;

namespace PKSim.Presentation.Core
{
   /// <summary>
   ///    Schedules an action to run once the application message queue is fully drained.
   ///    Abstraction over <c>Application.Idle</c> so the platform-neutral Presentation layer
   ///    does not need a WinForms reference.
   /// </summary>
   public interface IApplicationIdleScheduler
   {
      void ScheduleOnIdle(Action action);
   }
}
