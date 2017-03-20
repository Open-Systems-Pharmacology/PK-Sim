using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class CompoundXmlSerializer : BuildingBlockXmlSerializer<Compound>
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.CalculationMethodCache);
      }
   }
}