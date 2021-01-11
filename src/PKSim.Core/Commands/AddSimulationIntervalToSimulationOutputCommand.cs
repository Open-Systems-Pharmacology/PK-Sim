using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Events;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Commands
{
   public class AddSimulationIntervalToSimulationOutputCommand : AddEntityToContainerCommand<OutputInterval, OutputSchema, AddOutputIntervalToOutputSchemaEvent>
   {
      public AddSimulationIntervalToSimulationOutputCommand(OutputInterval simulationInterval, OutputSchema simulationOutput, IExecutionContext context)
         : base(simulationInterval, simulationOutput, context)
      {
         Description = PKSimConstants.Command.AddSimulationIntervalToSimulationOutputDescription;
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new RemoveSimulationIntervalFromSimulationOutputCommand(_entityToAdd, _parentContainer, context).AsInverseFor(this);
      }
   }
}