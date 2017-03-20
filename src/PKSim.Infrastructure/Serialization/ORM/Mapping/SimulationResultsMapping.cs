using FluentNHibernate.Mapping;
using PKSim.Core.Model;
using OSPSuite.Core.Domain.Data;

namespace PKSim.Infrastructure.Serialization.ORM.Mapping
{
   public class SimulationResultsMapping : ClassMap<SimulationResults>
   {
      public SimulationResultsMapping()
      {
         Table("SIMULATION_RESULTS");
         LazyLoad();
         Id(x => x.Id).GeneratedBy.Native();
       
         References(x => x.Time)
            .Not.LazyLoad()
            .Column("TimeId")
            .Cascade.All()
            .ForeignKey("fk_SimulationResults_QuantityValues");

         HasMany(x => x.AllIndividualResults)
            .LazyLoad()
            .Fetch.Join()
            .Table("INDIVIDUAL_RESULTS")
            .Cascade.AllDeleteOrphan()
            .KeyColumn("SimulationResultsId")
            .Inverse()
            .ForeignKeyConstraintName("fk_SimulationResults_IndividualResults")
            .AsSet();
      }
   }
}