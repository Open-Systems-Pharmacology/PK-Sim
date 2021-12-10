using System.Collections.Generic;
using System.Linq;
using PKSim.Core;
using static PKSim.Core.CoreConstants.Parameters;

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
         public static readonly string FRACTION_ENDOSOMAL = "Fraction endosomal";
         public static readonly string NORM_SUFFIX = " (normalized)";
         public const string VOLUME_FRACTION_LIPIDS = "Vf (lipid)";
         public const string VOLUME_FRACTION_NEUTRAL_LIPID_PT = "Vf (neutral lipid)-PT";
         public const string VOLUME_FRACTION_NEUTRAL_LIPID_RR = "Vf (neutral lipid)-RR";
         public const string VOLUME_FRACTION_NEUTRAL_LIPID_WS = "Vf (neutral lipid)-WS";
         public const string VOLUME_FRACTION_NEUTRAL_PHOSPHOLIPID_RR = "Vf (neutral phospholipid)-RR";
         public const string VOLUME_FRACTION_NEUTRAL_PHOSPHOLIPID_WS = "Vf (neutral phospholipid, plasma)-WS";
         public const string VOLUME_FRACTION_PHOSPHOLIPID_PT = "Vf (phospholipid)-PT";
         public const string VOLUME_FRACTION_PROTEINS = "Vf (protein)";
         public const string VOLUME_FRACTION_PROTEINS_WS = "Vf (protein)-WS";
         public const string VOLUME_FRACTION_WATER = "Vf (water)";
         public const string VOLUME_FRACTION_WATER_PT = "Vf (water)-PT";
         public const string VOLUME_FRACTION_WATER_WS = "Vf (water)-WS";
         public const string VOLUME_FRACTION_WATER_EXTRA_RR = "Vf (extracellular water)-RR";
         public const string VOLUME_FRACTION_WATER_INTRA_RR = "Vf (intracellular water)-RR";
         public const string E_GFR = "eGFR";


         public static readonly IReadOnlyList<string> VolumeFractionLipidsParameters = new[]
         {
            VOLUME_FRACTION_LIPIDS,
            VOLUME_FRACTION_NEUTRAL_LIPID_PT,
            VOLUME_FRACTION_NEUTRAL_LIPID_RR,
            VOLUME_FRACTION_NEUTRAL_LIPID_WS,
            VOLUME_FRACTION_NEUTRAL_PHOSPHOLIPID_RR,
            VOLUME_FRACTION_NEUTRAL_PHOSPHOLIPID_WS,
            VOLUME_FRACTION_PHOSPHOLIPID_PT
         };

         public static readonly IReadOnlyList<string> VolumeFractionProteinsParameters = new[]
         {
            VOLUME_FRACTION_PROTEINS,
            VOLUME_FRACTION_PROTEINS_WS
         };

         public static readonly IReadOnlyList<string> VolumeFractionWaterParameters = new[]
         {
            VOLUME_FRACTION_WATER,
            VOLUME_FRACTION_WATER_PT,
            VOLUME_FRACTION_WATER_WS,
            VOLUME_FRACTION_WATER_EXTRA_RR,
            VOLUME_FRACTION_WATER_INTRA_RR
         };

         /// <summary>
         ///    List of parameters that are indirectly varied by create individual (such as Blood Flow, GFR_spec etc.) and that
         ///    should be displayed in a distribution plot
         /// </summary>
         public static readonly IReadOnlyList<string> DerivedCreatedIndividualParameters = new[]
            {
               BLOOD_FLOW,
               GFR_SPEC,
               VOLUME_PLASMA,
            }
            .Union(OntogenyFactors)
            .Union(AllPlasmaProteinOntogenyFactors).ToList();

         public static string NormParameterFor(string parameter)
         {
            return $"{parameter}{NORM_SUFFIX}";
         }

         public static readonly string REL_EXP_NORM = NormParameterFor(REL_EXP);
         public static readonly string REL_EXP_BLOOD_CELLS_NORM = NormParameterFor(REL_EXP_BLOOD_CELLS);
         public static readonly string REL_EXP_PLASMA_NORM = NormParameterFor(REL_EXP_PLASMA);
         public static readonly string REL_EXP_VASCULAR_ENDOTHELIUM_NORM = NormParameterFor(REL_EXP_VASCULAR_ENDOTHELIUM);

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