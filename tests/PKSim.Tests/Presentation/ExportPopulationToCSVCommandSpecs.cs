using OSPSuite.BDDHelper;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation
{
   public abstract class concern_for_ExportPopulationToCSVCommand : ContextSpecification<ExportPopulationToCSVCommand>
   {
      protected IPopulationExportTask _populationExportTask;

      protected override void Context()
      {
         _populationExportTask = A.Fake<IPopulationExportTask>();
         sut = new ExportPopulationToCSVCommand(_populationExportTask);
      }
   }

   public class When_executing_the_export_population_to_csv_command : concern_for_ExportPopulationToCSVCommand
   {
      private Population _population;

      protected override void Context()
      {
         base.Context();
         _population = A.Fake<Population>();
         sut.For(_population);
      }

      protected override void Because()
      {
         sut.Execute();
      }

      [Observation]
      public void should_leverage_the_export_task_to_export_all_parameters_of_the_given_population_to_the_csv_file()
      {
         A.CallTo(() => _populationExportTask.ExportToCSV(_population)).MustHaveHappened();
      }
   }
}