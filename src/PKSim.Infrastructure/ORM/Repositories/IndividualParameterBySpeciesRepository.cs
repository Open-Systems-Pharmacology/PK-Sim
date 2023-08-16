using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class IndividualParameterBySpeciesRepository : StartableRepository<IndividualParameterBySpecies>, IIndividualParameterBySpeciesRepository
   {
      private readonly IFlatContainerRepository _flatContainerRepository;
      private readonly IFlatIndividualParametersNotCommonForAllSpeciesRepository _flatIndividualParametersNotCommonForAllSpeciesRepository;

      private readonly List<IndividualParameterBySpecies> _individualParametersNotCommonForAllSpecies = new List<IndividualParameterBySpecies>();
      private readonly Cache<string, HashSet<string>> _parametersNotCommonForAllSpeciesByContainerPath = new Cache<string, HashSet<string>>(onMissingKey: x => null);
      private readonly Cache<int, HashSet<string>> _parametersNotCommonForAllSpeciesByContainerId = new Cache<int, HashSet<string>>(onMissingKey: x => null);
      private readonly HashSet<string> _parametersNotCommonForAllSpeciesByFullPath = new HashSet<string>();

      public IndividualParameterBySpeciesRepository(
         IFlatContainerRepository flatContainerRepository,
         IFlatIndividualParametersNotCommonForAllSpeciesRepository flatIndividualParametersNotCommonForAllSpeciesRepository)
      {
         _flatContainerRepository = flatContainerRepository;
         _flatIndividualParametersNotCommonForAllSpeciesRepository = flatIndividualParametersNotCommonForAllSpeciesRepository;
      }

      protected override void DoStart()
      {
         var flatIndividualParametersNotCommonForAllSpecies = _flatIndividualParametersNotCommonForAllSpeciesRepository.All().ToList();

         foreach (var containerParameter in flatIndividualParametersNotCommonForAllSpecies)
         {
            var (containerId, parameterName, speciesCount, _) = containerParameter;
            var containerPath = _flatContainerRepository.ContainerPathFrom(containerId);
            var containerParameterBySpecies = new IndividualParameterBySpecies()
            {
               ContainerPath = containerPath,
               ParameterName = parameterName,
               SpeciesCount = speciesCount,
               ContainerId = containerId
            };
            _individualParametersNotCommonForAllSpecies.Add(containerParameterBySpecies);

            //cache by full path of parameter
            containerPath.Add(containerParameter.ParameterName);
            _parametersNotCommonForAllSpeciesByFullPath.Add(containerPath);
         }


         //cache the parameters by container path
         foreach (var containerParametersInContainer in _individualParametersNotCommonForAllSpecies.GroupBy(x => x.ContainerPath))
         {
            var containerId = containerParametersInContainer.First().ContainerId;
            var allParametersInContainer = containerParametersInContainer.Select(cp => cp.ParameterName).ToHashSet();
            _parametersNotCommonForAllSpeciesByContainerPath.Add(containerParametersInContainer.Key, allParametersInContainer);
            _parametersNotCommonForAllSpeciesByContainerId.Add(containerId, allParametersInContainer);
         }
      }

      public override IEnumerable<IndividualParameterBySpecies> All()
      {
         Start();
         return _individualParametersNotCommonForAllSpecies;
      }

      public bool UsedForAllSpecies(string containerPath, string parameterName)
      {
         Start();
         var parameterNotCommonForContainer = _parametersNotCommonForAllSpeciesByContainerPath[containerPath];
         return isUsedForAllSpecies(parameterNotCommonForContainer, parameterName);
      }

      public bool UsedForAllSpecies(string parameterFullPath)
      {
         Start();
         return !_parametersNotCommonForAllSpeciesByFullPath.Contains(parameterFullPath);
      }

      public bool UsedForAllSpecies(ParameterMetaData parameterMetaData)
      {
         Start();
         var parameterNotCommonForContainer = _parametersNotCommonForAllSpeciesByContainerId[parameterMetaData.ContainerId];
         return isUsedForAllSpecies(parameterNotCommonForContainer, parameterMetaData.ParameterName);
      }

      private bool isUsedForAllSpecies(HashSet<string> allParameterNames, string parameterName)
      {
         return allParameterNames == null || !allParameterNames.Contains(parameterName);
      }
   }
}