using System.Linq;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using IOutputIntervalFactory = PKSim.Core.Model.IOutputIntervalFactory;

namespace PKSim.Core.Services
{
   public interface ISimulationSettingsTask
   {
      ICommand AddSimulationIntervalTo(OutputSchema outputSchema);
      ICommand RemoveSimulationIntervalFrom(OutputInterval simulationInterval, OutputSchema simulationOutput);
   }

   public class SimulationSettingsTask : ISimulationSettingsTask
   {
      private readonly IExecutionContext _executionContext;
      private readonly IOutputIntervalFactory _outputIntervalFactory;

      public SimulationSettingsTask(IExecutionContext executionContext, IOutputIntervalFactory outputIntervalFactory)
      {
         _executionContext = executionContext;
         _outputIntervalFactory = outputIntervalFactory;
      }

      public ICommand AddSimulationIntervalTo(OutputSchema outputSchema)
      {
         var simulationInterval = _outputIntervalFactory.CreateDefaultFor(outputSchema);
         return new AddSimulationIntervalToSimulationOutputCommand(simulationInterval, outputSchema, _executionContext).Run(_executionContext);
      }

      public ICommand RemoveSimulationIntervalFrom(OutputInterval simulationInterval, OutputSchema simulationOutput)
      {
         if (simulationOutput.Intervals.Count() <= 1)
            throw new CannotDeleteSimulationIntervalException();

         return new RemoveSimulationIntervalFromSimulationOutputCommand(simulationInterval, simulationOutput, _executionContext).Run(_executionContext);
      }
   }
}