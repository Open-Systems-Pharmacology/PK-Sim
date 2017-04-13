using System;
using System.Collections.Generic;
using PKSim.Assets;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Mappers;
using PKSim.Infrastructure.ORM.Repositories;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Infrastructure.ORM.Queries
{
   public class ModelContainerQuery : IModelContainerQuery
   {
      private readonly IFlatModelContainerRepository _modelContainerRepo;
      private readonly IFlatSpeciesContainerRepository _speciesContainerRepo;
      private readonly IFlatContainerRepository _flatContainerRepo;
      private readonly IFlatContainerIdToContainerMapper _flatContainerIdToContainerMapper;
      private readonly IEntityPathResolver _entityPathResolver;

      public ModelContainerQuery(IFlatModelContainerRepository modelContainerRepo,
                                 IFlatSpeciesContainerRepository speciesContainerRepo,
                                 IFlatContainerRepository flatContainerRepo,
                                 IFlatContainerIdToContainerMapper flatContainerIdToContainerMapper,
                                 IEntityPathResolver entityPathResolver)
      {
         _modelContainerRepo = modelContainerRepo;
         _speciesContainerRepo = speciesContainerRepo;
         _flatContainerRepo = flatContainerRepo;
         _flatContainerIdToContainerMapper = flatContainerIdToContainerMapper;
         _entityPathResolver = entityPathResolver;
      }

      public IEnumerable<IContainer> SubContainersFor(Species species, ModelConfiguration modelConfiguration, IContainer parentContainer)
      {
         IList<IContainer> allSubContainers = new List<IContainer>();

         string pathToParentContainer = _entityPathResolver.PathFor(parentContainer);

         var flatParentContainer = _flatContainerRepo.ContainerFrom(pathToParentContainer);
         var flatModelSubContainers = _modelContainerRepo.AllSubContainerFor(modelConfiguration.ModelName, flatParentContainer.Id);
         var allSpeciesSubContainer = speciesSubContainers(species.Name, flatParentContainer.Id);

         foreach (var flatModelContainer in flatModelSubContainers)
         {
            // check if container available in species
            if (allSpeciesSubContainer.Contains(flatModelContainer.Id))
            {
               //model subcontainer available in the species structure - add to model subcontainers and continue
               allSubContainers.Add(_flatContainerIdToContainerMapper.MapFrom(flatModelContainer));
               continue;
            }

            // model subcontainer NOT available in species structure.
            // In this case, action depends on UsageInIndividuum-flag

            if (flatModelContainer.UsageInIndividual == CoreConstants.ORM.UsageInIndividualRequired)
               throw new ArgumentException(PKSimConstants.Error.ModelContainerNotAvailable(_flatContainerRepo.ContainerPathFrom(flatModelContainer.Id).ToString()));

            if (flatModelContainer.UsageInIndividual == CoreConstants.ORM.UsageInIndividualOptional)
               continue; 

            if (flatModelContainer.UsageInIndividual == CoreConstants.ORM.UsageInIndividualExtended)
            {
               allSubContainers.Add(_flatContainerIdToContainerMapper.MapFrom(flatModelContainer));
               continue;
            }

            throw new ArgumentException(PKSimConstants.Error.UnknownUsageInIndividualFlag(flatModelContainer.UsageInIndividual));
         }

         return allSubContainers;
      }

      private ICache<int, FlatSpeciesContainer> speciesSubContainers(string species, int parentContainerId)
      {
         var result = new Cache<int, FlatSpeciesContainer>(flatContainer => flatContainer.Id);
         result.AddRange(_speciesContainerRepo.AllSubContainer(species, parentContainerId));
         return result;
      }
   }
}