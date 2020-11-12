using System;
using System.Collections.Generic;
using OSPSuite.Core.Domain;
using PKSim.Core;

namespace PKSim.Infrastructure.ProjectConverter
{
   internal static class ConverterConstants
   {
      public const double OLD_DEFAULT_ABS_TOL = 1e-9;
      public const double OLD_DEFAULT_REL_TOL = 1e-4;

      public static string LumenSegmentExtensionFor(string lumenSegmentName)
      {
         if (lumenSegmentName.Equals(CoreConstants.Compartment.Stomach))
            return "_sto";

         if (lumenSegmentName.Equals(CoreConstants.Compartment.Duodenum))
            return "_duo";

         if (lumenSegmentName.Equals(CoreConstants.Compartment.UpperJejunum))
            return "_uje";

         if (lumenSegmentName.Equals(CoreConstants.Compartment.LowerJejunum))
            return "_lje";

         if (lumenSegmentName.Equals(CoreConstants.Compartment.UpperIleum))
            return "_uil";

         if (lumenSegmentName.Equals(CoreConstants.Compartment.LowerIleum))
            return "_lil";

         if (lumenSegmentName.Equals(CoreConstants.Compartment.Caecum))
            return "_cae";

         if (lumenSegmentName.Equals(CoreConstants.Compartment.ColonAscendens))
            return "_colasc";

         if (lumenSegmentName.Equals(CoreConstants.Compartment.ColonTransversum))
            return "_coltrans";

         if (lumenSegmentName.Equals(CoreConstants.Compartment.ColonDescendens))
            return "_coldesc";

         if (lumenSegmentName.Equals(CoreConstants.Compartment.ColonSigmoid))
            return "_colsigm";

         if (lumenSegmentName.Equals(CoreConstants.Compartment.Rectum))
            return "_rect";

         throw new ArgumentException(lumenSegmentName + " is not a valid lumen segment");
      }

      public static class Events
      {
         public static readonly string GallbladderEmptying = "GallbladderEmptying";
         public static readonly string EHCStartEvent = "EHC_Start_Event";
      }

