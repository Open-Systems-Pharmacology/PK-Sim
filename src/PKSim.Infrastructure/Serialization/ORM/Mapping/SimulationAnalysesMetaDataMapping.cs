using FluentNHibernate.Mapping;
using PKSim.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure.Serialization.ORM.Mapping
{
   public class SimulationAnalysesMetaDataMapping : ClassMap<SimulationAnalysesMetaData>
   {
      public SimulationAnalysesMetaDataMapping()
      {
         Table("SIMULATION_ANALYSES");
         LazyLoad();
         Id(x => x.Id).GeneratedBy.Native();

         //Content should not be lazy loaded for Simulation analyses
         References(x => x.Content)
            .Not.LazyLoad()
            .Column("ContentId")
            .Cascade.All()
            .ForeignKey("fk_SimulationAnalyses_Content");
      }
   }
}