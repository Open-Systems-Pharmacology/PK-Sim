using FluentNHibernate.Mapping;
using PKSim.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure.Serialization.ORM.Mapping
{
   public class PopulationSimulationComparisonMetaDataMapping : SubclassMap<PopulationSimulationComparisonMetaData>
   {
      public PopulationSimulationComparisonMetaDataMapping()
      {
         Table("POPULATION_SIMULATION_COMPARISONS");
         KeyColumn("Id");
      }
   }
}