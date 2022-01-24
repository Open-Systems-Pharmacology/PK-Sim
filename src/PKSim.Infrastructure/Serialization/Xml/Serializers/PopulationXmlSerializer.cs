using PKSim.Core.Model;

namespace PKSim.Infrastructure.Serialization.Xml.Serializers
{
   public abstract class PopulationXmlSerializer<TPopulation> : BuildingBlockXmlSerializer<TPopulation> where TPopulation : Population
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.IndividualValuesCache);
         Map(x => x.SelectedDistributions);
         Map(x => x.Seed);
      }
   }

   public class RandomPopulationXmlSerializer : PopulationXmlSerializer<RandomPopulation>
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         //Not required when serializing a template building block
         //but required when serializing the building block defined in the simulation
         Map(x => x.Settings);
      }
   }


   public class ImportPopulationXmlSerializer : PopulationXmlSerializer<ImportPopulation>
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         //Not required when serializing a template building block
         //but required when serializing the building block defined in the simulation
         Map(x => x.Settings);
      }
   }

   public class MoBiPopulationXmlSerializer : PopulationXmlSerializer<MoBiPopulation>
   {
      public override void PerformMapping()
      {
         base.PerformMapping();
         Map(x => x.NumberOfItems);
      }
   }
}