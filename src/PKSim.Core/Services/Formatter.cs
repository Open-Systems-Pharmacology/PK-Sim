using OSPSuite.Core.Domain;
using OSPSuite.Utility.Format;
using PKSim.Assets;

namespace PKSim.Core.Services
{
   public class NullIntParameterFormatter : NumericFormatter<int?>
   {
      public NullIntParameterFormatter()
         : base(NumericFormatterOptions.Instance)
      {
      }

      public override string Format(int? valueToFormat)
      {
         if (valueToFormat == null)
            return PKSimConstants.UI.NotAvailable;

         return base.Format(valueToFormat);
      }
   }

   public class PercentFormatter : UnitFormatter
   {
      public PercentFormatter()
         : base("%")
      {
      }
   }

   public class BooleanFormatter : NumericFormatter<double>
   {
      public BooleanFormatter()
         : base(NumericFormatterOptions.Instance)
      {
      }

      public override string Format(double valueToFormat)
      {
         return valueToFormat == 1 ? PKSimConstants.UI.Yes : PKSimConstants.UI.No;
      }
   }

   public class NullableBooleanFormatter : IFormatter<bool?>
   {
      public string Format(bool? valueToFormat)
      {
         if (valueToFormat == null)
            return string.Empty;

         return valueToFormat.Value ? PKSimConstants.UI.Yes : PKSimConstants.UI.No;
      }
   }
}