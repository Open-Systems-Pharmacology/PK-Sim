using System.Collections.Generic;
using System.Linq;

namespace PKSim
{
   public static class CoreConstantsForSpecs
   {
      public const string ExperimentalFeature = "(experimental)";

      public static class ActiveTransport
      {
         public static readonly string ActiveEffluxSpecificIntracellularToInterstitial = "ActiveEffluxSpecificIntracellularToInterstitial";
         public static readonly string ActiveEffluxSpecificIntracellularToInterstitial_MM = "ActiveEffluxSpecificIntracellularToInterstitial_MM";
         public static readonly string ActiveInfluxSpecificInterstitialToIntracellular_MM = "ActiveInfluxSpecificInterstitialToIntracellular_MM";
         public static readonly string ActiveEffluxSpecificWithCompetitiveInhibitionMM = "ActiveEffluxSpecific_CompetitiveInhibition_MM";
         public static readonly string ActiveEffluxSpecificIntracellularToInterstitial_Hill = "ActiveEffluxSpecificIntracellularToInterstitial_Hill";
      }

      public static class Process
      {
         public static readonly string LIVER_CLEARANCE = "LiverClearance";
         public static readonly string BILIARY_CLEARANCE = "BiliaryClearance";
         public static readonly string ACTIVE_TRANSPORT_SPECIFIC_MM = "ActiveTransportSpecific_MM";
         public static readonly string LIVER_ACTIVE_EFFLUX_TO_BILE_MM = "LiverActiveEffluxToBile_MM";
         public static readonly string GLOMERULAR_FILTRATION = "GlomerularFiltration";
         public static readonly string METABOLIZATION_SPECIFIC_FIRST_ORDER = "MetabolizationSpecific_FirstOrder";
         public static readonly string METABOLIZATION_LIVER_MICROSOME_FIRST_ORDER = "MetabolizationLiverMicrosomes_FirstOrder";
         public static readonly string LIVER_MICROSOME_RES = "LiverMicrosomeRes";
         public static readonly string ACTIVE_TRANSPORT_HILL = "ActiveTransportSpecific_Hill";
         public static readonly string COMPETITIVE_INHIBITION = "CompetitiveInhibition";
         public static readonly string UNCOMPETITIVE_INHIBITION = "UncompetitiveInhibition";
         public static readonly string NONCOMPETITIVE_INHIBITION = "NoncompetitiveInhibition";
         public static readonly string MIXED_INHIBITION = "MixedInhibition";
         public static readonly string IRREVERSIBLE_INHIBITION = "IrreversibleInhibition";
         public static readonly string INDUCTION = "Induction";
         public static readonly string HEPATOCYTESHALFTIME = "HepatocytesHalfTime";
         public static readonly string HEPATOCYTESRES = "HepatocytesRes";
         public static readonly string LIVERMICROSOMEHALFTIME = "LiverMicrosomeHalfTime";
         public static readonly string LIVERMICROSOMERES = "LiverMicrosomeRes";
      }

      public static class Events
      {
         public const string URINARY_BLADDER_EMPTYING = "Urinary bladder emptying";
      }
      public static class Parameters
      {
         public const string KINACT = "kinact";
         public const string EC50 = "EC50";
         public const string BLOOD_PLASMA_CONCENTRATION_RATIO = "Blood/Plasma concentration ratio";
         public const string ENZYME_CONCENTRATION = "Enzyme concentration";
         public const string CL_SPEC_PER_ENZYME = "CLspec/[Enzyme]";
         public const string NUMBER_OF_CELLS_PER_INCUBATION = "Number of cells/incubation";
         public const string INTESTINAL_TRANSIT_RATE_ABSOLUTE = "Intestinal transit rate (absolute)";
         public const string DRUG_MASS = "DrugMass";
         public const string PARTICLE_RADIUS_DISSOLVED = "Immediately dissolve particles smaller than";
         public const string THICKNESS_WATER_LAYER = "Thickness (unstirred water layer)";
         public const string INFUSION_TIME = "Infusion time";
         public const string START_TIME = "Start time";
         public const string VOLUME_OF_WATER_PER_BODYWEIGHT = "Volume of water/body weight";
         public const string HILL_COEFFICIENT = "Hill coefficient";
         public const string IN_VITRO_CL_FOR_LIVER_MICROSOMES = "In vitro CL for liver microsomes";
         public const string IN_VITRO_CL_FOR_RECOMBINANT_ENZYMES = "In vitro CL/recombinant enzyme";
         public const string IN_VITRO_VMAX_FOR_LIVER_MICROSOMES = "In vitro Vmax for liver microsomes";
         public const string IN_VITRO_VMAX_FOR_RECOMBINANT_ENZYMES = "In vitro Vmax/recombinant enzyme";
         public const string IN_VITRO_VMAX_FOR_TRANSPORTER = "In vitro Vmax/transporter";
         public const string INTRINSIC_CLEARANCE = "Intrinsic clearance";
         public const string KD = "Kd";
         public const string KM = "Km";
         public const string KOFF = "koff";
         public const string MEASURING_TIME = "Measuring time";
         public const string PLASMA_CLEARANCE = "Plasma clearance";
         public const string HALF_LIFE_HEPATOCYTE_ASSAY = "t1/2 (hepatocyte assay)";
         public const string HALF_LIFE_MICROSOMAL_ASSAY = "t1/2 (microsomal assay)";
         public const string TRANSPORTER_CONCENTRATION = "Transporter concentration";
         public const string TS_MAX = "TSmax";
         public const string TUBULAR_SECRETION = "Tubular secretion";
         public const string VMAX = "Vmax";
         public const string VMAX_LIVER_TISSUE = "Vmax (liver tissue)";

      }

      public static class ContainerName
      {
         public static readonly string IRREVERSIBLE_INHIBITION = "IrreversibleInhibition";
         
         public static IReadOnlyList<string> PregnancyOrgansWithBloodFlow = new[]
         {
            "Breasts", 
            "Endometrium", 
            "Myometrium", 
            "PlacentaFetal", 
            "PlacentaMaternal"
         };

         public static IReadOnlyList<string> PregnancyOrgans = PregnancyOrgansWithBloodFlow.Concat(new[]
         {
            "AmnioticFluid",
            "ArterialBlood_UmbilicalCord",
            "Fetus",
            "VenousBlood_UmbilicalCord"
         }).ToList().AsReadOnly();

      }

      public static class Neigborhood
      {
         public static readonly string PERIPORTAL_CELL_GALLBLADDER = "Periportal_cell_Gallbladder";
      }

      public static class Population
      {
         public const string Japanese = "Japanese_Population";
      }

      public static class Observer
      {
         public static readonly string WHOLE_ORGAN_INCLUDING_FCRN_COMPLEX = "Whole Organ incl. FcRn_Complex";
      }
   }
}