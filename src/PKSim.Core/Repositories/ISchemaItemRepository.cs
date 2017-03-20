using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface ISchemaItemRepository : IStartableRepository<ISchemaItem>
   {
      ISchemaItem SchemaItemBy(ApplicationType applicationType);
   }
}