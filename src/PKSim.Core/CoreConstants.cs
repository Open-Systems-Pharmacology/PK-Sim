using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using static OSPSuite.Core.Domain.Constants;

namespace PKSim.Core
{
   public static class CoreConstants
   {
      public const int LAYOUT_VERSION = 38;
      public const int DEFAULT_NUMBER_OF_BINS = 20;
      public const int DEFAULT_NUMBER_OF_INDIVIDUALS_PER_BIN = 100;
      public const int DEFAULT_NUMBER_OF_INDIVIDUALS_IN_POPULATION = 100;

      public const uint RESOLUTION_FOR_FORMULATION_PLOT = 20;
      public const uint DEFAULT_DECIMAL_PLACE = 2;
      public const uint DEFAULT_MRU_LIST_ITEM_COUNT = 10;

      public const double NOT_PRETERM_GESTATIONAL_AGE_IN_WEEKS = 40;
      public const double DEFAULT_PERCENTILE = 0.5;
      public const double DEFAULT_ABS_TOL = 1e-10;
      public const double DEFAULT_REL_TOL = 1e-5;
      public const double HIGH_RESOLUTION_END_TIME_IN_MIN = 120;    //2 hours
      public const double HIGH_RESOLUTION_IN_PTS_PER_MIN = 1.0 / 3; //20 pts per hours
      public const double LOW_RESOLUTION_IN_PTS_PER_MIN = 1.0 / 15; //4 pts per hours
      public const double DEFAULT_PROTOCOL_END_TIME_IN_MIN = 1440;
      public const double DEFAULT_ONTOGENY_FACTOR = 1;
      public const double DEFAULT_DISEASE_FACTOR = 1;
      public const double DEFAULT_REFERENCE_CONCENTRATION_VALUE = 1;
      public const double DEFAULT_MOLECULE_HALF_LIFE_LIVER_VALUE_IN_MIN = 36 * 60;
      public const double DEFAULT_MOLECULE_HALF_LIFE_INTESTINE_VALUE_IN_MIN = 23 * 60;
      public const double DEFAULT_MIN_PERCENTILE = 0.0001;
      public const double DEFAULT_MAX_PERCENTILE = 0.9999;
      public const string DEFAULT_TEMPLATE_VERSION = "1.0";
      public const string DEFAULT_FORMULATION_KEY = "Formulation";
      public const string DEFAULT_CALCULATION_METHODS_FILE_NAME_FOR_MOBI = "AllCalculationMethods";
      public const string DEFAULT_EXPRESSION_PROFILE_MOLECULE_NAME = "<MOLECULE>";
      public const TransportType DEFAULT_TRANSPORTER_TYPE = TransportType.Efflux;

      public const int NUMBER_OF_PKA_PARAMETERS = 3;

      public static readonly double[] DEFAULT_STATISTIC_PERCENTILES = {10, 25, 30, 37, 50, 63, 70, 75, 90};

      //DO NOT rename this constant (otherwise Matlab-Toolbox must be adjusted)
      public static readonly IEnumerable<int> PretermRange = Enumerable.Range(24, 17);

      public static class Filter
      {
         public const string PROJECT_EXTENSION = ".pksim5";
         public const string PROJECT_OLD_EXTENSION = ".pkprj";
         public const string MDB_EXTENSION = ".mdb";
         public const string MODEL_DB_EXTENSION = ".sqlite";
         public const string USER_TEMPLATE_DB_EXTENSION = ".templateDbUser";
         public const string SYSTEM_TEMPLATE_DB_EXTENSION = ".templateDbSystem";
         public const string GENE_DB_EXTENSION = ".expressionDb";
         public static readonly string MARKDOWN_EXTENSION = ".md";
         public static readonly string PROJECT_FILTER = $"*{PROJECT_EXTENSION}";
         public static readonly string SIMULATION_RESULTS_FILTER = $"*{Constants.Filter.CSV_EXTENSION}";
         public static readonly string EXPRESSION_DATABASE_FILE_FILTER = Constants.Filter.FileFilter("Gene Expression Database", GENE_DB_EXTENSION);

         public static readonly string TEMPLATE_DATABASE_FILE_FILTER =
            Constants.Filter.FileFilter("User Template Database", USER_TEMPLATE_DB_EXTENSION);

         public static readonly string MOBI_FILE_FILTER = Constants.Filter.FileFilter("MoBi", ".exe");

         public static readonly string POPULATION_FILE_FILTER = string.Format("Population File (*{0};*{1})|*{0};*{1}", Constants.Filter.CSV_EXTENSION,
            Constants.Filter.TEXT_EXTENSION);

         public static readonly string LOAD_PROJECT_FILTER = string.Format("{0} Project (*{1};*{2})|*{1};*{2}", PRODUCT_NAME_WITH_TRADEMARK,
            PROJECT_EXTENSION, PROJECT_OLD_EXTENSION);

         public static readonly string SAVE_PROJECT_FILTER = Constants.Filter.FileFilter($"{PRODUCT_NAME_WITH_TRADEMARK} Project", PROJECT_EXTENSION);
      }

      public static readonly string APPLICATION_FOLDER_PATH = @"Open Systems Pharmacology\PK-Sim";
      public static readonly string REMOTE_FOLDER_PATH = "Templates";

      public static readonly string TEMPLATE_SYSTEM_DATABASE = $"PKSimTemplateDBSystem{Filter.SYSTEM_TEMPLATE_DB_EXTENSION}";
      public static readonly string TEMPLATE_USER_DATABASE = $"PKSimTemplateDBUser{Filter.USER_TEMPLATE_DB_EXTENSION}";
      public static readonly string PK_SIM_DB_FILE = $"PKSimDB{Filter.MODEL_DB_EXTENSION}";
      public static readonly string TEMPLATE_USER_DATABASE_TEMPLATE = "PKSimTemplateDBUser.template";
      public static readonly string REMOTE_TEMPLATE_SUMMARY = "templates.json";
      public const string PRODUCT_NAME = "PK-Sim";
      public const string TEMPLATES_PRODUCT_NAME = "BuildingBlockTemplates";
      public static readonly string PRODUCT_NAME_WITH_TRADEMARK = "PK-Sim®";
      public static readonly string DEFAULT_SKIN = "Office 2013 Light Gray";
      public static readonly string VALUE_PROPERTY_NAME = "Value";
      public static readonly string PROJECT_UNDEFINED = "Undefined";
      
      public static readonly string VERSION_FILE_URL = "https://raw.githubusercontent.com/Open-Systems-Pharmacology/Suite/master/versions.json";
      public static readonly string REMOTE_TEMPLATE_FILE_URL = "https://raw.githubusercontent.com/Open-Systems-Pharmacology/OSPSuite.BuildingBlockTemplates/main/templates.json";
      public static readonly string ISSUE_TRACKER_URL = "https://github.com/open-systems-pharmacology/pk-sim/issues";

      public static readonly string TEMPLATE_DATABASE_CONVERSION_WIKI_URL =
         "https://github.com/Open-Systems-Pharmacology/OSPSuite.Documentation/wiki/Converting-User-Template-Database";

      public const string APPLICATION_NAME_TEMPLATE = "Application_";

      //tolerated precision to relatively compare to double values 
      public const double DOUBLE_RELATIVE_EPSILON = 1e-2;

      //not as readonly as the text will be updated with the current version
      public static string ProductDisplayName = PRODUCT_NAME_WITH_TRADEMARK;

      public static string DefaultResultsExportNameFor(string simulationName) => $"{simulationName}-Results";

      public static string DefaultPKAnalysesExportNameFor(string simulationName) => $"{simulationName}-PK-Analyses";

      public static string DefaultPopulationExportNameFor(string containerName) => $"{containerName}-Population";

      public static class DirectoryKey
      {
         public static readonly string BATCH_INPUT = "BatchInput";
         public static readonly string BATCH_OUTPUT = "BatchOutput";
         public static readonly string DATABASE = "Database";
      }

      public static class Units
      {
         public static readonly string KgPerM2 = "kg/m²";
         public static readonly string Kg = "kg";
         public static readonly string m2 = "m²";
         public static readonly string cm = "cm";
         public static readonly string mg = "mg";
         public static readonly string MicroMolPerLiter = "µmol/l";
         public static readonly string Percent = "%";
         public static readonly string Days = "day(s)";
         public static readonly string Weeks = "week(s)";
         public static readonly string Years = "year(s)";
         public static readonly string KgPerKg = "kg/kg";
         public static readonly string MgPerKg = "mg/kg";
         public static readonly string MgPerM2 = "mg/m²";
      }

