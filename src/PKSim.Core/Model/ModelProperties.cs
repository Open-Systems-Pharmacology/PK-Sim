using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class ModelProperties : IWithCalculationMethods
   {
      public virtual ModelConfiguration ModelConfiguration { get; set; }
      public virtual CalculationMethodCache CalculationMethodCache { get; private set; }

      public ModelProperties()
      {
         CalculationMethodCache = new CalculationMethodCache();
      }

      public virtual ModelProperties Clone(ICloneManager cloneManager)
      {
         return new ModelProperties
         {
            ModelConfiguration = ModelConfiguration,
            CalculationMethodCache = CalculationMethodCache.Clone()
         };
      }
   }
}