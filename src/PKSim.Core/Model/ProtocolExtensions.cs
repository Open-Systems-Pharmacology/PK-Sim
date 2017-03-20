using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public static class ProtocolExtensions
   {
      public static Protocol LongestProtocol(this IEnumerable<Protocol> allProtocols)
      {
         var protocolList = allProtocols.ToList();

         if (!protocolList.Any())
            return null;

         var maxEndTime = protocolList.Max(x => x.EndTime);
         return protocolList.Find(x => ValueComparer.AreValuesEqual(x.EndTime, maxEndTime));
      }
   }
}