using OSPSuite.Presentation.MenuAndBars;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Services;

namespace PKSim.Presentation.UICommands
{
   public abstract class RunActiveSimulationCommandBase : IUICommand
   {
      private readonly ISimulationRunner _simulationRunner;
      private readonly IActiveSubjectRetriever _activeSubjectRetriever;
      private readonly bool _defineSetting;

      protected RunActiveSimulationCommandBase(ISimulationRunner simulationRunner, IActiveSubjectRetriever activeSubjectRetriever, bool defineSetting)
      {
         _simulationRunner = simulationRunner;
         _activeSubjectRetriever = activeSubjectRetriever;
         _defineSetting = defineSetting;
      }

      public void Execute()
      {
         _simulationRunner.RunSimulation(_activeSubjectRetriever.Active<Simulation>(), _defineSetting);
      }
   }

   public class RunActiveSimulationWithSettingsCommand : RunActiveSimulationCommandBase
   {
      public RunActiveSimulationWithSettingsCommand(ISimulationRunner simulationRunner, IActiveSubjectRetriever activeSubjectRetriever) : base(simulationRunner, activeSubjectRetriever, true)
      {
      }
   }

   public class RunActiveSimulationCommand : RunActiveSimulationCommandBase
   {
      public RunActiveSimulationCommand(ISimulationRunner simulationRunner, IActiveSubjectRetriever activeSubjectRetriever)
         : base(simulationRunner, activeSubjectRetriever, false)
      {
      }
   }
}