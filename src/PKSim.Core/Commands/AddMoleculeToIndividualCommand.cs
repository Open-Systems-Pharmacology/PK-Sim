using OSPSuite.Core.Commands.Core;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class AddMoleculeToIndividualCommand : AddEntityToContainerCommand<IndividualMolecule, Individual, AddMoleculeToSimulationSubjectEvent<Individual>>
   {
      public AddMoleculeToIndividualCommand(IndividualMolecule molecule, Individual individual, IExecutionContext context)
         : base(molecule, individual, context)
      {
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         base.PerformExecuteWith(context);
         //we register the whole individual again as sub container for the molecule were added to the individual
         context.Register(_parentContainer);
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new RemoveMoleculeFromIndividualCommand(_entityToAdd, _parentContainer, context).AsInverseFor(this);
      }
   }
}