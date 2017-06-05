using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Services;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationResultsTask : ContextSpecification<ISimulationResultsTask>
   {
      private IChartTemplatingTask _chartTemplatingTask;
      protected IRenameBuildingBlockTask _renameBuildingBlockTask;
      protected ISimulationResultsSynchronizer _simulationResultsSynchronizer;
      protected ICloner _cloner;
      protected ISimulationResultsCreator _simulationResultsCreator;
      protected IDataRepositoryFromResultsCreator _dataRepositoryCreator;
      protected ICurveNamer _curveNamer;

      protected override void Context()
      {
         _chartTemplatingTask = A.Fake<IChartTemplatingTask>();
         _renameBuildingBlockTask = A.Fake<IRenameBuildingBlockTask>();
         _simulationResultsSynchronizer = A.Fake<ISimulationResultsSynchronizer>();
         _simulationResultsCreator = A.Fake<ISimulationResultsCreator>();
         _dataRepositoryCreator = A.Fake<IDataRepositoryFromResultsCreator>();
         _cloner = A.Fake<ICloner>();
         _curveNamer = A.Fake<ICurveNamer>();
         sut = new SimulationResultsTask(_chartTemplatingTask, _simulationResultsSynchronizer, _cloner, _simulationResultsCreator, _dataRepositoryCreator, _curveNamer);
      }
   }

   public class When_cloning_the_results_between_two_simulations_having_the_same_type : concern_for_SimulationResultsTask
   {
      private IndividualSimulation _simulationToClone;
      private IndividualSimulation _clonedSimulation;
      private DataRepository _clonedDataRepository;
      private SimulationResults _newResults;

      protected override void Context()
      {
         base.Context();
         _simulationToClone = A.Fake<IndividualSimulation>().WithName("OLD");
         _clonedSimulation = A.Fake<IndividualSimulation>().WithName("NEW");
         var dataRepository = A.Fake<DataRepository>();
         _newResults = new SimulationResults {Id = 1};
         _clonedDataRepository = new DataRepository("Clone");
         A.CallTo(() => _cloner.Clone(dataRepository)).Returns(_clonedDataRepository);
         _simulationToClone.DataRepository = dataRepository;
         _simulationToClone.ResultsVersion = 25;
         A.CallTo(() => _simulationResultsCreator.CreateResultsFrom(dataRepository)).Returns(new SimulationResults {Id = 2});
         A.CallTo(() => _simulationResultsCreator.CreateResultsFrom(_clonedDataRepository)).Returns(_newResults);
         A.CallTo(() => _dataRepositoryCreator.CreateResultsFor(_clonedSimulation)).Returns(_clonedDataRepository);
      }

      protected override void Because()
      {
         sut.CloneResults(_simulationToClone, _clonedSimulation);
      }

      [Observation]
      public void should_update_the_simulation_results_based_on_the_cloned_data_repository()
      {
         //use id otherwise collection will be compared
         _clonedSimulation.Results.Id.ShouldBeEqualTo(_newResults.Id);
      }

      [Observation]
      public void should_have_clone_the_results_from_the_source_simulation()
      {
         //use id otherwise collection will be compared
         _clonedSimulation.DataRepository.Id.ShouldBeEqualTo(_clonedDataRepository.Id);
      }

      [Observation]
      public void should_have_set_the_result_version_equal_to_the_previous_version()
      {
         _clonedSimulation.ResultsVersion.ShouldBeEqualTo(_simulationToClone.ResultsVersion);
      }
   }

   public class When_cloning_simulation_results_with_charts : concern_for_SimulationResultsTask
   {
      private IndividualSimulation _clonedSimulation;
      private IndividualSimulation _simulationToClone;
      private SimulationTimeProfileChart _clonedSimulationAnalysis;
      private SimulationTimeProfileChart _simulationAnalysisToClone;
      private Curve _clonedCurve;

      protected override void Context()
      {
         base.Context();
         _clonedSimulation = new IndividualSimulation();
         _simulationToClone = new IndividualSimulation {DataRepository = new DataRepository()};
         _clonedSimulationAnalysis = new SimulationTimeProfileChart();
         _simulationAnalysisToClone = new SimulationTimeProfileChart();

         _clonedSimulation.AddAnalysis(_clonedSimulationAnalysis);
         _simulationToClone.AddAnalysis(_simulationAnalysisToClone);

         _clonedCurve = new Curve();
         var dimensionFactory = A.Fake<IDimensionFactory>();
         _clonedCurve.SetyData(new DataColumn(), dimensionFactory);
         _clonedCurve.SetxData(new DataColumn(), dimensionFactory);
         _clonedSimulationAnalysis.AddCurve(_clonedCurve);
         var originalCurve = new Curve();
         originalCurve.SetyData(new DataColumn(), dimensionFactory);
         originalCurve.SetxData(new DataColumn(), dimensionFactory);
         _simulationAnalysisToClone.AddCurve(originalCurve);
         A.CallTo(() => _curveNamer.CurvesWithOriginalName(_simulationToClone, A<IEnumerable<ICurveChart>>._)).Returns(new[] {originalCurve});
      }

      protected override void Because()
      {
         sut.CloneResults(_simulationToClone, _clonedSimulation);
      }

      [Observation]
      public void the_curve_namer_should_rename_the_cloned_curve()
      {
         A.CallTo(() => _curveNamer.CurveNameForColumn(_clonedSimulation, _clonedCurve.yData)).MustHaveHappened();
      }
   }

   public class When_copying_simulation_results_from_one_simulation_to_the_other : concern_for_SimulationResultsTask
   {
      private ISimulationAnalysis _chart;
      private IndividualSimulation _simulationToConfigure;
      private IndividualSimulation _newSimulation;
      private DataRepository _oldResults;
      private const int _resultVersion = 15;

      protected override void Context()
      {
         base.Context();
         _chart = A.Fake<ISimulationAnalysis>();
         _simulationToConfigure = new IndividualSimulation().WithId("oldSim");
         _newSimulation = new IndividualSimulation().WithId("newSim");
         _simulationToConfigure.AddUsedBuildingBlock(new UsedBuildingBlock("C1", PKSimBuildingBlockType.Compound) {BuildingBlock = new Compound().WithName("C1")});
         _newSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("C2", PKSimBuildingBlockType.Compound) {BuildingBlock = new Compound().WithName("C2")});
         _oldResults = new DataRepository("oldResult");
         _simulationToConfigure.DataRepository = _oldResults;
         _simulationToConfigure.ResultsVersion = _resultVersion;
         _simulationToConfigure.AddAnalysis(_chart);
      }

      protected override void Because()
      {
         sut.CopyResults(_simulationToConfigure, _newSimulation);
      }

      [Observation]
      public void should_have_set_the_result_version_equal_to_the_previous_version()
      {
         _newSimulation.ResultsVersion.ShouldBeEqualTo(_resultVersion);
      }

      [Observation]
      public void should_have_added_the_charts_to_the_simulation()
      {
         _newSimulation.Analyses.ShouldContain(_chart);
      }
   }

   public class When_cloning_the_results_from_one_pop_simulation_to_another : concern_for_SimulationResultsTask
   {
      private PopulationSimulation _clonedDataCollector;
      private PopulationSimulation _dataCollectorToClone;

      protected override void Context()
      {
         base.Context();
         _dataCollectorToClone = A.Fake<PopulationSimulation>().WithName("OLD");
         A.CallTo(() => _dataCollectorToClone.CompoundNames).Returns(new[] {"C1"});
         _clonedDataCollector = A.Fake<PopulationSimulation>().WithName("NEW");
         A.CallTo(() => _clonedDataCollector.CompoundNames).Returns(new[] {"C2"});
         var results = A.Fake<SimulationResults>();
         _dataCollectorToClone.Results = results;
         _dataCollectorToClone.ResultsVersion = 25;
      }

      protected override void Because()
      {
         sut.CloneResults(_dataCollectorToClone, _clonedDataCollector);
      }

      [Observation]
      public void should_synchronize_the_result_in_the_new_simulation()
      {
         A.CallTo(() => _simulationResultsSynchronizer.Synchronize(_clonedDataCollector, _clonedDataCollector.Results)).MustHaveHappened();
      }
   }
}