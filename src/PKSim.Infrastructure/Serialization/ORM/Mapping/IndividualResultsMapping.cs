using FluentNHibernate.Mapping;
using PKSim.Core.Model;
using OSPSuite.Core.Domain.Data;

namespace PKSim.Infrastructure.Serialization.ORM.Mapping
{
   public class IndividualResultsMapping : ClassMap<IndividualResults>
   {
      public IndividualResultsMapping()
      {
         Table("INDIVIDUAL_RESULTS");
         Not.LazyLoad();
         Id(x => x.Id).GeneratedBy.Native();

         Map(x => x.IndividualId);

         References(x => x.Time)
            .Not.LazyLoad()
            .Column("TimeId")
            .Cascade.All()
            .ForeignKey("fk_IndividualResults_QuantityValues");

         References(x => x.SimulationResults)
            .Not.LazyLoad()
            .Column("SimulationResultsId")
            .Cascade.Delete()
            .ForeignKey("fk_IndividualResults_SimulationResults");

         HasManyToMany(x => x.AllValues)
            .Not.LazyLoad()
            .Fetch.Join()
            .Table("INDIVIDUAL_RESULTS_QUANTITY_VALUES")
            .Cascade.AllDeleteOrphan()
            .ParentKeyColumn("IndividualResultsId")
            .ChildKeyColumn("QuantityValuesId")
            .AsSet();
      }
   }
}