using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using PKSim.Core.Chart;

namespace PKSim.Core
{
   public abstract class concern_for_PopulationSimulationComparison : ContextSpecification<PopulationSimulationComparison>
   {
      protected PopulationSimulation _popSim1;
      protected PopulationSimulation _popSim2;
      protected PopulationSimulation _popSim3;

      protected override void Context()
      {
         sut = new PopulationSimulationComparison();
         _popSim1 = A.Fake<PopulationSimulation>().WithId("1").WithName("Sim1");
         _popSim2 = A.Fake<PopulationSimulation>().WithId("2").WithName("Sim2");
         _popSim3 = A.Fake<PopulationSimulation>().WithId("3").WithName("Sim3");
         sut.AddSimulation(_popSim1);
         sut.AddSimulation(_popSim2);
         sut.AddSimulation(_popSim3);
      }
   }

   public class When_inspecting_used_observed_data : concern_for_PopulationSimulationComparison
   {
      private DataRepository _observedData;

      protected override void Context()
      {
         base.Context();
         _observedData = new DataRepository();
         var simulationTimeProfileChart = new SimulationTimeProfileChart();
         simulationTimeProfileChart.AddObservedData(_observedData);
         sut.AddAnalysis(simulationTimeProfileChart);
      }

      [Observation]
      public void the_observed_data_should_be_indicated_as_used()
      {
         sut.UsesObservedData(_observedData).ShouldBeTrue();
      }
   }

