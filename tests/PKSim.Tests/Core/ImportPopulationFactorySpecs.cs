using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_ImportPopulationFactory : ContextSpecificationAsync<IImportPopulationFactory>
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
      protected readonly PathCache<IParameter> _allParameters = new PathCache<IParameter>(new EntityPathResolverForSpecs());
      protected readonly PathCache<IParameter> _allCreateIndividualParameters = new PathCache<IParameter>(new EntityPathResolverForSpecs());

      protected override Task Context()
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

         A.CallTo(() => _containerTask.CacheAllChildren<IParameter>(_cloneIndividual)).Returns(_allParameters);
         A.CallTo(() => _containerTask.CacheAllChildrenSatisfying(_cloneIndividual, A<Func<IParameter, bool>>._)).Returns(_allCreateIndividualParameters);

         _popFile1 = A.Fake<IndividualPropertiesCache>();
         _popFile2 = A.Fake<IndividualPropertiesCache>();
         A.CallTo(() => _individualCacheImporter.ImportFrom(_file1, _allParameters, A<IImportLogger>._)).Returns(_popFile1);
         A.CallTo(() => _individualCacheImporter.ImportFrom(_file2, _allParameters, A<IImportLogger>._)).Returns(_popFile2);

         return _completed;
      }
   }

   public class When_told_to_create_a_import_population_based_on_some_files : concern_for_ImportPopulationFactory
   {
      protected override async Task Because()
      {
         _population = await sut.CreateFor(new[] {_file1, _file2}, _individual, new CancellationToken());
      }

      [Observation]
      public void should_have_created_a_population_with_the_clone_of_the_base_individual()
      {
         _population.Settings.BaseIndividual.ShouldBeEqualTo(_cloneIndividual);
      }

      [Observation]
      public void should_return_a_population_containing_the_individuals_defined_in_these_files()
      {
         A.CallTo(() => _population.IndividualPropertiesCache.Merge(_popFile2, _allParameters)).MustHaveHappened();
      }
   }

   public class When_creating_a_population_from_files_containing_advanced_parameters : concern_for_ImportPopulationFactory
   {
      private readonly List<string> _allImportedParameters = new List<string>();
      private AdvancedParameter _advancedParameter;

      protected override async Task Context()
      {
         await base.Context();
         _allImportedParameters.AddRange(new[] {"P1", "P2"});
         _allParameters.Add("P1", new PKSimParameter().WithName("P1"));
         _allParameters.Add("P2", new PKSimParameter().WithName("P2"));
         _allCreateIndividualParameters.Add("P1", _allParameters.FindByName("P1"));
         _advancedParameter = new AdvancedParameter();
         A.CallTo(() => _advancedParameterFactory.Create(_allParameters.FindByName("P2"), DistributionTypes.Unknown)).Returns(_advancedParameter);
         A.CallTo(() => _createdPopulation.IndividualPropertiesCache.AllParameterPaths()).Returns(_allImportedParameters.ToArray());
      }

      protected override async Task Because()
      {
         _population = await sut.CreateFor(new[] {_file1, _file2}, _individual, new CancellationToken());
      }

      [Observation]
      public void should_have_added_the_parameter_as_advanced_parameters_in_the_population()
      {
         A.CallTo(() => _population.AddAdvancedParameter(_advancedParameter, false)).MustHaveHappened();
      }
   }
}