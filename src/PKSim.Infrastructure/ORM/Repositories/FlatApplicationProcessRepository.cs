using System.Collections.Generic;
using System.Linq;
using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatApplicationProcessRepository : IMetaDataRepository<FlatApplicationProcess>
   {
       IEnumerable<string> ProcessNamesFor(string applicationName);
   }

   public class FlatApplicationProcessRepository : MetaDataRepository<FlatApplicationProcess>, IFlatApplicationProcessRepository
   {
       public FlatApplicationProcessRepository(IDbGateway dbGateway,
                                         IDataTableToMetaDataMapper<FlatApplicationProcess> mapper)
           : base(dbGateway, mapper, CoreConstants.ORM.ViewApplicationProcesses)
       {}

       public IEnumerable<string> ProcessNamesFor(string applicationName)
       {
           return from flatAppProcess in All()
                  where flatAppProcess.Application.Equals(applicationName)
                  select flatAppProcess.Process;
       }
   }

}
