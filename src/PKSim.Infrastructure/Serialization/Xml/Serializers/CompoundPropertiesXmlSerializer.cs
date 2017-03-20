using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class CompoundPropertiesXmlSerializer : BaseXmlSerializer<CompoundProperties>
   {
      public override void PerformMapping()
      {
         Map(x => x.ProtocolProperties);
         Map(x => x.Processes);
         Map(x => x.CalculationMethodCache);
         MapReference(x => x.Compound);
         MapEnumerable(x => x.CompoundGroupSelections, x => x.AddCompoundGroupSelection);
      }
   }
}