      public static class Observer
      {
         public static readonly string FRACTION_OF_DOSE = "Fraction of dose";
         public static readonly string TOTAL_FRACTION_OF_DOSE = "Total fraction of dose";
         public static readonly string RECEPTOR_OCCUPANCY = "Receptor Occupancy";
         public static readonly string FRACTION_EXCRETED = "Fraction excreted";
         public static readonly string FRACTION_EXCRETED_TO_URINE = $"{FRACTION_EXCRETED} to urine";
         public static readonly string FRACTION_EXCRETED_TO_BILE = $"{FRACTION_EXCRETED} to bile";
         public static readonly string FRACTION_EXCRETED_TO_FECES = $"{FRACTION_EXCRETED} to feces";
         public static readonly string CONCENTRATION_IN_CONTAINER = "Concentration in container";
         public static readonly string PLASMA_PERIPHERAL_VENOUS_BLOOD = "Plasma (Peripheral Venous Blood)";
         public static readonly string TISSUE = "Tissue";
         public static readonly string WHOLE_BLOOD = "Whole Blood";
         public static readonly string PLASMA_UNBOUND = "Plasma Unbound";
         public static readonly string INTERSTITIAL_UNBOUND = "Interstitial Unbound";
         public static readonly string INTRACELLULAR_UNBOUND = "Intracellular Unbound";
         public static readonly string FRACTION_SOLID_PREFIX = "Fraction solid";
         public static readonly string FRACTION_DISSOLVED_PREFIX = "Fraction dissolved";
         public static readonly string FRACTION_INSOLUBLE_PREFIX = "Fraction insoluble";
         public static readonly string FABS_ORAL = "Fraction of oral drug mass absorbed into mucosa";
         public static readonly string PLASMA_UNBOUND_PERIPHERAL_VENOUS_BLOOD = "Plasma Unbound (Peripheral Venous Blood)";
         public static readonly string CONCENTRATION_IN_LUMEN = "Concentration in lumen";
         public static readonly string CONCENTRATION_IN_FECES = "Concentration in feces";

         public static IReadOnlyCollection<string> FractionObservers => new[]
         {
            FRACTION_EXCRETED, FRACTION_EXCRETED_TO_URINE, FRACTION_EXCRETED_TO_BILE, FRACTION_EXCRETED_TO_FECES, FRACTION_OF_DOSE, RECEPTOR_OCCUPANCY
         };

         public static string ObserverNameFrom(string observerName, string compoundName) => CompositeNameFor(observerName, compoundName);
      }

      public static class Rate
      {
         public static readonly string TableFormulaWithOffsetPrefix = "TableFormulaWithOffset_";
         public static readonly string TableFormulaWithXArgumentPrefix = "TableFormulaWithXArgument_";
         public static readonly string APPLICATION_DOSE_FROM_DOSE_PER_BODY_SURFACE_AREA = "PARAM_Application_DoseFromDosePerBodySurfaceArea";
         public static readonly string APPLICATION_DOSE_FROM_DOSE_PER_BODY_WEIGHT = "PARAM_Application_DoseFromDosePerBodyWeight";
         public static readonly string INITIAL_CONCENTRATION_BLOOD_CELLS = "InitialConcentrationBloodCells";
         public static readonly string INITIAL_CONCENTRATION_BLOOD_CELLS_TRANSPORTER = "InitialConcentrationBloodCells_Transporter";
         public static readonly string INITIAL_CONCENTRATION_INTRACELLULAR = "InitialConcentrationIntracellular";
         public static readonly string INITIAL_CONCENTRATION_INTRACELLULAR_TRANSPORTER = "InitialConcentrationIntracellular_Transporter";
         public static readonly string INITIAL_CONCENTRATION_INTERSTITIAL = "InitialConcentrationInterstitial";
         public static readonly string INITIAL_CONCENTRATION_INTERSTITIAL_TRANSPORTER = "InitialConcentrationInterstitial_Transporter";
         public static readonly string INITIAL_CONCENTRATION_PLASMA = "InitialConcentrationPlasma";
         public static readonly string INITIAL_CONCENTRATION_PLASMA_VASCULAR_SYSTEM = "InitialConcentrationPlasma_VascularSystem";
         public static readonly string INITIAL_CONCENTRATION_ENDOSOME = "InitialConcentrationEndosome";
         public static readonly string INITIAL_CONCENTRATION_PLASMA_TRANSPORTER = "InitialConcentrationPlasma_Transporter";
         public static readonly string INITIAL_CONCENTRATION_BRAIN_PLASMA_TRANSPORTER = "InitialConcentrationBrainPlasma_Transporter";
         public static readonly string INITIAL_CONCENTRATION_BRAIN_INTERSTITIAL_TRANSPORTER = "InitialConcentrationBrainInterstitial_Transporter";
         public static readonly string INITIAL_CONCENTRATION_LUMEN = "InitialConcentrationLumen";
         public static readonly string ONE_RATE = "One_Rate";
         public static readonly string ZERO_RATE = "Zero_Rate";
         public static readonly string PARAM_F_EXP_BC_MEMBRANE = "PARAM_f_exp_bc_membrane";
         public static readonly string PARAM_F_EXP_VASC_TISSUE_SIDE = "PARAM_f_exp_vasc_tissue_side";
         public static readonly string PARAM_F_EXP_INTERSTITIAL = "PARAM_f_exp_interstitial";
         public static readonly string PARAM_F_EXP_BASOLATERAL = "PARAM_f_exp_basolateral";
         public static readonly string PARAM_F_EXP_BRN_TISSUE = "PARAM_f_exp_brn_tissue";
         public static readonly string ONTOGENY_FACTOR_FROM_TABLE = "TableFormulaWithXArgument_OntogenyFactor";
         public static readonly string ONTOGENY_FACTOR_GI_FROM_TABLE = "TableFormulaWithXArgument_OntogenyFactorGI";
      }

      public static class Alias
      {
         public static readonly string TABLE = "Table";
         public static readonly string XARG = "XArg";
         public static readonly string OFFSET = "Offset";
         public static readonly string COMPETITIVE_INHIBITION_KI = "KcI";
         public static readonly string UNCOMPETITIVE_INHIBITION_KI = "KuI";
         public static readonly string NON_COMPETITIVE_INHIBITION_KI = "KnI";
         public static readonly string MIXED_COMPETITIVE_INHIBITION_KI = "KmcI";
         public static readonly string MIXED_UNCOMPETITIVE_INHIBITION_KI = "KmuI";
         public static readonly string IRREVERSIBLE_INHIBITION_KI = "KcTDI";
         public static readonly string COMPETITIVE_INHIBITION_I = "Ic";
         public static readonly string UNCOMPETITIVE_INHIBITION_I = "Iu";
         public static readonly string NON_COMPETITIVE_INHIBITION_I = "In";
         public static readonly string MIXED_INHIBITION_I = "Im";
         public static readonly string MIXED_COMPETITIVE_INHIBITION_I = "Im";
         public static readonly string IRREVERSIBLE_INHIBITION_I = "IcTD";
         public static readonly string COMPETITIVE_INHIBITION_K_WATER = "K_water_c";
         public static readonly string UNCOMPETITIVE_INHIBITION_K_WATER = "K_water_u";
         public static readonly string NON_COMPETITIVE_INHIBITION_K_WATER = "K_water_n";
         public static readonly string MIXED_INHIBITION_K_WATER = "K_water_m";
         public static readonly string IRREVERSIBLE_INHIBITION_K_WATER = "K_water_cTD";
      }

      public static class Application
      {
         public static class Name
         {
            public static readonly string Oral = "Oral";
            public static readonly string Intravenous = "Intravenous";
            public static readonly string IntravenousBolus = "IntravenousBolus";
            public static readonly string UserDefined = "UserDefined";
         }

         public static class Route
         {
            public static readonly string Oral = "Oral";
            public static readonly string UserDefined = "UserDefined";
            public static readonly string Intravenous = "Intravenous";
         }
      }

      public static class Category
      {
         public static readonly string DynamicFormulas = "DynamicFormulas";
         public static readonly string DistributionCellular = "DistributionCellular";
         public static readonly string DiffusionIntCell = "DiffusionIntCell";
         public static readonly string IntestinalPermeability = "IntestinalPermeability";
         public static readonly string DistributionInterstitial = "DistributionInterstitial";
      }

      public static class ParameterValueVersion
      {
         public static readonly string IndividualPKSim = "Individual_PKSim";
      }

      public static class CalculationMethod
      {
         public static readonly string LINKS_COMMON = "LinksCommon";
         public static readonly string DISTRIBUTION_IN_VITRO_PKSIM = "DistributionInVitro_PKSim";
         public static readonly string APPLICATION_PARAMETER = "ApplicationParameter";
         public static readonly string APPLICATION_PARAMETER_HUMAN = "ApplicationParameter_Human";
         public static readonly string SCHEMA_ITEM_PARAMETER = "SchemaItemParameter";
         public static readonly string COMPOUND_COMMON = "CompoundCommon";
         public static readonly string COMPOUND_MW_PKSIM = "CompoundMW_PKSim";
         public static readonly string COMPOUND_ACID_BASE_PKSIM = "CompoundAcidBase_PKSim";
         public static readonly string PLASMA_CLEARANCE_PKSIM = "PlasmaClearance_PKSim";
         public static readonly string DIFFUSION_COMMON_PKSIM = "DiffusionCommon_PKSim";
         public static readonly string INTESTINAL_PERMEABILITY_COMMON_PKSIM = "IntestinalPermeabilityCommon_PKSim";
         public static readonly string FORMULATION = "Formulation";
         public static readonly string SPECIFIC_CLEARANCE = "SpecificClearance_PKSim";
         public static readonly string EVENTS = "Events";
         public static readonly string INDIVIDUAL = "Individual";
         public static readonly string ACTIVE_PROCESS = "ActiveProcess_PKSim";
         public static readonly string FORMULATION_PARTICLES = "FormulationParticles";
         public static readonly string BLACK_BOX = "BlackBox_CalculationMethod";
         public static readonly string DYNAMIC_SUM_FORMULAS = "DynamicSumFormulas";
         public static readonly string RODGER_AND_ROWLAND = "Cellular partition coefficient method - Rodgers and Rowland";
         public static readonly string RENAL_AGING_HUMAN = "Renal_Aging_Human";
         public static readonly string RENAL_AGING_ANIMALS = "Renal_Aging_Animals";
         public static readonly string EXPRESSION_PARAMETERS = "ExpressionParameters";
         public static readonly string ONTOGENY_FACTORS = "OntogenyFactors";
         public static readonly string DISEASE_STATES = "DiseaseStates";

