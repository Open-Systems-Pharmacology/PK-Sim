using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using FakeItEasy;
using PKSim.Core.Commands;
using PKSim.Core.Model;


namespace PKSim.Core
{
   public abstract class concern_for_AddMoleculeToIndividualCommand : ContextSpecification<AddMoleculeToIndividualCommand>
   {
      protected IndividualProtein _protein;
      protected Individual _individual;
      protected IExecutionContext _context;

      protected override void Context()
      {
         _protein = new IndividualOtherProtein();
         _individual = new Individual();
         _context = A.Fake<IExecutionContext>();
         sut = new AddMoleculeToIndividualCommand(_protein, _individual, _context);
      }
   }

   public class When_executing_the_add_protein_expression_to_individual_command : concern_for_AddMoleculeToIndividualCommand
   {
      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void should_add_the_protein_to_the_individual()
      {
         _individual.AllMolecules().ShouldContain(_protein);
      }
   }

   public class The_inverse_of_the_add_protein_expression_to_individual_command : concern_for_AddMoleculeToIndividualCommand
   {
      private ICommand<IExecutionContext> _result;

      protected override void Because()
      {
         _result = sut.InverseCommand(_context);
      }

      [Observation]
      public void should_be_a_remove_protein_expression_to_individual_command()
      {
         _result.ShouldBeAnInstanceOf<RemoveMoleculeFromIndividualCommand>();
      }

      [Observation]
      public void should_have_been_marked_as_inverse_for_the_add_command()
      {
         _result.IsInverseFor(sut).ShouldBeTrue();
      }
   }
}