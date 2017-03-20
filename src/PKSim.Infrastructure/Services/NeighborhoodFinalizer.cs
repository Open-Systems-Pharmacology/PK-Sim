using System.Collections.Generic;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.Repositories;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;

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
         foreach (var neighborhood in individual.Neighborhoods.GetAllChildren<IPKSimNeighborhood>())
         {
            setNeighborsIn(neighborhood, neighborhood.Name, individual.Organism);
         }
      }

      public void SetNeighborsIn(Organism organism, IEnumerable<INeighborhoodBuilder> neighborhoods)
      {
         foreach (var neighborhood in neighborhoods)
         {
            setNeighborsIn(neighborhood, neighborhood.Name, organism);
         }
      }

      private void setNeighborsIn(INeighborhoodBase neighborhood, string neighborhoodName, Organism organism)
      {
         var flatNeighborhood = _flatNeighborhoodRepository.NeighborhoodFrom(neighborhoodName);

         neighborhood.FirstNeighbor = neighborFrom(flatNeighborhood.FirstNeighborId, organism);
         neighborhood.SecondNeighbor = neighborFrom(flatNeighborhood.SecondNeighborId, organism);
      }

      private IContainer neighborFrom(int neighborId, Organism organism)
      {
         var containerPath = _flatContainerRepository.ContainerPathFrom(neighborId);

         //path from the database do not start with root
         //if called for individual neighborhood (organism is NOT top container),
         //add ROOT as top path entry
         if (organism.ParentContainer != null)
            containerPath.AddAtFront(Constants.ROOT);

         return containerPath.Resolve<IContainer>(organism);
      }
   }
}