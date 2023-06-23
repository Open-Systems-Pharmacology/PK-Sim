using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class ContainerParametersNotCommonForAllSpeciesRepository : StartableRepository<ContainerParameterBySpecies>, IContainerParametersNotCommonForAllSpeciesRepository
   {
      private readonly IFlatContainerRepository _flatContainerRepository;
      private readonly IFlatContainerParametersNotCommonForAllSpeciesRepository _flatContainerParametersNotCommonForAllSpeciesRepository;

      private readonly List<ContainerParameterBySpecies> _containerParametersNotCommonForAllSpecies = new List<ContainerParameterBySpecies>();
      private readonly Cache<string, HashSet<string>> _parametersNotCommonForAllSpeciesByContainerPath = new Cache<string, HashSet<string>>(onMissingKey: x => null);
      private readonly Cache<int, HashSet<string>> _parametersNotCommonForAllSpeciesByContainerId = new Cache<int, HashSet<string>>(onMissingKey: x => null);
      private readonly HashSet<string> _parametersNotCommonForAllSpeciesByFullPath = new HashSet<string>();

      public ContainerParametersNotCommonForAllSpeciesRepository(
         IFlatContainerRepository flatContainerRepository,
         IFlatContainerParametersNotCommonForAllSpeciesRepository flatContainerParametersNotCommonForAllSpeciesRepository)
      {
         _flatContainerRepository = flatContainerRepository;
         _flatContainerParametersNotCommonForAllSpeciesRepository = flatContainerParametersNotCommonForAllSpeciesRepository;
      }

      protected override void DoStart()
      {
         var flatContainerParametersNotCommonForAllSpecies = _flatContainerParametersNotCommonForAllSpeciesRepository.All().ToList();

         foreach (var containerParameter in flatContainerParametersNotCommonForAllSpecies)
         {
            var (containerId, parameterName, speciesCount, _) = containerParameter;
            var containerPath = _flatContainerRepository.ContainerPathFrom(containerId);
            var containerParameterBySpecies = new ContainerParameterBySpecies()
            {
               ContainerPath = containerPath,
               ParameterName = parameterName,
               SpeciesCount = speciesCount,
               ContainerId = containerId
            };
            _containerParametersNotCommonForAllSpecies.Add(containerParameterBySpecies);

            //cache by full path of parameter
            containerPath.Add(containerParameter.ParameterName);
            _parametersNotCommonForAllSpeciesByFullPath.Add(containerPath);
         }


         //cache the parameters by container path
         foreach (var containerParametersInContainer in _containerParametersNotCommonForAllSpecies.GroupBy(x => x.ContainerPath))
         {
            var containerId = containerParametersInContainer.First().ContainerId;
            var allParametersInContainer = containerParametersInContainer.Select(cp => cp.ParameterName).ToHashSet();
            _parametersNotCommonForAllSpeciesByContainerPath.Add(containerParametersInContainer.Key, allParametersInContainer);
            _parametersNotCommonForAllSpeciesByContainerId.Add(containerId, allParametersInContainer);
         }
      }

      public override IEnumerable<ContainerParameterBySpecies> All()
      {
         Start();
         return _containerParametersNotCommonForAllSpecies;
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