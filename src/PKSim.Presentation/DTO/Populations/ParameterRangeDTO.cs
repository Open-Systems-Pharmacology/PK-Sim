using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using OSPSuite.Presentation.DTO;
using OSPSuite.Core.Domain.UnitSystem;

namespace PKSim.Presentation.DTO.Populations
{
   public class ParameterRangeDTO : DxValidatableDTO<ParameterRange>
   {
      public ParameterRange ParameterRange { get; }

      public ParameterRangeDTO(ParameterRange parameterRange)
         : base(parameterRange)
      {
         ParameterRange = parameterRange;
      }

      public string ParameterName => ParameterRange.ParameterName;

      public string ParameterDisplayName => ParameterRange.ParameterDisplayName;

      public double? MinValueInDisplayUnit
      {
         get => ParameterRange.MinValueInDisplayUnit;
         set => ParameterRange.MinValueInDisplayUnit = value;
      }

      public double? MaxValueInDisplayUnit
      {
         get => ParameterRange.MaxValueInDisplayUnit;
         set => ParameterRange.MaxValueInDisplayUnit = value;
      }

      public Unit Unit
      {
         get => ParameterRange.Unit;
         set
         {
            var minDisplayValue = MinValueInDisplayUnit;
            var maxDisplayValue = MaxValueInDisplayUnit;
            ParameterRange.Unit = value;
            MinValueInDisplayUnit = minDisplayValue;
            MaxValueInDisplayUnit = maxDisplayValue;
         }
      }

      public bool IsDiscrete => ParameterRange.IsAnImplementationOf<DiscreteParameterRange>();

      public IEnumerable<double> ListOfValues
      {
         get
         {
            if (!IsDiscrete)
               return Enumerable.Empty<double>();

            return ParameterRange.DowncastTo<DiscreteParameterRange>().ListOfValues;
         }
      }

   }
}