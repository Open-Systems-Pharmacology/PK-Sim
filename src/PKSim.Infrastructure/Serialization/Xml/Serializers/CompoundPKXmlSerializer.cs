using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class CompoundPKXmlSerializer : BaseXmlSerializer<CompoundPK>
   {
      public override void PerformMapping()
      {
         Map(x => x.CompoundName);
         Map(x => x.AucDDI);
         Map(x => x.CmaxDDI);
         Map(x => x.AucIV);
      }
   }
}