   public class When_retrieving_the_number_of_items_definined_in_the_comparison : concern_for_PopulationSimulationComparison
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _popSim1.NumberOfItems).Returns(2);
         A.CallTo(() => _popSim2.NumberOfItems).Returns(3);
         A.CallTo(() => _popSim3.NumberOfItems).Returns(4);
      }

      [Observation]
      public void should_return_the_sum_of_all_items_defined_in_each_simulations()
      {
         sut.NumberOfItems.ShouldBeEqualTo(2 + 3 + 4);
      }
   }

   public class When_retrieving_all_covariates_names_defined_in_the_comparison : concern_for_PopulationSimulationComparison
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _popSim1.AllCovariateNames).Returns(new[] {"cov1", "cov2"});
         A.CallTo(() => _popSim2.AllCovariateNames).Returns(new[] {"cov3", "cov2"});
         A.CallTo(() => _popSim3.AllCovariateNames).Returns(new[] {"cov1", "cov4"});
      }

      [Observation]
      public void should_return_the_union_of_all_the_covariates_defined_in_the_population_simulation_with_the_population_name_covariate()
      {
         sut.AllCovariateNames.ShouldOnlyContain("cov1", "cov2", "cov3", "cov4", CoreConstants.Covariates.SIMULATION_NAME);
      }
   }

   public class When_retrieving_all_covariates_values_for_a_given_covariates : concern_for_PopulationSimulationComparison
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _popSim1.AllCovariateValuesFor("cov")).Returns(new[] {"A", "B"});
         A.CallTo(() => _popSim2.AllCovariateValuesFor("cov")).Returns(new[] {"unknown"});
         A.CallTo(() => _popSim3.AllCovariateValuesFor("cov")).Returns(new[] {"A", "B", "C"});
      }

      [Observation]
      public void should_return_the_concatenated_list_of_all_covariates_in_the_accurate_order()
      {
         sut.AllCovariateValuesFor("cov").ShouldOnlyContainInOrder("A", "B", "unknown", "A", "B", "C");
      }
   }

   public class When_retrieving_all_races_defined_in_the_comparison : concern_for_PopulationSimulationComparison
   {
      private SpeciesPopulation _race1, _race2, _race3;

      protected override void Context()
      {
         base.Context();
         _race1 = new SpeciesPopulation();
         _race2 = new SpeciesPopulation();
         _race3 = new SpeciesPopulation();
         A.CallTo(() => _popSim1.AllRaces).Returns(new[] {_race1, _race2});
         A.CallTo(() => _popSim2.AllRaces).Returns(new[] {_race2, _race3});
         A.CallTo(() => _popSim3.AllRaces).Returns(new[] {_race3, _race1});
      }

      [Observation]
      public void should_return_the_concatenated_list_of_races_in_the_accurate_order()
      {
         sut.AllRaces.ShouldOnlyContainInOrder(_race1, _race2, _race2, _race3, _race3, _race1);
      }
   }

   public class When_retrieving_all_genders_defined_in_the_comparison : concern_for_PopulationSimulationComparison
   {
      private Gender _gender1, _gender2;

      protected override void Context()
      {
         base.Context();
         _gender1 = new Gender();
         _gender2 = new Gender();
         A.CallTo(() => _popSim1.AllGenders).Returns(new[] {_gender1, _gender2});
         A.CallTo(() => _popSim2.AllGenders).Returns(new[] {_gender2, _gender1});
         A.CallTo(() => _popSim3.AllGenders).Returns(new[] {_gender2, _gender1});
      }

      [Observation]
      public void should_return_the_concatenated_list_of_genders_in_the_accurate_order()
      {
         sut.AllGenders.ShouldOnlyContainInOrder(_gender1, _gender2, _gender2, _gender1, _gender2, _gender1);
      }
   }

   public class When_retrieving_all_pk_values_defined_in_the_comparison : concern_for_PopulationSimulationComparison
   {
      private const string _quantityPath = "Path";
      private const string _pkParameter = "pk";

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _popSim1.AllPKParameterValuesFor(_quantityPath, _pkParameter)).Returns(new[] {1d, 2d, 3d});
         A.CallTo(() => _popSim2.AllPKParameterValuesFor(_quantityPath, _pkParameter)).Returns(new[] {double.NaN, double.NaN});
         A.CallTo(() => _popSim3.AllPKParameterValuesFor(_quantityPath, _pkParameter)).Returns(new[] {1d, 4d});
      }

      [Observation]
      public void should_return_the_concatenated_list_of_pk_parameters_in_the_accurate_order()
      {
         sut.AllPKParameterValuesFor(_quantityPath, _pkParameter).ShouldOnlyContainInOrder(1d, 2d, 3d, double.NaN, double.NaN, 1d, 4d);
      }
   }

   public class When_retrieving_all_pk_parameters_defined_in_the_comparison : concern_for_PopulationSimulationComparison
   {
      private readonly QuantityPKParameter _pk1 = new QuantityPKParameter {Name = "P1"};
      private readonly QuantityPKParameter _pk2 = new QuantityPKParameter {Name = "P2"};
      private readonly QuantityPKParameter _pk3 = new QuantityPKParameter {Name = "P3"};
      private const string _quantityPath = "Path";

      protected override void Context()
      {
         base.Context();

         A.CallTo(() => _popSim1.AllPKParametersFor(_quantityPath)).Returns(new[] {_pk1, _pk2});
         A.CallTo(() => _popSim2.AllPKParametersFor(_quantityPath)).Returns(new[] {_pk2, _pk3});
         A.CallTo(() => _popSim3.AllPKParametersFor(_quantityPath)).Returns(new[] {_pk1, _pk2, _pk3});
      }

      [Observation]
      public void should_return_the_intersected_list_of_pk_parameters()
      {
         sut.AllPKParametersFor(_quantityPath).ShouldOnlyContain(_pk2);
      }
   }

   public class When_retrieving_all_parameters_values_defined_in_the_comparison : concern_for_PopulationSimulationComparison
   {
      private const string _parameterPath = "path";

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _popSim1.AllValuesFor(_parameterPath)).Returns(new[] {1d, 2d, 3d});
         A.CallTo(() => _popSim2.AllValuesFor(_parameterPath)).Returns(new[] {double.NaN, double.NaN});
         A.CallTo(() => _popSim3.AllValuesFor(_parameterPath)).Returns(new[] {1d, 4d});
      }

      [Observation]
      public void should_return_the_concatenated_list_of_parameter_values_in_the_accurate_order()
      {
         sut.AllValuesFor(_parameterPath).ShouldOnlyContainInOrder(1d, 2d, 3d, double.NaN, double.NaN, 1d, 4d);
      }
   }

   public class When_checking_if_a_population_simulation_comparision_has_up_to_date_results : concern_for_PopulationSimulationComparison
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _popSim1.HasUpToDateResults).Returns(true);
         A.CallTo(() => _popSim2.HasUpToDateResults).Returns(true);
      }

      [Observation]
      public void should_return_true_if_all_simulations_have_uptodate_results()
      {
         A.CallTo(() => _popSim3.HasUpToDateResults).Returns(true);
         sut.HasUpToDateResults.ShouldBeEqualTo(true);
      }

      [Observation]
      public void should_return_false_if_at_least_one_simulation_does_not_have_uptodate_results()
      {
         A.CallTo(() => _popSim3.HasUpToDateResults).Returns(false);
         sut.HasUpToDateResults.ShouldBeEqualTo(false);
      }
   }

   public class When_adding_an_analyse_to_a_comparison : concern_for_PopulationSimulationComparison
   {
      private ISimulationAnalysis _analysis;

      protected override void Context()
      {
         base.Context();
         _analysis = A.Fake<ISimulationAnalysis>();
      }

      protected override void Because()
      {
         sut.AddAnalysis(_analysis);
      }

      [Observation]
      public void should_sets_itself_as_the_analysable()
      {
         _analysis.Analysable.ShouldBeEqualTo(sut);
      }
   }

   public class When_retrieving_the_name_of_all_simulations : concern_for_PopulationSimulationComparison
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _popSim1.AllSimulationNames).Returns(new[] { _popSim1.Name, _popSim1.Name });
         A.CallTo(() => _popSim2.AllSimulationNames).Returns(new[] { _popSim2.Name, _popSim2.Name, _popSim2.Name });
         A.CallTo(() => _popSim3.AllSimulationNames).Returns(new[] { _popSim3.Name });
      }

      [Observation]
      public void should_return_the_name_of_all_simulations_used_in_the_comparison()
      {
         sut.AllSimulationNames.ShouldOnlyContainInOrder(_popSim1.Name, _popSim1.Name,_popSim2.Name,_popSim2.Name,_popSim2.Name,_popSim3.Name);
      }
   }
}