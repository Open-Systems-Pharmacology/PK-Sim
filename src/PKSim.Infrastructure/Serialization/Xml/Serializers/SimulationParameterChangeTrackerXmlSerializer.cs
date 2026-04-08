using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class SimulationParameterChangeTrackerXmlSerializer : BaseXmlSerializer<SimulationParameterChangeTracker>
   {
      public override void PerformMapping()
      {
         MapEnumerable(x => x.ChangedPaths, x => x.Track);
      }
   }
}
