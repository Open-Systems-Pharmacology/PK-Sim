using System.Collections.Generic;
using PKSim.Core;

namespace PKSim.Infrastructure.ProjectConverter
{
   internal static class ConverterConstants
   {
      public static class Serialization
      {
         public static readonly string DATA_TABLE_COLUMN = "DataTableColumn";

         public static class Attribute
         {
            public static readonly string ORGAN_TYPE = "organType";
         }
      }

      public static class Parameters
      {
         public static readonly string PeripheralBloodFlowFraction = "Peripheral blood flow fraction";
         public static readonly string PlasmaClearance = "Plasma clearance";
         public static readonly string GastricEmptyingTime = "Gastric emptying time";
         public static readonly string SITT = "Small intestinal transit time";
         public static readonly string CLspec = "Specific clearance";
         public static readonly string GFRspec = "GFR (specific)";
         public static readonly string Default_Volume = "Volume (default)";
         public static readonly string TM50 = "TM50 for GFR";
         public static readonly string Hill = "Hill coefficient for GFR";
         public static readonly string GFRmat = "GFRmat";
         public static readonly string GET_Alpha_Variability = "GET_alpha (Weibull function) variability factor";
         public static readonly string GET_Beta_Variability = "GET_beta (Weibull function) variability factor";
         public static readonly string BloodPlasmaConcentrationRatio = "Blood/Plasma concentration ratio";
         public static readonly string PartitionCoefficientWwaterProtein = "Partition coefficient (water/protein)";
         public static readonly string EffectiveSurfaceAreaVariabilityFactor = "Effective surface area variability factor";
         public static readonly string RenalAgingScaleFactor = "Renal aging scaling factor";
         public static readonly string RESIDUAL_FRACTION = "Residual fraction";
         public static readonly string ScalingExponentForFluidRecircFlowRate = "Scaling exponent for fluid recirculation flow rate";
         public static readonly string TabletTimeDelayFactor = "Tablet time delay factor";
         public static readonly string REL_EXP_NORM = NormParameterFor(CoreConstants.Parameters.REL_EXP);
         public static readonly string FRACTION_ENDOSOMAL = "Fraction endosomal";
         public static readonly string NORM_SUFFIX = " (normalized)";

         public static string NormParameterFor(string parameter)
         {
            return $"{parameter}{NORM_SUFFIX}";
         }


         public static IList<string> DistributedParametersWithOnlyOneSupportingPoint => new List<string>
         {
            Hill,
            TM50,
            GFRmat,
            GET_Alpha_Variability,
            GET_Beta_Variability,
            GastricEmptyingTime,
            SITT,
            EffectiveSurfaceAreaVariabilityFactor
         };

      }

      public static class CalculationMethod
      {
         public static readonly string RR = "Cellular partition coefficient method - Rodgers and Rowland";
         public static readonly string BER = "Cellular partition coefficient method - Berezhkovskiy";
         public static readonly string PKSim = "Cellular partition coefficient method - PK-Sim Standard";
         public static readonly string BSA_Mosteller = "Body surface area - Mosteller";
         public static readonly string DynamicSumFormulas = "DynamicSumFormulas";
      }

      public static class Category
      {
         public static readonly string BSA = "BSA";
         public static readonly string DynamicFormulas = "DynamicFormulas";
      }

      public static class Population
      {
         public const string RACE = "Race";
      }
   }
}