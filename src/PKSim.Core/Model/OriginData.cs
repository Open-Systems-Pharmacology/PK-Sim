using System.Collections.Generic;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;

namespace PKSim.Core.Model
{
   public class OriginDataParameter : IWithName
   {
      /// <summary>
      ///    Name of parameter. Can be null if parameter is used as field
      /// </summary>
      public string Name { get; set; }

      /// <summary>
      ///    Value of parameter, always in base unit
      /// </summary>
      public double Value { get; set; }

      /// <summary>
      ///    Unit used When the parameter was entered. This is the unit selected by the user and it not necessarily the base unit
      /// </summary>
      public string Unit { get; set; }

      public OriginDataParameter Clone() => new OriginDataParameter
      {
         Value = Value,
         Unit = Unit,
         Name = Name
      };

      public void Deconstruct(out double value, out string unit)
      {
         value = Value;
         unit = Unit;
      }

      public void Deconstruct(out double value, out string unit, out string name)
      {
         value = Value;
         unit = Unit;
         name = Name;
      }

      public OriginDataParameter()
      {
      }

      public OriginDataParameter(double value, string unit = "", string name = "")
      {
         Name = name;
         Value = value;
         Unit = unit;
      }
   }

   public class OriginData : IWithCalculationMethods, IWithValueOrigin
   {
      private DiseaseState _diseaseState;
      public readonly List<OriginDataParameter> _allDiseaseStateParameters = new List<OriginDataParameter>();
      public Species Species { get; set; }
      public SpeciesPopulation Population { get; set; }
      public SubPopulation SubPopulation { get; set; }
      public Gender Gender { get; set; }
      public CalculationMethodCache CalculationMethodCache { get; private set; }
      public ValueOrigin ValueOrigin { get; }

      public OriginDataParameter Age { get; set; }
      public OriginDataParameter GestationalAge { get; set; }
      public OriginDataParameter Height { get; set; }
      public OriginDataParameter BMI { get; set; }
      public OriginDataParameter Weight { get; set; }

      // Disease state associated with current origin data. null if no disease state associated with origin data (Healthy)
      // This is a reference to the db instance and will not be manipulated
      public DiseaseState DiseaseState
      {
         get => _diseaseState;
         set
         {
            _allDiseaseStateParameters.Clear();
            _diseaseState = value;
         }
      }

      public IReadOnlyList<OriginDataParameter> DiseaseStateParameters => _allDiseaseStateParameters;

      public void AddDiseaseStateParameter(OriginDataParameter diseaseStateParameter) => _allDiseaseStateParameters.Add(diseaseStateParameter);

      public bool IsPreterm
      {
         get
         {
            var gaValue = GestationalAge?.Value;
            return gaValue.GetValueOrDefault(CoreConstants.NOT_PRETERM_GESTATIONAL_AGE_IN_WEEKS) <= CoreConstants.NOT_PRETERM_GESTATIONAL_AGE_IN_WEEKS;
         }
      }

      public bool IsRealPreterm
      {
         get
         {
            //WHY THE DIFFERENCE?
            var gaValue = GestationalAge?.Value;
            return gaValue.GetValueOrDefault(CoreConstants.NOT_PRETERM_GESTATIONAL_AGE_IN_WEEKS) < CoreConstants.NOT_PRETERM_GESTATIONAL_AGE_IN_WEEKS;
         }
      }

      public string Comment { get; set; }

      public OriginData()
      {
         CalculationMethodCache = new CalculationMethodCache();
         ValueOrigin = new ValueOrigin();
         //Weight is the only parameter that should be defined for all species
         Weight = new OriginDataParameter();
      }

      public OriginData Clone()
      {
         var clone = new OriginData
         {
            //Weight is the only required parameter
            Weight = Weight.Clone(),
            Age = Age?.Clone(),
            Comment = Comment,
            Height = Height?.Clone(),
            BMI = BMI?.Clone(),
            Population = Population,
            Gender = Gender,
            Species = Species,
            DiseaseState = DiseaseState,
            SubPopulation = SubPopulation,
            GestationalAge = GestationalAge?.Clone(),
            CalculationMethodCache = CalculationMethodCache.Clone()
         };

         DiseaseStateParameters.Each(x => clone.AddDiseaseStateParameter(x.Clone()));
         clone.UpdateValueOriginFrom(ValueOrigin);
         return clone;
      }

      public override string ToString() => PKSimConstants.UI.OriginData;

      public void UpdateValueOriginFrom(ValueOrigin sourceValueOrigin)
      {
         ValueOrigin.UpdateFrom(sourceValueOrigin);
      }
   }
}