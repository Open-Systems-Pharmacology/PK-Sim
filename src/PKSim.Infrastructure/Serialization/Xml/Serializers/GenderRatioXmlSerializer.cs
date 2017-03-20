using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class GenderRatioXmlSerializer : BaseXmlSerializer<GenderRatio>
   {
      public override void PerformMapping()
      {
         Map(x => x.Gender);
         Map(x => x.Ratio);
      }
   }
}