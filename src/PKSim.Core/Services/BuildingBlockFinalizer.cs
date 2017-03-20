using PKSim.Core.Model;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Services
{
   public interface IBuildingBlockFinalizer
   {
      /// <summary>
      ///    This function should be called whenever a building block is created or loaded
      /// </summary>
      void Finalize(IPKSimBuildingBlock buildingBlock);
   }

   public class BuildingBlockFinalizer : IBuildingBlockFinalizer
   {
      private readonly IReferencesResolver _referencesResolver;
      private readonly IKeywordReplacerTask _keywordReplacerTask;
      private readonly INeighborhoodFinalizer _neighborhoodFinalizer;
      private readonly IBuildingBlockInSimulationManager _buildingBlockInSimulationManager;
      private readonly IIndividualPathWithRootExpander _individualPathWithRootExpander;

      public BuildingBlockFinalizer(IReferencesResolver referencesResolver, IKeywordReplacerTask keywordReplacerTask,
         INeighborhoodFinalizer neighborhoodFinalizer, IBuildingBlockInSimulationManager buildingBlockInSimulationManager, IIndividualPathWithRootExpander individualPathWithRootExpander)
      {
         _referencesResolver = referencesResolver;
         _keywordReplacerTask = keywordReplacerTask;
         _neighborhoodFinalizer = neighborhoodFinalizer;
         _buildingBlockInSimulationManager = buildingBlockInSimulationManager;
         _individualPathWithRootExpander = individualPathWithRootExpander;
      }

      public void Finalize(IPKSimBuildingBlock buildingBlock)
      {
         finalizeIndividual(buildingBlock as Individual);

         var population = buildingBlock as Population;
         if (population != null)
            finalizeIndividual(population.FirstIndividual);

         finalizeSimulation(buildingBlock as Simulation);

         buildingBlock.IsLoaded = true;
      }

      private void finalizeSimulation(Simulation simulation)
      {
         if (simulation == null) return;
         _referencesResolver.ResolveReferencesIn(simulation.Model);
         _buildingBlockInSimulationManager.UpdateBuildingBlockNamesUsedIn(simulation);
         finalizeIndividual(simulation.Individual);
      }

      private void finalizeIndividual(Individual individual)
      {
         if (individual == null) return;

         _neighborhoodFinalizer.SetNeighborsIn(individual);
         _individualPathWithRootExpander.AddRootToPathIn(individual);
         _keywordReplacerTask.ReplaceIn(individual);
         _referencesResolver.ResolveReferencesIn(individual);
      }
   }
}