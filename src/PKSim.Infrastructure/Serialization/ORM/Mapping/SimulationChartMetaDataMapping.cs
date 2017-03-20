using FluentNHibernate.Mapping;
using PKSim.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure.Serialization.ORM.Mapping
{
   public class SimulationChartMetaDataMapping : ClassMap<SimulationChartMetaData>
   {
      public SimulationChartMetaDataMapping()
      {
         Table("SIMULATION_CHARTS");
         Not.LazyLoad();
         Id(x => x.Id).GeneratedBy.Assigned();
         Map(x => x.Name).Not.Nullable();
         Map(x => x.Description);

         //Content should not be lazy loaded for simulation charts
         References(x => x.Content)
            .Not.LazyLoad()
            .Column("ContentId")
            .Cascade.All()
            .ForeignKey("fk_SimulationChart_Content");
      }
   }
}