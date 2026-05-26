using System.Drawing;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Core.Services;

namespace PKSim.Presentation
{
   public abstract class concern_for_ObservedDataInComparisonTask : ContextSpecification<IObservedDataInComparisonTask>
   {
      protected IExecutionContext _executionContext;
      protected PKSimProject _project;

      protected override void Context()
      {
         _executionContext = A.Fake<IExecutionContext>();
         _project = new PKSimProject();
         A.CallTo(() => _executionContext.CurrentProject).Returns(_project);

         sut = new ObservedDataInComparisonTask(_executionContext);
      }
   }

   public class When_adding_observed_data_to_a_time_profile_analysis : concern_for_ObservedDataInComparisonTask
   {
      private PopulationSimulationComparison _comparison;
      private PopulationSimulation _populationSimulation;
      private DataRepository _observedData;
      private TimeProfileAnalysisChart _targetChart;

      protected override void Context()
      {
         base.Context();
         _observedData = DomainHelperForSpecs.ObservedData("obs");
         _project.AddObservedData(_observedData);

         _populationSimulation = new PopulationSimulation().WithName("popSim");
         _populationSimulation.AddUsedObservedData(_observedData);

         _comparison = new PopulationSimulationComparison();
         _comparison.AddSimulation(_populationSimulation);

         _targetChart = new TimeProfileAnalysisChart();
      }

      protected override void Because()
      {
         sut.AddObservedDataToTimeProfile(_comparison, _targetChart);
      }

      [Observation]
      public void should_register_the_observed_data_referenced_by_the_compared_simulations()
      {
         _targetChart.UsesObservedData(_observedData).ShouldBeTrue();
      }
   }

   public class When_adding_observed_data_with_existing_source_curve_options : concern_for_ObservedDataInComparisonTask
   {
      private PopulationSimulationComparison _comparison;
      private PopulationSimulation _populationSimulation;
      private DataRepository _observedData;
      private TimeProfileAnalysisChart _targetChart;
      private CurveOptions _sourceCurveOptions;

      protected override void Context()
      {
         base.Context();
         _observedData = DomainHelperForSpecs.ObservedData("obs");
         _project.AddObservedData(_observedData);

         _populationSimulation = new PopulationSimulation().WithName("popSim");
         _populationSimulation.AddUsedObservedData(_observedData);

         //source: a time profile analysis on the simulation that already plots the observed
         //data with non-default styling.
         var sourceAnalysis = new TimeProfileAnalysisChart();
         sourceAnalysis.AddObservedData(_observedData);
         _sourceCurveOptions = sourceAnalysis.CurveOptionsFor(_observedData.ObservationColumns().First());
         _sourceCurveOptions.Color = Color.Red;
         _sourceCurveOptions.LineStyle = LineStyles.Dash;
         _populationSimulation.AddAnalysis(sourceAnalysis);

         _comparison = new PopulationSimulationComparison();
         _comparison.AddSimulation(_populationSimulation);

         _targetChart = new TimeProfileAnalysisChart();
      }

      protected override void Because()
      {
         sut.AddObservedDataToTimeProfile(_comparison, _targetChart);
      }

      [Observation]
      public void should_template_the_curve_options_from_the_source_time_profile_analysis()
      {
         var curveOptions = _targetChart.CurveOptionsFor(_observedData.ObservationColumns().First());
         curveOptions.Color.ShouldBeEqualTo(Color.Red);
         curveOptions.LineStyle.ShouldBeEqualTo(LineStyles.Dash);
      }
   }

   public class When_applying_source_curve_options_to_an_individual_comparison : concern_for_ObservedDataInComparisonTask
   {
      private IndividualSimulation _simulation;
      private DataRepository _observedData;
      private IndividualSimulationComparison _targetChart;
      private IDimensionFactory _dimensionFactory;

      protected override void Context()
      {
         base.Context();
         _observedData = DomainHelperForSpecs.ObservedData("obs");
         _project.AddObservedData(_observedData);

         _dimensionFactory = A.Fake<IDimensionFactory>();

         _simulation = new IndividualSimulation
         {
            DataRepository = DomainHelperForSpecs.IndividualSimulationDataRepositoryFor("sim")
         };
         _simulation.AddUsedObservedData(_observedData);

         //source: a time profile chart on the simulation that already plots the observed data
         //with non-default styling.
         var observation = _observedData.ObservationColumns().First();
         var sourceChart = new SimulationTimeProfileChart();
         var sourceCurve = new Curve();
         sourceCurve.SetxData(_observedData.BaseGrid, _dimensionFactory);
         sourceCurve.SetyData(observation, _dimensionFactory);
         //useAxisDefault: false so the axis default doesn't overwrite the styling we set below.
         sourceChart.AddCurve(sourceCurve, useAxisDefault: false);
         sourceCurve.Color = Color.Red;
         sourceCurve.LineStyle = LineStyles.Dash;
         _simulation.AddAnalysis(sourceChart);

         //target: the comparison chart with a curve already in place for the observation column.
         _targetChart = new IndividualSimulationComparison();
         var targetCurve = new Curve();
         targetCurve.SetxData(_observedData.BaseGrid, _dimensionFactory);
         targetCurve.SetyData(observation, _dimensionFactory);
         _targetChart.AddCurve(targetCurve, useAxisDefault: false);
      }

      protected override void Because()
      {
         sut.ApplySourceCurveOptionsTo(_targetChart, _simulation);
      }

      [Observation]
      public void should_apply_the_source_curve_options_to_the_observation_curve()
      {
         var observation = _observedData.ObservationColumns().First();
         var curve = _targetChart.FindCurveWithSameData(observation.BaseGrid, observation);
         curve.Color.ShouldBeEqualTo(Color.Red);
         curve.LineStyle.ShouldBeEqualTo(LineStyles.Dash);
      }
   }
}