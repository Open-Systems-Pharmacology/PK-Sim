using System;
using OSPSuite.Utility.Format;

namespace PKSim.Presentation.Services
{
   public class ValueWithUnitFormatter<TValue> : NumericFormatter<TValue>
   {
      private readonly Func<string> _displayUnitName;

      public ValueWithUnitFormatter(Func<string> displayUnitName) : base(NumericFormatterOptions.Instance)
      {
         _displayUnitName = displayUnitName;
      }

      public override string Format(TValue valueToFormat) => $"{base.Format(valueToFormat)} {_displayUnitName()}".TrimEnd();
   }
}
