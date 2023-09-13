using OSPSuite.Core.Domain.Services;
using PKSim.Core.Model;

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
      private readonly IBuildingBlockInProjectManager _buildingBlockInProjectManager;
      private readonly IIndividualPathWithRootExpander _individualPathWithRootExpander;
      private readonly IFormulaTask _formulaTask;

      public BuildingBlockFinalizer(IReferencesResolver referencesResolver, IKeywordReplacerTask keywordReplacerTask,
         INeighborhoodFinalizer neighborhoodFinalizer, IBuildingBlockInProjectManager buildingBlockInProjectManager,
         IIndividualPathWithRootExpander individualPathWithRootExpander,
         IFormulaTask formulaTask)
      {
         _referencesResolver = referencesResolver;
         _keywordReplacerTask = keywordReplacerTask;
         _neighborhoodFinalizer = neighborhoodFinalizer;
         _buildingBlockInProjectManager = buildingBlockInProjectManager;
         _individualPathWithRootExpander = individualPathWithRootExpander;
         _formulaTask = formulaTask;
      }

      public void Finalize(IPKSimBuildingBlock buildingBlock)
      {
         finalizeIndividual(buildingBlock as Individual);

         if (buildingBlock is Population population)
            finalizeIndividual(population.FirstIndividual);

         finalizeSimulation(buildingBlock as Simulation);

         buildingBlock.IsLoaded = true;
      }

      private void finalizeSimulation(Simulation simulation)
      {
         if (simulation == null) return;
         _referencesResolver.ResolveReferencesIn(simulation.Model);
         _buildingBlockInProjectManager.UpdateBuildingBlockNamesUsedIn(simulation);
         finalizeIndividual(simulation.Individual);
      }

      private void finalizeIndividual(Individual individual)
      {
         if (individual == null) return;

         _neighborhoodFinalizer.SetNeighborsIn(individual);
         _individualPathWithRootExpander.AddRootToPathIn(individual);
         
         _keywordReplacerTask.ReplaceIn(individual);
         _formulaTask.ExpandDynamicReferencesIn(individual.Root);
         _formulaTask.ExpandDynamicFormulaIn(individual);
         _referencesResolver.ResolveReferencesIn(individual);
      }
   }
}