using PKSim.Assets;
using PKSim.Core.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Extensions;

namespace PKSim.Core.Chart
{
   public class ObservedDataYValue : ITimeProfileYValue
   {
      public float Mean { get; set; }
      public float Error { get; set; }
      public AuxiliaryType ErrorType { get; set; }

      public bool IsValid
      {
         get { return Mean.IsValid(); }
      }

      public ObservedDataYValue()
      {
         ErrorType = AuxiliaryType.Undefined;
         Error = float.NaN;
         Mean = float.NaN;
      }

      public string ToString(IWithDisplayUnit unitConverter)
      {
         return float.IsNaN(Error)
                   ? PKSimConstants.Information.ObservedDataYAsTooltip(unitConverter.DisplayValue(Y))
                   : PKSimConstants.Information.ObservedDataYAsTooltip(unitConverter.DisplayValue(Y), 
                                                                       unitConverter.DisplayValue(LowerValue), 
                                                                       unitConverter.DisplayValue(UpperValue));
      }

      public float Y
      {
         get { return Mean; }
      }

      public float LowerValue
      {
         get
         {
            if (!hasError)
               return Mean;

            if (ErrorType == AuxiliaryType.ArithmeticStdDev)
               return Mean - Error;

            return Mean / Error;
         }
      }

      public float UpperValue
      {
         get
         {
            if (!hasError)
               return Mean;

            if (ErrorType == AuxiliaryType.ArithmeticStdDev)
               return Mean + Error;

            return Mean * Error;
         }
      }

      private bool hasError
      {
         get { return Error.IsValid() && Error > 0 && ErrorType != AuxiliaryType.Undefined; }
      }
   }

   public class ObservedCurveData : CurveData<TimeProfileXValue, ObservedDataYValue> 
   {
      public AuxiliaryType ErrorType { get; private set; }
      public bool Visible { get; set; }

      public ObservedCurveData(AuxiliaryType errorType)
      {
         ErrorType = errorType;
      }
   }
}