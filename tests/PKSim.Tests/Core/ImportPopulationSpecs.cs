using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_ImportPopulation : ContextSpecification<ImportPopulation>
   {
      protected PopulationFile _goodFile;
      protected PopulationFile _warningFile;
      protected PopulationFile _errorFile;
      protected PopulationFile _debugFile;

      protected override void Context()
      {
         _goodFile = new PopulationFile();
         _goodFile.AddInfo("Info");
         _warningFile = new PopulationFile();
         _warningFile.AddWarning("Warning");
         _errorFile = new PopulationFile();
         _errorFile.AddError("Error");

         _debugFile = new PopulationFile();
         _debugFile.AddDebug("Debug");
         sut = new ImportPopulation();
      }
   }

   public class When_checking_if_a_import_population_was_successfully_imported : concern_for_ImportPopulation
   {
      [Observation]
      public void should_return_false_if_no_file_was_imported()
      {
         sut.ImportSuccessful.ShouldBeFalse();
      }

      [Observation]
      public void should_return_true_if_the_imported_files_contain_no_error()
      {
         sut.Settings.AddFile(_goodFile);
         sut.ImportSuccessful.ShouldBeTrue();
      }

      [Observation]
      public void should_return_true_if_the_imported_files_contain_only_files_with_warnings_or_no_error()
      {
         sut.Settings.AddFile(_goodFile);
         sut.Settings.AddFile(_warningFile);
         sut.ImportSuccessful.ShouldBeTrue();
      }

      [Observation]
      public void should_return_true_if_the_imported_files_contain_only_files_with_warnings()
      {
         sut.Settings.AddFile(_warningFile);
         sut.ImportSuccessful.ShouldBeTrue();
      }

      [Observation]
      public void should_return_true_if_the_imported_files_contain_a_file_with_error()
      {
         sut.Settings.AddFile(_warningFile);
         sut.Settings.AddFile(_errorFile);
         sut.ImportSuccessful.ShouldBeFalse();
      }

      [Observation]
      public void should_return_true_if_the_imported_files_contain_all_have_errors()
      {
         sut.Settings.AddFile(_errorFile);
         sut.ImportSuccessful.ShouldBeFalse();
      }
   }

   public class When_checking_if_the_imported_population_has_error_or_warnings : concern_for_ImportPopulation
   {
      [Observation]
      public void should_return_true_if_one_file_has_error()
      {
         sut.Settings.AddFile(_errorFile);
         sut.ImportHasWarningOrError.ShouldBeTrue();
      }

      [Observation]
      public void should_return_true_if_one_file_has_warnings()
      {
         sut.Settings.AddFile(_warningFile);
         sut.ImportHasWarningOrError.ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_if_no_file_has_error_or_warning()
      {
         sut.Settings.AddFile(_goodFile);
         sut.Settings.AddFile(_debugFile);
         sut.ImportHasWarningOrError.ShouldBeFalse();
      }
   }
}