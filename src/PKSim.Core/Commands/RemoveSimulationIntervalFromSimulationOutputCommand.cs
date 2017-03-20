using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Events;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Commands
{
   public class RemoveSimulationIntervalFromSimulationOutputCommand : RemoveEntityFromContainerCommand<OutputInterval, OutputSchema, RemoveOutputIntervalFromOutputIntervalEvent>
   {
      public RemoveSimulationIntervalFromSimulationOutputCommand(OutputInterval simulationInterval, OutputSchema simulationOutput, IExecutionContext context)
         : base(simulationInterval, simulationOutput, context)
      {
         Description = PKSimConstants.Command.RemoveSimulationIntervalFromSimulationOutputDescription;
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new AddSimulationIntervalToSimulationOutputCommand(_entityToRemove, _parentContainer, context).AsInverseFor(this);
      }
   }
}