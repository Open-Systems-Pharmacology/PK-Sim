using OSPSuite.Presentation.DTO;
using OSPSuite.Utility.Validation;
using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;

namespace PKSim.Presentation.DTO.Parameters
{
   public class ParameterValuePointDTO : ValuePointDTO
   {
      private readonly IParameter _parameter;

      public ParameterValuePointDTO(IParameter parameter, TableFormula tableFormula, ValuePoint point) : base(tableFormula, point)
      {
         _parameter = parameter;
         Rules.AddRange(AllRules.All());
      }

      private bool validateYValue(double valueInDisplayUnit)
      {
         if (_parameter == null)
            return true;
         var yValue = _tableFormula.Dimension.UnitValueToBaseUnitValue(_tableFormula.YDisplayUnit, valueInDisplayUnit);
         return _parameter.Validate(x => x.Value, yValue).IsEmpty;
      }

      private string errorMessageForYValue(double valueInDisplayUnit)
      {
         if (_parameter == null)
            return string.Empty;
         var yValue = _tableFormula.Dimension.UnitValueToBaseUnitValue(_tableFormula.YDisplayUnit, valueInDisplayUnit);
         return _parameter.Validate(x => x.Value, yValue).Message;
      }

      private static class AllRules
      {
         private static IBusinessRule yValueShouldBeValidAccordingToParameter { get; } = CreateRule.For<ParameterValuePointDTO>()
            .Property(point => point.Y)
            .WithRule((point, valueInDisplayUnit) => point.validateYValue(valueInDisplayUnit))
            .WithError((point, valueInDisplayUnit) => point.errorMessageForYValue(valueInDisplayUnit));

         public static IEnumerable<IBusinessRule> All()
         {
            yield return yValueShouldBeValidAccordingToParameter;
         }
      }
   }
}
