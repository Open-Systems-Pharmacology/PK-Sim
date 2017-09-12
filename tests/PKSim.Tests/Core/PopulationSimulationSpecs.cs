using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;

namespace PKSim.Core
{
   public abstract class concern_for_PopulationSimulation : ContextSpecification<PopulationSimulation>
   {
      protected Container _liver;
      protected Population _population;

      protected override void Context()
      {
         sut = new PopulationSimulation().WithName("Sim");
         sut.Add(new AdvancedParameterCollection().WithName("AdvancedParameterCollection"));
         sut.Model = A.Fake<IModel>();
         sut.Model.Root = new Container().WithName(sut.Name);
         var drugContainer = new Container().WithName("DRUG");
         drugContainer.Add(DomainHelperForSpecs.ConstantParameterWithValue(10).WithName(CoreConstants.Parameter.TotalDrugMass));

         sut.Model.Root.Add(drugContainer);
         _liver = new Container().WithName("Liver");
         _population = A.Fake<Population>();
         A.CallTo(() => _population.NumberOfItems).Returns(3);
         sut.AddUsedBuildingBlock(new UsedBuildingBlock("Id", PKSimBuildingBlockType.Population) {BuildingBlock = _population});
         sut.Model.Root.Add(_liver);
      }
   }

   public class When_retrieving_the_values_for_an_existing_parameter_by_path : concern_for_PopulationSimulation
   {
      protected override void Context()
      {
         base.Context();
         sut.ParameterValuesCache.Add(new ParameterValues("Liver|P") {Values = new List<double> {1, 2, 3}});
      }

      [Observation]
      public void should_return_the_values_defined_for_this_parameter()
      {
         sut.AllValuesFor("Liver|P").ShouldOnlyContainInOrder(1d, 2d, 3d);
      }
   }

