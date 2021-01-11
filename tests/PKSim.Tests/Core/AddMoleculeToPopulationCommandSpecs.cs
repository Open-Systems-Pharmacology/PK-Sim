using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using FakeItEasy;
using PKSim.Core.Commands;
using PKSim.Core.Events;
using PKSim.Core.Model;


namespace PKSim.Core
{
   public abstract class concern_for_AddMoleculeToPopulationCommand : ContextSpecification<AddMoleculeToPopulationCommand>
   {
      protected IndividualProtein _protein;
      protected IExecutionContext _context;
      protected RandomPopulation _population;

      protected override void Context()
      {
         _protein = new IndividualOtherProtein();
         _population = new RandomPopulation {new AdvancedParameterCollection()};
         _population.Settings = new RandomPopulationSettings {BaseIndividual = new Individual()};
         _context = A.Fake<IExecutionContext>();
         sut = new AddMoleculeToPopulationCommand(_protein, _population, _context);
      }
   }

   public class When_executing_the_add_molecule_to_population_command : concern_for_AddMoleculeToPopulationCommand
   {
      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void should_add_the_molecule_to_the_population()
      {
         _population.AllMolecules().ShouldContain(_protein);
      }

      [Observation]
      public void should_raise_the_advanced_parameter_container_added_event()
      {
         A.CallTo(() => _context.PublishEvent(A<AddAdvancedParameterContainerToPopulationEvent>._)).MustHaveHappened();
      }
   }

   public class The_inverse_of_the_add_molecule_to_population_command : concern_for_AddMoleculeToPopulationCommand
   {
      private ICommand<IExecutionContext> _result;

      protected override void Because()
      {
         _result = sut.InverseCommand(_context);
      }

      [Observation]
      public void should_be_a_remove_molecule_from_population_command()
      {
         _result.ShouldBeAnInstanceOf<RemoveMoleculeFromPopulationCommand>();
      }

      [Observation]
      public void should_have_been_marked_as_inverse_for_the_add_command()
      {
         _result.IsInverseFor(sut).ShouldBeTrue();
      }
   }
}