         public static readonly IReadOnlyList<string> ForDiseaseStates = new List<string>
         {
            DISEASE_STATES
         };

         public static readonly IReadOnlyList<string> ForProcesses = new List<string>
         {
            LINKS_COMMON, 
            SPECIFIC_CLEARANCE, 
            DISTRIBUTION_IN_VITRO_PKSIM,
            DYNAMIC_SUM_FORMULAS
         };

         public static readonly IReadOnlyList<string> ForCompounds = new List<string>
         {
            COMPOUND_COMMON,
            COMPOUND_MW_PKSIM,
            COMPOUND_ACID_BASE_PKSIM,
            DISTRIBUTION_IN_VITRO_PKSIM,
            PLASMA_CLEARANCE_PKSIM,
            DIFFUSION_COMMON_PKSIM,
            INTESTINAL_PERMEABILITY_COMMON_PKSIM,
            DYNAMIC_SUM_FORMULAS,
            EXPRESSION_PARAMETERS,
            DISEASE_STATES
         };

         public static readonly IReadOnlyList<string> ForEvents = new List<string> {EVENTS};

         public static readonly IReadOnlyList<string> ForFormulations = new List<string> {FORMULATION, FORMULATION_PARTICLES};

         public static readonly IReadOnlyList<string> ForSchemaItems = new List<string> {SCHEMA_ITEM_PARAMETER};

         public static readonly IReadOnlyList<string> ForApplications = new List<string>
            {FORMULATION, APPLICATION_PARAMETER, SCHEMA_ITEM_PARAMETER, APPLICATION_PARAMETER_HUMAN};
      }

      public static class Compartment
      {
         public static readonly string INTERSTITIAL = "Interstitial";
         public static readonly string INTRACELLULAR = "Intracellular";
         public static readonly string BLOOD_CELLS = "BloodCells";
         public static readonly string PLASMA = "Plasma";
         public static readonly string ENDOSOME = "Endosome";
         public static readonly string VASCULAR_ENDOTHELIUM = "VascularEndothelium";
         public static readonly string URINE = "Urine";
         public static readonly string MUCOSA = "Mucosa";
         public static readonly string SALIVA = "Saliva";
         public static readonly string PERIPORTAL = "Periportal";
         public static readonly string PERICENTRAL = "Pericentral";
         public static readonly string FECES = "Feces";

         public static readonly IReadOnlyList<string> LiverZones = new List<string>
         {
            PERIPORTAL,
            PERICENTRAL
         };

         public static readonly IReadOnlyList<string> LiverCompartments = new List<string>
         {
            INTERSTITIAL,
            INTRACELLULAR,
            BLOOD_CELLS,
            PLASMA,
            ENDOSOME,
         };
      }

      public static class Compound
      {
         //const are needed since the values are used in an enum
         public const int COMPOUND_TYPE_ACID = -1;
         public const int COMPOUND_TYPE_BASE = 1;
         public const int COMPOUND_TYPE_NEUTRAL = 0;

         public const int BINDING_PARTNER_AGP = 0;
         public const int BINDING_PARTNER_ALBUMIN = 1;
         public const int BINDING_PARTNER_UNKNOWN = 2;
      }

      public static class Output
      {
         public static readonly string FractionDose = "Fraction Dose";
         public static readonly string Amount = "Amount";
         public static readonly string Concentration = "Concentration";
      }

      public static class ContainerName
      {
         public static readonly string ParticleBinPrefix = "ParticleBin_";
         public static readonly string NameTemplate = "<Template>";
         public static readonly string TypeTemplate = "<Type>";
         public static readonly string Drug = "DRUG";
         public static readonly string AdvancedParameterCollection = "AdvancedParameters";
         public static readonly string ProtocolSchemaItem = "ProtocolSchemaItem";
         public static readonly string ObservedData = "ObservedData";
         public static readonly string InsolubleDrug = "InsolubleDrug";
         public static readonly string EventGroupMainSubContainer = "EventGroupSubContainer";
         //only use for conversion of older snapshot. Do not use in code otherwise
         public static readonly string Applications = "Applications";

         public static string BuildingBlockInSimulationNameFor(string buildingBlockName, string simulationName)
         {
            return $"{buildingBlockName} [{simulationName}]";
         }

         public static string LumenSegmentNameFor(string segmentName) => CompositeNameFor(Organ.LUMEN, segmentName);

         public static string MucosaSegmentNameFor(string segmentName) => CompositeNameFor(Compartment.MUCOSA, segmentName);

         public static string PartialProcessName(string proteinName, string dataSource) => CompositeNameFor(proteinName, dataSource);

         public static string GlobalExpressionContainerNameFor(string expressionParameter)
         {
            switch (expressionParameter)
            {
               case Constants.Parameters.REL_EXP_PLASMA:
                  return Compartment.PLASMA;
               case Constants.Parameters.REL_EXP_BLOOD_CELLS:
               case Parameters.FRACTION_EXPRESSED_BLOOD_CELLS:
               case Parameters.FRACTION_EXPRESSED_BLOOD_CELLS_MEMBRANE:
                  return Compartment.BLOOD_CELLS;
               case Constants.Parameters.REL_EXP_VASCULAR_ENDOTHELIUM:
               case Parameters.FRACTION_EXPRESSED_VASC_ENDO_ENDOSOME:
               case Parameters.FRACTION_EXPRESSED_VASC_ENDO_PLASMA_SIDE:
               case Parameters.FRACTION_EXPRESSED_VASC_ENDO_TISSUE_SIDE:
                  return Compartment.VASCULAR_ENDOTHELIUM;

               default:
                  return string.Empty;
            }
         }
      }

      public static class ContainerType
      {
         public static readonly string COMPARTMENT = "COMPARTMENT";
         public static readonly string ORGAN = "ORGAN";
         public static readonly string APPLICATION = "APPLICATION";
         public static readonly string FORMULATION = "FORMULATION";
         public static readonly string NEIGHBORHOOD = "NEIGHBORHOOD";
         public static readonly string GENERAL = "GENERAL";
         public static readonly string EVENT = "EVENT";
         public static readonly string PROCESS = "PROCESS";
         public static readonly string EVENT_GROUP = "EVENTGROUP";
         public static readonly string COMPOUND = "COMPOUND";
         public static readonly string DISEASE_STATE = "DISEASE_STATE";
      }

      public static class Dimension
      {
         public static readonly string Auc = "AUC (mass)";
         public static readonly string Age = "Age in years";
         public static readonly string InversedLength = "Inversed length";
         public static readonly string AgeInWeeks = "Age in weeks";
         public static readonly string AucMolar = "AUC (molar)";
         public static readonly string Fraction = "Fraction";
         public static readonly string MASS_CONCENTRATION = Constants.Dimension.MASS_CONCENTRATION;
         public static readonly string DosePerBodyWeight = "Dose per body weight";
         public static readonly string VolumePerBodyWeight = "Volume per body weight";
         public static readonly string FlowPerWeight = "Flow per weight";
         public static readonly string Length = "Length";
         public static readonly string BMI = "BMI";
         public static readonly string InputDose = "Input dose";

         public static readonly IEnumerable<string> AllHardCoded = new List<string>
         {
            Auc,
            Age,
            AgeInWeeks,
            AucMolar,
            Fraction,
            MASS_CONCENTRATION,
            DosePerBodyWeight,
            Constants.Dimension.MASS_AMOUNT,
            VolumePerBodyWeight,
            FlowPerWeight,
            Length,
            BMI,
            InputDose
         };
      }

      public static class Distribution
      {
         public static readonly string Discrete = "Discrete";
         public static readonly string Normal = "Normal";
         public static readonly string LogNormal = "LogNormal";
         public static readonly string LogNormalGeo = "LogNormalGeo";
         public static readonly string Uniform = "Uniform";
         public static readonly string Unknown = "Unknown";
      }

      public static class Formula
      {
         public static readonly string Concentration = "ConcFormula";
         public static readonly string InsolubleDrugStartFormula = "InsolubleDrugStartFormula";
         public static readonly string TableFormula = "TableFormula";
      }

      public static class Formulation
      {
         public static readonly string EMPTY_FORMULATION = "Formulation_Empty";
         public static readonly string PARTICLES = "Formulation_Particles";
         public static readonly string FIRST_ORDER = "Formulation_FirstOrder";
         public static readonly string ZERO_ORDER = "Formulation_ZeroOrder";
         public static readonly string LINT80 = "Formulation_Tablet_Lint80";
         public static readonly string TABLE = "Formulation_Table";
         public static readonly string WEIBULL = "Formulation_Tablet_Weibull";
         public static readonly string DISSOLVED = "Formulation_Dissolved";
      }

      public static class Gender
      {
         public static readonly string MALE = "MALE";
         public static readonly string FEMALE = "FEMALE";
         public static readonly string UNDEFINED = "UNKNOWN";
      }

