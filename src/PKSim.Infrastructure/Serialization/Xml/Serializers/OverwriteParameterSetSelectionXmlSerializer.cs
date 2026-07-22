using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class OverwriteParameterSetSelectionXmlSerializer : BaseXmlSerializer<OverwriteParameterSetSelection>
   {
      public override void PerformMapping()
      {
         Map(x => x.CompoundName);
         MapReference(x => x.OverwriteParameterSet);
      }
   }
}
