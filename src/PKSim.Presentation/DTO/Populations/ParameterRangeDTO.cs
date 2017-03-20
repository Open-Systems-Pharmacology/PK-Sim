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
      public ParameterRange ParameterRange { get; private set; }

      public ParameterRangeDTO(ParameterRange parameterRange)
         : base(parameterRange)
      {
         ParameterRange = parameterRange;
      }

      public string ParameterName
      {
         get { return ParameterRange.ParameterName; }
      }

      public string ParameterDisplayName
      {
         get { return ParameterRange.ParameterDisplayName; }
      }

      public double? MinValueInDisplayUnit
      {
         get { return ParameterRange.MinValueInDisplayUnit; }
         set { ParameterRange.MinValueInDisplayUnit = value; }
      }

      public double? MaxValueInDisplayUnit
      {
         get { return ParameterRange.MaxValueInDisplayUnit; }
         set { ParameterRange.MaxValueInDisplayUnit = value; }
      }

      public Unit Unit
      {
         get { return ParameterRange.Unit; }
         set
         {
            var minDisplayValue = MinValueInDisplayUnit;
            var maxDisplayValue = MaxValueInDisplayUnit;
            ParameterRange.Unit = value;
            MinValueInDisplayUnit = minDisplayValue;
            MaxValueInDisplayUnit = maxDisplayValue;
         }
      }

      public bool IsDiscrete
      {
         get { return ParameterRange.IsAnImplementationOf<DiscreteParameterRange>(); }
      }

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