using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Utility.Reflection;
using OSPSuite.Utility.Validation;
using PKSim.Assets;

namespace PKSim.Core.Model
{
   public class ParameterRange : Notifier, IValidatable, IUpdatable
   {
      public string ParameterName { get; set; }
      public string ParameterDisplayName { get; set; }
      public double? DbMinValue { get; set; }
      public double? DbMaxValue { get; set; }
      public double? MinValue { get; set; }
      public double? MaxValue { get; set; }
      public IDimension Dimension { get; set; }
      public Unit Unit { get; set; }

      public ParameterRange()
      {
         Rules = new BusinessRuleSet();
         Rules.Add(AllRules.MinLessThanMax);
         Rules.Add(AllRules.MaxGreaterThanMin);
         Rules.Add(AllRules.MinGreaterThanDbMin);
         Rules.Add(AllRules.MaxLessThanDbMax);
         Rules.Add(AllRules.MinLessThanDbMax);
         Rules.Add(AllRules.MaxGreaterThanDbMin);
      }

      public bool IsConstant => MinValue.HasValue && MaxValue.HasValue && MinValue.Value == MaxValue.Value;

      public IBusinessRuleSet Rules { get; }

      public bool IsValueInRange(double valueInBaseUnit)
      {
         if (MinValue.HasValue && valueInBaseUnit < MinValue)
            return false;

         if (MaxValue.HasValue && valueInBaseUnit > MaxValue)
            return false;

         return true;
      }

      public double? MinValueInDisplayUnit
      {
         get => displayValueFrom(MinValue);
         set => MinValue = baseValueFrom(value);
      }

      public double? MaxValueInDisplayUnit
      {
         get => displayValueFrom(MaxValue);
         set => MaxValue = baseValueFrom(value);
      }

      private double? baseValueFrom(double? displayValue)
      {
         if (!displayValue.HasValue)
            return null;

         if (Unit == null || Dimension == null)
            return displayValue;

         return Dimension.UnitValueToBaseUnitValue(Unit, displayValue.Value);
      }

      private double? displayValueFrom(double? baseValue)
      {
         if (!baseValue.HasValue)
            return null;

         if (Unit == null || Dimension == null)
            return baseValue;

         return Dimension.BaseUnitValueToUnitValue(Unit, baseValue.Value);
      }

      public override string ToString()
      {
         return ParameterDisplayName;
      }

      private static class AllRules
      {
         public static IBusinessRule MinLessThanMax
         {
            get
            {
               return CreateRule.For<ParameterRange>()
                  .Property(item => item.MinValueInDisplayUnit)
                  .WithRule(minLessThanMax)
                  .WithError((param, value) => PKSimConstants.Rules.Parameter.MinLessThanMax);
            }
         }

         public static IBusinessRule MaxGreaterThanMin
         {
            get
            {
               return CreateRule.For<ParameterRange>()
                  .Property(item => item.MaxValueInDisplayUnit)
                  .WithRule(maxGreaterThanMin)
                  .WithError((param, value) => PKSimConstants.Rules.Parameter.MaxGreaterThanMin);
            }
         }

         public static IBusinessRule MinGreaterThanDbMin
         {
            get
            {
               return CreateRule.For<ParameterRange>()
                  .Property(item => item.MinValueInDisplayUnit)
                  .WithRule(minGreaterThanDbMin)
                  .WithError((param, value) => PKSimConstants.Rules.Parameter.MinGreaterThanDbMinValue(param.DbMinValue, param.Unit.ToString()));
            }
         }

         public static IBusinessRule MaxLessThanDbMax
         {
            get
            {
               return CreateRule.For<ParameterRange>()
                  .Property(item => item.MaxValueInDisplayUnit)
                  .WithRule(maxLessThanDbMax)
                  .WithError((param, value) => PKSimConstants.Rules.Parameter.MaxLessThanDbMaxValue(param.DbMaxValue, param.Unit.ToString()));
            }
         }

         public static IBusinessRule MinLessThanDbMax
         {
            get
            {
               return CreateRule.For<ParameterRange>()
                  .Property(item => item.MinValueInDisplayUnit)
                  .WithRule(minLessThanDbMax)
                  .WithError((param, value) => PKSimConstants.Rules.Parameter.MinLessThanDbMaxValue(param.DbMaxValue, param.Unit.ToString()));
            }
         }

         public static IBusinessRule MaxGreaterThanDbMin
         {
            get
            {
               return CreateRule.For<ParameterRange>()
                  .Property(item => item.MaxValueInDisplayUnit)
                  .WithRule(maxGreaterThanDbMin)
                  .WithError((param, value) => PKSimConstants.Rules.Parameter.MaxGreaterThanDbMinValue(param.DbMinValue, param.Unit.ToString()));
            }
         }

