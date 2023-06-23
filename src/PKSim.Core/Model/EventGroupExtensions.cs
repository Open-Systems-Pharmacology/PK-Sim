using System;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using PKSim.Assets;

namespace PKSim.Core.Model
{
   public static class EventGroupExtensions
   {
      public static EventGroupBuilder MainSubContainer(this EventGroupBuilder eventGroupBuilder)
      {
         return eventGroupBuilder.GetSingleChildByName<EventGroupBuilder>(CoreConstants.ContainerName.EventGroupMainSubContainer);
      }

      public static IParameter StartTime(this EventGroupBuilder eventGroupBuilder)
      {
         var startTimeParameter = eventGroupBuilder.Parameter(Constants.Parameters.START_TIME);

         if (startTimeParameter == null)
            throw new ArgumentException(PKSimConstants.Error.NoStartTimeInEventBuilder(eventGroupBuilder.Name, Constants.Parameters.START_TIME));

         return startTimeParameter;
      }
   }
}