using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class CompoundGroupSelectionXmlSerializer : BaseXmlSerializer<CompoundGroupSelection>
   {
      public override void PerformMapping()
      {
         Map(x => x.AlternativeName);
         Map(x => x.GroupName);
      }
   }
}