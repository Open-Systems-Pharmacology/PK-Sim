using OSPSuite.Core.Commands.Core;
using PKSim.Core.Events;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class RemoveProcessFromCompoundCommand : RemoveEntityFromContainerCommand<PKSim.Core.Model.CompoundProcess, Compound, RemoveCompoundProcessEvent>
   {
      public RemoveProcessFromCompoundCommand(PKSim.Core.Model.CompoundProcess process, Compound compound, IExecutionContext context)
         : base(process, compound, context)
      {
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new AddProcessToCompoundCommand(_entityToRemove, _parentContainer, context).AsInverseFor(this);
      }
   }
}