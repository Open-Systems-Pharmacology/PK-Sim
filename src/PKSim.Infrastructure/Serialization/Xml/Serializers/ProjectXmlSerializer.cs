using PKSim.Core.Model;
using OSPSuite.Core.Serialization.Xml;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class ProjectXmlSerializer : ProjectXmlSerializer<PKSimProject>, IPKSimXmlSerializer
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.OutputSelections);

         //no need to serialize charts that are saved in the database
      }
   }
}