using OSPSuite.Core.Domain.Builder;
using OSPSuite.Utility.Collections;

namespace PKSim.Core.Repositories
{
   /// <summary>
   ///    Repository for all predefined events which are NOT applications
   /// </summary>
   public interface IEventGroupRepository : IStartableRepository<EventGroupBuilder>
   {
   }
}