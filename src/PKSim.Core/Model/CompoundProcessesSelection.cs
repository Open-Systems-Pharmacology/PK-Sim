using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class CompoundProcessesSelection
   {
      public virtual ProcessSelectionGroup MetabolizationSelection { get; private set; }
      public virtual ProcessSelectionGroup TransportAndExcretionSelection { get; private set; }
      public virtual ProcessSelectionGroup SpecificBindingSelection { get; private set; }

      public CompoundProcessesSelection()
      {
         MetabolizationSelection = new ProcessSelectionGroup(CoreConstants.Molecule.Metabolite);
         TransportAndExcretionSelection = new ProcessSelectionGroup(string.Empty);
         SpecificBindingSelection = new ProcessSelectionGroup(CoreConstants.Molecule.Complex);
      }

      public CompoundProcessesSelection Clone(ICloneManager cloneManager)
      {
         return new CompoundProcessesSelection
         {
            MetabolizationSelection = MetabolizationSelection.Clone(cloneManager),
            TransportAndExcretionSelection = TransportAndExcretionSelection.Clone(cloneManager),
            SpecificBindingSelection = SpecificBindingSelection.Clone(cloneManager)
         };
      }

      public bool Any() => AllEnabledProcesses().Any();

      /// <summary>
      ///    returns the name of all molecules that will be created due to the active processes defined for a given protein
      /// </summary>
      public virtual IEnumerable<string> AllInducedMoleculeNames(IndividualMolecule molecule)
      {
         return AllProcessSelectionGroups.SelectMany(x => x.AllInducedMoleculeNames(molecule)).Distinct();
      }


      /// <summary>
      ///    Returns the distinct name of all well defined molecules that enable a process (Enzyme, Transport or Protein name)
      /// </summary>
      public virtual IEnumerable<string> AllEnablingMoleculeNames()
      {
         return AllProcessSelectionGroups.SelectMany(x => x.AllEnablingMoleculeNames()).Distinct();
      }

      public virtual IEnumerable<IReactionMapping> AllEnabledProcesses()
      {
         return AllProcessSelectionGroups.SelectMany(x => x.AllEnabledProcesses());
      }

      public virtual IEnumerable<ProcessSelectionGroup> AllProcessSelectionGroups
      {
         get
         {
            yield return MetabolizationSelection;
            yield return TransportAndExcretionSelection;
            yield return SpecificBindingSelection;
         }
      } 
   }
}