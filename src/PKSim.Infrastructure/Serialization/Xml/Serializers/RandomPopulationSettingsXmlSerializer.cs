using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public class RandomPopulationSettingsXmlSerializer : BaseXmlSerializer<RandomPopulationSettings>
   {
      public override void PerformMapping()
      {
         Map(x => x.BaseIndividual);
         Map(x => x.NumberOfIndividuals);
         MapEnumerable(x => x.GenderRatios, x => x.AddGenderRatio);
         MapEnumerable(x => x.ParameterRanges, x => x.AddParameterRange);
      }
   }
}