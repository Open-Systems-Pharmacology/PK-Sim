using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class InteractionProperties
   {
      private readonly List<InteractionSelection> _interactionSelections = new List<InteractionSelection>();

      public virtual InteractionProperties Clone(ICloneManager cloneManager)
      {
         var clone = new InteractionProperties();
         Interactions.Each(x => clone.AddInteraction(x.Clone(cloneManager)));
         return clone;
      }

      public virtual void AddInteraction(InteractionSelection interaction)
      {
         _interactionSelections.Add(interaction);
      }

      public virtual IReadOnlyList<InteractionSelection> Interactions => _interactionSelections;

      public virtual void ClearInteractions()
      {
         _interactionSelections.Clear();
      }

      /// <summary>
      ///    Returns true if any interaction is defined otherwise false
      /// </summary>
      /// <returns></returns>
      public virtual bool Any()
      {
         return _interactionSelections.Any();
      }

      /// <summary>
      ///    Returns the names of all molecules involved in interaction processes
      /// </summary>
      public virtual IReadOnlyList<string> InteractingMoleculeNames
      {
         get { return AllEnabledInteractions().Select(x => x.MoleculeName).Distinct().ToList(); }
      }

      public virtual IEnumerable<InteractionSelection> AllEnabledInteractions()
      {
         return _interactionSelections.Where(x => !string.IsNullOrEmpty(x.ProcessName));
      }

      /// <summary>
      ///    Returns <c>true</c> if the <paramref name="interactionProcess" /> is being used as part of the interactions
      ///    otherwise <c>false</c>
      /// </summary>
      public virtual bool Uses(InteractionProcess interactionProcess)
      {
         if (!Any())
            return false;

         return _interactionSelections.Any(x => x.Matches(interactionProcess));
      }
   }
}