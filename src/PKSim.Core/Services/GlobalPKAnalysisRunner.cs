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
      /// <param name="individualSimulation">Base individual simulation</param>
      /// <param name="compound">Compound for which the bioavailability should be calculated</param>
      /// <returns>The simulation that was created and run</returns>
      IndividualSimulation RunForBioavailability(SimpleProtocol simpleIvProtocol, IndividualSimulation individualSimulation, Compound compound);

      /// <summary>
      /// Creates a <see cref="Simulation"/> based on the given <paramref name="individualSimulation"/> for DDI Ratio calculations and run the simulation
      /// </summary>
      /// <param name="individualSimulation">Base individual simulation</param>
      /// <returns>The simulation that was created and run</returns>
      IndividualSimulation RunForDDIRatio(IndividualSimulation individualSimulation);
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

      public IndividualSimulation RunForBioavailability(SimpleProtocol simpleIvProtocol, IndividualSimulation individualSimulation, Compound compound)
      {
         return createAndRun(x => x.CreateForBioAvailability(simpleIvProtocol, compound, individualSimulation));
      }

      public IndividualSimulation RunForDDIRatio(IndividualSimulation individualSimulation)
      {
         return createAndRun(x => x.CreateForDDIRatio(individualSimulation));
      }

      private IndividualSimulation createAndRun(Func<ISimulationFactory,IndividualSimulation> individualSimulationCreator)
      {
         var individualSimulation = individualSimulationCreator(_simulationFactory);
         try
         {
            _registrationTask.Register(individualSimulation);
            _simulationRunner.RunSimulation(individualSimulation).Wait();
         }
         finally
         {
            _registrationTask.Unregister(individualSimulation);
         }

         return individualSimulation;
      }
   }
}