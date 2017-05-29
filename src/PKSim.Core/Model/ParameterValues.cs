using System;
using System.Collections.Generic;

namespace PKSim.Core.Model
{
   public class ParameterValues
   {
      public string ParameterPath { get; private set; }
      public List<double> Values { get; set; }
      public List<double> Percentiles { get; set; }

      [Obsolete("For serialization")]
      public ParameterValues()
      {
      }

      public ParameterValues(string parameterPath)
      {
         ParameterPath = parameterPath;
         Values = new List<double>();
         Percentiles = new List<double>();
      }

      public void Add(RandomValue value)
      {
         Add(value.Value, value.Percentile);
      }

      public void Add(double value, double percentile = CoreConstants.DEFAULT_PERCENTILE)
      {
         Values.Add(value);
         Percentiles.Add(percentile);
      }

      public int Count
      {
         get { return Values.Count; }
      }

      public ParameterValues Clone()
      {
         var clone = new ParameterValues(ParameterPath)
            {
               Values = new List<double>(Values),
               Percentiles = new List<double>(Percentiles)
            };
         return clone;
      }

      public void Clear()
      {
         Values.Clear();
         Percentiles.Clear();
      }

      public void Merge(ParameterValues parameterValues)
      {
         Values.AddRange(parameterValues.Values);
         Percentiles.AddRange(parameterValues.Percentiles);
      }

      public void AddEmptyItems(int count, double defaultValue = double.NaN)
      {
         for (int i = 0; i < count; i++)
         {
            Add(defaultValue);
         }
      }

      public void UpdatePath(string newPath)
      {
         ParameterPath = newPath;
      }
   }
}