using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class UsedObservedDataXmlSerializer : BaseXmlSerializer<UsedObservedData>
   {
      public override void PerformMapping()
      {
         Map(x => x.Id);
      }
   }
}