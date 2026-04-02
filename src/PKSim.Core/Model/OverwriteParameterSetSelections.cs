using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Model
{
   /// <summary>
   ///    Stores which <see cref="OverwriteParameterSet"/> is selected per compound in a simulation configuration.
   /// </summary>
   public class OverwriteParameterSetSelections
   {
      private readonly ICache<string, OverwriteParameterSetSelection> _selections = new Cache<string, OverwriteParameterSetSelection>(s => s.CompoundName, x => null);

      public IReadOnlyList<OverwriteParameterSetSelection> Selections => _selections.ToList();

      public void SetSelectionForCompound(string compoundName, OverwriteParameterSet overwriteParameterSet)
      {
         if (_selections.Contains(compoundName))
            _selections[compoundName].OverwriteParameterSet = overwriteParameterSet;
         else
            _selections.Add(new OverwriteParameterSetSelection { CompoundName = compoundName, OverwriteParameterSet = overwriteParameterSet });
      }

      public void SetSelectionForCompound(OverwriteParameterSetSelection selection)
      {
         if (_selections.Contains(selection.CompoundName))
            _selections[selection.CompoundName].OverwriteParameterSet = selection.OverwriteParameterSet;
         else
            _selections.Add(selection);
      }

      public void RemoveSelectionForCompound(string compoundName) =>
         _selections.Remove(compoundName);

      public OverwriteParameterSet SelectedSetFor(string compoundName) =>
         _selections[compoundName]?.OverwriteParameterSet;

      public OverwriteParameterSetSelections Clone()
      {
         var clone = new OverwriteParameterSetSelections();
         _selections.Each(s => clone._selections.Add(s.Clone()));
         return clone;
      }
   }

   public class OverwriteParameterSetSelection
   {
      public string CompoundName { get; init; }

      public OverwriteParameterSet OverwriteParameterSet { get; set; }

      public OverwriteParameterSetSelection Clone() =>
         new()
         {
            CompoundName = CompoundName,
            OverwriteParameterSet = OverwriteParameterSet
         };
   }
}
