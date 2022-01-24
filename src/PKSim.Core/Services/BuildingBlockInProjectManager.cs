using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Events;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.Core.Services
{
   public interface IBuildingBlockInProjectManager
   {
      /// <summary>
      ///    Each simulation using the given building block will notify a status update
      /// </summary>
      /// <param name="buildingBlock">building block used to filter the simulation that will notify an update</param>
      void UpdateStatusForSimulationUsing(IPKSimBuildingBlock buildingBlock);

      /// <summary>
      ///    Returns all <see cref="Simulation" /> using the given <paramref name="buildingBlock" />
      /// </summary>
      /// <param name="buildingBlock">building block used to filter the simulation</param>
      IReadOnlyList<Simulation> SimulationsUsing(IPKSimBuildingBlock buildingBlock);

      /// <summary>
      ///    Returns all <see cref="IPKSimBuildingBlock" /> using the given <paramref name="buildingBlock" />.
      ///    For instance Individual using a given expression profile. This also returns Simulation using the
      ///    <paramref name="buildingBlock" />
      /// </summary>
      IReadOnlyList<IPKSimBuildingBlock> BuildingBlockUsing(IPKSimBuildingBlock buildingBlock);

      /// <summary>
      ///    Returns the status for the used building block given as parameter
      /// </summary>
      /// <param name="usedBuildingBlock">Used building block for which the status should be retrieved</param>
      BuildingBlockStatus StatusFor(UsedBuildingBlock usedBuildingBlock);

      /// <summary>
      ///    Returns the status for the simulation given as parameter
      /// </summary>
      /// <param name="simulation">Simulation for which the status shoul be retrieved</param>
      BuildingBlockStatus StatusFor(Simulation simulation);

      /// <summary>
      ///    Set the name that should be used for the building blocks according to the  building block status
      /// </summary>
      void UpdateBuildingBlockNamesUsedIn(Simulation simulation);

      /// <summary>
      ///    Returns the template building blocks used for a given type in a simulation
      ///    If the template used in the simulation is the same as one available in the project as template, returns the
      ///    template otherwise returns the building block used when the simulation was created
      /// </summary>
      IReadOnlyList<TBuildingBlock> TemplateBuildingBlocksUsedBy<TBuildingBlock>(Simulation simulation) where TBuildingBlock : class, IPKSimBuildingBlock;

      /// <summary>
      ///    Returns the template building block used for a given used building block
      ///    If the template used in the simulation is the same as one available in the building block repository, returns the
      ///    repository item
      ///    otherwise returns the building block used when the simulation was created
      /// </summary>
      TBuildingBlock TemplateBuildingBlockUsedBy<TBuildingBlock>(UsedBuildingBlock usedBuildingBlock) where TBuildingBlock : class, IPKSimBuildingBlock;

      /// <summary>
      ///    Returns the template building block used for a given used building block
      ///    If the template used in the simulation is the same as one available in the building block repository, returns the
      ///    repository item
      ///    otherwise returns the building block used when the simulation was created
      /// </summary>
      IPKSimBuildingBlock TemplateBuildingBlockUsedBy(UsedBuildingBlock usedBuildingBlock);

      /// <summary>
      ///    Returns the template building block used to create <paramref name="buildingBlockInSimulation" /> used by
      ///    <paramref name="simulation" />.
      ///    If the template used in the simulation is the same as one available in the template building block repository,
      ///    returns the template building block
      ///    otherwise returns the <paramref name="buildingBlockInSimulation" />
      /// </summary>
      TBuildingBlock TemplateBuildingBlockUsedBy<TBuildingBlock>(Simulation simulation, TBuildingBlock buildingBlockInSimulation) where TBuildingBlock : class, IPKSimBuildingBlock;

      /// <summary>
      ///    Simulation given as parameter will notify status update
      /// </summary>
      void UpdateStatusForSimulation(Simulation simulation);

      /// <summary>
      ///    Returns the <see cref="Simulation" /> using the given <paramref name="usedBuildingBlock" /> or <c>null</c> if not
      ///    found
      /// </summary>
      Simulation SimulationUsing(UsedBuildingBlock usedBuildingBlock);
   }

   public class BuildingBlockInProjectManager : IBuildingBlockInProjectManager
   {
      private readonly IBuildingBlockRepository _buildingBlockRepository;
      private readonly IEventPublisher _eventPublisher;

      public BuildingBlockInProjectManager(IBuildingBlockRepository buildingBlockRepository, IEventPublisher eventPublisher)
      {
         _buildingBlockRepository = buildingBlockRepository;
         _eventPublisher = eventPublisher;
      }

      public void UpdateStatusForSimulationUsing(IPKSimBuildingBlock buildingBlock)
      {
         SimulationsUsing(buildingBlock).Each(UpdateStatusForSimulation);
      }

      public void UpdateStatusForSimulation(Simulation simulation)
      {
         UpdateBuildingBlockNamesUsedIn(simulation);
         _eventPublisher.PublishEvent(new SimulationStatusChangedEvent(simulation));
      }

      public Simulation SimulationUsing(UsedBuildingBlock usedBuildingBlock)
      {
         return _buildingBlockRepository.All<Simulation>()
            .FirstOrDefault(x => x.UsedBuildingBlockById(usedBuildingBlock.Id) != null);
      }

      public IReadOnlyList<Simulation> SimulationsUsing(IPKSimBuildingBlock buildingBlock)
      {
         return allSimulationUsingBuildingBlockWithId(buildingBlock.Id).ToList();
      }

      public IReadOnlyList<IPKSimBuildingBlock> BuildingBlockUsing(IPKSimBuildingBlock buildingBlock)
      {
         IReadOnlyList<IPKSimBuildingBlock> simulations = SimulationsUsing(buildingBlock);
         switch (buildingBlock)
         {
            case ExpressionProfile expressionProfile:
               return simulations.Union(allSimulationSubjectsUsing(expressionProfile)).ToList();
            default:
               return simulations;
         }
      }

      private IEnumerable<IPKSimBuildingBlock> allSimulationSubjectsUsing(ExpressionProfile expressionProfile)
         => _buildingBlockRepository.All<ISimulationSubject>(x => x.Uses(expressionProfile));

      private IEnumerable<Simulation> allSimulationUsingBuildingBlockWithId(string buildingBlockId)
      {
         return from simulation in _buildingBlockRepository.All<Simulation>()
            where simulation.UsesBuildingBlock(buildingBlockId)
            select simulation;
      }

      public BuildingBlockStatus StatusFor(UsedBuildingBlock usedBuildingBlock)
      {
         if (usedBuildingBlock.Altered)
            return BuildingBlockStatus.Red;

         //template does not exist. This could be a simple import
         var buildingBlock = _buildingBlockRepository.ById(usedBuildingBlock.TemplateId);
         if (buildingBlock == null)
            return BuildingBlockStatus.Red;

         return buildingBlock.Version == usedBuildingBlock.Version ? BuildingBlockStatus.Green : BuildingBlockStatus.Red;
      }

      public BuildingBlockStatus StatusFor(Simulation simulation)
      {
         if (simulation.IsImported)
            return BuildingBlockStatus.Green;

         foreach (var usedBuildingBlock in simulation.UsedBuildingBlocks)
         {
            switch (StatusFor(usedBuildingBlock))
            {
               case BuildingBlockStatus.Green:
                  break;
               default:
                  return BuildingBlockStatus.Red;
            }
         }

         return BuildingBlockStatus.Green;
      }

      public void UpdateBuildingBlockNamesUsedIn(Simulation simulation)
      {
         foreach (var usedBuildingBlock in simulation.UsedBuildingBlocks)
         {
            var templateBuildingBlock = _buildingBlockRepository.ById(usedBuildingBlock.TemplateId);
            if (templateBuildingBlock == null)
               continue;

            var bbStatus = StatusFor(usedBuildingBlock);
            if (bbStatus == BuildingBlockStatus.Green)
               usedBuildingBlock.Name = templateBuildingBlock.Name;
            else
               usedBuildingBlock.Name = CoreConstants.ContainerName.BuildingBlockInSimulationNameFor(templateBuildingBlock.Name, simulation.Name);

            //update name of internal building block (can be null if not loaded)
            if (usedBuildingBlock.BuildingBlock != null)
               usedBuildingBlock.BuildingBlock.Name = templateBuildingBlock.Name;
         }
      }

      public IReadOnlyList<TBuildingBlock> TemplateBuildingBlocksUsedBy<TBuildingBlock>(Simulation simulation) where TBuildingBlock : class, IPKSimBuildingBlock
      {
         return
            simulation.UsedBuildingBlocksInSimulation<TBuildingBlock>()
               .Select(TemplateBuildingBlockUsedBy<TBuildingBlock>).ToList();
      }

      public TBuildingBlock TemplateBuildingBlockUsedBy<TBuildingBlock>(UsedBuildingBlock usedBuildingBlock) where TBuildingBlock : class, IPKSimBuildingBlock
      {
         return TemplateBuildingBlockUsedBy(usedBuildingBlock) as TBuildingBlock;
      }

      public IPKSimBuildingBlock TemplateBuildingBlockUsedBy(UsedBuildingBlock usedBuildingBlock)
      {
         if (usedBuildingBlock == null)
            return null;

         if (StatusFor(usedBuildingBlock) == BuildingBlockStatus.Red)
            return usedBuildingBlock.BuildingBlock;

         return _buildingBlockRepository.ById(usedBuildingBlock.TemplateId);
      }

      public TBuildingBlock TemplateBuildingBlockUsedBy<TBuildingBlock>(Simulation simulation, TBuildingBlock buildingBlockInSimulation) where TBuildingBlock : class, IPKSimBuildingBlock
      {
         return TemplateBuildingBlockUsedBy<TBuildingBlock>(simulation.UsedBuildingBlockBy(buildingBlockInSimulation));
      }
   }
}