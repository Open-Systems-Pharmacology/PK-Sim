using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class AddMoleculeExpressionsFromQueryToPopulationCommand : AddMoleculeExpressionsFromQueryToSimulationSubjectCommand<Population>
   {
      public AddMoleculeExpressionsFromQueryToPopulationCommand(IndividualMolecule molecule, QueryExpressionResults queryExpressionResults, Population population)
         : base(molecule, queryExpressionResults, population)
      {
      }

      protected override ICommand AddMoleculeToSimulationSubjectCommand(IndividualMolecule molecule, Population population, IExecutionContext context)
      {
         return new AddMoleculeToPopulationCommand(molecule, population, context);
      }
   }
}