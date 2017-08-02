using OSPSuite.Core.Domain;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface IOrganTypeRepository
   {
      OrganType OrganTypeFor(string organName);
      OrganType OrganTypeFor(IContainer container);
   }
}