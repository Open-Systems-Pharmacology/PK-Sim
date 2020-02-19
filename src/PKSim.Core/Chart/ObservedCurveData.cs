using PKSim.Assets;
using PKSim.Core.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Extensions;

namespace PKSim.Core.Chart
{
   public class ObservedDataYValue : ITimeProfileYValue
   {
      public float Mean { get; set; }

      public float Error { get; set; }

      public AuxiliaryType ErrorType { get; set; }

      public bool IsValid => Mean.IsValid();

      public ObservedDataYValue()
      {
         ErrorType = AuxiliaryType.Undefined;
         Error = float.NaN;
         Mean = float.NaN;
      }

      public string ToString(IWithDisplayUnit objectWithTargetUnit, IDimension valueDimension)
      {
         return float.IsNaN(Error)
                   ? PKSimConstants.Information.ObservedDataYAsTooltip(objectWithTargetUnit.DisplayValueWithUnit(Y, valueDimension))
                   : PKSimConstants.Information.ObservedDataYAsTooltip(objectWithTargetUnit.DisplayValueWithUnit(Y, valueDimension),
                                                                       objectWithTargetUnit.DisplayValueWithUnit(LowerValue, valueDimension),
                                                                       objectWithTargetUnit.DisplayValueWithUnit(UpperValue, valueDimension));
      }

      public float Y => Mean;

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

      private bool hasError => Error.IsValid() && Error > 0 && ErrorType != AuxiliaryType.Undefined;
   }

   public class ObservedCurveData : CurveData<TimeProfileXValue, ObservedDataYValue> 
   {
      public AuxiliaryType ErrorType { get; }
      public bool Visible { get; set; }

      public ObservedCurveData(AuxiliaryType errorType)
      {
         ErrorType = errorType;
      }
   }
}