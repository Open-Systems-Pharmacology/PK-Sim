using FluentNHibernate.Mapping;
using PKSim.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure.Serialization.ORM.Mapping
{
   public class IndividualSimulationComparisonMetaDataMapping: SubclassMap<IndividualSimulationComparisonMetaData>
   {
      public IndividualSimulationComparisonMetaDataMapping()
      {
         Table("INDIVIDUAL_SIMULATION_COMPARISONS");
         KeyColumn("Id");
      }
   }
}