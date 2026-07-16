using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Populations;
using OSPSuite.Core.Domain.Services;
using PKSim.Core.Model;
using PKSim.R.Mappers;

namespace PKSim.R
{
   public abstract class concern_for_PopulationSimulationToIndividualValuesCacheMapper : ContextSpecification<IPopulationSimulationToIndividualValuesCacheMapper>
   {
      protected IEntityPathResolver _entityPathResolver;
      protected PopulationSimulation _populationSimulation;
      protected IndividualValuesCache _basePopulationCache;

      protected override void Context()
      {
         _entityPathResolver = A.Fake<IEntityPathResolver>();

         _basePopulationCache = new IndividualValuesCache();
         _basePopulationCache.SetValues("Organism|Weight", new[] { 70.0, 75.0, 80.0 });

         var population = A.Fake<Population>();
         A.CallTo(() => population.IndividualValuesCache).Returns(_basePopulationCache);

         _populationSimulation = A.Fake<PopulationSimulation>();
         A.CallTo(() => _populationSimulation.Population).Returns(population);

         sut = new PopulationSimulationToIndividualValuesCacheMapper(_entityPathResolver);
      }
   }

   public class When_mapping_a_population_simulation_carrying_advanced_parameters : concern_for_PopulationSimulationToIndividualValuesCacheMapper
   {
      private IndividualValuesCache _result;
      private IParameter _advancedParameter;

      protected override void Context()
      {
         base.Context();
         _advancedParameter = A.Fake<IParameter>();
         A.CallTo(() => _entityPathResolver.PathFor(_advancedParameter)).Returns("Organism|Liver|CL");
         A.CallTo(() => _populationSimulation.AllAdvancedParameters(_entityPathResolver)).Returns(new[] { _advancedParameter });
         A.CallTo(() => _populationSimulation.AllValuesFor("Organism|Liver|CL")).Returns(new[] { 1.0, 2.0, 3.0 });
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_populationSimulation);
      }

      [Observation]
      public void should_carry_the_base_population_parameters()
      {
         _result.Has("Organism|Weight").ShouldBeTrue();
         _result.ValuesFor("Organism|Weight").ShouldOnlyContainInOrder(70.0, 75.0, 80.0);
      }

      [Observation]
      public void should_merge_the_advanced_parameter_values_into_the_cache()
      {
         _result.Has("Organism|Liver|CL").ShouldBeTrue();
         _result.ValuesFor("Organism|Liver|CL").ShouldOnlyContainInOrder(1.0, 2.0, 3.0);
      }

      [Observation]
      public void should_return_a_clone_and_leave_the_source_population_cache_untouched()
      {
         _basePopulationCache.Has("Organism|Liver|CL").ShouldBeFalse();
      }
   }

   public class When_mapping_a_population_simulation_without_advanced_parameters : concern_for_PopulationSimulationToIndividualValuesCacheMapper
   {
      private IndividualValuesCache _result;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _populationSimulation.AllAdvancedParameters(_entityPathResolver)).Returns(new IParameter[] { });
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_populationSimulation);
      }

      [Observation]
      public void should_return_the_base_population_cache_only()
      {
         _result.AllParameterPaths().ShouldOnlyContain("Organism|Weight");
      }
   }
}
