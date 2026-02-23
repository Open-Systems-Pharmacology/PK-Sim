using System;
using System.Windows.Forms;
using PKSim.Presentation.Core;

namespace PKSim.UI
{
   /// <summary>
   ///    WinForms implementation: schedules an action via <c>Application.Idle</c>, which
   ///    fires once after the Windows message queue is completely drained — the earliest safe
   ///    moment to clear flags set by deferred DevExpress grid-paint events.
   /// </summary>
   public class ApplicationIdleScheduler : IApplicationIdleScheduler
   {
      public void ScheduleOnIdle(Action action)
      {
         EventHandler handler = null;
         handler = (sender, args) =>
         {
            Application.Idle -= handler;
            action();
         };
         Application.Idle += handler;
      }
   }
}
