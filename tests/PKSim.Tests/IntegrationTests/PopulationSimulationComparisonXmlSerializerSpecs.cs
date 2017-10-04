using System.Drawing;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using PKSim.Core.Chart;
using PKSim.Core.Model;
using PKSim.Core.Model.PopulationAnalyses;
using OSPSuite.Core.Chart;
using OSPSuite.Core.Domain;
using DistributionSettings = PKSim.Core.Chart.DistributionSettings;

namespace PKSim.IntegrationTests
{
   public class When_serializing_population_simulation_comparison_with_a_reference_simulation : ContextForSerialization<PopulationSimulationComparison>
   {
      private IWithIdRepository _objectBaseRepository;
      private PopulationSimulation _popSim1;
      private PopulationSimulation _popSim2;
      private PopulationSimulationComparison _populationSimulationComparison;
      private PopulationSimulationComparison _deserialized;
      private GroupingItem _groupingItem;

      protected override void Context()
      {
         base.Context();
         _popSim1 = new PopulationSimulation {IsLoaded = true}.WithId("PopSim1");
         _popSim2 = new PopulationSimulation {IsLoaded = true}.WithId("PopSim2");
         _objectBaseRepository = IoC.Resolve<IWithIdRepository>();
         _objectBaseRepository.Register(_popSim1);
         _objectBaseRepository.Register(_popSim2);
         _populationSimulationComparison = new PopulationSimulationComparison();
         _populationSimulationComparison.AddSimulation(_popSim1);
         _populationSimulationComparison.AddSimulation(_popSim2);
         _populationSimulationComparison.ReferenceSimulation = _popSim1;
         _groupingItem = new GroupingItem {Color = Color.Black, Label = "Reference", Symbol = Symbols.Circle};
         _populationSimulationComparison.ReferenceGroupingItem = _groupingItem;
         _populationSimulationComparison.SelectedDistributions.Add(new ParameterDistributionSettings{ParameterPath = "P1", Settings = new DistributionSettings{AxisCountMode = AxisCountMode.Percent}});
      }

      protected override void Because()
      {
         _deserialized = SerializeAndDeserialize(_populationSimulationComparison);
      }

      [Observation]
      public void should_be_able_to_deserialize_the_comparison_and_retrieve_the_simulations_used_in_the_comparison()
      {
         _deserialized.AllSimulations.ShouldOnlyContain(_popSim1, _popSim2);
         _deserialized.ReferenceGroupingItem.Color.ShouldBeEqualTo(_groupingItem.Color);
         _deserialized.ReferenceGroupingItem.Label.ShouldBeEqualTo(_groupingItem.Label);
         _deserialized.ReferenceGroupingItem.Symbol.ShouldBeEqualTo(_groupingItem.Symbol);
      }

      [Observation]
      public void should_be_able_to_retrieve_the_referenced_simulatio()
      {
         _deserialized.ReferenceSimulation.ShouldBeEqualTo(_popSim1);
      }

      [Observation]
      public void should_be_able_to_deserialize_the_comparison_and_retrieve_the_reference_grouping_item_settings()
      {
         _deserialized.ReferenceGroupingItem.Color.ShouldBeEqualTo(_groupingItem.Color);
         _deserialized.ReferenceGroupingItem.Label.ShouldBeEqualTo(_groupingItem.Label);
         _deserialized.ReferenceGroupingItem.Symbol.ShouldBeEqualTo(_groupingItem.Symbol);
      }

      [Observation]
      public void should_be_able_to_deserialize_the_selected_distributions_and_retrieve_the_reference_grouping_item_settings()
      {
         _deserialized.SelectedDistributions.Count.ShouldBeEqualTo(1);
         _deserialized.SelectedDistributions.First().ParameterPath.ShouldBeEqualTo("P1");
      }

      public override void Cleanup()
      {
         base.Cleanup();
         _objectBaseRepository.Unregister(_popSim1.Id);
         _objectBaseRepository.Unregister(_popSim2.Id);
      }
   }
}