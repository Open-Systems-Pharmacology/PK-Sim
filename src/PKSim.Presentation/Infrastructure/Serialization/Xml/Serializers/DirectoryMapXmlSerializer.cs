using OSPSuite.Presentation.Services;
using PKSim.Infrastructure.Serialization.Xml.Serializers;

namespace PKSim.Presentation.Infrastructure.Serialization.Xml.Serializers
{
   public class DirectoryMapXmlSerializer : BaseXmlSerializer<DirectoryMap>
   {
      public override void PerformMapping()
      {
         Map(x => x.Key);
         Map(x => x.Path);
      }
   }
}