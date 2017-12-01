using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Mappers;
using PKSim.Extensions;
using PopulationAnalysisChart = PKSim.Core.Model.PopulationAnalyses.PopulationAnalysisChart;

namespace PKSim.Core
{
   public abstract class concern_for_SimulationComparisonMapper : ContextSpecificationAsync<SimulationComparisonMapper>
   {
      protected IndividualSimulationComparisonMapper _individualSimulationComparisonMapper;
      protected PopulationAnalysisChartMapper _populationAnalysisChartMapper;
      protected IndividualSimulationComparison _individualSimulationComparison;
      protected SimulationComparison _snapshot;
      protected IndividualSimulation _individualSimulation1;
      protected IndividualSimulation _individualSimulation2;
      protected PopulationSimulation _populationSimulation1;
      protected PopulationSimulation _populationSimulation2;
      protected PopulationSimulationComparison _populationSimulationComparison;
      protected PopulationAnalysisChart _populationSimulationAnalysis;
      protected CurveChart _curveChart;
      protected Snapshots.PopulationAnalysisChart _populationAnalysisChartSnapshot;
      protected PopulationSimulation _referenceSimulation;
      protected IObjectBaseFactory _objectBaseFactory;
      protected PKSimProject _project;

      protected override Task Context()
      {
         _individualSimulationComparisonMapper = A.Fake<IndividualSimulationComparisonMapper>();
         _populationAnalysisChartMapper = A.Fake<PopulationAnalysisChartMapper>();
         _objectBaseFactory = A.Fake<IObjectBaseFactory>();
         sut = new SimulationComparisonMapper(_individualSimulationComparisonMapper, _populationAnalysisChartMapper, _objectBaseFactory);

         _individualSimulation1 = new IndividualSimulation().WithName("IndS1").WithId("IndS1");
         _individualSimulation2 = new IndividualSimulation().WithName("IndS2").WithId("IndS2");

         _individualSimulationComparison = new IndividualSimulationComparison
         {
            Name = "IndividualComparison",
            Description = "IndividualComparison Description",
         };

         _individualSimulationComparison.AddSimulation(_individualSimulation1);
         _individualSimulationComparison.AddSimulation(_individualSimulation2);


         _populationSimulation1 = new PopulationSimulation().WithName("PopS1").WithId("PopS1");
         _populationSimulation2 = new PopulationSimulation().WithName("PopS2").WithId("PopS2");
         _referenceSimulation = new PopulationSimulation().WithName("PopS3").WithId("PopS3");

         _populationSimulationComparison = new PopulationSimulationComparison
         {
            Name = "PopulationComparison",
            Description = "PopulationComparison Description",
         };

         _populationSimulationAnalysis = new BoxWhiskerAnalysisChart();
         _populationSimulationComparison.AddSimulation(_populationSimulation1);
         _populationSimulationComparison.AddSimulation(_populationSimulation2);
         _populationSimulationComparison.AddAnalysis(_populationSimulationAnalysis);

         _populationSimulationComparison.ReferenceGroupingItem = new GroupingItem();
         _populationSimulationComparison.ReferenceSimulation = _referenceSimulation;

         _curveChart = new CurveChart();
         A.CallTo(() => _individualSimulationComparisonMapper.MapToSnapshot(_individualSimulationComparison)).Returns(_curveChart);

         _populationAnalysisChartSnapshot = new Snapshots.PopulationAnalysisChart();
         A.CallTo(() => _populationAnalysisChartMapper.MapToSnapshot(_populationSimulationAnalysis)).Returns(_populationAnalysisChartSnapshot);


         _project = new PKSimProject();
         _project.AddBuildingBlock(_individualSimulation1);
         _project.AddBuildingBlock(_individualSimulation2);
         _project.AddBuildingBlock(_populationSimulation1);
         _project.AddBuildingBlock(_populationSimulation2);
         _project.AddBuildingBlock(_referenceSimulation);

         return _completed;
      }
   }

