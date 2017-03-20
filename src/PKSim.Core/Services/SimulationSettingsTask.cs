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
      ICommand AddSimulationIntervalTo(OutputSchema simulationOutput);
      ICommand RemoveSimulationIntervalFrom(OutputInterval simulationInterval, OutputSchema simulationOutput);
   }

   public class SimulationSettingsTask : ISimulationSettingsTask
   {
      private readonly IExecutionContext _executionContext;
      private readonly IOutputIntervalFactory _outputIntervalFactory;
      private readonly IContainerTask _containerTask;

      public SimulationSettingsTask(IExecutionContext executionContext, IOutputIntervalFactory outputIntervalFactory, IContainerTask containerTask)
      {
         _executionContext = executionContext;
         _outputIntervalFactory = outputIntervalFactory;
         _containerTask = containerTask;
      }

      public ICommand AddSimulationIntervalTo(OutputSchema simulationOutput)
      {
         var simulationInterval = _outputIntervalFactory.CreateDefault();
         simulationInterval.Name = _containerTask.CreateUniqueName(simulationOutput, simulationInterval.Name);
         return new AddSimulationIntervalToSimulationOutputCommand(simulationInterval, simulationOutput, _executionContext).Run(_executionContext);
      }

      public ICommand RemoveSimulationIntervalFrom(OutputInterval simulationInterval, OutputSchema simulationOutput)
      {
         if (simulationOutput.Intervals.Count() <= 1)
            throw new CannotDeleteSimulationIntervalException();

         return new RemoveSimulationIntervalFromSimulationOutputCommand(simulationInterval, simulationOutput, _executionContext).Run(_executionContext);
      }
   }
}