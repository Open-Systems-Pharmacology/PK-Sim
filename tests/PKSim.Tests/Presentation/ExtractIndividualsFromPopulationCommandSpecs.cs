using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.Core.Services;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.UICommands;

namespace PKSim.Presentation
{
   public abstract class concern_for_ExtractIndividualsFromPopulationCommand : ContextSpecification<ExtractIndividualsFromPopulationCommand>
   {
      protected IPopulationTask _populationTask;
      private IActiveSubjectRetriever _activeSubjectRetriever;

      protected override void Context()
      {
         _populationTask = A.Fake<IPopulationTask>();
         _activeSubjectRetriever = A.Fake<IActiveSubjectRetriever>();
         sut = new ExtractIndividualsFromPopulationCommand(_activeSubjectRetriever, _populationTask);
      }
   }

   public class When_executing_the_extract_individuals_from_population_command : concern_for_ExtractIndividualsFromPopulationCommand
   {
      private Population _population;

      protected override void Context()
      {
         base.Context();
         _population = new RandomPopulation();
         sut.Subject = _population;
      }

      protected override void Because()
      {
         sut.Execute();
      }

      [Observation]
      public void should_leverage_the_population_task_to_extract_individuals_from_the_selected_simulation()
      {
         A.CallTo(() => _populationTask.ExtractIndividuals(_population, null)).MustHaveHappened();
      }
   }
}