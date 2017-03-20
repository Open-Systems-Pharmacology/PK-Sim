using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Services;
using OSPSuite.Core.Domain.Data;

namespace PKSim.Core
{
   public abstract class concern_for_SimulationResultsCreator : ContextSpecification<ISimulationResultsCreator>
   {
      protected DataRepository _dataRepository;
      protected SimulationResults _simulationResults;
      
      protected override void Context()
      {
         sut = new SimulationResultsCreator();
      }
   }

   public class When_creating_a_simulation_results_based_on_a_given_data_repository : concern_for_SimulationResultsCreator
   {
      private IndividualResults _individualResults;

      protected override void Context()
      {
         base.Context();
         _dataRepository = DomainHelperForSpecs.IndividualSimulationDataRepositoryFor("sim");
      }
      protected override void Because()
      {
         _simulationResults = sut.CreateResultsFrom(_dataRepository);
         _individualResults = _simulationResults.First();
      }

      [Observation]
      public void should_return_a_simulation_results_containing_only_one_individual_result()
      {
         _simulationResults.Count.ShouldBeEqualTo(1);
      }

      [Observation]
      public void the_created_individual_results_should_contain_one_entry_for_each_available_column()
      {
         _individualResults.AllValues.Count.ShouldBeEqualTo(1);
         firstValue.Time.ShouldBeEqualTo(_individualResults.Time);
      }

      private QuantityValues firstValue
      {
         get { return _individualResults.AllValues.First(); }
      }

      [Observation]
      public void should_have_removed_the_first_entry_in_the_path_corresponding_to_the_simulation_name_of_the_returned_columns()
      {
         firstValue.QuantityPath.Contains("sim").ShouldBeFalse();
      }

      [Observation]
      public void should_use_the_expected_values()
      {
         firstValue.Values.ShouldOnlyContain(10f, 20f, 30f);
         firstValue.Time.Values.ShouldOnlyContain(1f, 2f, 3f);
      }
   }
}	