      public static class Groups
      {
         public static readonly string SIMULATION_ACTIVE_PROCESS = "SIMULATION_ACTIVE_PROCESS";
         public static readonly string PROTEIN_PARAMETERS = "INDIVIDUAL";
         public static readonly string PROTOCOL = "PROTOCOL";
         public static readonly string ENZYMATIC_STABILITY_PARTIAL = "ENZYMATIC_STABILITY";
         public static readonly string ENZYMATIC_STABILITY_INTRINSIC = "ENZYMATIC_STABILITY_INTRINSIC";
         public static readonly string SPECIFIC_BINDING = "SPECIFIC_BINDING";
         public static readonly string METABOLISM = "METABOLISM";
         public static readonly string COMPOUND_PROCESSES = "COMPOUND_PROCESSES";
         public static readonly string SYSTEMIC_PROCESSES = "SYSTEMIC_PROCESSES";
         public static readonly string IN_VITRO_PROCESSES = "IN_VITRO_PROCESSES";
         public static readonly string IN_VITRO_PROCESSES_TOTAL_HEPATIC = "IN_VITRO_PROCESSES_TOTAL_HEPATIC";
         public static readonly string IN_VITRO_PROCESSES_ENZYMATIC_STABILITY = "IN_VITRO_PROCESSES_ENZYMATIC_STABILITY";
         public static readonly string ACTIVE_TRANSPORT = "ACTIVE_TRANSPORT";
         public static readonly string TISSUE_COMPOSITION = "TISSUE_COMPOSITION";
         public static readonly string SPECIFIC_CLEARANCE = "SPECIFIC_CLEARANCE";
         public static readonly string INDIVIDUAL_PARAMETERS_IN_PROCESS = "INDIVIDUAL_PARAMETERS_IN_PROCESS";
         public static readonly string ACTIVE_TRANSPORT_INTRINSIC = "ACTIVE_TRANSPORT_INTRINSIC";
         public static readonly string COMPOUND = "COMPOUND";
         public static readonly string COMPOUND_INTESTINAL_PERMEABILITY = "COMPOUND_INTESTINAL_PERMEABILITY";
         public static readonly string COMPOUND_LIPOPHILICITY = "COMPOUND_LIPOPHILICITY";
         public static readonly string COMPOUND_MW = "COMPOUND_MW";
         public static readonly string COMPOUND_PERMEABILITY = "COMPOUND_PERMEABILITY";
         public static readonly string COMPOUND_PKA = "COMPOUND_PKA";
         public static readonly string COMPOUND_FRACTION_UNBOUND = "COMPOUND_FRACTION_UNBOUND";
         public static readonly string COMPOUND_SOLUBILITY = "COMPOUND_SOLUBILITY";
         public static readonly string COMPOUND_DISSOLUTION = "COMPOUND_PARTICLE_DISSOLUTION";
         public static readonly string COMPOUND_TWO_PORE = "COMPOUND_TWO_PORE";
         public static readonly string COMPOUND_PROCESS_ITEM = "COMPOUND_PROCESS_ITEM";
         public static readonly string COMPOUND_ITEM = "COMPOUND_ITEM";
         public static readonly string PROTOCOL_ITEM = "PROTOCOL_ITEM";
         public const string RELATIVE_EXPRESSION = "RELATIVE_EXPRESSIONS";
         public static readonly string RELATIVE_EXPRESSION_ITEM = "RELATIVE_EXPRESSIONS_ITEM";
         public static readonly string SIMULATION_SETTINGS = "SIMULATION_SETTINGS";
         public static readonly string SOLVER_SETTINGS = "SOLVER_SETTINGS";
         public static readonly string FORMULATIONS = "FORMULATIONS";
         public static readonly string EVENTS = "EVENTS";
         public static readonly string ALL = "ALL";
         public static readonly string FAVORITES = "FAVORITE";
         public static readonly string BLOOD_FLOW_RATES = "BLOOD_FLOW_RATES";
         public static readonly string VASCULAR_SYSTEM = "VASCULAR_SYSTEM";
         public static readonly string ORGANS_AND_TISSUES = "ORGANS_AND_TISSUES";
         public static readonly string GI_LUMEN = "GI_LUMEN";
         public static readonly string GI_MUCOSA = "GI_MUCOSA";
         public static readonly string GI_NON_MUCOSA_TISSUE = "GI_NON_MUCOSA_TISSUE";
         public static readonly string ONTOGENY_LIVER = "ONTOGENY_LIVER_NO_GI";
         public static readonly string ONTOGENY_DUODENUM = "ONTOGENY_DUODENUM";
         public static readonly string ONTOGENY_PLASMA = "ONTOGENY_PLASMA";
         public static readonly string INDIVIDUAL_CHARACTERISTICS = "BODY_ANATOMY";
         public static readonly string MOBI = "MOBI";
         public static readonly string PHYSIOLOGY = "PHYSIOLOGY";
         public static readonly string INHIBITION_PROCESSES = "INHIBITION_PROCESSES";
         public static readonly string INDUCTION_PROCESSES = "INDUCTION_PROCESSES";
         public static readonly string DISTRIBUTION = "DISTRIBUTION";
         public static readonly string PARTITION_COEFFICIENT = "PARTITION_COEFFICIENT";
         public static readonly string PARTITION_COEFFICIENT_K_CELL_PLS = "PARTITION_COEFFICIENT_K_CELL_PLS";
         public static readonly string PARTITION_COEFFICIENT_K_CELL_PLS_MUCOSA = "PARTITION_COEFFICIENT_K_CELL_PLS_MUCOSA";
         public static readonly string PARTITION_COEFFICIENT_K_INT_PLS = "PARTITION_COEFFICIENT_K_INT_PLS";
         public static readonly string PARTITION_COEFFICIENT_K_INT_PLS_MUCOSA = "PARTITION_COEFFICIENT_K_INT_PLS_MUCOSA";
         public static readonly string PERMEABILITY_INT_CELL = "PERMEABILITY_INT_CELL";
         public static readonly string PERMEABILITY_INT_CELL_MUCOSA = "PERMEABILITY_INT_CELL_MUCOSA";
         public static readonly string PERMEABILITY_CELL_INT = "PERMEABILITY_CELL_INT";
         public static readonly string PERMEABILITY_CELL_INT_MUCOSA = "PERMEABILITY_CELL_INT_MUCOSA";
         public static readonly string PERMEABILITY = "PERMEABILITY";
         public static readonly string PERMEABILITY_ENDOTHELIAL = "PERMEABILITY_ENDOTHELIAL";
         public static readonly string PERMEABILITY_ENDOTHELIAL_MUCOSA = "PERMEABILITY_ENDOTHELIAL_MUCOSA";
         public static readonly string PERMEABILITY_SMALL_PORES = "PERMEABILITY_SMALL_PORES";
         public static readonly string PERMEABILITY_LARGE_PORES = "PERMEABILITY_LARGE_PORES";
         public static readonly string REFELCTION_COEFF_LARGE_PORES = "REFELCTION_COEFF_LARGE_PORES";
         public static readonly string REFELCTION_COEFF_SMALL_PORES = "REFELCTION_COEFF_SMALL_PORES";
         public static readonly string INTESTINAL_SOLUBILITY = "INTESTINAL_SOLUBILITY";
         public static readonly string FRACTION_UNBOUND_PLASMA = "FRACTION_UNBOUND_PLASMA";
         public static readonly string ONTOGENY_FACTOR = "ONTOGENY_FACTOR";
         public static readonly string USER_DEFINED = "USER_DEFFINED";
         public static readonly string COMPOUNDPROCESS_SIMULATION_PARAMETERS = "COMPOUNDPROCESS_SIMULATION_PARAMETERS";
         public static readonly string COMPOUNDPROCESS_CALCULATION_PARAMETERS = "COMPOUNDPROCESS_CALCULATION_PARAMETERS";
         public static readonly string DISEASE_STATES = "DISEASE_STATES";

         public static readonly IReadOnlyList<string> GroupsWithCalculatedAlternative = new[]
         {
            COMPOUND_INTESTINAL_PERMEABILITY,
            COMPOUND_PERMEABILITY
         };

         public static readonly IReadOnlyList<string> GroupsWithAlternative = new List<string>(GroupsWithCalculatedAlternative)
         {
            COMPOUND_LIPOPHILICITY,
            COMPOUND_FRACTION_UNBOUND,
            COMPOUND_SOLUBILITY
         };

         public static readonly IReadOnlyList<string> GroupsWithAlternativeAndSpecies = new[]
         {
            COMPOUND_FRACTION_UNBOUND
         };

         public static IReadOnlyList<string> AllSimulationCompoundGroups = new[]
         {
            DISTRIBUTION,
            PARTITION_COEFFICIENT,
            PARTITION_COEFFICIENT_K_CELL_PLS,
            PARTITION_COEFFICIENT_K_CELL_PLS_MUCOSA,
            PERMEABILITY_INT_CELL,
            PERMEABILITY_INT_CELL_MUCOSA,
            PERMEABILITY_CELL_INT,
            PERMEABILITY_CELL_INT_MUCOSA,
            PARTITION_COEFFICIENT_K_INT_PLS,
            PARTITION_COEFFICIENT_K_INT_PLS_MUCOSA,
            PERMEABILITY,
            PERMEABILITY_ENDOTHELIAL,
            PERMEABILITY_ENDOTHELIAL_MUCOSA,
            PERMEABILITY_SMALL_PORES,
            PERMEABILITY_LARGE_PORES,
            REFELCTION_COEFF_LARGE_PORES,
            REFELCTION_COEFF_LARGE_PORES,
            INTESTINAL_SOLUBILITY,
            FRACTION_UNBOUND_PLASMA,
         };

