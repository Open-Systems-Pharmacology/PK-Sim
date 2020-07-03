using System.Collections.Generic;
using System.Linq;
using PKSim.Core.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;

namespace PKSim.Core.Services
{
   public interface IInteractionTask
   {
      /// <summary>
      ///    Returns <c>true</c> if a partial process induced by a molecule is defined for the <paramref name="compound" /> and
      ///    at the same time, an interaction process
      ///    is defined involving that same molecule otherwise <c>false</c>
      /// </summary>
      bool HasInteractionInvolving(Compound compound, Simulation simulation);

      /// <summary>
      ///    Returns <c>true</c> if the <paramref name="compound" /> is defined as metabolite of a metabolization process
      ///    otherwise <c>false</c>
      /// </summary>
      bool IsMetabolite(Compound compound, Simulation simulation);

      /// <summary>
      ///    Returns all interaction containers defined in the model of the simulation
      /// </summary>
      IEnumerable<IContainer> AllInteractionContainers(Simulation simulation);

      /// <summary>
      ///    Returns all enabled <see cref="InteractionSelection" /> defined in the <paramref name="simulation" /> for the
      ///    molecule named  <paramref name="moleculeName" />
      /// </summary>
      IReadOnlyList<InteractionSelection> AllInteractionsSelectionsFor(string moleculeName, Simulation simulation);

      /// <summary>
      ///    Returns all enabled <see cref="InteractionProcess" /> defined in the <paramref name="simulation" /> for the molecule
      ///    named  <paramref name="moleculeName" />
      ///    having the type <paramref name="interactionType" />. If <paramref name="compoundName" /> is specified, all
      ///    <see cref="InteractionProcess" /> induced by a compound
      ///    named <paramref name="compoundName" /> will be filtered out (defacto removing auto-inhibiting processes)
      /// </summary>
      IReadOnlyList<InteractionProcess> AllInteractionProcessesFor(string moleculeName, InteractionType interactionType, Simulation simulation, string compoundName = null);
   }

   public class InteractionTask : IInteractionTask
   {
      public virtual bool HasInteractionInvolving(Compound compound, Simulation simulation)
      {
         var compoundProperties = simulation.CompoundPropertiesFor(compound);
         if (compoundProperties == null)
            return false;

         var interactingMoleculeNames = simulation.InteractionProperties.InteractingMoleculeNames;
         if (!interactingMoleculeNames.Any())
            return false;

         var allDefinedEnablingMolecules = compoundProperties.Processes.AllEnablingMoleculeNames().Where(x => !x.IsUndefinedMolecule());
         return allDefinedEnablingMolecules.ContainsAny(interactingMoleculeNames);
      }

      public virtual bool IsMetabolite(Compound compound, Simulation simulation)
      {
         return simulation.CompoundPropertiesList
            .SelectMany(x => x.Processes.MetabolizationSelection.AllPartialProcesses()
               .OfType<EnzymaticProcessSelection>())
            .Any(x => string.Equals(compound.Name, x.MetaboliteName));
      }

      public virtual IEnumerable<IContainer> AllInteractionContainers(Simulation simulation)
      {
         var model = simulation.Model;
         if (model?.Root == null)
            yield break;

         foreach (var interactionSelection in simulation.InteractionProperties.AllEnabledInteractions())
         {
            yield return model.Root.EntityAt<IContainer>(interactionSelection.CompoundName, interactionSelection.ProcessName);
         }
      }

      public virtual IReadOnlyList<InteractionSelection> AllInteractionsSelectionsFor(string moleculeName, Simulation simulation)
      {
         return simulation.InteractionProperties
            .Interactions.Where(x => string.Equals(x.MoleculeName, moleculeName)).ToList();
      }

      public virtual IReadOnlyList<InteractionProcess> AllInteractionProcessesFor(string moleculeName, InteractionType interactionType, Simulation simulation, string compoundName = null)
      {
         var allInteractionsForMolecules = AllInteractionsSelectionsFor(moleculeName, simulation);

         var query = from interaction in allInteractionsForMolecules
            where !string.Equals(interaction.CompoundName, compoundName)
            let compound = simulation.Compounds.FindByName(interaction.CompoundName)
            where compound != null
            let process = compound.ProcessByName<InteractionProcess>(interaction.ProcessName)
            where process != null
            where process.InteractionType.Is(interactionType)
            select process;

         return query.ToList();
      }
   }
}