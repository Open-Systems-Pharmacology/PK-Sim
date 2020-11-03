using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class EditIndividualMoleculeExpressionInPopulationFromQueryCommand: EditIndividualMoleculeExpressionInSimulationSubjectFromQueryCommand<Population>
   {
      public EditIndividualMoleculeExpressionInPopulationFromQueryCommand(IndividualMolecule originalMolecule, IndividualMolecule editedMolecule, QueryExpressionResults queryExpressionResults,
         Population population) : base(originalMolecule, editedMolecule, queryExpressionResults, population)
      {
      }

      protected override ICommand RemoveMoleculeFromSimulationSubjectCommand(IndividualMolecule molecule, Population population, IExecutionContext context)
      {
         return new RemoveMoleculeFromPopulationCommand(molecule, population, context);
      }

      protected override ICommand AddMoleculeToSimulationSubjectCommand(IndividualMolecule molecule, Population population, IExecutionContext context)
      {
         return new AddMoleculeToPopulationCommand(molecule, population, context);
      }
   }
}