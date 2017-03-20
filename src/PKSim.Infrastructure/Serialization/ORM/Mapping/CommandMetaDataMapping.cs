using FluentNHibernate.Mapping;
using OSPSuite.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure.Serialization.ORM.Mapping
{
   public class CommandMetaDataMapping : ClassMap<CommandMetaData>
   {
      public CommandMetaDataMapping()
      {
         Table("COMMANDS");
         Not.LazyLoad();

         Id(x => x.Id).GeneratedBy.Assigned();
         Map(x => x.CommandId).Not.Nullable();
         Map(x => x.Discriminator).Not.Nullable();
         Map(x => x.CommandInverseId);
         Map(x => x.CommandType);
         Map(x => x.ObjectType);
         Map(x => x.Description);
         Map(x => x.ExtendedDescription);
         Map(x => x.Visible);
         Map(x => x.Comment);
         Map(x => x.Sequence);

         References(x => x.Parent)
            .Not.LazyLoad()
            .Column("ParentId")
            .Fetch.Join();

         HasMany(x => x.Properties)
            .LazyLoad()
            .Fetch.Subselect()
            .Cascade.AllDeleteOrphan()
            .AsMap(x => x.Name)
            .KeyColumn("CommandId");

      }
   }
}