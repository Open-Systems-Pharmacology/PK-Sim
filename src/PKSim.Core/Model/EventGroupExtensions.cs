using System;
using PKSim.Assets;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;

namespace PKSim.Core.Model
{
   public static class EventGroupExtensions
   {
      public static IEventGroupBuilder MainSubContainer(this IEventGroupBuilder eventGroupBuilder)
      {
         return eventGroupBuilder.GetSingleChildByName<IEventGroupBuilder>(CoreConstants.ContainerName.EventGroupMainSubContainer);
      }

      public static IParameter StartTime(this IEventGroupBuilder eventGroupBuilder)
      {
         var startTimeParameter = eventGroupBuilder.Parameter(Constants.Parameters.START_TIME);

         if (startTimeParameter == null)
            throw new ArgumentException(PKSimConstants.Error.NoStartTimeInEventBuilder.FormatWith(eventGroupBuilder.Name, Constants.Parameters.START_TIME));

         return startTimeParameter;
      }
   }
}