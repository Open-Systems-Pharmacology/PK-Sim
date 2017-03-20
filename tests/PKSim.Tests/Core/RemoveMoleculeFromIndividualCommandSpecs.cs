using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Commands.Core;
using FakeItEasy;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_RemoveMoleculeFromIndividualCommand : ContextSpecification<RemoveMoleculeFromIndividualCommand>
   {
      protected IndividualProtein _protein;
      protected IExecutionContext _context;
      protected Individual _individual;

      protected override void Context()
      {
         _protein = A.Fake<IndividualProtein>();
         _context = A.Fake<IExecutionContext>();
         _individual = new Individual();
         _individual.AddMolecule(_protein);
         sut = new RemoveMoleculeFromIndividualCommand(_protein, _individual, _context);
      }
   }

   public class When_executing_the_remove_protein_expression_from_individual_command : concern_for_RemoveMoleculeFromIndividualCommand
   {
      protected override void Because()
      {
         sut.Execute(_context);
      }

      [Observation]
      public void should_remove_the_expression_from_the_individual()
      {
         _individual.AllMolecules().Contains(_protein).ShouldBeFalse();
      }
   }

   public class The_inverse_of_the_remove_protein_expression_from_individual_command : concern_for_RemoveMoleculeFromIndividualCommand
   {
      private IReversibleCommand<IExecutionContext> _result;

      protected override void Because()
      {
         _result = sut.InverseCommand(_context);
      }

      [Observation]
      public void should_be_an_add_protein_expression_to_individual_command()
      {
         _result.ShouldBeAnInstanceOf<AddMoleculeToIndividualCommand>();
      }

      [Observation]
      public void should_have_beeen_marked_as_inverse_for_the_add_command()
      {
         _result.IsInverseFor(sut).ShouldBeTrue();
      }
   }
}