         public static readonly IEnumerable<string> AllSimulationActiveProcesses = new List<string>
         {
            SIMULATION_ACTIVE_PROCESS,
            SYSTEMIC_PROCESSES,
            IN_VITRO_PROCESSES_TOTAL_HEPATIC,
            IN_VITRO_PROCESSES_ENZYMATIC_STABILITY,
            SPECIFIC_BINDING,
            ENZYMATIC_STABILITY_PARTIAL,
            ENZYMATIC_STABILITY_INTRINSIC,
            INHIBITION_PROCESSES,
            INDUCTION_PROCESSES,
         };
      }

      public static class KeyWords
      {
         public static readonly string Molecule = "<MOLECULE>";
         public static readonly string Protein = "<PROTEIN>";
         public static readonly string Complex = "<COMPLEX>";
         public static readonly string Reaction = "<REACTION>";
      }

      public static class Molecule
      {
         public static string Dummy = "Dummy";
         public static string Drug = "DRUG";
         public static string UndefinedLiver = "Undefined Liver";
         public static string Metabolite = "Metabolite";
         public static string Complex = "Complex";
         public static string UndefinedLiverTransporter = "Undefined Liver Transporter";
         public static string FcRn = "FcRn";
         public static string LigandEndo = "LigandEndo";
         public static string LigandEndoComplex = "LigandEndo_Complex";
         public static string AGP = "AGP";
         public static string ALBUMIN = "ALB";
         public static string DrugFcRnComplexTemplate = "FcRn_Complex";

         public static string DrugFcRnComplexName(string compoundName)
         {
            return $"{compoundName}-{DrugFcRnComplexTemplate}";
         }

         public static string ProcessProductName(string compoundName, string proteinName, string productNameTemplate)
         {
            return $"{compoundName}-{proteinName} {productNameTemplate}";
         }
      }

      public static class ORM
      {
         public const string PROTEIN = "PROTEIN";
         public const string TRANSPORTER = "TRANSPORTER";
         public const string COMPOUND_ACTIVE_PROCESS_PREFIX = "COMPOUND_";
         public const string USAGE_IN_INDIVIDUAL_REQUIRED = "REQUIRED";
         public const string USAGE_IN_INDIVIDUAL_OPTIONAL = "OPTIONAL";
         public const string USAGE_IN_INDIVIDUAL_EXTENDED = "EXTENDED";
         public const string CONTAINER_ME = ".";
         public const string PROCESS_MOLECULE_DIRECTION_IN = "IN";
         public const string PROCESS_MOLECULE_DIRECTION_OUT = "OUT";
         public const string PROCESS_MOLECULE_DIRECTION_MODIFIER = "MODIFIER";

         public const string VIEW_EVENT_CONDITIONS = "VIEW_EVENT_CONDITIONS";
         public const string VIEW_EVENT_CHANGED_OBJECTS = "VIEW_EVENT_CHANGED_OBJECTS";
         public const string VIEW_APPLICATION_PROCESSES = "VIEW_APPLICATION_PROCESSES";
         public const string VIEW_APPLICATIONS = "VIEW_APPLICATIONS";
         public const string VIEW_MOLECULES = "VIEW_MOLECULES";
         public const string VIEW_PROCESS_DESCRIPTOR_CONDITIONS = "VIEW_PROCESS_DESCRIPTOR_CONDITIONS";
         public const string VIEW_CONTAINER_TAGS = "VIEW_CONTAINER_TAGS";
         public const string VIEW_PARAMETER_RATES = "VIEW_PARAMETER_RATES";
         public const string VIEW_PARAMETER_VALUES = "VIEW_PARAMETER_VALUES";
         public const string VIEW_PARAMETER_DISTRIBUTIONS = "VIEW_PARAMETER_DISTRIBUTIONS";
         public const string VIEW_PARAMETERS_IN_CONTAINERS = "VIEW_PARAMETERS_IN_CONTAINERS";
         public const string VIEW_GROUPS = "VIEW_GROUPS";
         public const string VIEW_POPULATION_GENDERS = "VIEW_POPULATION_GENDERS";
         public const string VIEW_POPULATIONS = "VIEW_POPULATIONS";
         public const string VIEW_POPULATION_AGE = "VIEW_POPULATION_AGE";
         public const string VIEW_GENDERS = "VIEW_GENDERS";
         public const string VIEW_SPECIES = "VIEW_SPECIES";
         public const string VIEW_PARAMETER_VALUE_VERSIONS = "VIEW_PARAMETER_VALUE_VERSIONS";
         public const string VIEW_POPULATION_CONTAINERS = "VIEW_POPULATION_CONTAINERS";
         public const string VIEW_CONTAINERS = "VIEW_CONTAINERS";
         public const string VIEW_REPRESENTATION_INFOS = "VIEW_REPRESENTATION_DATA";
         public const string VIEW_RATE_OBJECT_PATHS = "VIEW_RATE_OBJECT_PATHS";
         public const string VIEW_CALCULATION_METHOD_RATE_FORMULA = "VIEW_CALCULATION_METHOD_RATE_FORMULA";
         public const string VIEW_MODEL_CONTAINERS = "VIEW_MODEL_CONTAINERS";
         public const string VIEW_MODEL_PROCESSES = "VIEW_MODEL_PROCESSES";
         public const string VIEW_SCHEMA_ITEMS = "VIEW_SCHEMA_ITEM_CONTAINERS";
         public const string VIEW_OBSERVERS = "VIEW_OBSERVERS";
         public const string VIEW_OBSERVER_DESCRIPTOR_CONDITIONS = "VIEW_OBSERVER_DESCRIPTOR_CONDITIONS";
         public const string VIEW_KNOWN_TRANSPORTER_CONTAINERS = "VIEW_KNOWN_TRANSPORTER_CONTAINERS";
         public const string VIEW_KNOWN_TRANSPORTERS = "VIEW_KNOWN_TRANSPORTERS";
         public const string VIEW_PROTEIN_SYNONYMS = "VIEW_PROTEIN_SYNONYMS";
         public const string VIEW_PARAMETER_RHS = "VIEW_PARAMETER_RHS";
         public const string VIEW_CALCULATION_METHODS = "VIEW_CALCULATION_METHODS";
         public const string VIEW_MODELS = "VIEW_MODELS";
         public const string VIEW_MODEL_SPECIES = "VIEW_MODEL_SPECIES";
         public const string VIEW_MODEL_CALCULATION_METHODS = "VIEW_MODEL_CALCULATION_METHODS";
         public const string VIEW_PROCESSES = "VIEW_PROCESSES";
         public const string VIEW_OBJECT_PATHS = "VIEW_OBJECT_PATHS";
         public const string VIEW_NEIGHBORHOODS = "VIEW_NEIGHBORHOODS";
         public const string VIEW_MODEL_OBSERVERS = "VIEW_MODEL_OBSERVERS";
         public const string VIEW_FORMULATION_ROUTES = "VIEW_FORMULATION_ROUTES";
         public const string VIEW_ORGAN_TYPES = "VIEW_ORGAN_TYPES";
         public const string VIEW_SPECIES_CALCULATION_METHODS = "VIEW_SPECIES_CALCULATION_METHODS";
         public const string VIEW_COMPOUND_PROCESS_PARAMETER_MAPPINGS = "VIEW_COMPOUND_PROCESS_PARAMETER_MAPPING";
         public const string VIEW_MOLECULE_START_FORMULAS = "VIEW_MOLECULE_START_FORMULAS";
         public const string VIEW_ONTOGENIES = "VIEW_ONTOGENIES";
         public const string VIEW_CATEGORY = "VIEW_CATEGORIES";
         public const string VIEW_CALCULATION_METHOD_PARAMETER_RATES = "VIEW_CALCULATION_METHOD_PARAMETER_RATES";
         public const string VIEW_CALCULATION_METHOD_PARAMETER_DESCRIPTOR_CONDITIONS = "VIEW_CALCULATION_METHOD_PARAMETER_DESCRIPTOR_CONDITIONS";
         public const string VIEW_DYNAMIC_FORMULA_CRITERIA_REPOSITORY = "VIEW_CALCULATION_METHOD_RATE_DESCRIPTOR_CONDITIONS";
         public const string VIEW_MODEL_PASSIVE_TRANSPORT_MOLECULE_NAMES = "VIEW_MODEL_TRANSPORT_MOLECULE_NAMES";
         public const string VIEW_MODEL_CONTAINER_MOLECULES = "VIEW_MODEL_CONTAINER_MOLECULES";
         public const string VIEW_REACTION_PARTNERS = "VIEW_REACTION_PARTNERS";
         public const string VIEW_MOLECULE_PARAMETERS = "VIEW_MOLECULE_PARAMETERS";
         public const string VIEW_VALUE_ORIGIN = "VIEW_VALUE_ORIGINS";
         public const string VIEW_TRANSPORT_DIRECTIONS = "VIEW_TRANSPORT_DIRECTIONS";
         public const string VIEW_TRANSPORTS = "VIEW_TRANSPORTS";
         public const string VIEW_DISEASE_STATES = "VIEW_DISEASE_STATES";
         public const string VIEW_POPULATION_DISEASE_STATES = "VIEW_POPULATION_DISEASE_STATES";
         public const string VIEW_CONTAINER_PARAMETER_DESCRIPTOR_CONDITIONS = "VIEW_CONTAINER_PARAMETER_DESCRIPTOR_CONDITIONS";
         public const string VIEW_INDIVIDUAL_PARAMETER_NOT_FOR_ALL_SPECIES = "VIEW_INDIVIDUAL_PARAMETER_NOT_FOR_ALL_SPECIES";
         public const string VIEW_INDIVIDUAL_PARAMETER_SAME_FORMULA_OR_VALUE_FOR_ALL_SPECIES = "VIEW_INDIVIDUAL_PARAMETER_SAME_FORMULA_OR_VALUE_FOR_ALL_SPECIES";
      }

