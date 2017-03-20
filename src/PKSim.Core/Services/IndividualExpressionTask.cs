using OSPSuite.Core.Commands.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public class IndividualExpressionTask : ISimulationSubjectExpressionTask<Individual>
   {
      private readonly IExecutionContext _executionContext;

      public IndividualExpressionTask(IExecutionContext executionContext)
      {
         _executionContext = executionContext;
      }

      public ICommand RemoveMoleculeFrom(IndividualMolecule molecule, Individual individual)
      {
         return new RemoveMoleculeFromIndividualCommand(molecule, individual, _executionContext).Run(_executionContext);
      }

      public ICommand AddMoleculeTo(IndividualMolecule molecule, Individual individual)
      {
         return new AddMoleculeToIndividualCommand(molecule, individual, _executionContext).Run(_executionContext);
      }

      public ICommand EditMolecule(IndividualMolecule moleculeToEdit, IndividualMolecule editedMolecule, QueryExpressionResults queryResults, Individual individual)
      {
         return new EditIndividualMoleculeExpressionInIndividualFromQueryCommand(moleculeToEdit, editedMolecule, queryResults, individual).Run(_executionContext);
      }

      public ICommand AddMoleculeTo(IndividualMolecule molecule, Individual individual, QueryExpressionResults queryExpressionResults)
      {
         return new AddMoleculeExpressionsFromQueryToIndividualCommand(molecule, queryExpressionResults, individual).Run(_executionContext);
      }
   }
}