using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class EditIndividualMoleculeExpressionInIndividualFromQueryCommand : EditIndividualMoleculeExpressionInSimulationSubjectFromQueryCommand<
      Individual>
   {
      public EditIndividualMoleculeExpressionInIndividualFromQueryCommand(IndividualMolecule originalMolecule, IndividualMolecule editedMolecule,
         QueryExpressionResults queryExpressionResults,
         Individual individual) : base(originalMolecule, editedMolecule, queryExpressionResults, individual)
      {
      }

      protected override ICommand RemoveMoleculeFromSimulationSubjectCommand(IndividualMolecule molecule, Individual individual,
         IExecutionContext context)
      {
         return new RemoveMoleculeFromIndividualCommand(molecule, individual, context);
      }

      protected override ICommand AddMoleculeToSimulationSubjectCommand(IndividualMolecule molecule, Individual individual, IExecutionContext context)
      {
         return new AddMoleculeToIndividualCommand(molecule, individual, context);
      }
   }
}