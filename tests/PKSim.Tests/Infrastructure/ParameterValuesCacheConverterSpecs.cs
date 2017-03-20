using System.Linq;
using System.Xml.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using FakeItEasy;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure.ProjectConverter.v5_2;
using PKSim.Infrastructure.Serialization.Xml;
using PKSim.IntegrationTests;
using OSPSuite.Core.Converter.v5_2;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using IContainer = OSPSuite.Utility.Container.IContainer;
using IParameterFactory = PKSim.Core.Model.IParameterFactory;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_ParameterValuesCacheConverter : ContextSpecification<ParameterValuesCacheConverter>
   {
      private IParameterFactory _parameterFactory;
      private IContainerTask _containerTask;
      private IEntityPathResolver _entityPathResolver;
      private IParameterQuery _parameterQuery;
      private IGenderRepository _genderRepository;
      private IContainer _container;
      private IDimensionMapper _dimensionMapper;

      protected override void Context()
      {
         _parameterFactory = A.Fake<IParameterFactory>();
         _containerTask = A.Fake<IContainerTask>();
         _entityPathResolver = A.Fake<IEntityPathResolver>();
         _parameterQuery = A.Fake<IParameterQuery>();
         _genderRepository = A.Fake<IGenderRepository>();
         _container = A.Fake<IContainer>();
         _dimensionMapper= A.Fake<IDimensionMapper>();
         sut = new ParameterValuesCacheConverter(_containerTask, _entityPathResolver, _parameterQuery, _parameterFactory,
            _genderRepository,_container,_dimensionMapper);
      }

      protected RandomPopulation CreateRandomPopulation()
      {
         var pop = new RandomPopulation();
         pop.Settings = new RandomPopulationSettings();
         var cache = new PathCache<IParameter>(_entityPathResolver);
         cache.Add("PATH1", DomainHelperForSpecs.ConstantParameterWithValue(4).WithName("PARA1"));
         cache.Add("PATH2", DomainHelperForSpecs.ConstantParameterWithValue(5).WithName("PARA2"));
         pop.IndividualPropertiesCache.ParameterValuesCache.Add(new[] {new ParameterValue("PATH1", 1, 0.1), new ParameterValue("PATH2", 2, 0.2)});
         pop.IndividualPropertiesCache.ParameterValuesCache.Add(new[] {new ParameterValue("PATH1", 11, 0.11), new ParameterValue("PATH2", 11, 0.22)});
         pop.Settings.BaseIndividual = DomainHelperForSpecs.CreateIndividual();
         A.CallTo(() => _containerTask.CacheAllChildren<IParameter>(pop.Settings.BaseIndividual)).Returns(cache);

         return pop;
      }
   }

   public class When_converting_the_parameter_value_cache_for_a_population_simulation : concern_for_ParameterValuesCacheConverter
   {
      private ParameterValuesCache _parameterValueCache;
      private RandomPopulation _population;
      private PopulationSimulation _populationSimulation;

      protected override void Context()
      {
         base.Context();
         _populationSimulation = A.Fake<PopulationSimulation>();
         _parameterValueCache = new ParameterValuesCache();
         _parameterValueCache.Add(new[] {new ParameterValue("PATH1", 1, 0.1), new ParameterValue("PATH2", 2, 0.2)});
         _parameterValueCache.Add(new[] {new ParameterValue("PATH1", 11, 0.11), new ParameterValue("PATH2", 11, 0.22)});

         _population = CreateRandomPopulation();
         A.CallTo(() => _populationSimulation.BuildingBlock<RandomPopulation>()).Returns(_population);
         A.CallTo(() => _populationSimulation.ParameterValuesCache).Returns(_parameterValueCache);
      }

      protected override void Because()
      {
         sut.Convert(_populationSimulation);
      }

      [Observation]
      public void should_have_set_all_the_percentile_to_the_defualt_percentile_value()
      {
         _parameterValueCache.PercentilesFor("PATH1").ShouldOnlyContain(CoreConstants.DEFAULT_PERCENTILE, CoreConstants.DEFAULT_PERCENTILE);
      }
   }

   public class When_converting_the_parameter_value_cache_for_a_random_population : concern_for_ParameterValuesCacheConverter
   {
      private RandomPopulation _population;

      protected override void Context()
      {
         base.Context();
         _population = CreateRandomPopulation();
      }

      protected override void Because()
      {
         sut.Convert(_population);
      }


      [Observation]
      public void should_have_set_all_the_percentile_to_the_defualt_percentile_value()
      {
         _population.IndividualPropertiesCache.PercentilesFor("PATH1").ShouldOnlyContain(CoreConstants.DEFAULT_PERCENTILE, CoreConstants.DEFAULT_PERCENTILE);
      }
   }

}