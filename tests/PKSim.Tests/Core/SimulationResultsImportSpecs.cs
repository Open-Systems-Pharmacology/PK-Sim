using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_SimulationResultsImport : ContextSpecification<SimulationResultsImport>
   {
      protected override void Context()
      {
         sut = new SimulationResultsImport();
      }
   }

   public class When_adding_an_error_to_a_simulation_results_import : concern_for_SimulationResultsImport
   {
      protected override void Because()
      {
         sut.AddError("TOTO");
         sut.AddError("TATA");
      }

      [Observation]
      public void the_import_should_have_the_error_status()
      {
         sut.HasError.ShouldBeTrue();
         sut.Status.ShouldBeEqualTo(NotificationType.Error);
      }

      [Observation]
      public void the_messages_should_contain_all_errors_added_during_the_import()
      {
         sut.Log.ShouldContain("Error: TOTO", "Error: TATA");
      }
   }

   public class When_retrieving_the_status_of_a_simulation_result_import : concern_for_SimulationResultsImport
   {
      private SimulationResultsFile _simulationResultFile;

      protected override void Context()
      {
         base.Context();
         sut.AddInfo("TOTO");
         _simulationResultFile = new SimulationResultsFile();
         sut.SimulationResultsFiles.Add(_simulationResultFile);
      }

      [Observation]
      public void should_combine_the_status_with_the_one_of_the_imported_simulation_result_files()
      {
         _simulationResultFile.AddWarning("Warning");
         sut.HasError.ShouldBeFalse();
         sut.Status.Is(NotificationType.Warning).ShouldBeTrue();
      }

      [Observation]
      public void the_messages_should_contain_all_errors_added_during_the_import()
      {
         _simulationResultFile.AddError("Error");
         sut.HasError.ShouldBeTrue();
      }
   }
}