using System.Threading.Tasks;
using OSPSuite.Core.Services;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public class RunSimulationQualificationStepRunner : QualificationStepRunner<RunSimulationQualificationStep>
   {
      private readonly ISimulationRunner _simulationRunner;

      public RunSimulationQualificationStepRunner(ISimulationRunner simulationRunner, ILogger logger) : base(logger)
      {
         _simulationRunner = simulationRunner;
      }

      public override Task RunAsync(RunSimulationQualificationStep qualificationStep)
      {
         var simulation = qualificationStep.Simulation;

         return _simulationRunner.RunSimulation(simulation);
      }
   }
}