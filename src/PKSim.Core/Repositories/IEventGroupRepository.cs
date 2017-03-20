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
      /// <summary>
      ///    Returns all event groups which can be created by user
      ///    <para></para>
      ///    (some event groups are e.g. only used for project conversion)
      /// </summary>
      IEnumerable<IEventGroupBuilder> AllForCreationByUser();
   }
}