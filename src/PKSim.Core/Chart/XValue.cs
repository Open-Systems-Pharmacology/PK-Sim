using System;
using PKSim.Assets;
using PKSim.Core.Extensions;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Chart
{
   public interface IXValue : IComparable<IXValue>
   {
      float X { get; set; }
      string ToString(IWithDisplayUnit unitConverter);
   }

   public interface IYValue
   {
      float Y { get; }
      string ToString(IWithDisplayUnit unitConverter);
      bool IsValid { get; }
   }

   public abstract class XValue : IXValue
   {
      public float X { get; set; }

      protected XValue(float value)
      {
         X = value;
      }

      public virtual string ToString(IWithDisplayUnit unitConverter)
      {
         return PKSimConstants.Information.XAsTooltip(unitConverter.DisplayValue(X));
      }


      public int CompareTo(IXValue valueToCompareTo)
      {
         return X.CompareTo(valueToCompareTo.X);
      }
   }
}