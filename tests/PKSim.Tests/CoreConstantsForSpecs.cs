namespace PKSim
{
   public static class CoreConstantsForSpecs
   {
      public static class ActiveTransport
      {
         public static readonly string ActiveEffluxSpecificMM = "ActiveEffluxSpecific_MM";
         public static readonly string ActiveEffluxSpecificWithCompetitiveInhibitionMM = "ActiveEffluxSpecific_CompetitiveInhibition_MM";
         public static readonly string ActiveEffluxSpecificHill = "ActiveEffluxSpecific_Hill";
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

      public static class Parameter
      {
         public static readonly string KINACT = "kinact";
         public static readonly string EC50 = "EC50";
         public static readonly string BLOOD_PLASMA_CONCENTRATION_RATIO = "Blood/Plasma concentration ratio";
         public static readonly string ENZYME_CONCENTRATION = "Enzyme concentration";
         public static readonly string CL_SPEC_PER_ENZYME = "CLspec/[Enzyme]";
         public static readonly string NUMBER_OF_CELLS_PER_INCUBATION = "Number of cells/incubation";
      }

      public static class Neigborhood
      {
         public static readonly string PERIPORTAL_CELL_GALLBLADDER = "Periportal_cell_Gallbladder";
      }

      public static class Population
      {
         public const string Japanese = "Japanese_Population";
      }
   }
}