using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.DTO;
using OSPSuite.Utility.Validation;
using PKSim.Assets;

namespace PKSim.Presentation.DTO
{
   public class ValuePointDTO : DxValidatableDTO
   {
      private readonly IParameter _parameter;
      private readonly TableFormula _tableFormula;

      private double _x;

      //x value in display unit of the table formula
      public double X
      {
         get => _x;
         set => SetProperty(ref _x, value);
      }

      private double _y;

      //y value in display unit of the table formula
      public double Y
      {
         get => _y;
         set => SetProperty(ref _y, value);
      }

      public ValuePointDTO(IParameter parameter, TableFormula tableFormula, ValuePoint point)
      {
         _parameter = parameter;
         _tableFormula = tableFormula;
         X = convertToDisplayUnit(tableFormula.XDimension, tableFormula.XDisplayUnit, point.X);
         Y = convertToDisplayUnit(tableFormula.Dimension, tableFormula.YDisplayUnit, point.Y);
         Rules.AddRange(AllRules.All());
      }

      private double convertToDisplayUnit(IDimension dimension, Unit displayUnit, double value)
      {
         return dimension.BaseUnitValueToUnitValue(displayUnit, value);
      }

      private bool validateYValue(double valueInDisplayUnit)
      {
         double yValue = _tableFormula.Dimension.UnitValueToBaseUnitValue(_tableFormula.YDisplayUnit, valueInDisplayUnit);
         return _parameter.Validate(x => x.Value, yValue).IsEmpty;
      }

      private string errorMessageForYValue(double valueInDisplayUnit)
      {
         double yValue = _tableFormula.Dimension.UnitValueToBaseUnitValue(_tableFormula.YDisplayUnit, valueInDisplayUnit);
         return _parameter.Validate(x => x.Value, yValue).Message;
      }

      private static class AllRules
      {
         private static IBusinessRule yValueShouldBeValidAccordingToParameter { get; } = CreateRule.For<ValuePointDTO>()
            .Property(point => point.Y)
            .WithRule((point, valueInDisplayUnit) => point.validateYValue(valueInDisplayUnit))
            .WithError((point, valueInDisplayUnit) => point.errorMessageForYValue(valueInDisplayUnit));

         private static IBusinessRule xValueShouldBeGreatherOrEqualThanZero { get; } = CreateRule.For<ValuePointDTO>()
            .Property(point => point.X)
            .WithRule((point, value) => value >= 0)
            .WithError((point, value) => PKSimConstants.Rules.Parameter.ValueShouldBeGreaterThanOrEqualToZero(point._tableFormula.XName));

         public static IEnumerable<IBusinessRule> All()
         {
            yield return xValueShouldBeGreatherOrEqualThanZero;
            yield return yValueShouldBeValidAccordingToParameter;
         }
      }
   }
}