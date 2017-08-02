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
      private readonly IFlatPopulationContainerRepository _populationContainerRepo;
      private readonly IFlatContainerRepository _flatContainerRepo;
      private readonly IFlatContainerIdToContainerMapper _flatContainerIdToContainerMapper;
      private readonly IEntityPathResolver _entityPathResolver;

      public ModelContainerQuery(IFlatModelContainerRepository modelContainerRepo,
                                 IFlatPopulationContainerRepository populationContainerRepo,
                                 IFlatContainerRepository flatContainerRepo,
                                 IFlatContainerIdToContainerMapper flatContainerIdToContainerMapper,
                                 IEntityPathResolver entityPathResolver)
      {
         _modelContainerRepo = modelContainerRepo;
         _populationContainerRepo = populationContainerRepo;
         _flatContainerRepo = flatContainerRepo;
         _flatContainerIdToContainerMapper = flatContainerIdToContainerMapper;
         _entityPathResolver = entityPathResolver;
      }

      public IReadOnlyList<IContainer> SubContainersFor(SpeciesPopulation population, ModelConfiguration modelConfiguration, IContainer parentContainer)
      {
         var allSubContainers = new List<IContainer>();

         string pathToParentContainer = _entityPathResolver.PathFor(parentContainer);

         var flatParentContainer = _flatContainerRepo.ContainerFrom(pathToParentContainer);
         var flatModelSubContainers = _modelContainerRepo.AllSubContainerFor(modelConfiguration.ModelName, flatParentContainer.Id);
         var allSpeciesSubContainer = populationSubContainers(population, flatParentContainer);

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

      private ICache<int, FlatPopulationContainer> populationSubContainers(SpeciesPopulation population, FlatContainer parentContainer)
      {
         var result = new Cache<int, FlatPopulationContainer>(x => x.Id);
         result.AddRange(_populationContainerRepo.AllSubContainerFor(population.Name, parentContainer.Id));
         return result;
      }
   }
}