using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Collections;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class ContainerParametersNotCommonForAllSpeciesRepository: StartableRepository<ContainerParameterBySpecies>, IContainerParametersNotCommonForAllSpeciesRepository
   {
      private readonly IFlatContainerRepository _flatContainerRepository;
      private readonly IFlatContainerParametersNotCommonForAllSpeciesRepository _flatContainerParametersNotCommonForAllSpeciesRepository;

      private readonly List<ContainerParameterBySpecies> _containerParametersNotCommonForAllSpecies;
      private readonly ICache<string, List<string>> _parametersNotCommonForAllSpeciesByContainer;
      private readonly ICache<string, string> _parametersNotCommonForAllSpeciesByFullPath;

      public ContainerParametersNotCommonForAllSpeciesRepository(
         IFlatContainerRepository flatContainerRepository,
         IFlatContainerParametersNotCommonForAllSpeciesRepository flatContainerParametersNotCommonForAllSpeciesRepository)
      {
         _flatContainerRepository = flatContainerRepository;
         _flatContainerParametersNotCommonForAllSpeciesRepository = flatContainerParametersNotCommonForAllSpeciesRepository;

         _containerParametersNotCommonForAllSpecies = new List<ContainerParameterBySpecies>();
         _parametersNotCommonForAllSpeciesByContainer = new Cache<string, List<string>>();
         _parametersNotCommonForAllSpeciesByFullPath = new Cache<string, string>();
      }

      protected override void DoStart()
      {
         var flatContainerParametersNotCommonForAllSpecies = _flatContainerParametersNotCommonForAllSpeciesRepository.All().ToList();

         foreach(var containerParameter in flatContainerParametersNotCommonForAllSpecies)
         {
            var containerPath = _flatContainerRepository.ContainerPathFrom(containerParameter.ContainerId).ToString();
            var containerParameterBySpecies = new ContainerParameterBySpecies()
            {
               ContainerPath = containerPath,
               ParameterName = containerParameter.ParameterName,
               SpeciesCount = containerParameter.SpeciesCount
            };
            _containerParametersNotCommonForAllSpecies.Add(containerParameterBySpecies);

            //cache by full path
            var fullPath = $"{containerPath}{ObjectPath.PATH_DELIMITER}{containerParameter.ParameterName}";
            _parametersNotCommonForAllSpeciesByFullPath.Add(fullPath, fullPath);
         }

         //cache the parameters by container path
         foreach (var containerParametersInContainer in _containerParametersNotCommonForAllSpecies.GroupBy(x => x.ContainerPath))
         {
            _parametersNotCommonForAllSpeciesByContainer.Add(containerParametersInContainer.Key,  containerParametersInContainer.Select(cp=>cp.ParameterName).ToList());
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
         var parametersUsedNotForAllSpeciesInContainer = _parametersNotCommonForAllSpeciesByContainer[containerPath];

         return parametersUsedNotForAllSpeciesInContainer == null || !parametersUsedNotForAllSpeciesInContainer.Contains(parameterName);
      }

      public bool UsedForAllSpecies(string parameterFullPath)
      {
         Start();
         return !_parametersNotCommonForAllSpeciesByFullPath.Contains(parameterFullPath);
      }
   }
}