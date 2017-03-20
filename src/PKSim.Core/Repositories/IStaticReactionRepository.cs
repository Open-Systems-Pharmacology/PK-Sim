using System.Collections.Generic;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   /// <summary>
   /// Repository for all static (=non template) reactions
   /// </summary>
   public interface IStaticReactionRepository : IStartableRepository<PKSimReaction>
   {
      IEnumerable<PKSimReaction> AllFor(string modelName);
   }
}
