using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using OSPSuite.Core.Domain;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Core
{
   public abstract class concern_for_RemoveMoleculeFromIndividualCommand : ContextSpecification<RemoveMoleculeFromIndividualCommand>
   {
      protected IndividualProtein _protein;
      protected IExecutionContext _context;
      protected Individual _individual;
      private Organism _organism;
      protected Container _localMoleculeContainer;
      protected Organ _liver;

      protected override void Context()
      {
         _protein = new IndividualOtherProtein().WithName("CYP3A4");
         _context = A.Fake<IExecutionContext>();
         _organism = new Organism();
         _liver = new Organ().WithName("liver").WithParentContainer(_organism);
         _localMoleculeContainer = new Container().WithName(_protein.Name).WithParentContainer(_liver);
         _individual = new Individual {_organism};
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
         _liver.Container(_localMoleculeContainer.Name).ShouldBeNull();
      }
   }
}