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
         //we register the whole population again as sub container for the molecule were added to the population
         context.Register(_parentContainer);
         context.PublishEvent(new AddAdvancedParameterContainerToPopulationEvent(_parentContainer));
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new RemoveMoleculeFromPopulationCommand(_entityToAdd, _parentContainer, context).AsInverseFor(this);
      }
   }
}
