using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public interface IEventMapping
   {
      /// <summary>
      ///    Id of template event used in mapping
      /// </summary>
      string TemplateEventId { get; set; }

      /// <summary>
      ///    Start time at which the event will be triggered
      /// </summary>
      IParameter StartTime { get; }

      IEventMapping Clone(ICloneManager cloneManager);
   }

   public class EventMapping : IEventMapping
   {
      /// <summary>
      ///    Event id use in mapping
      /// </summary>
      public string TemplateEventId { get; set; }

      public IParameter StartTime { get; set; }

      public IEventMapping Clone(ICloneManager cloneManager)
      {
         return new EventMapping {TemplateEventId = TemplateEventId, StartTime = cloneManager.Clone(StartTime)};
      }
   }
}