using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Extensions;

namespace PKSim.Core.Model
{
   public class FloatMatrix 
   {
      private readonly List<float[]> _rows;

      public FloatMatrix()
      {
         _rows = new List<float[]>();
      }

      public int NumberOfRows => _rows.Count;

      /// <summary>
      /// Add sorted and validated values (i.e NaN were removed)
      /// </summary>
      public void AddSortedValues(float[] sortedValues)
      {
         _rows.Add(sortedValues);
      }

      /// <summary>
      ///    Add values by removing all nan and sorting the values
      /// </summary>
      public void AddValuesAndSort(IEnumerable<float> valuesToFilterAndSort)
      {
         var validValues = valuesToFilterAndSort.Where(x => x.IsValid()).ToArray();
         Array.Sort(validValues);
         AddSortedValues(validValues);
      }

      public float[] SortedValueAt(int index0)
      {
         return _rows[index0];
      }

      public float[] SliceAt(int index)
      {
         return _rows.Select(r => r[index]).ToArray();
      }
   }
}