      public static class Neighborhoods
      {
         public static readonly string LiverIntToLiverCell = "Liver_int_Liver_cell";
         public static readonly string HeartPlasmaToHeartInterstial = "Heart_pls_Heart_int";
         public static readonly string GallbladderLumenDuo = "Gallbladder_Lumen_duo";
         public static readonly string Periportal_int_Periportal_cell = "Periportal_int_Periportal_cell";
      }

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
         public static readonly string DrugAbsorptionLumenToMucosaRate = "Drug absorption rate (lumen to mucosa)";
         public static readonly string PeripheralBloodFlowFraction = "Peripheral blood flow fraction";
         public static readonly string TotalDrugMass = "TotalDrugMass";
         public static readonly string PlasmaClearance = "Plasma clearance";
         public static readonly string SA_int_cell = "Surface area (interstitial/intracellular)";
         public static readonly string GastricEmptyingTime = "Gastric emptying time";
         public static readonly string LITT = "Large intestinal transit time";
         public static readonly string LITT_factor = "Large intestinal transit time factor";
         public static readonly string LITT_factor_intercept = "Large intestinal transit time factor intercept";
         public static readonly string LITT_factor_slope = "Large intestinal transit time factor slope";
         public static readonly string SITT = "Small intestinal transit time";
         public static readonly string SITT_factor = "Small intestinal transit time factor";
         public static readonly string SITT_factor_intercept = "Small intestinal transit time factor intercept";
         public static readonly string SITT_factor_slope = "Small intestinal transit time factor slope";
         public static readonly string P_endothelial = "P (plasma<->interstitial)";
         public static readonly string CLspec = "Specific clearance";
         public static readonly string ParticleRadiusDissolved = "Immediately dissolve particles smaller than";
         public static readonly string VolumeVascularEndothelium = "Volume (endothelium)";
         public static readonly string GFRspec = "GFR (specific)";
         public static readonly string Default_Volume = "Volume (default)";
         public static readonly string A_to_V_bc = "Surface/Volume ratio (blood cells)";
         public static readonly string ColonArrivalTime = "Colon arrival time";
         public static readonly string TM50 = "TM50 for GFR";
         public static readonly string Hill = "Hill coefficient for GFR";
         public static readonly string ExcretionTime = "Excretion time";
         public static readonly string GFRmat = "GFRmat";
         public static readonly string GET_Alpha_Variability = "GET_alpha (Weibull function) variability factor";
         public static readonly string GET_Beta_Variability = "GET_beta (Weibull function) variability factor";
         public static readonly string SpecificClearance = "Specific clearance";
         public static readonly string EndTime = "EndTime";
         public static readonly string PARAM_k_SA = "SA proportionality factor";
         public static readonly string Kass_FcRn_ligandEndo = "kass (FcRn, endogenous IgG)";
         public static readonly string Kd_FcRn_LigandEndo = "Kd (FcRn, endogenous IgG) in endosomal space";
         public static readonly string Kd_FcRn_Endo = "Kd (FcRn) in endosomal space";
         public static readonly string Kd_FcRn_ligandEndo_pls_int = "Kd (FcRn, endogenous IgG) in plasma/interstitial";
         public static readonly string Start_concentration_FcRn_endosome = "Start concentration of free FcRn (endosome)";
         public static readonly string Start_concentration_endogenous_plasma = "Start concentration of free endogenous IgG (plasma)";
         public static readonly string Kass_FcRn = "kass (FcRn)";
         public static readonly string Kd_FcRn_pls_int = "Kd (FcRn) in plasma/interstitial";
         public static readonly string BP_AGP = "BP_AGP";
         public static readonly string BP_ALBUMIN = "BP_ALBUMIN";
         public static readonly string BP_UNKNOWN = "BP_UNKNOWN";
         public static readonly string FractionUnboundPlasma = "Fraction unbound (plasma)";
         public static readonly string GET_Alpha_variability_factor = "GET_alpha (Weibull function) variability factor";
         public static readonly string GET_Beta_variability_factor = "GET_beta (Weibull function) variability factor";
         public static readonly string Lipophilicity = "Lipophilicity";
         public static readonly string BloodPlasmaConcentrationRatio = "Blood/Plasma concentration ratio";
         public static readonly string PartitionCoefficientWwaterProtein = "Partition coefficient (water/protein)";
         public static readonly string Gallbladder_emptying_rate = "Gallbladder emptying rate";
         public static readonly string Gallbladder_emptying_active = "Gallbladder emptying active";
         public static readonly string EffectiveSurfaceAreaVariabilityFactor = "Effective surface area variability factor";
         public static readonly string EffectiveSurfaceArea = "Effective surface area";
         public static readonly string RenalAgingScaleFactor = "Renal aging scaling factor";
         public static readonly string RESIDUAL_FRACTION = "Residual fraction";
         public static readonly string ScalingExponentForFluidRecircFlowRate = "Scaling exponent for fluid recirculation flow rate";
         public static readonly string TabletTimeDelayFactor = "Tablet time delay factor";
         public static readonly string REL_EXP_BLOOD_CELL_NORM = NormParameterFor(CoreConstants.Parameters.REL_EXP_BLOOD_CELLS);
         public static readonly string REL_EXP_PLASMA_NORM = NormParameterFor(CoreConstants.Parameters.REL_EXP_PLASMA);
         public static readonly string REL_EXP_VASC_ENDO_NORM = NormParameterFor(CoreConstants.Parameters.REL_EXP_VASC_ENDO);
         public static readonly string REL_EXP_NORM = NormParameterFor(CoreConstants.Parameters.REL_EXP);
         public static readonly string FRACTION_ENDOSOMAL = "Fraction endosomal";

         public static string NormParameterFor(string parameter)
         {
            return $"{parameter}{CoreConstants.Parameters.NORM_SUFFIX}";
         }


         public static IList<string> AllCompoundGlobalParameters => new List<string>
         {
            BloodPlasmaConcentrationRatio,
            PartitionCoefficientWwaterProtein,
            Constants.Parameters.USE_PENALTY_FACTOR,
         };

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

      public static class ContainerName
      {
         public static readonly string IGG_ENDOSOME = "IgG_Endosome";
         public static readonly string ENDOSOMAL_CLEARANCE = "EndosomalClearance";
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