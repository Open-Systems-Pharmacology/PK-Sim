using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.Mappers;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.Infrastructure.ORM.Queries
{
   public class SpeciesContainerQuery : ISpeciesContainerQuery
   {
      private readonly IFlatPopulationContainerRepository _populationContainerRepository;
      private readonly IFlatContainerRepository _flatContainerRepository;
      private readonly IFlatContainerIdToContainerMapper _containerMapper;
      private readonly IEntityPathResolver _entityPathResolver;

      public SpeciesContainerQuery(IFlatPopulationContainerRepository populationContainerRepository,
         IFlatContainerRepository flatContainerRepository,
         IFlatContainerIdToContainerMapper containerMapper,
         IEntityPathResolver entityPathResolver)
      {
         _populationContainerRepository = populationContainerRepository;
         _flatContainerRepository = flatContainerRepository;
         _containerMapper = containerMapper;
         _entityPathResolver = entityPathResolver;
      }

      public IReadOnlyList<IContainer> SubContainersFor(SpeciesPopulation speciesPopulation, IContainer parentContainer)
      {
         var pathToParentContainer = _entityPathResolver.PathFor(parentContainer);
         var flatParentContainer = _flatContainerRepository.ContainerFrom(pathToParentContainer);
         return _populationContainerRepository.AllSubContainerFor(speciesPopulation.Name, flatParentContainer.Id).MapAllUsing(_containerMapper);
      }
   }
}