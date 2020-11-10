using OSPSuite.Core.Domain;
using PKSim.Core.Model;

namespace PKSim.Core.Repositories
{
   public interface IOrganTypeRepository
   {
      OrganType OrganTypeFor(string organName);

      /// <summary>
      /// Returns the organ type of the given <paramref name="container"/> or Unknown if undefined
      /// </summary>
      OrganType OrganTypeFor(IContainer container);
   }
}