      public static class Organ
      {
         public static readonly string ARTERIAL_BLOOD = "ArterialBlood";
         public static readonly string BONE = "Bone";
         public static readonly string BRAIN = "Brain";
         public static readonly string ENDOGENOUS_IGG = "EndogenousIgG";
         public static readonly string FAT = "Fat";
         public static readonly string GALLBLADDER = "Gallbladder";
         public static readonly string GONADS = "Gonads";
         public static readonly string HEART = "Heart";
         public static readonly string KIDNEY = "Kidney";
         public static readonly string LARGE_INTESTINE = "LargeIntestine";
         public static readonly string LUMEN = Constants.Organ.LUMEN;
         public static readonly string LUNG = "Lung";
         public static readonly string LIVER = "Liver";
         public static readonly string MUSCLE = "Muscle";
         public static readonly string PANCREAS = "Pancreas";
         public static readonly string PORTAL_VEIN = "PortalVein";
         public static readonly string SALIVA = "Saliva";
         public static readonly string SKIN = "Skin";
         public static readonly string SMALL_INTESTINE = "SmallIntestine";
         public static readonly string SPLEEN = "Spleen";
         public static readonly string STOMACH = "Stomach";
         public static readonly string VENOUS_BLOOD = "VenousBlood";
         public static readonly string PERIPHERAL_VENOUS_BLOOD = "PeripheralVenousBlood";
         public static readonly string TISSUE_ORGAN = "TissueOrgan";

         public static readonly IReadOnlyList<string> StandardOrgans = new List<string>
         {
            GONADS,
            HEART,
            KIDNEY,
            LARGE_INTESTINE,
            LIVER,
            MUSCLE,
            PANCREAS,
            PORTAL_VEIN,
            SMALL_INTESTINE,
            SPLEEN,
            STOMACH,
            VENOUS_BLOOD,
            BONE,
            FAT,
            BRAIN,
            SKIN,
            LUNG,
            ARTERIAL_BLOOD
         };

         public static IReadOnlyList<string> PolarizedMembraneOrgans = new[]
         {
            BRAIN,
            KIDNEY,
            Compartment.PERICENTRAL,
            Compartment.PERIPORTAL
         };
      }

      public static class PKAnalysis
      {
         public const string TotalPlasmaCL = "Total plasma clearance";
         public const string TotalPlasmaCLOverF = "Total plasma clearance/F";
         public const string VssPlasma = "Vss (plasma)";
         public const string VssPlasmaOverF = "Vss (plasma)/F";
         public const string VssPhysChem = "Vss (phys-chem)";
         public const string FractionAbsorbed = "Fraction absorbed";
         public const string Bioavailability = "Bioavailability";
         public const string VdPlasma = "Vd (plasma)";
         public const string VdPlasmaOverF = "Vd (plasma)/F";
         public const string AUCRatio = "AUCRatio";
         public const string C_maxRatio = "C_maxRatio";

         public static readonly IReadOnlyList<string> AllParametersInfluencedByFractionAbsorbed = new[]
         {
            PKParameters.Vd,
            PKParameters.Vss,
            PKParameters.MRT,
            PKParameters.Thalf,
            PKParameters.AUC_inf,
            PKParameters.AUC_inf_norm,
            PKParameters.AUC_inf_tD1,
            PKParameters.AUC_inf_tD1_norm,
            PKParameters.AUC_inf_tDLast,
            PKParameters.AUC_inf_tLast_norm
         };
      }

      public static class Model
      {
         public const string FOUR_COMP = "4Comp";
         public const string TWO_PORES = "TwoPores";
      }

      public static class Parameters
      {
         public static readonly string PH = "pH";
         public static readonly string IS_NEUTRAL = "Is neutral";
         public static readonly string EFFECTIVE_MOLECULAR_WEIGHT = "Effective molecular weight";
         public static readonly string LIPOPHILICITY = "Lipophilicity";
         public static readonly string CONCENTRATION = Constants.Parameters.CONCENTRATION;
         public static readonly string MOLECULAR_WEIGHT = Constants.Parameters.MOL_WEIGHT;
         public static readonly string REFERENCE_CONCENTRATION = "Reference concentration";

         public static readonly IReadOnlyList<string> AllGlobalMoleculeParameters = new[]
         {
            REFERENCE_CONCENTRATION,
            HALF_LIFE_LIVER,
            HALF_LIFE_INTESTINE
         };

         public static readonly IReadOnlyList<string> AllGlobalRelExpParameters = new[]
         {
            Constants.Parameters.REL_EXP_BLOOD_CELLS,
            Constants.Parameters.REL_EXP_PLASMA,
            Constants.Parameters.REL_EXP_VASCULAR_ENDOTHELIUM,
         };

         public static string OntogenyTableParameterFor(string parameter) => $"{parameter} table";

         public static readonly string ONTOGENY_FACTOR = Constants.ONTOGENY_FACTOR;
         public static readonly string ONTOGENY_FACTOR_GI = "Ontogeny factor GI";
         public static readonly string ONTOGENY_FACTOR_ALBUMIN = "Ontogeny factor (albumin)";
         public static readonly string ONTOGENY_FACTOR_AGP = "Ontogeny factor (alpha1-acid glycoprotein)";

         public static readonly string ONTOGENY_FACTOR_TABLE = OntogenyTableParameterFor(ONTOGENY_FACTOR);
         public static readonly string ONTOGENY_FACTOR_GI_TABLE = OntogenyTableParameterFor(ONTOGENY_FACTOR_GI);
         public static readonly string ONTOGENY_FACTOR_ALBUMIN_TABLE = OntogenyTableParameterFor(ONTOGENY_FACTOR_ALBUMIN);
         public static readonly string ONTOGENY_FACTOR_AGP_TABLE = OntogenyTableParameterFor(ONTOGENY_FACTOR_AGP);
         
         public static readonly string PARTICLE_BIN_DRUG_MASS = "DrugMass of particle bin";
         public static readonly string NUMBER_OF_REPETITIONS = "NumberOfRepetitions";
         public static readonly string TIME_BETWEEN_REPETITIONS = "TimeBetweenRepetitions";
         public static readonly string INPUT_DOSE = "InputDose";
         public static readonly string DOSE = "Dose";
         public static readonly string DOSE_PER_BODY_WEIGHT = "DosePerBodyWeight";
         public static readonly string DOSE_PER_BODY_SURFACE_AREA = "DosePerBodySurfaceArea";
         public static readonly string AGE = "Age";
         public static readonly string AGE_0 = "Age of individual at t=0";
         public static readonly string PMA = "Post menstrual age";
         public static readonly string WEIGHT = "Weight";
         public static readonly string HCT = "Hematocrit";
         public static readonly string WEIGHT_IN_PROCESS = "Body weight";
         public static readonly string MEAN_WEIGHT = "MeanBW";
         public static readonly string MEAN_HEIGHT = "MeanHeight";
         public static readonly string HEIGHT = "Height";
         public static readonly string BMI = "BMI";
         public static readonly string BSA = "BSA";
         public static readonly string BLOOD_FLOW = "Blood flow rate";
         public static readonly string SPECIFIC_BLOOD_FLOW_RATE = "Specific blood flow rate";
         public static readonly string DENSITY = "Density (tissue)";
         public static readonly string DRUG = "DRUG";
         public static readonly string ALLOMETRIC_SCALE_FACTOR = "Allometric scale factor";
         public static readonly string PARAMETER_PKA_BASE = "pKa value ";
         public static readonly string GFR_SPEC = "GFR (specific)";
         public static readonly string MIN_TO_YEAR_FACTOR = "Minute to year unit conversion factor";
         public static readonly string GFR_FRACTION = "GFR fraction";
         public static readonly string INTESTINAL_PERMEABILITY = "Intestinal permeability (transcellular)";
         public static readonly string VOLUME_MOUSE = "Organ volume mouse";

         public static string ParameterPKa(int index)
         {
            return $"{PARAMETER_PKA_BASE}{index}";
         }

         public static readonly string PARAMETER_PKA1 = ParameterPKa(0);
         public static readonly string PARAMETER_PKA2 = ParameterPKa(1);
         public static readonly string PARAMETER_PKA3 = ParameterPKa(NUMBER_OF_PKA_PARAMETERS - 1);

