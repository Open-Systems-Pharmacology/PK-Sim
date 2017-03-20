using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Model.Extensions;
using OSPSuite.Core.Domain.Data;

namespace PKSim.Core
{
   public abstract class concern_for_DataColumnExtensions : StaticContextSpecification
   {
      protected IndividualSimulation _individualSimulation;
      protected PopulationSimulation _populationSimulation;

      protected override void Context()
      {
         _individualSimulation = new IndividualSimulation();
         _populationSimulation = new PopulationSimulation();
         _populationSimulation.Results = new SimulationResults {new IndividualResults()};
      }
   }

   public class A_calculated_column : concern_for_DataColumnExtensions
   {
      private DataColumn _calculatedColumn;

      protected override void Context()
      {
         base.Context();
         _calculatedColumn = new DataColumn();
         _calculatedColumn.DataInfo.Origin = ColumnOrigins.Calculation;
      }

      [Observation]
      public void should_always_belong_into_a_population_simulation()
      {
         _calculatedColumn.BelongsTo(_populationSimulation).ShouldBeTrue();
      }

      [Observation]
      public void should_belong_into_an_individual_simulation_if_the_simulation_results_contains_the_column()
      {
         var dataRepository = new DataRepository();
         dataRepository.Add(_calculatedColumn);
         _individualSimulation.DataRepository = dataRepository;
         _calculatedColumn.BelongsTo(_individualSimulation).ShouldBeTrue();
      }

      [Observation]
      public void should_not_belong_into_an_individual_simulation_if_the_simulation_results_does_not_contain_the_column()
      {
         var dataRepository = new DataRepository();
         dataRepository.Add(new DataColumn());
         _individualSimulation.DataRepository = dataRepository;
         _calculatedColumn.BelongsTo(_individualSimulation).ShouldBeFalse();
      }
   }
}