   public class When_retrieving_the_values_for_a_parameter_that_does_not_exist_in_the_simulation : concern_for_PopulationSimulation
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _population.AllValuesFor("Liver|P")).Returns(new List<double> {double.NaN, double.NaN, double.NaN});
      }

      [Observation]
      public void should_return_the_default_missing_values()
      {
         sut.AllValuesFor("Liver|P").ShouldOnlyContainInOrder(double.NaN, double.NaN, double.NaN);
      }
   }

   public class When_retrieving_the_pk_parameter_values_for_an_existing_parameter_by_path : concern_for_PopulationSimulation
   {
      private QuantityPKParameter _pkParameter;

      protected override void Context()
      {
         base.Context();
         sut.PKAnalyses = A.Fake<PopulationSimulationPKAnalyses>();
         _pkParameter = A.Fake<QuantityPKParameter>();
         A.CallTo(() => _pkParameter.Values).Returns(new List<float> {1, 2, 3});
         A.CallTo(() => sut.PKAnalyses.PKParameterFor("Path", "Cmax")).Returns(_pkParameter);
      }

      [Observation]
      public void should_return_the_values_defined_for_this_parameter()
      {
         sut.AllPKParameterValuesFor("Path", "Cmax").ShouldOnlyContainInOrder(1d, 2d, 3d);
      }
   }

   public class When_retrieving_the_values_for_a_pk_parameter_that_does_not_exist_in_the_simulation : concern_for_PopulationSimulation
   {
      protected override void Context()
      {
         base.Context();
         sut.PKAnalyses = A.Fake<PopulationSimulationPKAnalyses>();
         A.CallTo(() => sut.PKAnalyses.PKParameterFor("Path", "Cmax")).Returns(null);
      }

      [Observation]
      public void should_return_the_default_missing_values()
      {
         sut.AllPKParameterValuesFor("Path", "Cmax").ShouldOnlyContainInOrder(float.NaN, float.NaN, float.NaN);
      }
   }

   public class When_retrievng_the_list_of_all_available_values : concern_for_PopulationSimulation
   {
      protected override void Context()
      {
         base.Context();
         var population = A.Fake<Population>();
         A.CallTo(() => population.NumberOfItems).Returns(3);
         A.CallTo(() => population.AllCovariateValuesFor("cov1")).Returns(new[] {"A", "B"});
         sut.AddUsedBuildingBlock(new UsedBuildingBlock("Id", PKSimBuildingBlockType.Population) {BuildingBlock = population});
      }

      [Observation]
      public void should_return_the_simulation_names_for_the_simulation_name_covariate()
      {
         sut.AllCovariateValuesFor(CoreConstants.Covariates.SIMULATION_NAME).ShouldOnlyContainInOrder(sut.Name, sut.Name, sut.Name);
      }

      [Observation]
      public void should_return_the_covariates_of_the_underlying_population_for_any_other_covariates()
      {
         sut.AllCovariateValuesFor("cov1").ShouldOnlyContainInOrder("A", "B");
      }
   }

   public class When_retrieving_all_simulation_names_for_a_population_simulation : concern_for_PopulationSimulation
   {
      protected override void Context()
      {
         base.Context();
         var population = A.Fake<Population>();
         A.CallTo(() => population.NumberOfItems).Returns(3);
         sut.Name = "TOTO";
         sut.AddUsedBuildingBlock(new UsedBuildingBlock("Id", PKSimBuildingBlockType.Population) {BuildingBlock = population});
      }

      [Observation]
      public void should_always_return_an_empty_string()
      {
         sut.AllSimulationNames.ShouldOnlyContain(sut.Name, sut.Name, sut.Name);
      }
   }

   public class When_retrieving_the_total_drug_mass_per_body_weight_for_an_imported_population_simulation_for_which_no_application_could_be_found : concern_for_PopulationSimulation
   {
      protected override void Context()
      {
         base.Context();
         var ind = A.Fake<Individual>();
         var weightParameter = A.Fake<IParameter>();
         weightParameter.Value = 20;
         A.CallTo(() => ind.WeightParameter).Returns(weightParameter);
         A.CallTo(() => _population.FirstIndividual).Returns(ind);
      }

      [Observation]
      public void should_return_null()
      {
         sut.TotalDrugMassPerBodyWeightFor("TOTO").ShouldBeNull();
      }
   }

   public class When_retrieving_all_possible_advanced_parameters_from_a_population_simulation : concern_for_PopulationSimulation
   {
      private IEnumerable<string> _allParameters;

      protected override void Context()
      {
         base.Context();
         sut.Model.Root.Add(new PKSimParameter {Name = "P1", BuildingBlockType = PKSimBuildingBlockType.Compound});
         sut.Model.Root.Add(new PKSimParameter {Name = "P2", BuildingBlockType = PKSimBuildingBlockType.Simulation});
         sut.Model.Root.Add(new PKSimParameter {Name = "P3", BuildingBlockType = PKSimBuildingBlockType.Event});
         sut.Model.Root.Add(new PKSimParameter {Name = "P4", BuildingBlockType = PKSimBuildingBlockType.Formulation});
         sut.Model.Root.Add(new PKSimParameter {Name = "P5", BuildingBlockType = PKSimBuildingBlockType.Individual});
      }

      protected override void Because()
      {
         _allParameters = sut.AllPotentialAdvancedParameters.Select(x => x.Name);
      }

      [Observation]
      public void should_return_all_compound_parameters()
      {
         _allParameters.Contains("P1").ShouldBeTrue();
      }

      [Observation]
      public void should_return_all_simulation_parameters()
      {
         _allParameters.Contains("P2").ShouldBeTrue();
      }

      [Observation]
      public void should_return_all_events_parameters()
      {
         _allParameters.Contains("P3").ShouldBeTrue();
      }

      [Observation]
      public void should_return_all_formulation_parameters()
      {
         _allParameters.Contains("P4").ShouldBeTrue();
      }

      [Observation]
      public void should_not_contain_other_parameter_types()
      {
         _allParameters.Contains("P5").ShouldBeFalse();
      }
   }

   public class When_retrieving_the_simulation_results_for_a_given_quantity_path_for_which_the_calculation_was_incomplete : concern_for_PopulationSimulation
   {
      private string _quantityPath;
      private IReadOnlyList<QuantityValues> _results;
      private QuantityValues _qv0;
      private QuantityValues _qv2;

      protected override void Context()
      {
         base.Context();
         _quantityPath = "Path";
         sut.Results = new SimulationResults();
         _qv0 = new QuantityValues {QuantityPath = _quantityPath};
         var indResults0 = new IndividualResults {_qv0};
         indResults0.IndividualId = 0;

         _qv2 = new QuantityValues {QuantityPath = _quantityPath};
         var indResults2 = new IndividualResults {_qv2};
         indResults2.IndividualId = 2;

         sut.Results.Add(indResults0);
         sut.Results.Add(indResults2);
      }

      protected override void Because()
      {
         _results = sut.AllOutputValuesFor(_quantityPath);
      }

      [Observation]
      public void should_patch_up_the_existing_results_by_adding_empty_elements()
      {
         _results.Count.ShouldBeEqualTo(3);
         _results[0].ShouldBeEqualTo(_qv0);
         _results[1].ShouldBeAnInstanceOf<NullQuantityValues>();
         _results[2].ShouldBeEqualTo(_qv2);
      }
   }

   public class When_retrieving_the_simulation_results_for_a_given_quantity_path_for_which_the_number_of_existing_data_is_greater_than_the_number_of_individual : concern_for_PopulationSimulation
   {
      private string _quantityPath;
      private IReadOnlyList<QuantityValues> _results;

      protected override void Context()
      {
         base.Context();
         _quantityPath = "Path";
         sut.Results = new SimulationResults();

         //add one result more that the number of item in the population
         for (int i = 0; i < _population.NumberOfItems + 1; i++)
         {
            sut.Results.Add(new IndividualResults {new QuantityValues {QuantityPath = _quantityPath}});
         }
      }

      protected override void Because()
      {
         _results = sut.AllOutputValuesFor(_quantityPath);
      }

      [Observation]
      public void should_return_null_quantity_values()
      {
         for (int i = 0; i < _results.Count; i++)
         {
            _results[i].ShouldBeAnInstanceOf<NullQuantityValues>();
         }
      }
   }

   public class When_retrieving_the_simulation_results_for_a_given_quantity_path_for_which_the_calculation_was_complete : concern_for_PopulationSimulation
   {
      private string _quantityPath;
      private IReadOnlyList<QuantityValues> _results;
      private QuantityValues _qv0;
      private QuantityValues _qv1;
      private QuantityValues _qv2;

      protected override void Context()
      {
         base.Context();
         _quantityPath = "Path";
         sut.Results = new SimulationResults();
         _qv0 = new QuantityValues {QuantityPath = _quantityPath};
         var indResults0 = new IndividualResults {_qv0};
         indResults0.IndividualId = 0;

         _qv1 = new QuantityValues {QuantityPath = _quantityPath};
         var indResults1 = new IndividualResults {_qv1};
         indResults1.IndividualId = 1;

         _qv2 = new QuantityValues {QuantityPath = _quantityPath};
         var indResults2 = new IndividualResults {_qv2};
         indResults2.IndividualId = 3;

         sut.Results.Add(indResults0);
         sut.Results.Add(indResults1);
         sut.Results.Add(indResults2);
      }

      protected override void Because()
      {
         _results = sut.AllOutputValuesFor(_quantityPath);
      }

      [Observation]
      public void should_return_all_exsiting_values()
      {
         _results.Count.ShouldBeEqualTo(3);
         _results[0].ShouldBeEqualTo(_qv0);
         _results[1].ShouldBeEqualTo(_qv1);
         _results[2].ShouldBeEqualTo(_qv2);
      }
   }

   public class When_updating_the_values_from_an_original_population_simulation : concern_for_PopulationSimulation
   {
      private PopulationSimulation _originalSimulation;
      private AdvancedParameter _advancedParameter;
      private string _parameterPath;

      protected override void Context()
      {
         base.Context();
         _parameterPath = "Liver|P1";
         _originalSimulation = new PopulationSimulation();
         _originalSimulation.AddUsedBuildingBlock(new UsedBuildingBlock("Pop",PKSimBuildingBlockType.Population){BuildingBlock = _population});
         _originalSimulation.Add(new AdvancedParameterCollection().WithName("AdvancedParameterCollection"));
         _advancedParameter = A.Fake<AdvancedParameter>();
         _advancedParameter.ParameterPath=_parameterPath;
         A.CallTo(() => _advancedParameter.GenerateRandomValues(3)).Returns(new []
         {
            new RandomValue{Percentile = 0.5,Value=1},
            new RandomValue{Percentile = 0.5,Value=2},
            new RandomValue{Percentile = 0.5,Value=3},
         });
         _originalSimulation.AddAdvancedParameter(_advancedParameter, generateRandomValues: true);
      }

      protected override void Because()
      {
         sut.UpdateFromOriginalSimulation(_originalSimulation);
      }

      [Observation]
      public void should_generate_new_values_for_advanced_parameters_if_not_defined_already()
      {
         sut.ParameterValuesCache.Has(_parameterPath).ShouldBeTrue();
         sut.ParameterValuesCache.ValuesFor(_parameterPath).ShouldOnlyContainInOrder(1,2,3);
      }
   }
}