         public const string APPLICATION_RATE = "Application rate";
         public const string PERMEABILITY = "Permeability";
         public const string SPECIFIC_INTESTINAL_PERMEABILITY = "Specific intestinal permeability (transcellular)";
         public const string FRACTION_UNBOUND_PLASMA_REFERENCE_VALUE = "Fraction unbound (plasma, reference value)";
         public const string SOLUBILITY_AT_REFERENCE_PH = "Solubility at reference pH";
         public const string SOLUBILITY = "Solubility";
         public const string SOLUBILITY_TABLE = "Solubility table";
         public const string SOLUBILITY_GAIN_PER_CHARGE = "Solubility gain per charge";
         public const string REFERENCE_PH = "Reference pH";
         public const string UNDEFINED_ONTOGENY = "Undefined";
         public const string TOTAL_DRUG_MASS = "Total drug mass";
         public const string FRACTION_INTRACELLULAR = "Fraction intracellular";
         public const string FRACTION_INTERSTITIAL = "Fraction interstitial";
         public const string FRACTION_VASCULAR = "Fraction vascular";
         public const string NUMBER_OF_PARTICLES_FACTOR = "Number_Of_Particles_Factor";
         public const string START_PARTICLE_RADIUS = "Particle radius (at t=0)";
         public const string PARTICLE_RADIUS_MEAN = "Particle radius (mean)";
         public const string PARTICLE_RADIUS_MIN = "Particle radius (min)";
         public const string PARTICLE_RADIUS_MAX = "Particle radius (max)";
         public const string PARTICLE_RADIUS_STD_DEVIATION = "Particle radius (SD)";
         public const string PARTICLE_LOG_DISTRIBUTION_MEAN = "Particle radius (geomean)";
         public const string PARTICLE_LOG_VARIATION_COEFF = "Coefficient of variation";
         public const string K_CELL_PLS = "Partition coefficient (intracellular/plasma)";
         public const string K_WATER = "Partition coefficient (water/container)";
         public const string SPECIFIC_CLEARANCE = "Specific clearance";
         public const int PARTICLE_SIZE_DISTRIBUTION_NORMAL = 0;
         public const int PARTICLE_SIZE_DISTRIBUTION_LOG_NORMAL = 1;
         public const int MAX_NUMBER_OF_BINS = 20;
         public const int MAX_NUMBER_OF_HALOGENS = 10;
         public const int MONODISPERSE = 0;
         public const int POLYDISPERSE = 1;
         public const int SOLUBLE = 1;
         public const int INSOLUBLE = 0;
         public const int SINK_CONDITION = 1;
         public const int NO_SINK_CONDITION = 0;
         public const string IS_FLOATING_IN_LUMEN = "Is floating in lumen";
         public const string FRACTION_UNBOUND_EXPERIMENT = "Fraction unbound (experiment)";
         public const string LIPOPHILICITY_EXPERIMENT = "Lipophilicity (experiment)";
         public const string FRACTION_DOSE = "Fraction (dose)";
         public const string SOLUBILITY_P_KA__P_H_FACTOR = "Solubility_pKa_pH_Factor";
         public const string HALF_LIFE = "t1/2";
         public const string HALF_LIFE_LIVER = "t1/2 (liver)";
         public const string HALF_LIFE_INTESTINE = "t1/2 (intestine)";
         public const string T_END = "End time";
         public const string LAG_TIME = "Lag time";
         public const string DISS_TIME80 = "Dissolution time (80% dissolved)";
         public const string DISS_TIME50 = "Dissolution time (50% dissolved)";
         public const string DISS_SHAPE = "Dissolution shape";
         public const string LYMPH_FLOW = "Lymph flow rate";
         public const string LYMPH_FLOW_INCL_MUCOSA = "Lymph flow rate (incl. mucosa)";
         public const string RECIRCULATION_FLOW = "Fluid recirculation flow rate";
         public const string RECIRCULATION_FLOW_INCL_MUCOSA = "Fluid recirculation flow rate (incl. mucosa)";
         public const string PLASMA_PROTEIN_SCALE_FACTOR = "Plasma protein scale factor";
         public const string SMALL_INTESTINAL_TRANSIT_TIME = "Small intestinal transit time";
         public const string GASTRIC_EMPTYING_TIME = "Gastric emptying time";
         public const string KM_INTERACTION_FACTOR = "Km interaction factor";
         public const string KCAT_INTERACTION_FACTOR = "kcat interaction factor";
         public const string CL_SPEC_PER_ENZYME_INTERACTION_FACTOR = "CLspec/[Enzyme] interaction factor";
         public const string KI = "Ki";
         public const string KI_C = "Ki_c";
         public const string KI_U = "Ki_u";
         public const string VOLUME_PLASMA = "Volume (plasma)";
         public const string MUCOSA_PERMEABILITY_SCALE_FACTOR = "Mucosa permeability scale factor (transcellular)";
         public const string START_AMOUNT = "Start amount";
         public const string K_KINACT_HALF = "K_kinact_half";
         public const string K_KINACT_HALF_INTERACTION_FACTOR = "K_kinact_half interaction factor";
         public const string KINACT_INTERACTION_FACTOR = "kinact interaction factor";
         public const string KINACT = "kinact";
         public const string EMAX = "Emax";
         public const string EC50 = "EC50";
         public const string WEIGHT_TISSUE = "Weight (tissue)";
         public const string FRACTION_EXPRESSED_PREFIX = "Fraction expressed";
         public const string FRACTION_EXPRESSED_BLOOD_CELLS = "Fraction expressed in blood cells";
         public const string FRACTION_EXPRESSED_BLOOD_CELLS_MEMBRANE = "Fraction expressed in blood cells membrane";
         public const string FRACTION_EXPRESSED_VASC_ENDO_PLASMA_SIDE = "Fraction expressed on plasma-side membrane of vascular endothelium";
         public const string FRACTION_EXPRESSED_VASC_ENDO_TISSUE_SIDE = "Fraction expressed on tissue-side membrane of vascular endothelium";
         public const string FRACTION_EXPRESSED_VASC_ENDO_ENDOSOME = "Fraction expressed in endosomes";
         public const string FRACTION_EXPRESSED_INTRACELLULAR = "Fraction expressed intracellular";
         public const string FRACTION_EXPRESSED_INTERSTITIAL = "Fraction expressed interstitial";
         public const string FRACTION_EXPRESSED_APICAL = "Fraction expressed apical";
         public const string FRACTION_EXPRESSED_BASOLATERAL = "Fraction expressed basolateral";
         public const string FRACTION_EXPRESSED_AT_BLOOD_BRAIN_BARRIER = "Fraction expressed at blood brain barrier";
         public const string FRACTION_EXPRESSED_BRAIN_TISSUE = "Fraction expressed brain tissue";
         public const string INITIAL_CONCENTRATION = "Initial concentration";
         public const string DISEASE_FACTOR = "Disease factor";


         public static readonly IReadOnlyList<string> OntogenyFactors = new[]
         {
            ONTOGENY_FACTOR_GI,
            ONTOGENY_FACTOR
         };

         public static readonly IReadOnlyList<string> OntogenyFactorTables = new[]
         {
            ONTOGENY_FACTOR_GI_TABLE,
            ONTOGENY_FACTOR_TABLE
         };

         public static readonly IReadOnlyList<string> AllPlasmaProteinOntogenyFactors = new[]
         {
            ONTOGENY_FACTOR_ALBUMIN,
            ONTOGENY_FACTOR_AGP
         };

         public static readonly IReadOnlyList<string> AllPlasmaProteinOntogenyFactorTables = new[]
         {
            ONTOGENY_FACTOR_ALBUMIN_TABLE, 
            ONTOGENY_FACTOR_AGP_TABLE, 
         };

         public static readonly IReadOnlyList<string> HiddenParameterForMonodisperse = new[]
         {
            Constants.Parameters.PARTICLE_SIZE_DISTRIBUTION,
            PARTICLE_RADIUS_MIN,
            PARTICLE_RADIUS_MAX,
            Constants.Parameters.NUMBER_OF_BINS,
            PARTICLE_RADIUS_STD_DEVIATION,
            PARTICLE_LOG_DISTRIBUTION_MEAN,
            PARTICLE_LOG_VARIATION_COEFF
         };

         public static readonly IReadOnlyList<string> HiddenParameterForPolydisperseNormal = new[]
         {
            PARTICLE_LOG_DISTRIBUTION_MEAN,
            PARTICLE_LOG_VARIATION_COEFF
         };

         public static readonly IReadOnlyList<string> HiddenParameterForPolydisperseLogNormal = new[]
         {
            PARTICLE_RADIUS_MEAN,
            PARTICLE_RADIUS_STD_DEVIATION
         };

         public static readonly IReadOnlyList<string> ParticleDistributionStructuralParameters = new[]
         {
            Constants.Parameters.PARTICLE_DISPERSE_SYSTEM,
            Constants.Parameters.NUMBER_OF_BINS,
            PARTICLE_RADIUS_MEAN,
            Constants.Parameters.PARTICLE_SIZE_DISTRIBUTION,
            PARTICLE_RADIUS_MIN,
            PARTICLE_RADIUS_MAX,
            PARTICLE_RADIUS_STD_DEVIATION,
            PARTICLE_LOG_DISTRIBUTION_MEAN,
            PARTICLE_LOG_VARIATION_COEFF
         };

       

