using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IInteractionProcessRetriever
   {
      /// <summary>
      ///    Returns all partial processes that could be created for the given simulation over all compounds..
      ///    One item will be created for each molecule in individual. Match will be performed according to molecule name in
      ///    individual and compound
      /// </summary>
      /// <param name="simulation">simulation</param>
      /// <param name="processSelections">possibly used processes in the simulation for the given type</param>
      /// <param name="addDefaultPartialProcess">
      ///    if set to true, the default mapping logic will be used to create the simulation partial
      ///    process
      /// </param>
      IReadOnlyList<SimulationPartialProcess> AllFor(Simulation simulation, IReadOnlyList<IProcessMapping> processSelections, bool addDefaultPartialProcess);

      InteractionProcess NotSelectedInteractionProcess { get; set; }
   }

   public class InteractionProcessRetriever : IInteractionProcessRetriever
   {
      private readonly IPartialProcessRetriever _partialProcessRetriever;

      public InteractionProcess NotSelectedInteractionProcess
      {
         get { return _partialProcessRetriever.NotSelectedPartialProcess.DowncastTo<InteractionProcess>(); }
         set { _partialProcessRetriever.NotSelectedPartialProcess = value; }
      }

      public InteractionProcessRetriever(IPartialProcessRetriever partialProcessRetriever)
      {
         _partialProcessRetriever = partialProcessRetriever;
      }

      public IReadOnlyList<SimulationPartialProcess> AllFor(Simulation simulation, IReadOnlyList<IProcessMapping> processSelections, bool addDefaultPartialProcess)
      {
         var allSimulationPartialProcesses = new List<SimulationPartialProcess>();
         var allNoSelectionPartialProcesses = new List<SimulationPartialProcess>();

         foreach (var compound in simulation.Compounds)
         {
            var allPossibleProcesses = allPossibleInteractionProcessesFor(simulation, compound, processSelections, addDefaultPartialProcess);
            //add all well mapped processes
            allSimulationPartialProcesses.AddRange(allPossibleProcesses.Where(x => !Equals(x.CompoundProcess, NotSelectedInteractionProcess)));

            //store the not mapped that will be added later if required
            allNoSelectionPartialProcesses.AddRange(allPossibleProcesses.Where(x => Equals(x.CompoundProcess, NotSelectedInteractionProcess)));
         }

         allSimulationPartialProcesses.AddRange(addNotSelectedPartialProcessIfPreviouslySelected(processSelections, allNoSelectionPartialProcesses));
         return allSimulationPartialProcesses;
      }

      private IReadOnlyList<SimulationPartialProcess> allPossibleInteractionProcessesFor(Simulation simulation, Compound compound, IReadOnlyList<IProcessMapping> processSelections, bool addDefaultPartialProcess)
      {
         return _partialProcessRetriever.AllFor<IndividualMolecule, InhibitionProcess>(simulation, compound, processSelections, addDefaultPartialProcess)
            .Union(
               _partialProcessRetriever.AllFor<IndividualMolecule, InductionProcess>(simulation, compound, processSelections, addDefaultPartialProcess)).ToList();
      }

      private IEnumerable<SimulationPartialProcess> addNotSelectedPartialProcessIfPreviouslySelected(IEnumerable<IProcessMapping> processSelections,
         IEnumerable<SimulationPartialProcess> allNoSelectionPartialProcesses) 
      {
         var moleculesWithEmptySelections = processSelections.Where(x => string.IsNullOrEmpty(x.ProcessName))
            .Select(x => x.MoleculeName).Distinct();

         //can be null if molecule was removed from individual
         return moleculesWithEmptySelections.Select(moleculeName => allNoSelectionPartialProcesses.FirstOrDefault(x => string.Equals(x.MoleculeName, moleculeName)))
            .Where(noSelectionProcess => noSelectionProcess != null);
      }
   }
}