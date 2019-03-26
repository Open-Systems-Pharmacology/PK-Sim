using System.Linq;
using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Core.Comparison;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Services
{
   public interface ISimulationTask : IBuildingBlockTask<Simulation>
   {
      /// <summary>
      ///    update the given template building block into the simulation
      /// </summary>
      /// <typeparam name="TBuildingBlock">Type of building block</typeparam>
      /// <param name="templateBuildingBlock">tempalte building block used as source of the update</param>
      /// <param name="usedBuildingBlock">used building block whose value/structure will be updated</param>
      /// <param name="simulation">simulation containing the used building block</param>
      void UpdateUsedBuildingBlockInSimulation<TBuildingBlock>(TBuildingBlock templateBuildingBlock, UsedBuildingBlock usedBuildingBlock, Simulation simulation) where TBuildingBlock : class, IPKSimBuildingBlock;

      /// <summary>
      ///    Save the change made in the used building block belonging in the simulation into the given template building block
      /// </summary>
      /// <typeparam name="TBuildingBlock">Type of building block</typeparam>
      /// <param name="templateBuildingBlock">Tempalte building block that will be updated</param>
      /// <param name="usedBuildingBlock">used building block from which the values will be taken</param>
      /// <param name="simulation">simulation containing the used building block</param>
      void CommitBuildingBlockToRepository<TBuildingBlock>(TBuildingBlock templateBuildingBlock, UsedBuildingBlock usedBuildingBlock, Simulation simulation) where TBuildingBlock : class, IPKSimBuildingBlock;

      /// <summary>
      ///    Shows the difference between the template building block and the building block used in the simulation
      /// </summary>
      /// <typeparam name="TBuildingBlock">Type of building block</typeparam>
      /// <param name="templateBuildingBlock">Tempalte building block </param>
      /// <param name="usedBuildingBlock">used building block</param>
      /// <param name="simulation">containing the used building block</param>
      void ShowDifferencesBetween<TBuildingBlock>(TBuildingBlock templateBuildingBlock, UsedBuildingBlock usedBuildingBlock, Simulation simulation) where TBuildingBlock : class, IPKSimBuildingBlock;

      /// <summary>
      ///    Import external results into simulation (for example from cluster export)
      /// </summary>
      /// <param name="populationSimulation">PopulationSimulation for which results should be imported</param>
      void ImportResultsIn(PopulationSimulation populationSimulation);

      /// <summary>
      ///    Import external PK-Analyses into simulation (for example from matlab user defined scripts)
      /// </summary>
      /// <param name="populationSimulation">PopulationSimulation for which pk-analyses should be imported</param>
      void ImportPKAnalyses(PopulationSimulation populationSimulation);
   }

   public class SimulationTask : BuildingBlockTask<Simulation>, ISimulationTask
   {
      private readonly ISimulationBuildingBlockUpdater _simulationBuildingBlockUpdater;
      private readonly IConfigureSimulationTask _configureSimulationTask;
      private readonly IBuildingBlockParametersToSimulationUpdater _blockParametersToSimulationUpdater;
      private readonly ISimulationParametersToBuildingBlockUpdater _simulationParametersToBlockUpdater;

      public SimulationTask(IExecutionContext executionContext, IBuildingBlockTask buildingBlockTask, IApplicationController applicationController,
         ISimulationBuildingBlockUpdater simulationBuildingBlockUpdater, IConfigureSimulationTask configureSimulationTask,
         IBuildingBlockParametersToSimulationUpdater blockParametersToSimulationUpdater, ISimulationParametersToBuildingBlockUpdater simulationParametersToBlockUpdater)
         : base(executionContext, buildingBlockTask, applicationController, PKSimBuildingBlockType.Simulation)
      {
         _simulationBuildingBlockUpdater = simulationBuildingBlockUpdater;
         _configureSimulationTask = configureSimulationTask;
         _blockParametersToSimulationUpdater = blockParametersToSimulationUpdater;
         _simulationParametersToBlockUpdater = simulationParametersToBlockUpdater;
      }

      public override Simulation AddToProject()
      {
         var simulation = AddToProject<ICreateSimulationPresenter>();
         if (simulation == null)
            return null;
         //after creation => we go in edit mode
         Edit(simulation);

         return simulation;
      }

      public void UpdateUsedBuildingBlockInSimulation<TBuildingBlock>(TBuildingBlock templateBuildingBlock, UsedBuildingBlock usedBuildingBlock, Simulation simulation) where TBuildingBlock : class, IPKSimBuildingBlock
      {
         _buildingBlockTask.Load(templateBuildingBlock);
         _buildingBlockTask.Load(simulation);

         //check if quick update possible. if yes =>performe quick update
         if (_simulationBuildingBlockUpdater.QuickUpdatePossibleFor(templateBuildingBlock, usedBuildingBlock))
         {
            var updateCommand = _blockParametersToSimulationUpdater.UpdateParametersFromBuildingBlockInSimulation(templateBuildingBlock, simulation);
            _buildingBlockTask.AddCommandToHistory(updateCommand);
         }
         else
            //we have to start the configuration workflow
            _configureSimulationTask.Configure(simulation, templateBuildingBlock);
      }

      public void CommitBuildingBlockToRepository<TBuildingBlock>(TBuildingBlock templateBuildingBlock, UsedBuildingBlock usedBuildingBlock, Simulation simulation) where TBuildingBlock : class, IPKSimBuildingBlock
      {
         _buildingBlockTask.Load(templateBuildingBlock);
         _buildingBlockTask.Load(simulation);

         //check if quick update possible. if yes =>perform quick update
         if (_simulationBuildingBlockUpdater.QuickUpdatePossibleFor(templateBuildingBlock, usedBuildingBlock))
         {
            var updateCommand = _simulationParametersToBlockUpdater.UpdateParametersFromSimulationInBuildingBlock(simulation, templateBuildingBlock);
            _buildingBlockTask.AddCommandToHistory(updateCommand);
         }
         else
            throw new PKSimException(PKSimConstants.Error.AdvancedCommitNotAvailable);
      }

      public void ShowDifferencesBetween<TBuildingBlock>(TBuildingBlock templateBuildingBlock, UsedBuildingBlock usedBuildingBlock, Simulation simulation) where TBuildingBlock : class, IPKSimBuildingBlock
      {
         _buildingBlockTask.Load(templateBuildingBlock);
         _buildingBlockTask.Load(simulation);

         if (!_simulationBuildingBlockUpdater.BuildingBlockSupportComparison(templateBuildingBlock))
            throw new PKSimException(PKSimConstants.Error.ComparisonWithTemplateNotSupportedForBuildingBlockOfType(_executionContext.TypeFor(templateBuildingBlock)));

         //Uses the UsedBuildingBlockById since the usedBuildingBlock might have been lazy loaded
         var simulationBuildingBlock = simulation.UsedBuildingBlockById(usedBuildingBlock.Id).BuildingBlock;
         _executionContext.PublishEvent(new StartComparisonEvent(
            leftObject: templateBuildingBlock,
            leftCaption: PKSimConstants.UI.BuildingBlock,
            rightObject: simulationBuildingBlock,
            rightCaption: PKSimConstants.ObjectTypes.Simulation));
      }

      public void ImportResultsIn(PopulationSimulation populationSimulation)
      {
         _buildingBlockTask.Load(populationSimulation);
         using (var presenter = _applicationController.Start<IImportSimulationResultsPresenter>())
         {
            var simulationResults = presenter.ImportResultsFor(populationSimulation);
            if (simulationResults.IsNull())
               return;

            _buildingBlockTask.AddCommandToHistory(new SetPopulationSimulationResultsCommand(populationSimulation, simulationResults).Run(_executionContext));
         }
      }

      public void ImportPKAnalyses(PopulationSimulation populationSimulation)
      {
         _buildingBlockTask.Load(populationSimulation);
         using (var presenter = _applicationController.Start<IImportSimulationPKAnalysesPresenter>())
         {
            var pkAnalyses = presenter.ImportPKAnalyses(populationSimulation);
            if (!pkAnalyses.Any())
               return;

            _buildingBlockTask.AddCommandToHistory(new AddPKAnalysesToSimulationCommand(populationSimulation, pkAnalyses, presenter.FileName).Run(_executionContext));
         }
      }
   }
}