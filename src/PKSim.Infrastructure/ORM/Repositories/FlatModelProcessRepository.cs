using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public interface IFlatModelProcessRepository : IMetaDataRepository<FlatModelProcess>
   {
      bool Contains(string modelName, string processName);
      IEnumerable<FlatModelProcess> AllFor(string modelName);
   }

   public class FlatModelProcessRepository : MetaDataRepository<FlatModelProcess>, IFlatModelProcessRepository
   {
      private readonly ICache<string, ICache<string, FlatModelProcess>> _processesSortedByModel;

      public FlatModelProcessRepository(IDbGateway dbGateway,
         IDataTableToMetaDataMapper<FlatModelProcess> mapper)
         : base(dbGateway, mapper, CoreConstants.ORM.ViewModelProcesses)
      {
         _processesSortedByModel = new Cache<string, ICache<string, FlatModelProcess>>();
      }

      public bool Contains(string modelName, string processName)
      {
         Start();

         return _processesSortedByModel[modelName].Contains(processName);
      }

      protected override void PerformPostStartProcessing()
      {
         var allModelNames = AllElements().Select(x => x.Model).Distinct();
         allModelNames.Each(fillProcessesForModel);
      }

      public IEnumerable<FlatModelProcess> AllFor(string modelName)
      {
         return All().Where(x => string.Equals(x.Model, modelName));
      }

      private void fillProcessesForModel(string modelName)
      {
         var processesForModel = new Cache<string, FlatModelProcess>(fp => fp.Process);

         foreach (var flatModelProcess in All())
         {
            if (flatModelProcess.Model == modelName)
               processesForModel.Add(flatModelProcess);
         }

         _processesSortedByModel.Add(modelName, processesForModel);
      }
   }
}