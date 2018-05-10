using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class OriginDataXmlSerializer : BaseXmlSerializer<OriginData>
   {
      public override void PerformMapping()
      {
         Map(x => x.Species);
         Map(x => x.SpeciesPopulation);
         Map(x => x.SubPopulation);
         Map(x => x.Gender);
         Map(x => x.Age);
         Map(x => x.AgeUnit);
         Map(x => x.GestationalAge);
         Map(x => x.GestationalAgeUnit);
         Map(x => x.BMI);
         Map(x => x.BMIUnit);
         Map(x => x.Height);
         Map(x => x.HeightUnit);
         Map(x => x.Weight);
         Map(x => x.WeightUnit);
         Map(x => x.Comment);
         Map(x => x.CalculationMethodCache);
         Map(x => x.ValueOrigin);
      }
   }
}