using System;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IGlobalPKAnalysisRunner
   {
      /// <summary>
      /// Creates a <see cref="Simulation"/> based on the given <paramref name="individualSimulation"/> for bioavailability calculations and run the simulation
      /// </summary>
      /// <param name="simpleIvProtocol">Iv protocol that will be used in the created simulation for the <paramref name="compound"/></param>
      /// <param name="simulation">Base simulation</param>
      /// <param name="compound">Compound for which the bioavailability should be calculated</param>
      /// <returns>The simulation that was created and run</returns>
      Simulation RunForBioavailability(SimpleProtocol simpleIvProtocol, Simulation simulation, Compound compound);

      /// <summary>
      /// Creates a <see cref="Simulation"/> based on the given <paramref name="simulation"/> for DDI Ratio calculations and run the simulation
      /// </summary>
      /// <param name="simulation">Base simulation</param>
      /// <returns>The simulation that was created and run</returns>
      Simulation RunForDDIRatio(Simulation simulation);
   }

   public class GlobalPKAnalysisRunner : IGlobalPKAnalysisRunner
   {
      private readonly ISimulationRunner _simulationRunner;
      private readonly ISimulationFactory _simulationFactory;
      private readonly IRegistrationTask _registrationTask;

      public GlobalPKAnalysisRunner(ISimulationRunner simulationRunner, ISimulationFactory simulationFactory, IRegistrationTask registrationTask)
      {
         _simulationRunner = simulationRunner;
         _simulationFactory = simulationFactory;
         _registrationTask = registrationTask;
      }

      public Simulation RunForBioavailability(SimpleProtocol simpleIvProtocol, Simulation simulation, Compound compound)
      {
         return createAndRun(x => x.CreateForBioAvailability(simpleIvProtocol, compound, simulation));
      }

      public Simulation RunForDDIRatio(Simulation simulation)
      {
         return createAndRun(x => x.CreateForDDIRatio(simulation));
      }

      private Simulation createAndRun(Func<ISimulationFactory, Simulation> simulationCreator)
      {
         var simulation = simulationCreator(_simulationFactory);
         try
         {
            _registrationTask.Register(simulation);
            _simulationRunner.RunSimulation(simulation).Wait();
         }
         finally
         {
            _registrationTask.Unregister(simulation);
         }

         return simulation;
      }
   }
}