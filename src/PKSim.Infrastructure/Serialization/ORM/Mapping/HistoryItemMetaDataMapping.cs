using FluentNHibernate.Mapping;
using OSPSuite.Infrastructure.Serialization.ORM.History;

namespace PKSim.Infrastructure.Serialization.ORM.Mapping
{
   public class HistoryItemMetaDataMapping : ClassMap<HistoryItemMetaData>
   {
      public HistoryItemMetaDataMapping()
      {
         Table("HISTORY_ITEMS");
         Not.LazyLoad();
         Id(x => x.Id).GeneratedBy.Assigned();
         Map(x => x.User).Column("UserId");
         Map(x => x.DateTime);
         Map(x => x.State);
         Map(x => x.Sequence);
         References(x => x.Command)
            .Not.LazyLoad()
            .Column("CommandId")
            .Fetch.Join()
            .Cascade.All()
            .ForeignKey("fk_HistoryItem_Command");
      }
   }
}