         private static bool minLessThanMax(ParameterRange param, double? minValueInDisplayUnit)
         {
            if (doesNotNeedCheck(param.MaxValue, minValueInDisplayUnit))
               return true;

            return param.baseValueFrom(minValueInDisplayUnit) <= param.MaxValue;
         }

         private static bool minGreaterThanDbMin(ParameterRange param, double? minValueInDisplayUnit)
         {
            if (doesNotNeedCheck(param.DbMinValue, minValueInDisplayUnit))
               return true;

            return param.baseValueFrom(minValueInDisplayUnit) >= param.DbMinValue;
         }

         private static bool maxGreaterThanDbMin(ParameterRange param, double? maxValueInDisplayUnit)
         {
            if (doesNotNeedCheck(param.DbMinValue, maxValueInDisplayUnit))
               return true;

            return param.baseValueFrom(maxValueInDisplayUnit) >= param.DbMinValue;
         }

         private static bool maxLessThanDbMax(ParameterRange param, double? maxValueInDisplayUnit)
         {
            if (doesNotNeedCheck(param.DbMaxValue, maxValueInDisplayUnit))
               return true;

            return param.baseValueFrom(maxValueInDisplayUnit) <= param.DbMaxValue;
         }

         private static bool minLessThanDbMax(ParameterRange param, double? minValueInDisplayUnit)
         {
            if (doesNotNeedCheck(param.DbMaxValue, minValueInDisplayUnit))
               return true;

            return param.baseValueFrom(minValueInDisplayUnit) <= param.DbMaxValue;
         }

         private static bool maxGreaterThanMin(ParameterRange param, double? maxValueInDisplayUnit)
         {
            if (doesNotNeedCheck(param.MinValue, maxValueInDisplayUnit))
               return true;

            return param.baseValueFrom(maxValueInDisplayUnit) >= param.MinValue;
         }

         private static bool doesNotNeedCheck(double? referenceValue, double? valueToCheck)
         {
            //no constrained defined. OK
            if (!referenceValue.HasValue)
               return true;

            //no value set to check! ok 
            if (!valueToCheck.HasValue)
               return true;

            return false;
         }
      }

      public virtual void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         var sourceParameterRange = source as ParameterRange;
         if (sourceParameterRange == null) return;
         Dimension = sourceParameterRange.Dimension;
         MaxValue = sourceParameterRange.MaxValue;
         MinValue = sourceParameterRange.MinValue;
         DbMaxValue = sourceParameterRange.DbMaxValue;
         DbMinValue = sourceParameterRange.DbMinValue;
         ParameterDisplayName = sourceParameterRange.ParameterDisplayName;
         ParameterName = sourceParameterRange.ParameterName;
         Unit = sourceParameterRange.Unit;
      }

      public virtual ParameterRange Clone()
      {
         var clone = new ParameterRange();
         clone.UpdatePropertiesFrom(this, null);
         return clone;
      }
   }

   public class ConstrainedParameterRange : ParameterRange
   {
      public ConstrainedParameterRange()
      {
         Rules.Add(AllRules.MinShouldBeDefined);
         Rules.Add(AllRules.MaxShouldBeDefined);
      }

      public override ParameterRange Clone()
      {
         var clone = new ConstrainedParameterRange();
         clone.UpdatePropertiesFrom(this, null);
         return clone;
      }

      private static class AllRules
      {
         public static IBusinessRule MinShouldBeDefined
         {
            get
            {
               return CreateRule.For<ParameterRange>()
                  .Property(item => item.MinValue)
                  .WithRule((param, value) => value.HasValue)
                  .WithError((param, value) => PKSimConstants.Rules.Parameter.MinShouldBeDefined);
            }
         }

         public static IBusinessRule MaxShouldBeDefined
         {
            get
            {
               return CreateRule.For<ParameterRange>()
                  .Property(item => item.MaxValue)
                  .WithRule((param, value) => value.HasValue)
                  .WithError((param, value) => PKSimConstants.Rules.Parameter.MaxShouldBeDefined);
            }
         }
      }
   }

   public class DiscreteParameterRange : ConstrainedParameterRange
   {
      public IEnumerable<double> ListOfValues { get; set; }

      public DiscreteParameterRange()
      {
         ListOfValues = new List<double>();
      }

      public override ParameterRange Clone()
      {
         var clone = new DiscreteParameterRange();
         clone.UpdatePropertiesFrom(this, null);
         return clone;
      }

      public override void UpdatePropertiesFrom(IUpdatable source, ICloneManager cloneManager)
      {
         base.UpdatePropertiesFrom(source, cloneManager);
         var sourceDiscreteParameterRange = source as DiscreteParameterRange;
         if (sourceDiscreteParameterRange == null) return;
         ListOfValues = sourceDiscreteParameterRange.ListOfValues;
      }
   }
}