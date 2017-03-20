using FluentNHibernate.Mapping;
using PKSim.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure.Serialization.ORM.Mapping
{
   public class SimulationMetaDataMapping : SubclassMap<SimulationMetaData>
   {
      public SimulationMetaDataMapping()
      {
         Table("SIMULATIONS");
         KeyColumn("SimulationId");

         Map(x => x.SimulationMode);
         Map(x => x.DataRepositoryId).Column("DataRepositoryId").Nullable();

         HasMany(x => x.BuildingBlocks)
            .Not.LazyLoad()
            .Fetch.Join()
            .Cascade.AllDeleteOrphan()
            .ForeignKeyConstraintName("fk_Simulation_BuildingBlocks")
            .KeyColumn("SimulationId")
            .AsSet();

         //observed data mapped as collection of strings
         HasMany(x => x.UsedObservedData)
            .Not.LazyLoad()
            .Table("USED_OBSERVED_DATA")
            .Cascade.AllDeleteOrphan()
            .KeyColumn("SimulationId")
            .Element("ObservedDataId")
            .ForeignKeyConstraintName("fk_Simulation_ObservedData")
            .AsSet();

         HasMany(x => x.Charts)
            .Not.LazyLoad()
            .Fetch.Join()
            .Cascade.AllDeleteOrphan()
            .ForeignKeyConstraintName("fk_Simulation_Charts")
            .KeyColumn("SimulationId")
            .AsSet();

         References(x => x.SimulationResults)
            .LazyLoad()
            .Column("SimulationResultsId")
            .Cascade.All()
            .ForeignKey("fk_Simulation_SimulationResults");
           

         References(x => x.SimulationAnalyses)
            .LazyLoad()
            .Column("SimulationAnalysesId")
            .Cascade.All()
            .ForeignKey("fk_Simulation_SimulationAnalyses");
      }
   }
}