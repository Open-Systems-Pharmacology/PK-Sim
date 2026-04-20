using System.Collections.Generic;
using System.Linq;
using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class SimulationParameterChangeTrackerXmlSerializer : BaseXmlSerializer<SimulationParameterChangeTracker>
   {
      public override void PerformMapping()
      {
         // Map as string to use the stringMap. Current mapping of objectPath is only as attribute, and therefore only singular
         MapEnumerable(x => stringFrom(x), x => x.Track);
      }

      private static IReadOnlyList<string> stringFrom(SimulationParameterChangeTracker tracker)
      {
         return tracker.ChangedPaths.Select(x => x.ToString()).ToList();
      }
   }
}