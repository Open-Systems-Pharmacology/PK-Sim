using System.Collections.Generic;
using FluentNHibernate.Utils;
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
         individual.Neighborhoods.GetAllChildren<PKSimNeighborhood>().Each(x => setNeighborsIn(x, individual.Organism));
      }

      public void SetNeighborsIn(Organism organism, IReadOnlyList<NeighborhoodBuilder> neighborhoods)
      {
         neighborhoods.Each(x => setNeighborsIn(x, organism));
      }

      private void setNeighborsIn(Neighborhood neighborhood, Organism organism)
      {
         var flatNeighborhood = _flatNeighborhoodRepository.NeighborhoodFrom(neighborhood.Name);

         neighborhood.FirstNeighbor = neighborFrom(flatNeighborhood.FirstNeighborId, organism);
         neighborhood.SecondNeighbor = neighborFrom(flatNeighborhood.SecondNeighborId, organism);
      }

      private void setNeighborsIn(NeighborhoodBuilder neighborhood, Organism organism)
      {
         var flatNeighborhood = _flatNeighborhoodRepository.NeighborhoodFrom(neighborhood.Name);

         neighborhood.FirstNeighborPath = neighborPathFrom(flatNeighborhood.FirstNeighborId, organism);
         neighborhood.SecondNeighborPath = neighborPathFrom(flatNeighborhood.SecondNeighborId, organism);

         neighborhood.ResolveReference(new[] {organism});
      }

      private IContainer neighborFrom(int neighborId, Organism organism) => neighborPathFrom(neighborId, organism).Resolve<IContainer>(organism);

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