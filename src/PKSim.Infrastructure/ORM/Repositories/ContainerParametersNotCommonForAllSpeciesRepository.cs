using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using PKSim.Core.Repositories;

namespace PKSim.Infrastructure.ORM.Repositories
{
   public class ContainerParametersNotCommonForAllSpeciesRepository: StartableRepository<(string ContainerPath, string ParameterName, int SpeciesCount)>, IContainerParametersNotCommonForAllSpeciesRepository
   {
      private readonly IFlatContainerRepository _flatContainerRepository;
      private readonly IFlatContainerParametersNotCommonForAllSpeciesRepository _flatContainerParametersNotCommonForAllSpeciesRepository;

      private readonly List<(string ContainerPath, string ParameterName, int SpeciesCount)> _containerParametersNotCommonForAllSpecies;
      private readonly ICache<string, List<string>> _parametersNotCommonForAllSpeciesByContainer;

      public ContainerParametersNotCommonForAllSpeciesRepository(
         IFlatContainerRepository flatContainerRepository,
         IFlatContainerParametersNotCommonForAllSpeciesRepository flatContainerParametersNotCommonForAllSpeciesRepository)
      {
         _flatContainerRepository = flatContainerRepository;
         _flatContainerParametersNotCommonForAllSpeciesRepository = flatContainerParametersNotCommonForAllSpeciesRepository;

         _containerParametersNotCommonForAllSpecies = new List<(string ContainerPath, string ParameterName, int SpeciesCount)>();
         _parametersNotCommonForAllSpeciesByContainer = new Cache<string, List<string>>();
   }

      protected override void DoStart()
      {
         var flatContainerParametersNotCommonForAllSpecies = _flatContainerParametersNotCommonForAllSpeciesRepository.All().ToList();

         foreach(var containerParameter in flatContainerParametersNotCommonForAllSpecies)
         {
            var containerPath = _flatContainerRepository.ContainerPathFrom(containerParameter.ContainerId).ToString();
            _containerParametersNotCommonForAllSpecies.Add((containerPath, containerParameter.ParameterName, containerParameter.SpeciesCount));
         }

         //cache the parameters by container path
         foreach (var containerParametersInContainer in _containerParametersNotCommonForAllSpecies.GroupBy(x => x.ContainerPath))
         {
            _parametersNotCommonForAllSpeciesByContainer.Add(containerParametersInContainer.Key,  containerParametersInContainer.Select(cp=>cp.ParameterName).ToList());
         }
      }

      public override IEnumerable<(string ContainerPath, string ParameterName, int SpeciesCount)> All()
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
   }
}