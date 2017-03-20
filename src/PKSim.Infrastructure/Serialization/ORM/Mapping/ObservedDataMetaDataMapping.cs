using FluentNHibernate.Mapping;
using OSPSuite.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure.Serialization.ORM.Mapping
{
   public class ObservedDataMetaDataMapping : ClassMap<ObservedDataMetaData>
   {
      public ObservedDataMetaDataMapping()
      {
         Table("OBSERVED_DATA");
         Not.LazyLoad();
         Id(x => x.Id).GeneratedBy.Assigned();

         References(x => x.DataRepository)
            .Column("DataRepositoryId")
            .Cascade.All()
            .ForeignKey("fk_ObservedData_DataRepository");
      }
   }
}