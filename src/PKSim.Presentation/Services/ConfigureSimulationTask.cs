using System;
using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Services
{
   public interface IConfigureSimulationTask
   {
      /// <summary>
      ///    Starts the configure simulation workflow
      /// </summary>
      /// <param name="simulationToConfigure"></param>
      void Configure(Simulation simulationToConfigure);

      /// <summary>
      ///    Stats the configure and select the given building block
      /// </summary>
      void Configure(Simulation simulationToConfigure, IPKSimBuildingBlock buildingBlockToActivate);
   }

   public class ConfigureSimulationTask : IConfigureSimulationTask
   {
      private readonly IBuildingBlockTask _buildingBlockTask;
      private readonly IActiveSubjectRetriever _activeSubjectRetriever;
      private readonly ISimulationSettingsRetriever _simulationSettingsRetriever;
      private readonly IApplicationController _applicationController;
      private readonly IExecutionContext _executionContext;
      private readonly IParameterIdUpdater _parameterIdUpdater;
      private readonly ISimulationResultsTask _simulationResultsTask;

      public ConfigureSimulationTask(IBuildingBlockTask buildingBlockTask, IActiveSubjectRetriever activeSubjectRetriever,
         ISimulationSettingsRetriever simulationSettingsRetriever, IApplicationController applicationController, IExecutionContext executionContext,
         IParameterIdUpdater parameterIdUpdater, ISimulationResultsTask simulationResultsTask)
      {
         _buildingBlockTask = buildingBlockTask;
         _activeSubjectRetriever = activeSubjectRetriever;
         _simulationSettingsRetriever = simulationSettingsRetriever;
         _applicationController = applicationController;
         _executionContext = executionContext;
         _parameterIdUpdater = parameterIdUpdater;
         _simulationResultsTask = simulationResultsTask;
      }

      public void Configure(Simulation simulationToConfigure)
      {
         configure(simulationToConfigure, x => x.ConfigureSimulation(simulationToConfigure));
      }

      public void Configure(Simulation simulationToConfigure, IPKSimBuildingBlock buildingBlockToActivate)
      {
         configure(simulationToConfigure, x => x.ConfigureSimulationWithBuildingBlock(simulationToConfigure, buildingBlockToActivate));
      }

      private void configure(Simulation simulationToConfigure, Func<IConfigureSimulationPresenter, IPKSimCommand> configureAction)
      {
         _buildingBlockTask.Load(simulationToConfigure);
         using (var presenter = _applicationController.Start<IConfigureSimulationPresenter>())
         {
            var activeSubject = _activeSubjectRetriever.Active<IPKSimBuildingBlock>();
            var configureCommand = configureAction(presenter);

            //User cancel action. return
            if (configureCommand.IsEmpty())
               return;

            //Before swapping simulation=>update results and charts information from the original simulation
            var simulation = presenter.Simulation;
            _simulationResultsTask.CopyResults(simulationToConfigure, simulation);
            _simulationSettingsRetriever.SynchronizeSettingsIn(simulation);

            //We have the same simulation=> we should update the id
            simulation.Id = simulationToConfigure.Id;
            _parameterIdUpdater.UpdateSimulationId(simulation);

            //was the presenter open for the simulation? if yes 
            var presenterWasOpen = _applicationController.HasPresenterOpenedFor(simulationToConfigure);
            simulation.Creation.CreationMode = CreationMode.Configure;
            var swapSimulationCommand = new SwapSimulationCommand(simulationToConfigure, simulation, _executionContext).Run(_executionContext);

            swapSimulationCommand.ReplaceNameTemplateWithName(simulation.Name);
            swapSimulationCommand.ReplaceTypeTemplateWithType(PKSimConstants.ObjectTypes.Simulation);

            _buildingBlockTask.AddCommandToHistory(swapSimulationCommand);

            //no active presenter: nothing to do
            if (activeSubject == null)
               return;

            //presenter was not open, nothing to do
            if (!presenterWasOpen)
               return;

            //edit the simulation back, since it was edited
            _buildingBlockTask.Edit(simulation);

            //now, was the simulation the active presenter? if no, we need to active the other presenter
            if (activeSubject != simulationToConfigure)
               _buildingBlockTask.Edit(activeSubject);
         }
      }
   }
}