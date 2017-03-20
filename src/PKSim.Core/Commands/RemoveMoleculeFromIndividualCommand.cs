using OSPSuite.Core.Commands.Core;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class RemoveMoleculeFromIndividualCommand : RemoveEntityFromContainerCommand<IndividualMolecule, Individual, RemoveMoleculeFromSimulationSubjectEvent<Individual>>
   {
      public RemoveMoleculeFromIndividualCommand(IndividualMolecule molecule, Individual individual, IExecutionContext context) :
         base(molecule, individual, context)
      {
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new AddMoleculeToIndividualCommand(_entityToRemove, _parentContainer, context).AsInverseFor(this);
      }
   }
}