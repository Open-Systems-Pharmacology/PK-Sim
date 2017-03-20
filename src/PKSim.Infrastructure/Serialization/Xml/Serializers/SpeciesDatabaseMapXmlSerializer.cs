using PKSim.Core;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class SpeciesDatabaseMapXmlSerializer : BaseXmlSerializer<SpeciesDatabaseMap>
   {
      public override void PerformMapping()
      {
         Map(x => x.Species);
         Map(x => x.DatabaseFullPath);
      }
   }
}