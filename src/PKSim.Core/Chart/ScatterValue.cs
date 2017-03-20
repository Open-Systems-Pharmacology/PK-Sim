using PKSim.Assets;
using PKSim.Core.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;

namespace PKSim.Core.Chart
{
   public class ScatterXValue : XValue
   {
      public ScatterXValue(float value) : base(value)
      {
      }
   }

   public class ScatterYValue : IYValue
   {
      public float Value { get; private set; }

      public ScatterYValue(float value)
      {
         Value = value;
      }

      public bool IsValid
      {
         get { return Value.IsValid(); }
      }

      public string ToString(IWithDisplayUnit unitConverter)
      {
         return PKSimConstants.Information.ScatterYAsTooltip(unitConverter.DisplayValue(Value));
      }

      public float Y { get { return Value; } }
   }
}