using System.IO;
using System.Linq;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Populations;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Infrastructure.Import.Services;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_IndividualPropertiesCacheImporter : ContextForIntegration<IIndividualPropertiesCacheImporter>
   {
      protected IImportLogger _logger;
      protected PathCache<IParameter> _allParameters;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _logger = A.Fake<IImportLogger>();
         var individual = DomainFactoryForSpecs.CreateStandardIndividual();
         var containerTask = IoC.Resolve<IContainerTask>();
         _allParameters = containerTask.CacheAllChildren<IParameter>(individual);
      }

   }

   public class When_importing_a_population_from_a_file_that_was_saved_in_the_old_format_that_is_not_supported : concern_for_IndividualPropertiesCacheImporter
   {
      private IndividualValuesCache _results;

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
      private IndividualValuesCache _results;

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
      private IndividualValuesCache _results;

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

   public class When_importing_a_population_from_a_file_that_is_using_the_new_format_with_comment_but_with_old_entries_for_gender_and_race : concern_for_IndividualPropertiesCacheImporter
   {
      private IndividualValuesCache _results;

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
         var genotypeCovariate = _results.AllCovariateValuesFor("Genotype");
         genotypeCovariate.Count.ShouldBeEqualTo(50);
         genotypeCovariate.Distinct().ShouldOnlyContain("A", "B", "C");
      }

      [Observation]
      public void should_have_converted_the_old_gender_and_race_to_use_the_display_name()
      {
         var genderCovariates = _results.AllCovariateValuesFor(Constants.Population.GENDER);
         genderCovariates.Count.ShouldBeEqualTo(50);
         genderCovariates.Distinct().ShouldOnlyContain("Male", "Female");
         var raceCovariate = _results.AllCovariateValuesFor(Constants.Population.RACE);
         raceCovariate.Count.ShouldBeEqualTo(50);
         raceCovariate.Distinct().ShouldOnlyContain("European (ICRP, 2002)");

      }
   }



   public class When_importing_a_file_containing_path_with_units : concern_for_IndividualPropertiesCacheImporter
   {
      private IndividualValuesCache _results;


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
      private IndividualValuesCache _results;

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
      private IndividualValuesCache _results;
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