using FluentNHibernate.Mapping;
using OSPSuite.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure.Serialization.ORM.Mapping
{
   public class CommandPropertyMetaDataMapping : ClassMap<CommandPropertyMetaData>
   {
      public CommandPropertyMetaDataMapping()
      {
         Table("COMMAND_PROPERTIES");
         Not.LazyLoad();
         Id(x => x.Id).GeneratedBy.Native();
         Map(x => x.Name);
         Map(x => x.Value);
      }
   }
}