using OSPSuite.Presentation.Services;
using PKSim.Infrastructure.Serialization.Xml.Serializers;

namespace PKSim.Presentation.Infrastructure.Serialization.Xml.Serializers
{
   public class DirectoryMapSettingsXmlSerializer: BaseXmlSerializer<DirectoryMapSettings>
   {
      public override void PerformMapping()
      {
         MapEnumerable(x => x.UsedDirectories, x => x.UsedDirectories.Add);
      }
   }
}