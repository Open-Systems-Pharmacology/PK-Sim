using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core.Repositories
{
   public interface IEventKeyRepository : IRepository<string>
   {
   }

   public class EventKeyRepository : IEventKeyRepository
   {
      private readonly IPKSimProjectRetriever _projectRetriever;

      public EventKeyRepository(IPKSimProjectRetriever projectRetriever)
      {
         _projectRetriever = projectRetriever;
      }

      public IEnumerable<string> All()
      {
         return _projectRetriever.CurrentProject
            .All<Protocol>()
            .Where(p => p.IsLoaded)
            .SelectMany(p => p.UsedEventKeys)
            .Where(key => !key.IsNullOrEmpty())
            .Distinct();
      }
   }
}
