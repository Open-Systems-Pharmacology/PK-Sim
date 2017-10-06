using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Services
{
   /// <summary>
   ///    Retrieve the possible partial processes that can be created based on the one hand on the compound and individual
   ///    defined in the simulation,
   ///    and on the other hand on the model structure in the simulation
   /// </summary>
   public interface IPartialProcessRetriever
   {
      /// <summary>
      ///    Returns all partial processes that could be created for the given simulation, and the given compound.
      ///    One item will be created for each molecule in individual. Match will be performed according to molecule name in
      ///    individual and compound
      /// </summary>
      /// <typeparam name="TIndividualMolecule">Type of molecule for which the processes need to be retrieved</typeparam>
      /// <typeparam name="TPartialProcess">Type of partial processes used n the compound</typeparam>
      /// <param name="simulation">simulation</param>
      /// <param name="compound">compound</param>
      /// <param name="processSelections">possibly used processes in the simulation for the given type</param>
      /// <param name="addDefaultPartialProcess">
      ///    if set to true, the default mapping logic will be used to create the simulation partial
      ///    process
      /// </param>
      IReadOnlyList<SimulationPartialProcess> AllFor<TIndividualMolecule, TPartialProcess>(Simulation simulation, Compound compound,
         IReadOnlyList<IProcessMapping> processSelections, bool addDefaultPartialProcess) where TIndividualMolecule : IndividualMolecule where TPartialProcess : PartialProcess;

      PartialProcess NotSelectedPartialProcess { get; set; }
   }

   public class PartialProcessRetriever : IPartialProcessRetriever
   {
      public PartialProcess NotSelectedPartialProcess { get; set; }

      public PartialProcessRetriever()
      {
         NotSelectedPartialProcess = null;
      }

      public IReadOnlyList<SimulationPartialProcess> AllFor<TIndividualMolecule, TPartialProcess>(Simulation simulation, Compound compound,
         IReadOnlyList<IProcessMapping> processSelections, bool addDefaultPartialProcess)
         where TIndividualMolecule : IndividualMolecule
         where TPartialProcess : PartialProcess
      {
         var allSimulationPartialProcesses = new List<SimulationPartialProcess>();
         var individual = simulation.Individual;
         var allIndividualMolecules = individual.AllMolecules<TIndividualMolecule>().Where(moleculeWasDefinedByUser).ToList();
         var allProcesses = compound.AllProcesses<TPartialProcess>().ToList();

         var steps = new List<Func<TIndividualMolecule, IEnumerable<TPartialProcess>, IEnumerable<IProcessMapping>, IEnumerable<SimulationPartialProcess>>> {addExistingProcessSelection};

         if (addDefaultPartialProcess)
            steps.Add(addDefaultProcessForMolecule);

         steps.Add(addNotSelectedPartialProcess);


         foreach (var individualMolecule in allIndividualMolecules)
         {
            allSimulationPartialProcesses.AddRange(allPartialProcessesFor(individualMolecule, allProcesses, processSelections, steps));
         }

         return allSimulationPartialProcesses;
      }

      private IEnumerable<SimulationPartialProcess> addNotSelectedPartialProcess<TIndividualMolecule>(TIndividualMolecule individualMolecule, IEnumerable<PartialProcess> allProcesses, IEnumerable<IProcessMapping> processSelections)
         where TIndividualMolecule : IndividualMolecule
      {
         if (allProcesses.Any())
            yield return noSelectionFor(individualMolecule);
      }

      private SimulationPartialProcess noSelectionFor<TIndividualMolecule>(TIndividualMolecule individualMolecule)
         where TIndividualMolecule : IndividualMolecule
      {
         return newSimulationPartialProcess(individualMolecule, NotSelectedPartialProcess);
      }

      private IEnumerable<SimulationPartialProcess> addDefaultProcessForMolecule<TIndividualMolecule, TPartialProcess>(TIndividualMolecule individualMolecule, IEnumerable<TPartialProcess> allProcesses, IEnumerable<IProcessMapping> processSelections)
         where TIndividualMolecule : IndividualMolecule
         where TPartialProcess : PartialProcess
      {
         var firstProcessForMolecule = allProcesses.FirstOrDefault(x => string.Equals(x.MoleculeName, individualMolecule.Name));
         if (firstProcessForMolecule != null)
            yield return newSimulationPartialProcess(individualMolecule, firstProcessForMolecule);
      }

      private IEnumerable<SimulationPartialProcess> addExistingProcessSelection<TIndividualMolecule, TPartialProcess>(TIndividualMolecule individualMolecule, IEnumerable<TPartialProcess> allProcesses, IEnumerable<IProcessMapping> processSelections)
         where TIndividualMolecule : IndividualMolecule
         where TPartialProcess : PartialProcess
      {
         var allPreselectedProcessesForMolecule = from processSelectionForIndividualMolecule in processSelections.Where(x => string.Equals(x.MoleculeName, individualMolecule.Name))
            select new {Mapping = processSelectionForIndividualMolecule, CompoundProcess = allProcesses.FindByName(processSelectionForIndividualMolecule.ProcessName)};

         foreach (var partialProcess in allPreselectedProcessesForMolecule)
         {
            var compoundProcess = partialProcess.CompoundProcess;
            var mapping = partialProcess.Mapping;
            //selected was explictely removed by the user
            if (isSelected(compoundProcess, mapping))
               yield return newSimulationPartialProcess(individualMolecule, compoundProcess, mapping);
            else
               yield return noSelectionFor(individualMolecule);
         }
      }

      private static bool isSelected<TPartialProcess>(TPartialProcess compoundProcess, IProcessMapping mapping) where TPartialProcess : PartialProcess
      {
         return compoundProcess != null && string.Equals(compoundProcess.ParentCompound.Name, mapping.CompoundName);
      }

      private static SimulationPartialProcess newSimulationPartialProcess<TIndividualMolecule>(TIndividualMolecule individualMolecule, PartialProcess partialProcess, IProcessMapping partialProcessMapping = null)
         where TIndividualMolecule : IndividualMolecule
      {
         return new SimulationPartialProcess { CompoundProcess = partialProcess, IndividualMolecule = individualMolecule, PartialProcessMapping = partialProcessMapping};
      }

      private IReadOnlyList<SimulationPartialProcess> allPartialProcessesFor<TIndividualMolecule, TPartialProcess>(
         TIndividualMolecule individualMolecule,
         IReadOnlyList<TPartialProcess> allAvailablePartialProcesses,
         IReadOnlyList<IProcessMapping> processSelections,
         IEnumerable<Func<TIndividualMolecule, IEnumerable<TPartialProcess>, IEnumerable<IProcessMapping>, IEnumerable<SimulationPartialProcess>>> steps)
         where TIndividualMolecule : IndividualMolecule
         where TPartialProcess : PartialProcess
      {
         var allPartialProcesses = new List<SimulationPartialProcess>();

         foreach (var step in steps)
         {
            if (!allPartialProcesses.Any())
               allPartialProcesses.AddRange(step(individualMolecule, allAvailablePartialProcesses, processSelections));
         }

         return allPartialProcesses;
      }

      private bool moleculeWasDefinedByUser(IndividualMolecule molecule)
      {
         return !molecule.Name.IsUndefinedMolecule();
      }
   }
}