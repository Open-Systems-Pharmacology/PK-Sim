using OSPSuite.Core.Commands.Core;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class AddProcessToCompoundCommand : AddEntityToContainerCommand<CompoundProcess, Compound, AddCompoundProcessEvent>
   {
      public AddProcessToCompoundCommand(CompoundProcess process, Compound compound, IExecutionContext context)
         : base(process, compound, context)
      {
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new RemoveProcessFromCompoundCommand(_entityToAdd, _parentContainer, context).AsInverseFor(this);
      }
   }
}