using OSPSuite.Core.Domain;
using PKSim.Assets;

namespace PKSim.Core.Model
{
   public class OriginData : IWithCalculationMethods, IWithValueOrigin
   {
      public virtual Species Species { get; set; }
      public virtual SpeciesPopulation SpeciesPopulation { get; set; }
      public virtual SubPopulation SubPopulation { get; set; }
      public virtual Gender Gender { get; set; }
      public virtual CalculationMethodCache CalculationMethodCache { get; private set; }
      public ValueOrigin ValueOrigin { get; }

      public virtual double? Age { get; set; }
      public virtual string AgeUnit { get; set; }

      public virtual double? GestationalAge { get; set; }
      public virtual string GestationalAgeUnit { get; set; }

      public virtual double? Height { get; set; }
      public virtual string HeightUnit { get; set; }

      public virtual double? BMI { get; set; }
      public virtual string BMIUnit { get; set; }

      public virtual double Weight { get; set; }
      public virtual string WeightUnit { get; set; }

      public virtual string Comment { get; set; }

      public OriginData()
      {
         CalculationMethodCache = new CalculationMethodCache();
         ValueOrigin = new ValueOrigin();
      }

      public virtual OriginData Clone()
      {
         return new OriginData
         {
            Age = Age,
            AgeUnit = AgeUnit,
            Comment = Comment,
            Height = Height,
            HeightUnit = HeightUnit,
            BMI = BMI,
            BMIUnit = BMIUnit,
            SpeciesPopulation = SpeciesPopulation,
            Gender = Gender,
            Species = Species,
            SubPopulation = SubPopulation,
            Weight = Weight,
            WeightUnit = WeightUnit,
            GestationalAge = GestationalAge,
            CalculationMethodCache = CalculationMethodCache.Clone()
         };
      }

      public override string ToString()
      {
         return PKSimConstants.UI.OriginData;
      }

      public void UpdateValueOriginFrom(ValueOrigin sourceValueOrigin)
      {
         ValueOrigin.UpdateFrom(sourceValueOrigin);
      }
   }
}