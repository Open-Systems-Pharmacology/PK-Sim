using System.Collections.Generic;
using System.Linq;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using OSPSuite.Core;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Services
{
   public class ApplicationDiscriminator : IApplicationDiscriminator
   {
      private readonly IBuildingBlockRepository _buildingBlockRepository;

      public ApplicationDiscriminator(IBuildingBlockRepository buildingBlockRepository)
      {
         _buildingBlockRepository = buildingBlockRepository;
      }

      public string DiscriminatorFor<T>(T item) where T : IObjectBase
      {
         var bb = item as IPKSimBuildingBlock;
         if (bb != null)
            return bb.BuildingBlockType.ToString();

         return item.GetType().Name;
      }

      public IReadOnlyCollection<IObjectBase> AllFor(string discriminator)
      {
         return _buildingBlockRepository.All().Where(bb => string.Equals(DiscriminatorFor(bb), discriminator)).ToList();
      }
   }
}