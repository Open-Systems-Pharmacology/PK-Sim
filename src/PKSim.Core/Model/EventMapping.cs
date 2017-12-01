using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class EventMapping
   {
      /// <summary>
      ///    Id of template event used in mapping
      /// </summary>
      public string TemplateEventId { get; set; }

      /// <summary>
      ///    Start time at which the event will be triggered
      /// </summary>
      public IParameter StartTime { get; set; }

      public EventMapping Clone(ICloneManager cloneManager)
      {
         return new EventMapping {TemplateEventId = TemplateEventId, StartTime = cloneManager.Clone(StartTime)};
      }
   }
}