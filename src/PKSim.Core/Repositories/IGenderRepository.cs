using OSPSuite.Utility.Collections;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface IGenderRepository : IStartableRepository<Gender>
   {
      Gender Male { get; }
      Gender Female { get; }
      Gender FindByIndex(int index);
   }
}
