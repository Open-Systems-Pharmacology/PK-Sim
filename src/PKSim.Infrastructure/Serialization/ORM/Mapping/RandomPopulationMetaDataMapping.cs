using FluentNHibernate.Mapping;
using PKSim.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure.Serialization.ORM.Mapping
{
   public class RandomPopulationMetaDataMapping : SubclassMap<RandomPopulationMetaData>
   {
      public RandomPopulationMetaDataMapping()
      {
         Table("RANDOM_POPULATIONS");
         KeyColumn("PopulationId");
      }
   }
}