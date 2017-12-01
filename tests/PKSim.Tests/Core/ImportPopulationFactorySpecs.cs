using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Events;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core
{
   public abstract class concern_for_ImportPopulationFactory : ContextSpecification<IImportPopulationFactory>
   {
      protected IContainerTask _containerTask;
      protected IObjectBaseFactory _objectBaseFactory;
      protected IProgressManager _progressManager;
      protected ICloner _cloner;
      protected IIndividualPropertiesCacheImporter _individualCacheImporter;
      protected string _file1 = "file1";
      protected string _file2 = "file2";
      protected Individual _individual;
      protected ImportPopulation _population;
      protected Individual _cloneIndividual;
      protected IAdvancedParameterFactory _advancedParameterFactory;
      protected ImportPopulation _createdPopulation;
      protected IndividualPropertiesCache _popFile1;
      protected IndividualPropertiesCache _popFile2;

      protected override void Context()
      {
         _containerTask = A.Fake<IContainerTask>();
         _progressManager = A.Fake<IProgressManager>();
         _individualCacheImporter = A.Fake<IIndividualPropertiesCacheImporter>();
         _cloner = A.Fake<ICloner>();
         _objectBaseFactory = A.Fake<IObjectBaseFactory>();
         _advancedParameterFactory = A.Fake<IAdvancedParameterFactory>();
         _createdPopulation = A.Fake<ImportPopulation>();
         _individual = new Individual();
         _cloneIndividual = new Individual();
         A.CallTo(() => _cloner.Clone(_individual)).Returns(_cloneIndividual);
         A.CallTo(() => _objectBaseFactory.Create<ImportPopulation>()).Returns(_createdPopulation);
         A.CallTo(() => _createdPopulation.IndividualPropertiesCache).Returns(A.Fake<IndividualPropertiesCache>());
         sut = new ImportPopulationFactory(_objectBaseFactory, _progressManager, _individualCacheImporter, _cloner, _containerTask, _advancedParameterFactory);

         _popFile1 = A.Fake<IndividualPropertiesCache>();
         _popFile2 = A.Fake<IndividualPropertiesCache>();
         A.CallTo(() => _individualCacheImporter.ImportFrom(_file1, A<IImportLogger>._)).Returns(_popFile1);
         A.CallTo(() => _individualCacheImporter.ImportFrom(_file2, A<IImportLogger>._)).Returns(_popFile2);
      }
   }

   public class When_told_to_create_a_import_population_based_on_some_files : concern_for_ImportPopulationFactory
   {
      private ParameterValues _parameterValues;
      private IndividualPropertiesCache _individualPropertiesCache;

      protected override void Context()
      {
         base.Context();
         _individualPropertiesCache = new IndividualPropertiesCache();
         _parameterValues = new ParameterValues("A|Path|With|Unit [l]");
         _individualPropertiesCache.Add(_parameterValues);
         A.CallTo(() => _individualCacheImporter.ImportFrom(_file1, A<IImportLogger>._)).Returns(_individualPropertiesCache);


         var pathCache = A.Fake<PathCache<IParameter>>();
         A.CallTo(() => pathCache.Contains("A|Path|With|Unit")).Returns(true);
         A.CallTo(() => _containerTask.CacheAllChildren<IParameter>(_cloneIndividual)).Returns(pathCache);
      }

      [Observation]
      public async Task should_have_created_a_population_with_the_clone_of_the_base_individual()
      {
         _population = await sut.CreateFor(new[] {_file1, _file2}, _individual, new CancellationToken());
         _population.Settings.BaseIndividual.ShouldBeEqualTo(_cloneIndividual);
      }

      [Observation]
      public async Task should_return_a_population_containing_the_individuals_defined_in_these_files()
      {
         _population = await sut.CreateFor(new[] {_file1, _file2}, _individual, new CancellationToken());
         A.CallTo(() => _population.IndividualPropertiesCache.Merge(_popFile2, A<PathCache<IParameter>>._)).MustHaveHappened();
      }

      [Observation]
      public async Task should_remove_units_when_path_is_not_found()
      {
         _population = await sut.CreateFor(new[] { _file1 }, _individual, new CancellationToken());
         _individualPropertiesCache.AllParameterPaths().Contains("A|Path|With|Unit").ShouldBeTrue();
         _individualPropertiesCache.AllParameterPaths().Contains("A|Path|With|Unit [l]").ShouldBeFalse();
         _parameterValues.ParameterPath.ShouldBeEqualTo("A|Path|With|Unit");
      }
   }

   public class When_creating_a_population_from_files_containing_advanced_parameters : concern_for_ImportPopulationFactory
   {
      private readonly List<string> _allImportedParameters = new List<string>();
      private readonly PathCache<IParameter> _allParameters = new PathCache<IParameter>(new EntityPathResolverForSpecs());
      private readonly PathCache<IParameter> _allCreateIndividualParameters = new PathCache<IParameter>(new EntityPathResolverForSpecs());
      private AdvancedParameter _advancedParameter;

      protected override void Context()
      {
         base.Context();
         _allImportedParameters.AddRange(new[] {"P1", "P2"});
         _allParameters.Add("P1", new PKSimParameter().WithName("P1"));
         _allParameters.Add("P2", new PKSimParameter().WithName("P2"));
         _allCreateIndividualParameters.Add("P1", _allParameters.FindByName("P1"));
         _advancedParameter = new AdvancedParameter();
         A.CallTo(() => _containerTask.CacheAllChildren<IParameter>(_cloneIndividual)).Returns(_allParameters);
         A.CallTo(() => _containerTask.CacheAllChildrenSatisfying(_cloneIndividual, A<Func<IParameter, bool>>._)).Returns(_allCreateIndividualParameters);
         A.CallTo(() => _advancedParameterFactory.Create(_allParameters.FindByName("P2"), DistributionTypes.Unknown)).Returns(_advancedParameter);
         A.CallTo(() => _createdPopulation.IndividualPropertiesCache.AllParameterPaths()).Returns(_allImportedParameters);
      }

      [Observation]
      public async Task should_have_added_the_parameter_as_advanced_parameters_in_the_population()
      {
         _population = await sut.CreateFor(new[] { _file1, _file2 }, _individual, new CancellationToken());
         A.CallTo(() => _population.AddAdvancedParameter(_advancedParameter, false)).MustHaveHappened();
      }
   }
}