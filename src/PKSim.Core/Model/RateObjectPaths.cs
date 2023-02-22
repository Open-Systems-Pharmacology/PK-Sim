using System.Collections.Generic;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public interface IRateObjectPaths
   {
      string Rate { get; }
      string CalculationMethod { get; }
      IEnumerable<FormulaUsablePath> ObjectPaths { get; }
      void AddObjectPath(FormulaUsablePath objectPath);
   }

   public class RateObjectPaths : IRateObjectPaths
   {
      private readonly IList<FormulaUsablePath> _objectPaths;
      public string Rate { get; private set; }
      public string CalculationMethod { get; private set; }

      public RateObjectPaths(string calculationMethod, string rate)
      {
         CalculationMethod = calculationMethod;
         Rate = rate;
         _objectPaths = new List<FormulaUsablePath>();
      }

      public IEnumerable<FormulaUsablePath> ObjectPaths
      {
         get { return _objectPaths; }
      }

      public void AddObjectPath(FormulaUsablePath objectPath)
      {
         _objectPaths.Add(objectPath);
      }
   }

   public class NullObjectPaths : IRateObjectPaths
   {
      public string Rate { get; private set; }
      public string CalculationMethod { get; private set; }

      public IEnumerable<FormulaUsablePath> ObjectPaths
      {
         get { return new List<FormulaUsablePath>(); }
      }

      public void AddObjectPath(FormulaUsablePath objectPath)
      {
      }
   }
}