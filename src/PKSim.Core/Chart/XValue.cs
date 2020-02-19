using System;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Assets;
using PKSim.Core.Extensions;

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
      string ToString(IWithDisplayUnit objectWithTargetUnit, IDimension valueDimension);
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
         return PKSimConstants.Information.XAsTooltip(unitConverter.DisplayValueWithUnit(X));
      }

      public int CompareTo(IXValue valueToCompareTo)
      {
         return X.CompareTo(valueToCompareTo.X);
      }
   }
}