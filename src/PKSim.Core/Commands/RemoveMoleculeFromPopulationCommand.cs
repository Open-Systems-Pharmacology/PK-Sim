using OSPSuite.Core.Commands.Core;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class RemoveMoleculeFromPopulationCommand : RemoveEntityFromContainerCommand<IndividualMolecule, Population, RemoveMoleculeFromSimulationSubjectEvent<Population>> 
   {
      public RemoveMoleculeFromPopulationCommand(IndividualMolecule molecule, Population population, IExecutionContext context) :
         base(molecule, population, context, x => x.RemoveMolecule)
      {
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         base.PerformExecuteWith(context);
         context.PublishEvent(new RemoveAdvancedParameterContainerFromPopulationEvent(_parentContainer));
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new AddMoleculeToPopulationCommand(_entityToRemove, _parentContainer, context).AsInverseFor(this);
      }
   }
}