using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{

   public class OriginDataParameterXmlSerializer : BaseXmlSerializer<OriginDataParameter>
   {
      public override void PerformMapping()
      {
         Map(x => x.Value);
         Map(x => x.Unit);
      }
   }

   public class OriginDataXmlSerializer : BaseXmlSerializer<OriginData>
   {
      public override void PerformMapping()
      {
         Map(x => x.Species);
         Map(x => x.SpeciesPopulation);
         Map(x => x.SubPopulation);
         Map(x => x.Gender);
         Map(x => x.Age);
         Map(x => x.GestationalAge);
         Map(x => x.BMI);
         Map(x => x.Height);
         Map(x => x.Weight);
         Map(x => x.Comment);
         Map(x => x.CalculationMethodCache);
         Map(x => x.ValueOrigin);
      }
   }
}