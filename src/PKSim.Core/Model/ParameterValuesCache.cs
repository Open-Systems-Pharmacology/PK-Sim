using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public interface IParameterValueCache
   {
      bool Has(string parameterPath);
      IReadOnlyList<double> ValuesFor(string parameterPath);
      IReadOnlyList<double> PercentilesFor(string parameterPath);
      IEnumerable<string> AllParameterPaths();
      void Remove(string parameterPath);
      void SetValues(string parameterPath, IEnumerable<RandomValue> newValues);
      void SetValues(string parameterPath, IEnumerable<double> newValues);
      IEnumerable<ParameterValues> AllParameterValues { get; }
      ParameterValues ParameterValuesFor(string parameterPath);
      ParameterValues ParameterValuesAt(int index);
      void Add(ParameterValues parameterValues);
      void Add(IReadOnlyCollection<ParameterValue> parameterValues);
      void RenamePath(string oldPath, string newPath);
      int Count { get; }
   }

   /// <summary>
   ///    Represents a cache containing the population values. Values are stored like so:
   ///    1 - One column per parameter path
   ///    2 - All values for one parameter are stored in one column. That means that one individual can be read as a row
   /// </summary>
   public class ParameterValuesCache : IParameterValueCache
   {
      private readonly ICache<string, ParameterValues> _parameterValuesCache = new Cache<string, ParameterValues>(x => x.ParameterPath);

      public virtual void Add(IReadOnlyCollection<ParameterValue> parameterValues)
      {
         addValues(parameterValues);
      }

      public virtual void RenamePath(string oldPath, string newPath)
      {
         if (!Has(oldPath))
            return;

         var values = _parameterValuesCache[oldPath];
         Remove(oldPath);
         _parameterValuesCache.Add(newPath, values);
      }

      public virtual int Count
      {
         get
         {
            if (!_parameterValuesCache.Any())
               return 0;
            return _parameterValuesCache.First().Count;
         }
      }

      public virtual ParameterValues ParameterValuesFor(string parameterPath)
      {
         if (!Has(parameterPath))
            _parameterValuesCache.Add(new ParameterValues(parameterPath));

         return _parameterValuesCache[parameterPath];
      }

      public virtual ParameterValues ParameterValuesAt(int index)
      {
         return _parameterValuesCache.ElementAt(index);
      }

      public virtual void Add(ParameterValues parameterValues)
      {
         _parameterValuesCache.Add(parameterValues);
      }

      private void addValues(IEnumerable<ParameterValue> parameterValues)
      {
         parameterValues.Each(pv => ParameterValuesFor(pv.ParameterPath).Add(pv));
      }

      public virtual bool Has(string parameterPath)
      {
         return _parameterValuesCache.Contains(parameterPath);
      }

      public virtual IReadOnlyList<double> ValuesFor(string parameterPath)
      {
         return ParameterValuesFor(parameterPath).Values;
      }

      public virtual IReadOnlyList<double> PercentilesFor(string parameterPath)
      {
         return ParameterValuesFor(parameterPath).Percentiles;
      }

      public virtual IEnumerable<string> AllParameterPaths()
      {
         return _parameterValuesCache.Keys;
      }

      public virtual ParameterValuesCache Clone()
      {
         var clone = new ParameterValuesCache();
         AllParameterValues.Each(x => clone.Add(x.Clone()));
         return clone;
      }

      public virtual void Remove(string parameterPath)
      {
         _parameterValuesCache.Remove(parameterPath);
      }

      public virtual void SetValues(string parameterPath, IEnumerable<RandomValue> newValues)
      {
         var parameterValues = ParameterValuesFor(parameterPath);
         parameterValues.Clear();
         newValues.Each(parameterValues.Add);
      }

      public virtual void SetValues(string parameterPath, IEnumerable<double> newValues)
      {
         var parameterValues = ParameterValuesFor(parameterPath);
         parameterValues.Clear();
         newValues.Each(x => parameterValues.Add(x));
      }

      public virtual IEnumerable<ParameterValues> AllParameterValues
      {
         get { return _parameterValuesCache; }
      }

      public virtual void Merge(ParameterValuesCache parameterValuesCache, PathCache<IParameter> parameterCache)
      {
         var numberOfNewItems = parameterValuesCache.Count;
         var currentCount = Count;

         foreach (var parameterPath in parameterValuesCache.AllParameterPaths())
         {
            if (!Has(parameterPath))
            {
               addDefaultValues(parameterCache, parameterPath, currentCount);
            }
            ParameterValuesFor(parameterPath).Merge(parameterValuesCache.ParameterValuesFor(parameterPath));
         }

         //fill up the one missing
         foreach (var parameterPath in AllParameterPaths())
         {
            if (!parameterValuesCache.Has(parameterPath))
            {
               addDefaultValues(parameterCache, parameterPath, numberOfNewItems);
            }
         }
      }

      private void addDefaultValues(PathCache<IParameter> parameterCache, string parameterPath, int numberOfItems)
      {
         var parameterValues = ParameterValuesFor(parameterPath);
         var parameter = parameterCache[parameterPath];
         var defaultValue = double.NaN;
         if (parameter != null)
            defaultValue = parameter.Value;

         parameterValues.AddEmptyItems(numberOfItems, defaultValue);
      }
   }
}