   public class When_mapping_an_individual_simulation_comparison_to_snapshot : concern_for_SimulationComparisonMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_individualSimulationComparison);
      }

      [Observation]
      public void should_return_a_snapshot_with_the_expected_comparison_properties()
      {
         _snapshot.Name.ShouldBeEqualTo(_individualSimulationComparison.Name);
         _snapshot.Description.ShouldBeEqualTo(_individualSimulationComparison.Description);
      }

      [Observation]
      public void should_map_the_simulation_names_used_in_the_comparison()
      {
         _snapshot.Simulations.ShouldContain(_individualSimulation1.Name, _individualSimulation2.Name);
      }

      [Observation]
      public void should_map_the_comparison_to_chart()
      {
         _snapshot.IndividualComparison.ShouldBeEqualTo(_curveChart);
      }

      [Observation]
      public void should_set_irrelevant_properties_to_null()
      {
         _snapshot.PopulationComparisons.ShouldBeNull();
         _snapshot.ReferenceGroupingItem.ShouldBeNull();
         _snapshot.ReferenceSimulation.ShouldBeNull();
      }
   }

   public class When_mapping_an_individual_simulation_comparison_snapshot_to_simulation_comparison : concern_for_SimulationComparisonMapper
   {
      private ISimulationComparison _simulationComparison;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_individualSimulationComparison);
         var individualSimulationComparison = new IndividualSimulationComparison();
         A.CallTo(() => _individualSimulationComparisonMapper.MapToModel(_snapshot.IndividualComparison, A<SimulationAnalysisContext>._)).Returns(individualSimulationComparison);
      }

      protected override async Task Because()
      {
         _simulationComparison = await sut.MapToModel(_snapshot, _project);
      }

      [Observation]
      public void should_return_an_individual_simulation_comparison_comparing_the_expected_simulations()
      {
         _simulationComparison.ShouldBeAnInstanceOf<IndividualSimulationComparison>();
         _simulationComparison.AllBaseSimulations.ShouldOnlyContain(_individualSimulation1, _individualSimulation2);
      }
   }

   public class When_mapping_a_population_simulation_comparison_to_snapshot : concern_for_SimulationComparisonMapper
   {
      protected override async Task Because()
      {
         _snapshot = await sut.MapToSnapshot(_populationSimulationComparison);
      }

      [Observation]
      public void should_return_a_snapshot_with_the_expected_comparison_properties()
      {
         _snapshot.Name.ShouldBeEqualTo(_populationSimulationComparison.Name);
         _snapshot.Description.ShouldBeEqualTo(_populationSimulationComparison.Description);
         _snapshot.ReferenceGroupingItem.ShouldBeEqualTo(_populationSimulationComparison.ReferenceGroupingItem);
      }

      [Observation]
      public void should_map_the_simulation_names_used_in_the_comparison()
      {
         _snapshot.Simulations.ShouldContain(_populationSimulation1.Name, _populationSimulation2.Name);
      }

      [Observation]
      public void should_map_the_name_of_the_reference_simulation_if_defined()
      {
         _snapshot.ReferenceSimulation.ShouldBeEqualTo(_referenceSimulation.Name);
      }

      [Observation]
      public void should_map_the_comparison_to_chart()
      {
         _snapshot.PopulationComparisons.ShouldOnlyContain(_populationAnalysisChartSnapshot);
      }

      [Observation]
      public void should_set_irrelevant_properties_to_null()
      {
         _snapshot.IndividualComparison.ShouldBeNull();
      }
   }

   public class When_mapping_a_population_simulation_comparison_snapshot_to_simulation_comparison : concern_for_SimulationComparisonMapper
   {
      private PopulationSimulationComparison _simulationComparison;

      protected override async Task Context()
      {
         await base.Context();
         _snapshot = await sut.MapToSnapshot(_populationSimulationComparison);
         var populationSimulationComparison = new PopulationSimulationComparison();
         A.CallTo(() => _objectBaseFactory.Create<PopulationSimulationComparison>()).Returns(populationSimulationComparison);

         A.CallTo(() => _populationAnalysisChartMapper.MapToModel(_populationAnalysisChartSnapshot, A<SimulationAnalysisContext>._)).Returns(_populationSimulationAnalysis );
      }

      protected override async Task Because()
      {
         _simulationComparison = await sut.MapToModel(_snapshot, _project) as PopulationSimulationComparison;
      }

      [Observation]
      public void should_return_an_population_simulation_comparison_comparing_the_expected_simulations()
      {
         _simulationComparison.ShouldNotBeNull();
         _simulationComparison.AllBaseSimulations.ShouldOnlyContain(_populationSimulation1, _populationSimulation2);
         _simulationComparison.ReferenceGroupingItem.ShouldBeEqualTo(_snapshot.ReferenceGroupingItem);
         _simulationComparison.ReferenceSimulation.ShouldBeEqualTo(_referenceSimulation);
      }

      [Observation]
      public void should_add_the_population_analyses_from_snapshot()
      {
         _simulationComparison.Analyses.ShouldContain(_populationSimulationAnalysis);
      }
   }
}