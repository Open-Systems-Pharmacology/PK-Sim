using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.Infrastructure.Services
{
   public class NeighborhoodFinalizer : INeighborhoodFinalizer
   {
      private readonly IFlatNeighborhoodRepository _flatNeighborhoodRepository;
      private readonly IFlatContainerRepository _flatContainerRepository;

      public NeighborhoodFinalizer(IFlatNeighborhoodRepository flatNeighborhoodRepository, IFlatContainerRepository flatContainerRepository)
      {
         _flatNeighborhoodRepository = flatNeighborhoodRepository;
         _flatContainerRepository = flatContainerRepository;
      }

      public void SetNeighborsIn(Individual individual)
      {
         foreach (var neighborhood in individual.Neighborhoods.GetAllChildren<PKSimNeighborhood>())
         {
            setNeighborsIn(neighborhood, neighborhood.Name, individual.Organism);
         }
      }

      public void SetNeighborsIn(Organism organism, IEnumerable<NeighborhoodBuilder> neighborhoods)
      {
         foreach (var neighborhood in neighborhoods)
         {
            setNeighborsIn(neighborhood, neighborhood.Name, organism);
         }
      }

      private void setNeighborsIn(NeighborhoodBuilder neighborhood, string neighborhoodName, Organism organism)
      {
         var flatNeighborhood = _flatNeighborhoodRepository.NeighborhoodFrom(neighborhoodName);

         neighborhood.FirstNeighborPath = neighborPathFrom(flatNeighborhood.FirstNeighborId, organism);
         neighborhood.SecondNeighborPath = neighborPathFrom(flatNeighborhood.SecondNeighborId, organism);

         neighborhood.ResolveReference(new[] {organism,});
      }

      private ObjectPath neighborPathFrom(int neighborId, Organism organism)
      {
         var containerPath = _flatContainerRepository.ContainerPathFrom(neighborId);

         //path from the database do not start with root
         //if called for individual neighborhood (organism is NOT top container),
         //add ROOT as top path entry
         if (organism.ParentContainer != null)
            containerPath.AddAtFront(Constants.ROOT);

         return containerPath;
      }
   }
}