         public static readonly IReadOnlyList<string> StandardCreateIndividualParameters = new[]
         {
            Constants.Parameters.VOLUME,
            AGE,
            Constants.Parameters.GESTATIONAL_AGE,
            HEIGHT,
            WEIGHT,
            MEAN_WEIGHT,
            MEAN_HEIGHT,
            BMI,
            BSA
         };

       

         public static readonly IReadOnlyCollection<string> CompoundMustInputParameters = new[]
         {
            LIPOPHILICITY, Constants.Parameters.MOL_WEIGHT, FRACTION_UNBOUND_PLASMA_REFERENCE_VALUE
         };

         public static readonly IReadOnlyCollection<string> Halogens = new[]
         {
            Constants.Parameters.CL,
            Constants.Parameters.BR,
            Constants.Parameters.F,
            Constants.Parameters.I
         };

         public static readonly IReadOnlyCollection<string> AllParametersWithLockedValueOriginInSimulation = new List<string>(Halogens)
         {
            Constants.Parameters.MOL_WEIGHT,
            EFFECTIVE_MOLECULAR_WEIGHT,
            Constants.Parameters.COMPOUND_TYPE1,
            Constants.Parameters.COMPOUND_TYPE2,
            Constants.Parameters.COMPOUND_TYPE3,
            PARAMETER_PKA1,
            PARAMETER_PKA2,
            PARAMETER_PKA3,
            REFERENCE_PH,
            SOLUBILITY_AT_REFERENCE_PH,
            SOLUBILITY_GAIN_PER_CHARGE,
            Constants.Parameters.START_TIME,
            DOSE,
         };

         public static readonly IReadOnlyCollection<string> AllDistributionParameters = new List<string>
         {
            Constants.Distribution.DEVIATION,
            Constants.Distribution.GEOMETRIC_DEVIATION,
            Constants.Distribution.MAXIMUM,
            Constants.Distribution.MEAN,
            Constants.Distribution.MINIMUM,
            Constants.Distribution.PERCENTILE,
         };

      }

      public static class SimulationResults
      {
         public const string INDIVIDUAL_ID = "IndividualId";
         public const string TIME = "Time";
         public const string QUANTITY_PATH = "Quantity Path";
         public const string PARAMETER = "Parameter";
         public const string VALUE = "Value";
         public const string UNIT = "Unit";
      }

      public static class Population
      {
         public const string ICRP = "European_ICRP_2002";
         public const string PRETERM = "Preterm";
         public const string TABLE_PARAMETER_EXPORT = "_TableParameters";
         public const string AGING_PARAMETER_EXPORT = "_Aging";
         public const string AGING_DATA_TABLE_NAME = "AgingData";
         public const string PREGNANT = "Pregnant";
      }

      public static class Process
      {
         public static readonly string BILIARY_CLEARANCE_TO_GALL_BLADDER = "LiverActiveEffluxToGallbladder_FirstOrder";
         public static readonly string BILIARY_CLEARANCE_TO_DUODENUM = "LiverActiveEffluxToDuodenum_FirstOrder";
         public static readonly string KIDNEY_CLEARANCE = "KidneyClearance";
      }

      public static class ProcessType
      {
         public static readonly string PASSIVE = "Passive";
         public static readonly string APPLICATION = "Application";
         public static readonly string ELIMINATION = "Elimination";
         public static readonly string METABOLIZATION = "Metabolization";
         public static readonly string SECRETION = "Secretion";
         public static readonly string ELIMINATION_GFR = "EliminationGFR";
         public static readonly string ACTIVE_TRANSPORT = "ActiveTransport";
      }

      public static class Serialization
      {
         public static readonly string UserSettings = "UserSettings";
         public static readonly string ApplicationSettings = "ApplicationSettings";
         public static readonly string Compressed = "Compressed";
         public static readonly string SimulationList = "SimulationList";
         public static readonly string Simulation = "Simulation";
         public static readonly string RootNode = "RootNode";
         public static readonly string ObservedDataList = "ObservedDataList";
         public static readonly string ObservedData = "ObservedData";
         public static readonly string ProjectFile = "ProjectFile";
         public static readonly string Parameter = "Para";
         public static readonly string DistributedParameter = "DistPara";
         public static readonly string ExpressionDataSet = "ExpressionDataSet";
         public static readonly string LayoutSettings = "LayoutSettings";
         public static readonly string Route = "Route";
         public static readonly string Origin = "Origin";
         public static readonly string OriginData = "OriginData";
         public static readonly string Project = "Project";
         public static readonly string SummaryChart = "SummaryChart";
         public static readonly string PopulationSettings = "PopulationSettings";
         public static readonly string Percentiles = "Percentiles";
         public static readonly string PivotPositionList = "PivotPositionList";
         public static readonly string WorkspaceLayout = "WorkspaceLayout";

         public static class Attribute
         {
            public static readonly string DefaultValue = "default";
            public static readonly string TimeUnit = "TimeUnit";
            public static readonly string Type = "type";
            public static readonly string Path = "path";
            public static readonly string Name = "name";
            public static readonly string Species = "species";
            public static readonly string Area = "area";
            public static readonly string BuildingBlockType = "bb";
            public static readonly string Mode = "mode";
            public static readonly string ParameterId = "para";
            public static readonly string BuildingBlockId = "bb";
            public static readonly string RHSFormula = "rhs";
            public static readonly string ProteinName = "proteinName";
            public static readonly string SelectedUnit = "Unit";
            public static readonly string XmlVersion = "xmlVersion";
            public static readonly string Sequence = "seq";
            public static readonly string Id = "id";
            public static readonly string Expression = "expression";
            public static readonly string ReferenceSimulation = "referenceSimulation";
         }
      }

      public static class Species
      {
         public static readonly string HUMAN = "Human";
         public static readonly string RAT = "Rat";
         public static readonly string MOUSE = "Mouse";
         public static readonly string RABBIT = "Rabbit";
         public static readonly string CAT = "Cat";
         public static readonly string CATTLE = "Cattle";
         public static readonly string BEAGLE = "Beagle";
         public static readonly string MINIPIG = "Minipig";

         public static IEnumerable<string> SpeciesUsingVenousBlood = new List<string>
         {
            MOUSE,
            RAT,
         };
      }

      public static class Tags
      {
         public static readonly string EVENTS = "Events";
         public static readonly string APPLICATION = "Application";
         public static readonly string APPLICATION_ROOT = "ApplicationRoot";
         public static readonly string MOLECULE = "MOLECULE";
         public static readonly string LUMEN_SEGMENT = "LumenSegment";

         public static string ParticlesApplicationWithNBins(int binIndex)
         {
            return $"ParticlesApplication_{binIndex}_Bins";
         }
      }

      
      public static class Covariates
      {
         public static readonly string GENDER = "Gender";
         public static readonly string RACE = "Race";
         public static readonly string POPULATION_NAME = "Population Name";
         public static readonly string SIMULATION_NAME = "Simulation Name";
      }

      public static class ObservedData
      {
     
         public static IReadOnlyList<string> DefaultProperties = new List<string>
         {
            Constants.ObservedData.ORGAN,
            Constants.ObservedData.COMPARTMENT,
            Constants.ObservedData.MOLECULE,
            Constants.ObservedData.SPECIES,
            Constants.ObservedData.GENDER
         };
      }

      public static class ProcessClasses
      {
         public static readonly string ENZYMATIC = "Enzymatic";
         public static readonly string INHIBITION = "Inhibition";
         public static readonly string INDUCTION = "Induction";
         public static readonly string NONE = "None";
         public static readonly string SPECIFIC_BINDING = "SpecificBinding";
         public static readonly string TRANSPORT = "Transport";
      }

      public static class Reaction
      {
         public static readonly string FC_RN_BINDING_DRUG_ENDOSOME = FcRnBindingDrugEndosomeNameFrom("drug");
         public static readonly string FC_RN_BINDING_DRUG_INTERSTITIAL = FcRnBindingDrugInterstitialNameFrom("drug");
         public static readonly string FC_RN_BINDING_DRUG_PLASMA = FcRnBindingDrugPlasmaNameFrom("drug");
         public static readonly string FC_RN_BINDING_TISSUE = "FcRn binding tissue";
         public static readonly string TURNOVER = ProteinTurnoverNameFor("Protein");

         public static readonly string[] REACTIONS_WHICH_REQUIRE_RENAMING = 
         {
            FC_RN_BINDING_DRUG_ENDOSOME,
            FC_RN_BINDING_DRUG_INTERSTITIAL,
            FC_RN_BINDING_DRUG_PLASMA,
            FC_RN_BINDING_TISSUE,
            TURNOVER
         };

         public static string ProteinTurnoverNameFor(string proteinName)
         {
            return $"{proteinName} Turnover";
         }

         public static string FcRnBindingDrugEndosomeNameFrom(string compoundName)
         {
            return $"FcRn binding {compoundName} in endosomal space";
         }

         public static string FcRnBindingDrugInterstitialNameFrom(string compoundName)
         {
            return $"FcRn binding {compoundName} in interstitial";
         }

         public static string FcRnBindingDrugPlasmaNameFrom(string compoundName)
         {
            return $"FcRn binding {compoundName} in plasma";
         }

         public static string FcRnBindingTissueNameFrom(string compoundName)
         {
            return $"{FC_RN_BINDING_TISSUE} {compoundName}";
         }
      }

      public static class DiseaseStates
      {
         public const string CKD = "CKD";
         public const string HI = "HI";
         public const string HEALTHY = "Healthy";

      }
   }
}