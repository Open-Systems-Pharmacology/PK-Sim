using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_RandomPopulation : ContextSpecification<RandomPopulation>
   {
      protected Individual _baseIndividual;
      protected IEntityPathResolver _entityPathResolver;
      protected IParameter _oneAdvancedParameter;
      protected IParameter _individualParameter;
      protected IParameter _anotherParameter;
      protected IndividualProperties _indvidualProperties;
      protected string _pathParameterAdvanced;

      protected override void Context()
      {
         var pathParameterIndividual = "individual";
         _pathParameterAdvanced = "advanced";
         var anotherPath = "another";
         sut = new RandomPopulation();
         sut.SetAdvancedParameters(new AdvancedParameterCollection());
         _baseIndividual = A.Fake<Individual>();
         sut.Settings = new RandomPopulationSettings {BaseIndividual = _baseIndividual};
         _entityPathResolver = A.Fake<IEntityPathResolver>();

         _indvidualProperties = new IndividualProperties();
         _indvidualProperties.AddParameterValue(new ParameterValue(pathParameterIndividual, 10, 0.1));
         _indvidualProperties.AddParameterValue(new ParameterValue(_pathParameterAdvanced, 20, 0.2));
         _oneAdvancedParameter = A.Fake<IParameter>().WithName("Advanced");
         A.CallTo(() => _oneAdvancedParameter.IsChangedByCreateIndividual).Returns(false);
         _oneAdvancedParameter.Visible = true;
         _individualParameter = A.Fake<IParameter>().WithName("Individual");
         A.CallTo(() => _individualParameter.IsChangedByCreateIndividual).Returns(true);
         _individualParameter.Visible = true;
         _anotherParameter = A.Fake<IParameter>().WithName("Another");
         A.CallTo(() => _anotherParameter.IsChangedByCreateIndividual).Returns(true);
         _anotherParameter.Visible = true;
         A.CallTo(() => _entityPathResolver.PathFor(_oneAdvancedParameter)).Returns(_pathParameterAdvanced);
         A.CallTo(() => _entityPathResolver.PathFor(_individualParameter)).Returns(pathParameterIndividual);
         A.CallTo(() => _entityPathResolver.PathFor(_anotherParameter)).Returns(anotherPath);
         A.CallTo(() => _baseIndividual.GetAllChildren<IParameter>()).Returns(new[] {_individualParameter, _oneAdvancedParameter, _anotherParameter});
         sut.AddIndividualProperties(_indvidualProperties);
      }
   }

   public class When_retrieving_the_distributed_parameter_defined_in_a_random_population : concern_for_RandomPopulation
   {
      private IEnumerable<IParameter> _results;

      protected override void Because()
      {
         _results = sut.AllVectorialParameters(_entityPathResolver);
      }

      [Observation]
      public void should_return_the_parameters_marked_has_changed_by_the_create_individual_algorithm()
      {
         _results.Contains(_individualParameter).ShouldBeTrue();
      }

      [Observation]
      public void should_return_the_parameters_that_were_defined_as_advanced_parameters_for_the_population()
      {
         _results.Contains(_oneAdvancedParameter).ShouldBeTrue();
      }

      [Observation]
      public void should_not_returns_the_standard_parameter_that_were_not_varied_in_the_population()
      {
         _results.Contains(_oneAdvancedParameter).ShouldBeTrue();
      }
   }

   public class When_retrieving_the_distributed_parameter_defined_in_an_empty_random_population : concern_for_RandomPopulation
   {
      protected override void Context()
      {
         base.Context();
         sut = new RandomPopulation();
         sut.SetAdvancedParameters(new AdvancedParameterCollection());
         sut.Settings = new RandomPopulationSettings {BaseIndividual = _baseIndividual};
      }

      [Observation]
      public void should_return_an_empty_list_of_parameters()
      {
         sut.AllVectorialParameters(_entityPathResolver).ShouldBeEmpty();
      }
   }

   public class When_retrieving_the_advanced_parameters_defined_in_a_population : concern_for_RandomPopulation
   {
      private IEnumerable<IParameter> _results;

      protected override void Because()
      {
         _results = sut.AllAdvancedParameters(_entityPathResolver);
      }

      [Observation]
      public void should_return_the_parameters_marked_as_advanced_in_the_population()
      {
         _results.ShouldOnlyContain(_oneAdvancedParameter);
      }
   }

   public class When_retrieving_the_advanced_parameter_by_path : concern_for_RandomPopulation
   {
      [Observation]
      public void should_return_the_epected_parmameter_if_the_parameter_exists()
      {
         sut.ParameterByPath(_pathParameterAdvanced, _entityPathResolver).ShouldBeEqualTo(_oneAdvancedParameter);
      }

      [Observation]
      public void should_return_null_if_the_parameter_does_not_exist()
      {
         sut.ParameterByPath("toto", _entityPathResolver).ShouldBeNull();
      }
   }

   public class When_retrieving_the_constant_parameters_defined_in_a_population : concern_for_RandomPopulation
   {
      private IEnumerable<IParameter> _results;

      protected override void Because()
      {
         _results = sut.AllConstantParameters(_entityPathResolver);
      }

      [Observation]
      public void should_return_the_parameters_marked_as_advanced_in_the_population()
      {
         _results.ShouldOnlyContain(_anotherParameter);
      }
   }

   public class When_adding_an_advanced_parameter_to_the_population : concern_for_RandomPopulation
   {
      private AdvancedParameter _advancedParameter;
      private IParameter _parameter;
      private List<RandomValue> _randomValues;

      protected override void Context()
      {
         base.Context();
         _parameter = A.Fake<IParameter>();
         _advancedParameter = A.Fake<AdvancedParameter>().WithId("Id");
         _advancedParameter.ParameterPath = "A NEW PARAMETER PATH";
         A.CallTo(() => _entityPathResolver.PathFor(_parameter)).Returns(_advancedParameter.ParameterPath);
         _randomValues = new List<RandomValue>
         {
            new RandomValue {Value = 1, Percentile = 0.5},
            new RandomValue {Value = 2, Percentile = 0.6}
         };
         A.CallTo(() => _advancedParameter.GenerateRandomValues(sut.NumberOfItems)).Returns(_randomValues);
      }

      protected override void Because()
      {
         sut.AddAdvancedParameter(_advancedParameter);
      }

      [Observation]
      public void should_add_the_parameter_to_the_underlying_list_of_advanced_parameters()
      {
         var advParam = sut.AdvancedParameters.ElementAt(0);
         advParam.ShouldBeEqualTo(_advancedParameter);
      }

      [Observation]
      public void should_generate_one_random_value_for_each_individuals_defined_in_the_population_for_that_parameter()
      {
         sut.AllValuesFor(_advancedParameter.ParameterPath).ShouldOnlyContain(_randomValues.Select(x => x.Value));
      }

      [Observation]
      public void should_generate_the_percentile_value_for_each_individuals_defined_in_the_population_for_that_parameter()
      {
         var values = sut.AllPercentilesFor(_advancedParameter.ParameterPath).ToArray();
         var sourceValues = _randomValues.Select(x => x.Percentile).ToArray();

         values[0].ShouldBeEqualTo(sourceValues[0], 1e-2);
         values[1].ShouldBeEqualTo(sourceValues[1], 1e-2);
      }
   }

   public class When_removing_an_advanced_parameter : concern_for_RandomPopulation
   {
      private AdvancedParameter _advancedParameter;

      protected override void Context()
      {
         base.Context();
         _advancedParameter = A.Fake<AdvancedParameter>();
         _advancedParameter.ParameterPath = "A NEW PARAMETER PATH";
         sut.AddAdvancedParameter(_advancedParameter);
      }

      protected override void Because()
      {
         sut.RemoveAdvancedParameter(_advancedParameter);
      }

      [Observation]
      public void should_remove_the_parameter_to_the_underlying_list_of_advanced_parameters()
      {
         sut.AdvancedParameters.Contains(_advancedParameter).ShouldBeFalse();
      }

      [Observation]
      public void should_not_contain_any_value_for_the_parameter_with_the_given_path_anymore()
      {
         sut.IndividualPropertiesCache.Has(_advancedParameter.ParameterPath).ShouldBeFalse();
      }
   }

   public class When_returning_all_covariates_name : concern_for_RandomPopulation
   {
      protected override void Context()
      {
         base.Context();
         sut.IndividualPropertiesCache.AddConvariate("Cov1", new List<string> {"Male"});
         sut.IndividualPropertiesCache.AddConvariate("Cov2", new List<string> {"EU"});
      }

      [Observation]
      public void should_return_the_convariate_defined_in_the_individual_properties_cache_extended_with_the_population_name()
      {
         sut.AllCovariateNames.ShouldOnlyContain("Cov1", "Cov2", CoreConstants.Covariates.POPULATION_NAME);
      }

      [Observation]
      public void the_population_name_should_only_be_available_once()
      {
         sut.IndividualPropertiesCache.AddConvariate(CoreConstants.Covariates.POPULATION_NAME, new List<string> {"TOTO"});
         sut.AllCovariateNames.ShouldOnlyContain("Cov1", "Cov2", CoreConstants.Covariates.POPULATION_NAME);
      }
   }

   public class When_retrieving_the_default_species_information_for_a_population_whose_individual_is_not_defined : concern_for_RandomPopulation
   {
      protected override void Context()
      {
         base.Context();
         sut = new RandomPopulation();
      }

      [Observation]
      public void should_return_that_the_species_is_not_human()
      {
         sut.IsHuman.ShouldBeFalse();
      }

      [Observation]
      public void should_return_that_the_species_is_not_age_dependent_human()
      {
         sut.IsAgeDependent.ShouldBeFalse();
      }

      [Observation]
      public void should_return_that_the_species_is_not_preterm()
      {
         sut.IsPreterm.ShouldBeFalse();
      }
   }
}