using OSPSuite.BDDHelper;
using PKSim.Core.Services;
using PKSim.Presentation.UICommands;
using FakeItEasy;

namespace PKSim.Presentation
{
   public abstract class concern_for_ImportObservedDataCommmand : ContextSpecification<ImportConcentrationDataCommmand>
   {
      protected IImportObservedDataTask _observedDataTask;

      protected override void Context()
      {
         _observedDataTask = A.Fake<IImportObservedDataTask>();
         sut = new ImportConcentrationDataCommmand(_observedDataTask);
      }
   }

   
   public class When_executing_the_import_observed_data_command : concern_for_ImportObservedDataCommmand
   {
      protected override void Because()
      {
         sut.Execute();
      }

      [Observation]
      public void should_add_the_selected_observed_data_to_the_current_project()
      {
         A.CallTo(() => _observedDataTask.AddConcentrationDataToProject()).MustHaveHappened();
      }
   }
}