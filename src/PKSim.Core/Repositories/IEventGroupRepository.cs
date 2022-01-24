using System.Collections.Generic;
using OSPSuite.Utility.Collections;
using OSPSuite.Core.Domain.Builder;

namespace PKSim.Core.Repositories
{
   /// <summary>
   ///    Repository for all predefined events which are NOT applications
   /// </summary>
   public interface IEventGroupRepository : IStartableRepository<IEventGroupBuilder>
   {
     
   }
}