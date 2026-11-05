using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class TemplateSourceXmlSerializer : BaseXmlSerializer<TemplateSource>
   {
      public override void PerformMapping()
      {
         Map(x => x.Name);
         Map(x => x.Url);
      }
   }
}
