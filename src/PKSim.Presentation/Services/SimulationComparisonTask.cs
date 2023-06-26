using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Chart;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using ISimulationAnalysisCreator = PKSim.Core.Services.ISimulationAnalysisCreator;

namespace PKSim.Presentation.Services
{
   public class SimulationComparisonTask : ISimulationComparisonTask
   {
      private readonly IPKSimChartFactory _chartFactory;
      private readonly IContainerTask _containerTask;
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IApplicationController _applicationController;
      private readonly ISingleStartPresenterTask _singleStartPresenterTask;
      private readonly IExecutionContext _executionContext;
      private readonly ISimulationAnalysisCreator _simulationAnalysisCreator;
      private readonly IDialogCreator _dialogCreator;
      private readonly SimulationComparisonMapper _simulationComparisonMapper;

      public SimulationComparisonTask(
         IPKSimChartFactory chartFactory,
         IContainerTask containerTask,
         IObjectBaseFactory objectBaseFactory,
         IApplicationController applicationController,
         ISingleStartPresenterTask singleStartPresenterTask,
         IExecutionContext executionContext,
         ISimulationAnalysisCreator simulationAnalysisCreator,
         IDialogCreator dialogCreator,
         SimulationComparisonMapper simulationComparisonMapper
      )
      {
         _chartFactory = chartFactory;
         _containerTask = containerTask;
         _objectBaseFactory = objectBaseFactory;
         _applicationController = applicationController;
         _singleStartPresenterTask = singleStartPresenterTask;
         _executionContext = executionContext;
         _simulationAnalysisCreator = simulationAnalysisCreator;
         _dialogCreator = dialogCreator;
         _simulationComparisonMapper = simulationComparisonMapper;
      }

      public ISimulationComparison CreateIndividualSimulationComparison(IndividualSimulation individualSimulation = null)
      {
         var simulationComparison = _chartFactory.CreateIndividualSimulationComparison();
         addComparisonToProject(simulationComparison);
         if (individualSimulation != null && individualSimulation.HasResults)
            simulationComparison.AddSimulation(individualSimulation);

         return simulationComparison;
      }

      private void addComparisonToProject(ISimulationComparison simulationComparison)
      {
         simulationComparison.Name = createUniqueComparisonName();
         _executionContext.CurrentProject.AddSimulationComparison(simulationComparison);
         simulationComparison.IsLoaded = true;
         _executionContext.PublishEvent(new SimulationComparisonCreatedEvent(_executionContext.CurrentProject, simulationComparison));
      }

      private string createUniqueComparisonName()
      {
         return _containerTask.CreateUniqueName(_executionContext.CurrentProject.AllSimulationComparisons, PKSimConstants.UI.SimulationComparison);
      }

      public ISimulationComparison CreatePopulationSimulationComparison()
      {
         using (var presenter = _applicationController.Start<ISimulationSelectionForComparisonPresenter>())
         {
            var comparison = _objectBaseFactory.Create<PopulationSimulationComparison>();
            if (!presenter.Edit(comparison))
               return null;

            addComparisonToProject(comparison);

            //create a default analysis
            _simulationAnalysisCreator.CreatePopulationAnalysisFor(comparison);

            return comparison;
         }
      }

      public void ConfigurePopulationSimulationComparison(PopulationSimulationComparison simulationComparison)
      {
         _executionContext.Load(simulationComparison);
         using (var presenter = _applicationController.Start<ISimulationSelectionForComparisonPresenter>())
         {
            if (!presenter.Edit(simulationComparison))
               return;

            _executionContext.ProjectChanged();
            var presenterWasOpen = _applicationController.HasPresenterOpenedFor(simulationComparison);

            //presenter was not open, nothing to do
            if (!presenterWasOpen)
               return;

            _applicationController.Close(simulationComparison);
            _singleStartPresenterTask.StartForSubject(simulationComparison);
         }
      }

      public bool Delete(IReadOnlyList<ISimulationComparison> simulationComparisons)
      {
         if (!simulationComparisons.Any())
            return true;

         var res = _dialogCreator.MessageBoxYesNo(PKSimConstants.UI.ReallyDeleteSimulationComparisons(simulationComparisons.AllNames()));
         if (res == ViewResult.No)
            return false;

         simulationComparisons.Each(deleteSimulationComparison);

         return true;
      }

      public async Task<ISimulationComparison> CloneSimulationComparision(ISimulationComparison simulationComparison)
      {
         _executionContext.Load(simulationComparison);
         //We clone from snapshot as cloning charts is too complicated. This ensures that we are using
         //a fully tested approach to cloning
         var snapshotContext = new SnapshotContext(_executionContext.CurrentProject, ProjectVersions.Current);
         var snapshot = await _simulationComparisonMapper.MapToSnapshot(simulationComparison);
         var clone = await _simulationComparisonMapper.MapToModel(snapshot, snapshotContext);
         //this will ensure that the names are uniques
         addComparisonToProject(clone);
         return clone;
      }

      private void deleteSimulationComparison(ISimulationComparison simulationComparison)
      {
         _applicationController.Close(simulationComparison);
         _executionContext.CurrentProject.RemoveSimulationComparison(simulationComparison);
         _executionContext.Unregister(simulationComparison);
         _executionContext.PublishEvent(new SimulationComparisonDeletedEvent(_executionContext.CurrentProject, simulationComparison));
      }
   }
}