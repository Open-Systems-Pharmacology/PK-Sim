using OSPSuite.Presentation.Services;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
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