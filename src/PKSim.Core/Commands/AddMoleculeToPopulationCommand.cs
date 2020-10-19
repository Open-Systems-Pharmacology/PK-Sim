using OSPSuite.Core.Commands.Core;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class AddMoleculeToPopulationCommand : AddEntityToContainerCommand<IndividualMolecule, Population,
      AddMoleculeToSimulationSubjectEvent<Population>>
   {
      public AddMoleculeToPopulationCommand(IndividualMolecule molecule, Population population, IExecutionContext context)
         : base(molecule, population, context, x => x.AddMolecule)
      {
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         base.PerformExecuteWith(context);
         context.PublishEvent(new AddAdvancedParameterContainerToPopulationEvent(_parentContainer));
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new RemoveMoleculeFromPopulationCommand(_entityToAdd, _parentContainer, context).AsInverseFor(this);
      }
   }
}