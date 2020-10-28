using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using FakeItEasy;
using PKSim.Core.Commands;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_RemoveMoleculeFromPopulationCommand : ContextSpecification<RemoveMoleculeFromPopulationCommand>
   {
      protected IndividualProtein _protein;
      protected Individual _individual;
      protected IExecutionContext _context;
      protected RandomPopulation _population;

      protected override void Context()
      {
         _protein = new IndividualEnzyme();
         _population = new RandomPopulation {new AdvancedParameterCollection()};
         _population.Settings = new RandomPopulationSettings {BaseIndividual = new Individual()};
         _population.AddMolecule(_protein);
         _context = A.Fake<IExecutionContext>();

         sut = new RemoveMoleculeFromPopulationCommand(_protein, _population, _context);
      }
   }

   public class When_executing_the_remove_molecule_from_population_command : concern_for_RemoveMoleculeFromPopulationCommand
   {
      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void should_remove_the_molecule_from_the_population()
      {
         _population.AllMolecules().ShouldNotContain(_protein);
      }

      [Observation]
      public void should_raise_the_advanced_parameter_container_remove_event()
      {
         A.CallTo(() => _context.PublishEvent(A<RemoveAdvancedParameterContainerFromPopulationEvent>._)).MustHaveHappened();
      }
   }
}