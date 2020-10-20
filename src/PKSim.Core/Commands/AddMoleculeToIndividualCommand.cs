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

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         //TODO Implement 
         // https: //github.com/Open-Systems-Pharmacology/OSPSuite.Core/issues/831
         // return new RemoveMoleculeFromIndividualCommand(_entityToAdd, _parentContainer, context).AsInverseFor(this);
         return new PKSimMacroCommand();
      }
   }
}