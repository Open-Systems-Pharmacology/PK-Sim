using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using FakeItEasy;

namespace PKSim.Core
{
   public abstract class concern_for_OriginDataTask : ContextSpecification<OriginDataTask>
   {
      internal ICalculationMethodCategoryRepository _calculationMethodRepository;
      protected IList<CalculationMethodCategory> _allCategories;

      protected override void Context()
      {
         _calculationMethodRepository =A.Fake<ICalculationMethodCategoryRepository>();
         _allCategories = new List<CalculationMethodCategory>();
         A.CallTo(() => _calculationMethodRepository.All()).Returns(_allCategories);
         sut = new OriginDataTask(_calculationMethodRepository);
      }
   }

   
   public class When_retrieving_the_default_population_for_a_species : concern_for_OriginDataTask
   {
      private Species _species;
      private SubPopulation _result;
      private ParameterValueVersion _pvv1;
      private ParameterValueVersion _pvv2;

      protected override void Context()
      {
         base.Context();
         _species = A.Fake<Species>();
         var pvvc1 =A.Fake<ParameterValueVersionCategory>();
         var pvvc2 = A.Fake<ParameterValueVersionCategory>();
         _pvv1 =new ParameterValueVersion().WithName("toto");
         _pvv2 = new ParameterValueVersion().WithName("tata");
         A.CallTo(() => pvvc1.DefaultItem).Returns(_pvv1);
         A.CallTo(() => pvvc2.DefaultItem).Returns(_pvv2);
         A.CallTo(() => _species.PVVCategories).Returns(new[] {pvvc1, pvvc2});
      }

      protected override void Because()
      {
         _result = sut.DefaultSubPopulationFor(_species);
      }

      [Observation]
      public void should_return_a_sub_population_containing_the_default_parameter_value_version_for_the_species()
      {
         _result.ParameterValueVersions.ShouldOnlyContain(_pvv1, _pvv2);
      }
   }

   
   public class When_retrieving_all_calculation_methods_category_defined_for_a_given_species : concern_for_OriginDataTask
   {
      private CalculationMethodCategory _notIndividualCategory;
      private CalculationMethodCategory _humanCategory;
      private CalculationMethodCategory _dogCategory;
      private CalculationMethod _cmHuman;
      private CalculationMethod _cmDog;
      private Species _species;
      private IEnumerable<CalculationMethodCategory> _results;

      protected override void Context()
      {
         base.Context();
         _notIndividualCategory = new CalculationMethodCategory {CategoryType = CategoryType.Model};
         _humanCategory = new CalculationMethodCategory { CategoryType = CategoryType.Individual };
         _dogCategory = new CalculationMethodCategory { CategoryType = CategoryType.Individual };
         _cmHuman = new CalculationMethod();
         _cmHuman.AddSpecies(CoreConstants.Species.Human);
         _humanCategory.Add(_cmHuman);
         _cmDog = new CalculationMethod();
         _cmDog.AddSpecies("Dog");
         _dogCategory.Add(_cmDog);
         _allCategories.Add(_notIndividualCategory);
         _allCategories.Add(_humanCategory);
         _allCategories.Add(_dogCategory);
         _species = new Species {Name = CoreConstants.Species.Human};
      }

      protected override void Because()
      {
         _results = sut.AllCalculationMethodCategoryFor(_species);
      }
      [Observation]
      public void should_return_the_defined_calculation_method_for_individual_available_for_the_given_species()
      {
         _results.ShouldOnlyContain(_humanCategory);
      }
   }
}