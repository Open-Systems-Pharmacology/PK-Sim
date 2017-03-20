using FluentNHibernate.Mapping;
using PKSim.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure.Serialization.ORM.Mapping
{
   public class SimulationComparisonMetaDataMapping : ClassMap<SimulationComparisonMetaData>
   {
      public SimulationComparisonMetaDataMapping()
      {
         Table("SIMULATION_COMPARISONS");
         Not.LazyLoad();
         Id(x => x.Id).GeneratedBy.Assigned();
         Map(x => x.Name).Not.Nullable();
         Map(x => x.Description);
 
         //Content should be lazy loaded for a summary block
         References(x => x.Content)
            .LazyLoad()
            .Column("ContentId")
            .Cascade.All()
            .ForeignKey("fk_SimulationComparison_Content");
      }
   }
}