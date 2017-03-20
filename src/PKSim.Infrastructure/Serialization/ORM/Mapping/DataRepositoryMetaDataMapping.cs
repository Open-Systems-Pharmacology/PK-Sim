using FluentNHibernate.Mapping;
using OSPSuite.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure.Serialization.ORM.Mapping
{
   public class DataRepositoryMetaDataMapping : ClassMap<DataRepositoryMetaData>
   {
      public DataRepositoryMetaDataMapping()
      {
         Table("DATA_REPOSITORIES");
         Id(x => x.Id).GeneratedBy.Assigned();
         Map(x => x.Name).Not.Nullable();
         Map(x => x.Description);

         References(x => x.Content)
            .Not.LazyLoad()
            .Column("ContentId")
            .Cascade.All()
            .ForeignKey("fk_DataRepository_Content");

      }
   }
}