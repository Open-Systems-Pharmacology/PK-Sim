using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.Mappers;
using PKSim.Infrastructure.ORM.Repositories;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Infrastructure.ORM.Queries
{
   public class SpeciesContainerQuery : ISpeciesContainerQuery
   {
      private readonly IFlatSpeciesContainerRepository _speciesContainerRepository;
      private readonly IFlatContainerRepository _flatContainerRepository;
      private readonly IFlatContainerIdToContainerMapper _containerMapper;
      private readonly IEntityPathResolver _entityPathResolver;

      public SpeciesContainerQuery(IFlatSpeciesContainerRepository speciesContainerRepository,
                                   IFlatContainerRepository flatContainerRepository,
                                   IFlatContainerIdToContainerMapper containerMapper,
                                   IEntityPathResolver entityPathResolver)
      {
         _speciesContainerRepository = speciesContainerRepository;
         _flatContainerRepository = flatContainerRepository;
         _containerMapper = containerMapper;
         _entityPathResolver = entityPathResolver;
      }

      public IEnumerable<IContainer> SubContainersFor(Species species, IContainer parentContainer)
      {
         var pathToParentContainer = _entityPathResolver.PathFor(parentContainer);

         var flatParentContainer = _flatContainerRepository.ContainerFrom(pathToParentContainer);

         return _speciesContainerRepository.AllSubContainer(species.Name, flatParentContainer.Id)
                                     .Select(speciesContainer => _containerMapper.MapFrom(speciesContainer));
      }

 
   }
}