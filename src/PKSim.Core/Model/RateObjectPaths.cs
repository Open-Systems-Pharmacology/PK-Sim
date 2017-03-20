using System.Collections.Generic;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public interface IRateObjectPaths
   {
      string Rate { get; }
      string CalculationMethod { get; }
      IEnumerable<IFormulaUsablePath> ObjectPaths { get; }
      void AddObjectPath(IFormulaUsablePath objectPath);
   }

   public class RateObjectPaths : IRateObjectPaths
   {
      private readonly IList<IFormulaUsablePath> _objectPaths;
      public string Rate { get; private set; }
      public string CalculationMethod { get; private set; }

      public RateObjectPaths(string calculationMethod, string rate)
      {
         CalculationMethod = calculationMethod;
         Rate = rate;
         _objectPaths = new List<IFormulaUsablePath>();
      }

      public IEnumerable<IFormulaUsablePath> ObjectPaths
      {
         get { return _objectPaths; }
      }

      public void AddObjectPath(IFormulaUsablePath objectPath)
      {
         _objectPaths.Add(objectPath);
      }
   }

   public class NullObjectPaths : IRateObjectPaths
   {
      public string Rate { get; private set; }
      public string CalculationMethod { get; private set; }

      public IEnumerable<IFormulaUsablePath> ObjectPaths
      {
         get { return new List<IFormulaUsablePath>(); }
      }

      public void AddObjectPath(IFormulaUsablePath objectPath)
      {
      }
   }
}