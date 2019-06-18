using System.IO;
using System.Linq;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure.Services;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_IndividualPropertiesCacheImporter : ContextSpecification<IIndividualPropertiesCacheImporter>
   {
      private IGenderRepository _genderRepository;
      protected IImportLogger _logger;
      private IPopulationRepository _populationRepository;
      protected Gender _male = new Gender();
      protected Gender _female = new Gender();
      protected SpeciesPopulation _population = new SpeciesPopulation();
      protected PathCache<IParameter> _allParameters;

      protected override void Context()
      {
         _genderRepository = A.Fake<IGenderRepository>();
         A.CallTo(() => _genderRepository.FindByIndex(1)).Returns(_male);
         A.CallTo(() => _genderRepository.FindByIndex(2)).Returns(_female);
         _populationRepository = A.Fake<IPopulationRepository>();
         A.CallTo(() => _populationRepository.FindByIndex(0)).Returns(_population);
         _logger = A.Fake<IImportLogger>();
         sut = new IndividualPropertiesCacheImporter(_genderRepository, _populationRepository);
         _allParameters = A.Fake<PathCache<IParameter>>();
      }
   }

   public class When_importing_a_population_from_a_file_that_was_saved_in_the_old_format_that_is_not_supported : concern_for_IndividualPropertiesCacheImporter
   {
      private IndividualPropertiesCache _results;

      protected override void Because()
      {
         _results = sut.ImportFrom(DomainHelperForSpecs.PopulationFilePathFor("pop_10_old_format"), _allParameters, _logger);
      }

      [Observation]
      public void should_not_have_imported_any_individuals()
      {
         _results.Count.ShouldBeEqualTo(0);
      }
   }

   public class When_importing_a_population_from_a_file_that_has_the_expected_format_different_delimiters : concern_for_IndividualPropertiesCacheImporter
   {
      [TestCase("tab")]
      public void should_have_imported_the_individuals(string delimiter)
      {
         var result = sut.ImportFrom(DomainHelperForSpecs.PopulationFilePathFor("new_format_with_comment_" + delimiter), _allParameters, _logger);
         result.Count.ShouldBeEqualTo(50);
      }
   }

   public class When_importing_a_population_from_a_file_that_is_using_the_old_format_with_semi_colon : concern_for_IndividualPropertiesCacheImporter
   {
      private IndividualPropertiesCache _results;

      protected override void Because()
      {
         _results = sut.ImportFrom(DomainHelperForSpecs.PopulationFilePathFor("pop_10_semi_colon"), _allParameters, _logger);
      }

      [Observation]
      public void should_have_imported_the_individuals()
      {
         _results.Count.ShouldBeEqualTo(10);
      }
   }

   public class When_importing_a_population_from_a_file_that_is_corrupted : concern_for_IndividualPropertiesCacheImporter
   {
      private IndividualPropertiesCache _results;

      protected override void Because()
      {
         _results = sut.ImportFrom(DomainHelperForSpecs.PopulationFilePathFor("corrupt1"), _allParameters, _logger);
      }

      [Observation]
      public void should_not_have_imported_any_individuals()
      {
         _results.Count.ShouldBeEqualTo(0);
      }

      [Observation]
      public void should_have_added_some_log_info_explaining_why_the_file_is_corrupted()
      {
         A.CallTo(() => _logger.AddToLog(A<string>._, LogLevel.Error, A<string>._)).MustHaveHappened();
      }
   }

   public class When_importing_a_population_from_a_file_that_is_using_the_new_format_with_comment : concern_for_IndividualPropertiesCacheImporter
   {
      private IndividualPropertiesCache _results;

      protected override void Because()
      {
         _results = sut.ImportFrom(DomainHelperForSpecs.PopulationFilePathFor("new_format_with_comment"), _allParameters, _logger);
      }

      [Observation]
      public void should_have_imported_the_individuals()
      {
         _results.Count.ShouldBeEqualTo(50);
      }

      [Observation]
      public void should_have_imported_the_covariates()
      {
         var genotypeCovariate = _results.AllCovariates.Select(x => x.Covariate("Genotype")).ToList();
         genotypeCovariate.Count.ShouldBeEqualTo(50);
         genotypeCovariate.Distinct().ShouldOnlyContain("A", "B", "C");
      }
   }

   public class When_importing_a_file_containing_path_with_units : concern_for_IndividualPropertiesCacheImporter
   {
      private IndividualPropertiesCache _results;

      protected override void Context()
      {
         base.Context();
         A.CallTo(() => _allParameters.Contains("Organism|VenousBlood|Volume [l]")).Returns(false);
         A.CallTo(() => _allParameters.Contains("Organism|VenousBlood|Volume")).Returns(true);
      }

      protected override void Because()
      {
         _results = sut.ImportFrom(DomainHelperForSpecs.PopulationFilePathFor("path_with_units"), _allParameters, _logger);
      }

      [Observation]
      public void should_have_imported_the_individuals()
      {
         _results.Count.ShouldBeEqualTo(3);
      }

      [Observation]
      public void should_remove_units_when_path_is_not_found()
      {
         _results.AllParameterPaths().Contains("Organism|VenousBlood|Volume").ShouldBeTrue();
         _results.AllParameterPaths().Contains("Organism|VenousBlood|Volume [l]").ShouldBeFalse();
         _results.ParameterValuesCache.Has("Organism|VenousBlood|Volume [l]").ShouldBeFalse();
         _results.ParameterValuesCache.Has("Organism|VenousBlood|Volume").ShouldBeTrue();
      }
   }

   public class When_importing_a_population_from_a_file_that_contains_junk : concern_for_IndividualPropertiesCacheImporter
   {
      private IndividualPropertiesCache _results;

      protected override void Because()
      {
         _results = sut.ImportFrom(DomainHelperForSpecs.PopulationFilePathFor("junk"), _allParameters, _logger);
      }

      [Observation]
      public void should_not_have_imported_any_individuals()
      {
         _results.Count.ShouldBeEqualTo(0);
      }

      [Observation]
      public void should_have_added_some_log_info_explaining_why_the_file_is_corrupted()
      {
         A.CallTo(() => _logger.AddToLog(A<string>._, LogLevel.Error, A<string>._)).MustHaveHappened();
      }
   }

   public class When_importing_a_file_that_is_already_open : concern_for_IndividualPropertiesCacheImporter
   {
      private IndividualPropertiesCache _results;
      private string _fileName;
      private FileStream _fileStream;

      protected override void Context()
      {
         base.Context();
         _fileName = DomainHelperForSpecs.PopulationFilePathFor("pop_3");
         _fileStream = File.Open(_fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
      }

      protected override void Because()
      {
         _results = sut.ImportFrom(_fileName, _allParameters, _logger);
      }

      [Observation]
      public void should_be_able_to_import_the_individuals()
      {
         _results.Count.ShouldBeEqualTo(3);
      }

      public override void Cleanup()
      {
         _fileStream.Close();
         base.Cleanup();
      }
   }
}