using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation
{
   public abstract class concern_for_AddAmountObservedDataForCompoundUICommand : ContextSpecification<AddAmountObservedDataForCompoundUICommand>
   {
      protected IImportObservedDataTask _observedDataTask;
      protected Compound _compound;

      protected override void Context()
      {
         _observedDataTask = A.Fake<IImportObservedDataTask>();
         _compound = new Compound();
         sut = new AddAmountObservedDataForCompoundUICommand(_observedDataTask) {Subject = _compound};
      }
   }


   public class When_executing_the_import_amount_data_for_a_selected_compound_command : concern_for_AddAmountObservedDataForCompoundUICommand
   {
      protected override void Because()
      {
         sut.Execute();
      }

      [Observation]
      public void should_add_the_selected_amount_data_for_the_compound_to_the_current_project()
      {
         A.CallTo(() => _observedDataTask.AddAmountDataToProjectForCompound(_compound)).MustHaveHappened();
      }
   }
}	