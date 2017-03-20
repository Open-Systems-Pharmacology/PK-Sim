using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class AddMoleculeExpressionsFromQueryToIndividualCommand : AddMoleculeExpressionsFromQueryToSimulationSubjectCommand<Individual>
   {
      public AddMoleculeExpressionsFromQueryToIndividualCommand(IndividualMolecule molecule, QueryExpressionResults queryExpressionResults, Individual individual) : 
         base(molecule, queryExpressionResults, individual)
      {
      }

      protected override ICommand AddMoleculeToSimulationSubjectCommand(IndividualMolecule molecule, Individual individual, IExecutionContext context)
      {
         return new AddMoleculeToIndividualCommand(molecule, individual, context);
      }
   }
}