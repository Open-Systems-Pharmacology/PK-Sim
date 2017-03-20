using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Chart;
using PKSim.Core.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.PopulationAnalyses;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Services;
using ISimulationAnalysisCreator = PKSim.Core.Services.ISimulationAnalysisCreator;

namespace PKSim.Presentation.Services
{
   public class SimulationComparisonCreator : ISimulationComparisonCreator
   {
      private readonly IPKSimChartFactory _chartFactory;
      private readonly IContainerTask _containerTask;
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IApplicationController _applicationController;
      private readonly ISingleStartPresenterTask _singleStartPresenterTask;
      private readonly IExecutionContext _executionContext;
      private readonly ISimulationAnalysisCreator _simulationAnalysisCreator;

      public SimulationComparisonCreator(IPKSimChartFactory chartFactory, IContainerTask containerTask,
         IObjectBaseFactory objectBaseFactory, IApplicationController applicationController,
         ISingleStartPresenterTask singleStartPresenterTask, IExecutionContext executionContext,ISimulationAnalysisCreator simulationAnalysisCreator)
      {
         _chartFactory = chartFactory;
         _containerTask = containerTask;
         _objectBaseFactory = objectBaseFactory;
         _applicationController = applicationController;
         _singleStartPresenterTask = singleStartPresenterTask;
         _executionContext = executionContext;
         _simulationAnalysisCreator = simulationAnalysisCreator;
      }

      public ISimulationComparison CreateIndividualSimulationComparison()
      {
         var chart = _chartFactory.CreateSummaryChart();
         addComparisonToProject(chart);
         return chart;
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
   }
}