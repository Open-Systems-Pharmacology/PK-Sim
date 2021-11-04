using PKSim.Presentation.Services;
using PKSim.Presentation.UICommands;
using FakeItEasy;
using OSPSuite.BDDHelper;

namespace PKSim.Presentation
{
   public abstract class concern_for_NewFormulationCommand : ContextSpecification<AddFormulationCommand>
   {
      protected IFormulationTask _formulationTask;

      protected override void Context()
      {
         _formulationTask =A.Fake<IFormulationTask>();
         sut = new AddFormulationCommand(_formulationTask);
      }
   }

   
   public class When_asked_to_create_a_new_formulation : concern_for_NewFormulationCommand
   {
      protected override void Because()
      {
         sut.Execute();
      }

      [Observation]
      public void should_delegate_to_the_formulation_task_to_create_a_new_formulation()
      {
         A.CallTo(() => _formulationTask.AddToProject()).MustHaveHappened();   
      }
   }
}	