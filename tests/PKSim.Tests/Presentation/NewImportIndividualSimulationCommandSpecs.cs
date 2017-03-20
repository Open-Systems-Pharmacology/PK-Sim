using OSPSuite.Core.Services;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Services;
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation
{
   public abstract class concern_for_NewImportIndividualSimulationCommand : ContextSpecification<NewImportIndividualSimulationCommand>
   {
      protected IDialogCreator _dialogCreator;
      protected IImportSimulationTask _importSimulationTask;
      protected ISimulationTask _simulationTask;

      protected override void Context()
      {
         _dialogCreator= A.Fake<IDialogCreator>();
         _importSimulationTask= A.Fake<IImportSimulationTask>();
         _simulationTask= A.Fake<ISimulationTask>();
         sut = new NewImportIndividualSimulationCommand(_importSimulationTask,_simulationTask,_dialogCreator);
      }

      protected override void Because()
      {
         sut.Execute();
      }
   }

   public class When_importing_an_individual_simulation_and_the_import_is_canceled : concern_for_NewImportIndividualSimulationCommand
   {
      protected override void Context()
      {
         base.Context();
         A.CallTo(_dialogCreator).WithReturnType<string>().Returns("file");
         A.CallTo(() => _importSimulationTask.ImportIndividualSimulation("file")).Returns(null);
      }
   
      [Observation]
      public void should_not_add_the_simulation_to_the_project()
      {
         A.CallTo(() => _simulationTask.AddToProject(A<Simulation>._,A<bool>._)).MustNotHaveHappened();
      }
   }

   public class When_importing_an_individual_simulation_and_the_import_is_successfull : concern_for_NewImportIndividualSimulationCommand
   {
      private IndividualSimulation _importedSimulation;

      protected override void Context()
      {
         base.Context();
         _importedSimulation = A.Fake<IndividualSimulation>();
         A.CallTo(_dialogCreator).WithReturnType<string>().Returns("file");
         A.CallTo(() => _importSimulationTask.ImportIndividualSimulation("file")).Returns(_importedSimulation);
      }

      [Observation]
      public void should_add_the_simulation_to_the_project()
      {
         A.CallTo(() => _simulationTask.AddToProject(_importedSimulation, true)).MustHaveHappened();
      }
   }
}	