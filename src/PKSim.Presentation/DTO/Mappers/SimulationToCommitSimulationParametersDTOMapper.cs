using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.DTO.Simulations;


namespace PKSim.Presentation.DTO.Mappers
{
   public interface ISimulationToCommitSimulationParametersDTOMapper : IMapper<Simulation, CommitSimulationParametersDTO>
   {
   }

   public class SimulationToCommitSimulationParametersDTOMapper : ISimulationToCommitSimulationParametersDTOMapper
   {
      private readonly IContainerTask _containerTask;
      private readonly IBuildingBlockInProjectManager _buildingBlockInProjectManager;

      public SimulationToCommitSimulationParametersDTOMapper(IContainerTask containerTask, IBuildingBlockInProjectManager buildingBlockInProjectManager)
      {
         _containerTask = containerTask;
         _buildingBlockInProjectManager = buildingBlockInProjectManager;
      }

      public CommitSimulationParametersDTO MapFrom(Simulation simulation)
      {
         var dto = new CommitSimulationParametersDTO();
         var parameterCache = _containerTask.CacheAllChildren<IParameter>(simulation.Model.Root);

         var pathsByCompound = simulation.ParameterChangeTracker.ChangedPaths
            .Select(p => p.PathAsString)
            .GroupBy(path => simulation.CompoundNameForParameterPath(path))
            .Where(g => g.Key != null);

         pathsByCompound.Each(group =>
         {
            var compoundName = group.Key;
            var templateCompound = templateCompoundFor(simulation, compoundName);
            if (templateCompound == null)
               return;

            dto.Compounds.Add(mapCompoundCommitDTO(compoundName, templateCompound, group, parameterCache));
         });

         return dto;
      }

      private Compound templateCompoundFor(Simulation simulation, string compoundName)
      {
         var simulationCompound = simulation.Compounds.FirstOrDefault(c => c.Name == compoundName);
         if (simulationCompound == null)
            return null;

         return _buildingBlockInProjectManager.TemplateBuildingBlockUsedBy<Compound>(simulation, simulationCompound);
      }

      private CompoundCommitDTO mapCompoundCommitDTO(string compoundName, Compound templateCompound, IGrouping<string, string> paths, PathCache<IParameter> parameterCache)
      {
         return new CompoundCommitDTO
         {
            CompoundName = compoundName,
            TemplateCompound = templateCompound,
            AvailableExistingSets = templateCompound.OverwriteParameterSets,
            NewSetName = compoundName,
            Parameters = paths.Select(path => mapParameterCommitDTO(path, parameterCache)).ToList()
         };
      }

      private ParameterCommitDTO mapParameterCommitDTO(string path, PathCache<IParameter> parameterCache)
      {
         var parameter = parameterCache[path];
         return new ParameterCommitDTO
         {
            Path = path,
            DisplayPath = path,
            Value = parameter?.Value ?? double.NaN
         };
      }
   }
}
