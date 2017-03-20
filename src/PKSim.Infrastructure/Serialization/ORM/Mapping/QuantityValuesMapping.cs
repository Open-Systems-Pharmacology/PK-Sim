using FluentNHibernate.Mapping;
using OSPSuite.Core.Domain.Data;

namespace PKSim.Infrastructure.Serialization.ORM.Mapping
{
   public class QuantityValuesMapping : ClassMap<QuantityValues>
   {
      public QuantityValuesMapping()
      {
         Table("QUANTITY_VALUES");
         Not.LazyLoad();
         Id(x => x.Id).GeneratedBy.Native();
         Map(x => x.QuantityPath);
         Map(x => x.ColumnId);

         Map(x => x.Data).CustomType("BinaryBlob")
            .CustomSqlType("image");
      }
   }
}