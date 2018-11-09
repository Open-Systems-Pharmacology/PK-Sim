using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Services;
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation
{
   public abstract class concern_for_ImportAmountDataCommmand : ContextSpecification<ImportAmountDataCommmand>
   {
      protected IImportObservedDataTask _observedDataTask;

      protected override void Context()
      {
         _observedDataTask = A.Fake<IImportObservedDataTask>();
         sut = new ImportAmountDataCommmand(_observedDataTask);
      }
   }

   public class When_executing_the_import_amount_data_command : concern_for_ImportAmountDataCommmand
   {
      protected override void Because()
      {
         sut.Execute();
      }

      [Observation]
      public void should_add_the_selected_amount_data_to_the_current_project()
      {
         A.CallTo(() => _observedDataTask.AddAmountDataToProject()).MustHaveHappened();
      }
   }
}	