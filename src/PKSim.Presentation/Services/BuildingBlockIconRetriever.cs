using OSPSuite.Assets;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.ParameterIdentifications;
using OSPSuite.Core.Domain.SensitivityAnalyses;

namespace PKSim.Presentation.Services
{
   public interface IBuildingBlockIconRetriever
   {
      ApplicationIcon IconFor(Simulation simulation);
      ApplicationIcon IconFor(UsedBuildingBlock usedBuildingBlock);
      ApplicationIcon IconFor(ISimulationComparison simulationComparison);
      ApplicationIcon IconFor(ParameterIdentification parameterIdentification);
      ApplicationIcon IconFor(SensitivityAnalysis sensitivityAnalysis);
   }

   public class BuildingBlockIconRetriever : IBuildingBlockIconRetriever
   {
      private readonly IBuildingBlockInSimulationManager _buildingBlockInSimulationManager;
      private readonly IBuildingBlockRepository _buildingBlockRepository;

      public BuildingBlockIconRetriever(IBuildingBlockInSimulationManager buildingBlockInSimulationManager, IBuildingBlockRepository buildingBlockRepository)
      {
         _buildingBlockInSimulationManager = buildingBlockInSimulationManager;
         _buildingBlockRepository = buildingBlockRepository;
      }

      public ApplicationIcon IconFor(Simulation simulation)
      {
         string iconName = simulation.BuildingBlockType.ToString();

         //for pop simulation, we use another icon
         if (simulation.IsAnImplementationOf<PopulationSimulation>())
            iconName = ApplicationIcons.PopulationSimulation.IconName;

         if (simulation.AllowAging)
            iconName = string.Format("Aging{0}", iconName);

         return retrieveIconForStatus(iconName, _buildingBlockInSimulationManager.StatusFor(simulation));
      }

      public ApplicationIcon IconFor(UsedBuildingBlock usedBuildingBlock)
      {
         var iconName = usedBuildingBlock.BuildingBlockType.ToString();
         if (usedBuildingBlock.BuildingBlockType == PKSimBuildingBlockType.Individual)
            iconName = speciesNameFrom(usedBuildingBlock);

         return retrieveIconForStatus(iconName, _buildingBlockInSimulationManager.StatusFor(usedBuildingBlock));
      }

      public ApplicationIcon IconFor(ISimulationComparison simulationComparison)
      {
         if(simulationComparison.IsAnImplementationOf<PopulationSimulationComparison>())
            return ApplicationIcons.PopulationSimulationComparison;

         return ApplicationIcons.IndividualSimulationComparison;
      }

      public ApplicationIcon IconFor(ParameterIdentification parameterIdentification)
      {
         return ApplicationIcons.ParameterIdentification;
      }

      public ApplicationIcon IconFor(SensitivityAnalysis sensitivityAnalysis)
      {
         return ApplicationIcons.SensitivityAnalysis;
      }

      private string speciesNameFrom(UsedBuildingBlock usedBuildingBlock)
      {
         var individual = _buildingBlockRepository.ById(usedBuildingBlock.TemplateId) as Individual;
         return individual == null ? usedBuildingBlock.BuildingBlockType.ToString() : individual.Species.Name;
      }

      private ApplicationIcon retrieveIconForStatus(string iconName, BuildingBlockStatus status)
      {
         return ApplicationIcons.IconByName(string.Format("{0}{1}", iconName, status));
      }
   }
}