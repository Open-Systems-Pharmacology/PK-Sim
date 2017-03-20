using System.Windows.Forms;
using OSPSuite.Utility.Events;

namespace PKSim.Presentation.Services
{
   public class PKSimProgressUpdater : ProgressUpdater
   {
      public PKSimProgressUpdater(IEventPublisher eventPublisher) : base(eventPublisher)
      {
      }

      public override void ReportStatusMessage(string message)
      {
         base.ReportStatusMessage(message);
         Application.DoEvents();
      }
   }
}