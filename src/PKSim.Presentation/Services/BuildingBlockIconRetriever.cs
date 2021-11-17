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
      ApplicationIcon IconFor(QualificationPlan qualificationPlan);
   }

   public class BuildingBlockIconRetriever : IBuildingBlockIconRetriever
   {
      private readonly IBuildingBlockInProjectManager _buildingBlockInProjectManager;
      private readonly IBuildingBlockRepository _buildingBlockRepository;

      public BuildingBlockIconRetriever(IBuildingBlockInProjectManager buildingBlockInProjectManager, IBuildingBlockRepository buildingBlockRepository)
      {
         _buildingBlockInProjectManager = buildingBlockInProjectManager;
         _buildingBlockRepository = buildingBlockRepository;
      }

      public ApplicationIcon IconFor(Simulation simulation)
      {
         var iconName = simulation.BuildingBlockType.ToString();

         //for pop simulation, we use another icon
         if (simulation.IsAnImplementationOf<PopulationSimulation>())
            iconName = ApplicationIcons.PopulationSimulation.IconName;

         if (simulation.AllowAging)
            iconName = $"Aging{iconName}";

         return retrieveIconForStatus(iconName, _buildingBlockInProjectManager.StatusFor(simulation));
      }

      public ApplicationIcon IconFor(UsedBuildingBlock usedBuildingBlock)
      {
         var iconName = usedBuildingBlock.BuildingBlockType.ToString();

         switch (usedBuildingBlock.BuildingBlockType)
         {
            case PKSimBuildingBlockType.Individual:
               iconName = individualIconFor(usedBuildingBlock);
               break;
            case PKSimBuildingBlockType.ObserverSet:
               iconName = ApplicationIcons.Observer.IconName;
               break;
         }

         return retrieveIconForStatus(iconName, _buildingBlockInProjectManager.StatusFor(usedBuildingBlock));
      }

      public ApplicationIcon IconFor(ISimulationComparison simulationComparison)
      {
         if(simulationComparison.IsAnImplementationOf<PopulationSimulationComparison>())
            return ApplicationIcons.PopulationSimulationComparison;

         return ApplicationIcons.IndividualSimulationComparison;
      }

      public ApplicationIcon IconFor(ParameterIdentification parameterIdentification) => ApplicationIcons.ParameterIdentification;

      public ApplicationIcon IconFor(SensitivityAnalysis sensitivityAnalysis) => ApplicationIcons.SensitivityAnalysis;

      public ApplicationIcon IconFor(QualificationPlan qualificationPlan) => ApplicationIcons.Formula;

      private string individualIconFor(UsedBuildingBlock usedBuildingBlock)
      {
         var individual = _buildingBlockRepository.ById(usedBuildingBlock.TemplateId) as Individual;
         return individual == null ? usedBuildingBlock.BuildingBlockType.ToString() : individual.Icon;
      }

      private ApplicationIcon retrieveIconForStatus(string iconName, BuildingBlockStatus status) => ApplicationIcons.IconByName($"{iconName}{status}");
   }
}