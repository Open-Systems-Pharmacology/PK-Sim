using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OSPSuite.Assets.Extensions;
using OSPSuite.Utility.Extensions;

namespace PKSim.Assets
{
   public static class PKSimConstants
   {
      public static class Warning
      {
         public const string RenalAndGFRSelected = "Renal plasma clearance should not be used in conjunction with GFR";
         public const string HepaticAndSpecific = "Using hepatic plasma clearance in conjunction with metabolism processes might lead to more clearance than expected";
         public const string ThisItNotATemplateBuildingBlock = "This is not the template building block!";
         public const string FractionAbsorbedAndEHC = "Please note that, e.g. in the case of enterohepatic circulation, the calculated fraction of dose absorbed may exceed 1";
         public const string BioAvailabilityAndFractionAbsorbed = "For proper calculation of the AUCinf (PO) it is recommended to simulate as long as total gastrointestinal transit takes.";
         public const string PopulationFileIsUsingOldFormatAndWontBeSupportedInTheNextVersion = "Population file is using an old format that will not be supported in future version";
         public const string InhibitorClearanceMustBeDefinedSeparately = "Please note that for the mechanism-based inactivator no clearance process is defined via the inactivation process by default. In theory, for every inactivated target molecule, also one inactivator molecule is cleared; this must be separately defined by the user in form of additional clearance processes for the inhibitor.";
         public const string FractionAbsorbedSmallerThanOne = "Absorption seems to be incomplete or absorption process is not finished. Vd, Vss (or Vd/F and Vss/F), t1/2, MRT and AUC_inf should be compared with respective PK-parameters from an IV simulation.";

         public static string OneMetaboliteWasRenamed(string parentCompound)
         {
            return $"At least one metabolite was renamed.\nThe default metabolite name (found in the metabolism process) associated with the parent compound '{parentCompound}' will remain unchanged.";
         }

         public static string SystemicProcessAvailableInCompoundButWasNotSelected(string systemicProcessType)
         {
            return $"{systemicProcessType} is available in compound but was not activated.";
         }

         public static string ProteinAvailableButProcessNotSelected(string name)
         {
            return $"No compound process selected for protein '{name}'.";
         }

         public static string NoTransporterTemplateFoundForTransporter(string transporterName, string transportType)
         {
            return $"The transporter '{transporterName}' was not found in the database. The transport direction is therefore set to the default setting '{transportType}'";
         }

         public static string ParameterWithPathNotFoundInBaseIndividual(string parameterPath)
         {
            return $"Parameter '{parameterPath}' was not found in individual and will be ignored.";
         }

         public static string PKParameterAlreadyExistsAndWillBeOverwritten(string name, string quantityPath)
         {
            return $"PK-Parameter '{name}' for output '{quantityPath}' already exists in simulation and will be overwritten.";
         }

         public static string DerivedFieldWasSavedForAnotherField(string referencedFieldName, string dataField)
         {
            return $"Grouping field was defined for '{referencedFieldName}'. However you are trying to use it for '{dataField}'. Do you want to continue?";
         }

         public static string ParameterPathNotFoundInSimulationAndWillBeIgnored(string parameterPath)
         {
            return $"Parameter '{parameterPath}' was not found in simulation and will be ignored";
         }

         public static string MissingSimulationParametersWereOverwritten(IEnumerable<string> missingParameters)
         {
            return $"These parameters were changed by the user. Because of a simulation reconfiguration, they will not be used for this simulation:\n\n{missingParameters.ToString("\n")}";
         }

         public static string StaticInhibitionRemovedFromApplication(string description)
         {
            var sb = new StringBuilder("WARNING: Static inhibition was removed with version 5.6 of the software. This process was converted automatically to a non inhibition process.");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(description);
            return sb.ToString();
         }

         public static string StaticInhibitionRemovedFromSimulationMapping(IEnumerable<string> processes)
         {
            var sb = new StringBuilder("WARNING: Static inhibition was removed with version 5.6 of the software. The following processe(s) won't be used when cloning or configuring the simulation.\n");
            sb.AppendLine();
            sb.AppendLine(processes.ToString("\n"));
            sb.AppendLine();
            sb.AppendLine("Please check/adjust process mapping in the PROCESSES Tab of the Clone/Configure dialog if needed.");
            return sb.ToString();
         }

         public static string UnitNotFoundInDimensionForParameter(string unit, string dimension, string parameterName)
         {
            return $"Unit '{unit}' not found for parameter {parameterName} with dimension '{dimension}";
         }
      }

      public static class Command
      {
         public static readonly string CommandTypeConfigure = OSPSuite.Assets.Command.CommandTypeConfigure;
         public static readonly string CommandTypeAdd = OSPSuite.Assets.Command.CommandTypeAdd;
         public static readonly string CommandTypeEdit = OSPSuite.Assets.Command.CommandTypeEdit;
         public static readonly string CommandTypeUpdate = OSPSuite.Assets.Command.CommandTypeUpdate;
         public static readonly string CommandTypeDelete = OSPSuite.Assets.Command.CommandTypeDelete;
         public static readonly string CommandTypeSwap = OSPSuite.Assets.Command.CommandTypeSwap;
         public static readonly string CommandTypeReset = OSPSuite.Assets.Command.CommandTypeReset;
         public static readonly string CommandTypeScale = "Scale";
         public static readonly string ConfigureSimulationDescription = "Configure simulation";
         public static readonly string ResetParametersDescription = "Reset parameters to their default values.";
         public static string ScaleParametersDescription(double factor) => $"Scaling parameters with factor '{factor}'.";
         public static readonly string SetParameterValueAndDisplayUnitDescription = "Value and unit updated for parameter.";
         public static readonly string SetPercentileValueDescription = "Percentile of parameter '{0}' set from '{1}' to '{2}.";
         public static readonly string SetParameterFormulaDescription = "Formula of parameter '{0}' was set.";
         public static readonly string UpdateTableParameterFormula = "Table formula of parameter '{0}' was updated.";
         public static readonly string CreateCompoundDescription = "Create and add compound to project";
         public static readonly string UpdateBuildingBlockInfoCommandDescription = "Building block info updated in simulation.";
         public static readonly string SetUsedBuildingBlockAlteredFlagCommandDescription = "Set altered flag for {0} '{1}' to {2} in simulation '{3}'";
         public static readonly string SetUsedBuildingBlockVersionCommandDescription = "Set version for {0} '{1}' to {2} in simulation '{3}'";
         public static readonly string PerformScalingDescription = "Scaling individual parameters";
         public static readonly string CreateIndividualDescripton = "Create and add individual to project";
         public static readonly string CreateBuildingBlockDescripton = "Create and add {0} to project";
         public static readonly string CreateAdministrationProtocolDescripton = "Create and add administration protocol to project";
         public static readonly string CreateFormulationDescription = "Create and add formulation to project";
         public static readonly string CreateSimulationDescription = "Create and add simulation to project";
         public static string SetSimpleProtocolDosingIntervalDescription(string oldDosingInterval, string newDosingInterval) => $"Dosing interval changed from '{oldDosingInterval}' to '{newDosingInterval}'";
         public static string RenameEnzymeInPartialProcess(string processName, string name) => $"Enzyme name in process '{processName}' set to '{name}'";
         public static string CloneEntity(string type, string sourceName, string cloneName) => $"Cloning {type} '{sourceName}' to '{cloneName}'";

         public static readonly string AddSimulationIntervalToSimulationOutputDescription = "Output interval added to simulation output.";
         public static readonly string RemoveSimulationIntervalFromSimulationOutputDescription = "Output interval removed from simulation output.";
         public static readonly string ObservedDataDeletedFromProject = "Observed data deleted from project";
         public static readonly string MetaDataAddedToDataRepositories = "Meta Data added to multiple repositories";
         public static readonly string MetaDataRemovedFromDataRepositories = "Meta Data removed from multiple repositories";
         public static readonly string MetaDataModifiedInDataRepositories = "Meta Data modified in multiple repositories";
         public static readonly string ChartTemplate = "Chart Template";

         public static string ProtocolModeChangingFrom(string oldProtocol, string newProtocol) => $"Protocol mode changing from '{oldProtocol}' to '{newProtocol}'";
         public static string ProtocolModeChangedFrom(string oldProtocol, string newProtocol) => $"Protocol mode changed from '{oldProtocol}' to '{newProtocol}'";
         public static string SetProtocolModeCommandDescription(string oldProtocol, string newProtocol) => $"Set administration protocol mode from '{oldProtocol}' to '{newProtocol}'";

         public static string ObjectsDeletedFromProject(string objectType)
         {
            return $"{objectType.Pluralize()} deleted from project";
         }

         public static string SetPopulationSimulationResultsCommandDescription(string simulationName)
         {
            return $"Simulation results imported in simulation '{simulationName}'.";
         }

         public static string AddPKAnalysesToSimulationCommandDescription(string simulationName, string fileName)
         {
            return $"PK-Analyses imported in simulation '{simulationName}' from file '{fileName}'.";
         }

         public static string SetParameterValueDescription(string parameterName, string oldValue, string newValue)
         {
            return $"Value of parameter '{parameterName}' set from '{oldValue}' to '{newValue}'.";
         }

         public static string ResetParameterValueDescription(string parameterName, string oldValue, string newValue)
         {
            return $"Parameter '{parameterName}'  reset from '{oldValue}' to '{newValue}'.";
         }

         public static string ScaleIndividualDescription(string originIndividual, string newIndividual)
         {
            return $"Scaling individual '{originIndividual}' to '{newIndividual}'";
         }

         public static string SetCompoundTypeParameterDescription(string parameterName, string oldType, string newType)
         {
            return $"'{parameterName}' set from '{oldType}' to '{newType}'.";
         }

         public static string SwitchAdvancedParameterDistributionTypeDescription(string parameterName, string oldType, string newType)
         {
            return $"Change distribution type for parameter '{parameterName}' from '{oldType}' to '{newType}'";
         }

         public static string RenameMoleculeInPartialProcessesCommandDescription(string moleculeName)
         {
            return $"Rename molecule '{moleculeName}";
         }

         public static string RenameEntityCommandDescripiton(string objectType, string oldName, string newName)
         {
            return $"{objectType} '{oldName}' renamed to '{newName}'.";
         }

         public static string UpdateUsedBuildingBlockParameterCommandDescription(string buildingBlockName, string buildingBlockType, string simulationName)
         {
            return string.Format("Update {0} parameters in simulation '{1}' from {0} '{2}'", buildingBlockType.ToLower(), simulationName, buildingBlockName);
         }

         public static string UpdateTemplateParameterCommandDescription(string buildingBlockName, string buildingBlockType, string simulationName)
         {
            return string.Format("Update {0} '{1}' from {0} parameters in simulation '{2}'", buildingBlockType.ToLower(), buildingBlockName, simulationName);
         }

         public static string SetParameterUnitDescription(string parameterName, string value, string oldUnit, string newUnit)
         {
            return string.Format("Value of parameter '{0}' set from '{1} {2}' to '{1} {3}'.", parameterName, value, oldUnit, newUnit);
         }

         public static string SetParameterDisplayUnitDescription(string parameterName, string oldUnit, string newUnit)
         {
            return $"Display unit of parameter '{parameterName}' set from '{oldUnit}' to '{newUnit}'.";
         }

         public static string SetApplicationSchemaItemApplicationTypeDescription(string oldApplicationType, string newApplicationType)
         {
            return $"Administration type changed from '{oldApplicationType}' to '{newApplicationType}'";
         }

         public static string SetApplicationSchemaItemFormulationKeyDescription(string oldFormulaKey, string newFormulaKey)
         {
            if (string.IsNullOrEmpty(newFormulaKey))
               return $"Removing formulation {oldFormulaKey}";

            if (string.IsNullOrEmpty(oldFormulaKey))
               return $"Setting formulation to {newFormulaKey}";

            return $"Formulation name changed from '{oldFormulaKey}' to '{newFormulaKey}'";
         }

         public static string SetApplicationSchemaItemTargetCompartment(string oldTargetCompartment, string newTargetCompartment)
         {
            return $"Target compartment changed from '{oldTargetCompartment}' to '{newTargetCompartment}'";
         }

         public static string SetApplicationSchemaItemTargetOrgan(string oldTargetOrgan, string newTargetOrgan)
         {
            return $"Target organ changed from '{oldTargetOrgan}' to '{newTargetOrgan}'";
         }

         public static string SwapBuildingCommandDescription(string buildingBlockType, string buildingBlockName)
         {
            return $"Update {buildingBlockType} '{buildingBlockName}'";
         }

         public static string UpdateBuildingBlockCommandDescription(string buildingBlockType, string buildingBlockName, string simulationName)
         {
            return $"{buildingBlockType} '{buildingBlockName}' updated in simulation '{simulationName}'.";
         }

         public static string UpdateTemplateBuildingBlockCommandDescription(string buildingBlockType, string buildingBlockName, string simulationName)
         {
            return $"{buildingBlockType} '{buildingBlockName}' updated from simulation '{simulationName}'.";
         }

         public static string EditEntityDescriptionCommandDescripiton(string entityType, string entityName)
         {
            return $"Description updated for {entityType} '{entityName}'";
         }

         public static string SwitchPartialProcessKineticType(string oldKinetic, string newKinetic)
         {
            return $"Switch kinetic from '{oldKinetic}' to '{newKinetic}'";
         }

         public static string SetSpeciesInSpeciesDependentEntityDescription(string objectType, string name, string oldSpecies, string newSpecies)
         {
            return $"Species in {objectType} '{name}' was set from '{oldSpecies}' to '{newSpecies}'";
         }

         public static string SetDefaultAlternativeParameterDescription(string groupDisplayName, string oldDefaultAlternative, string newDefaultAlternative)
         {
            return $"Default alternative for {groupDisplayName} was set from '{oldDefaultAlternative}' to '{newDefaultAlternative}'";
         }

         public static string AddCompoundParameterGroupAlternativeDescription(string alternativeName, string groupName)
         {
            return AddEntityToContainer(ObjectTypes.ParameterGroupAlternative, alternativeName, ObjectTypes.ParameterGroup, groupName);
         }

         public static string RemoveCompoundParameterGroupAlternativeDescription(string alternativeName, string groupName)
         {
            return RemoveEntityFromContainer(ObjectTypes.ParameterGroupAlternative, alternativeName, ObjectTypes.ParameterGroup, groupName);
         }

         public static string AddEntityToContainer(string entityType, string entityName, string containerType, string containerName)
         {
            var lowerEntityType = string.IsNullOrEmpty(entityType) ? entityType : entityType.ToLower();
            var lowerContainerType = string.IsNullOrEmpty(containerType) ? containerType : containerType.ToLower();

            return $"Add {lowerEntityType} '{entityName}' to {lowerContainerType} '{containerName}'";
         }

         public static string RemoveEntityFromContainer(string entityType, string entityName, string containerType, string containerName)
         {
            var lowerEntityType = string.IsNullOrEmpty(entityType) ? entityType : entityType.ToLower();
            var lowerContainerType = string.IsNullOrEmpty(containerType) ? containerType : containerType.ToLower();
            return $"Delete {lowerEntityType} '{entityName}' from {lowerContainerType} '{containerName}'";
         }

         public static string SetOntogenyInProteinDescription(string individualName, string proteinName, string oldOntogeny, string newOntogeny)
         {
            return string.Format("Set ontogeny for protein '{1}' in individual '{0}' from '{2}' to '{3}'", individualName, proteinName, oldOntogeny, newOntogeny);
         }

         public static string SetTransportTypeCommandDescription(string transporterName, string oldTransporterType, string newTransporterType)
         {
            return $"Transporter type for '{transporterName}' was changed from '{oldTransporterType}' to '{newTransporterType}'";
         }

         public static string SetMembraneTypeCommandDescription(string transporterName, string containerName, string oldMembraneType, string newMembraneType)
         {
            return $"Membrane location for '{transporterName}' in '{containerName}' was changed from '{oldMembraneType}' to '{newMembraneType}'";
         }

         public static string SetCompartmentTypeInAllContainerCommandDescription(string proteinName, string oldCompartmentName, string newCompartmentName)
         {
            return $"Compartment set for '{proteinName}' from '{oldCompartmentName}' to '{newCompartmentName}'";
         }

         public static string SetProteinMembraneLocationDescription(string oldMembraneLocation, string newMembraneLocation)
         {
            return $"Set protein membrane location  from '{oldMembraneLocation}' to '{newMembraneLocation}'";
         }

         public static string SetProteinTissueLocationDescription(string oldTissueLocation, string newTissueLocation)
         {
            return $"Set protein tissue location  from '{oldTissueLocation}' to '{newTissueLocation}'";
         }

         public static string RunSimulationDescription(string simulationName, string formattedTime)
         {
            return $"Run simulation '{simulationName}' in {formattedTime}";
         }

         public static string ObjectConvertedDescription(string buildingBlockName, string buildingBlockType, string fromVersion, string toVersion)
         {
            return string.Format("{1} '{0}' converted from version '{2}' to version '{3}'", buildingBlockName, buildingBlockType, fromVersion, toVersion);
         }

         public static string ProjectRenamedDescription(string oldName, string newName)
         {
            return RenameEntityCommandDescripiton(ObjectTypes.Project, oldName, newName);
         }

         public static string ProjectConvertedFrom(string projectFile)
         {
            return $"Project converted from '{projectFile}'";
         }

         public static string SetMetaboliteForProcess(string metaboliteName, string processName, string oldMetaboliteName)
         {
            if (!string.IsNullOrEmpty(oldMetaboliteName))
               return $"Changing metabolite for process '{processName}' from '{oldMetaboliteName}' to '{metaboliteName}'";

            return $"Adding metabolite '{metaboliteName}' to process '{processName}'";
         }

         public static string SetCalculationMethodFor(string entityName, string oldCalculationName, string newCalculationName)
         {
            return $"Changing calculation method for {entityName} from {oldCalculationName} to {newCalculationName}";
         }

         public static string AddChartTemplateToSimulation(string templateName, string simulationName)
         {
            return $"Adding template '{templateName}' to '{simulationName}'";
         }

         public static string RemoveChartTemplateFromSimulation(string templateName, string simulationName)
         {
            return $"Removing template '{templateName}' from '{simulationName}'";
         }

         public static string EditChartTemplatesFor(string simulationName)
         {
            return $"Editing templates in simulation '{simulationName}'";
         }

         public static string UpdateChartTemplate(string templateName, string simulationName)
         {
            return $"Updating chart template '{templateName}' in simulation '{simulationName}'";
         }

         public static string AddDefaultVariabilityToPopulation(string populationName)
         {
            return $"Add known molecule variability to molecules defined in '{populationName}'";
         }

         public static string RemoveAdvancedParametersForMoleculeInPopulation(string moleculeName, string populationName)
         {
            return $"Remove advanced parameters defined for molecule '{moleculeName}' in population '{populationName}'";
         }

         public static string ExtractingIndividualsDescription(string populationName)
         {
            return $"Extracting individuals from population '{populationName}'";
         }
      }

      public static class Error
      {
         public const string ValueIsRequired = "Value is required.";
         public const string DescriptionIsRequired = "Description is required.";
         public const string UnknownObserverBuilderType = "Observer builer type unknown.";
         public static string CompoundProcessNotExists(string processName) => $"Compound process {processName} does not exist.";
         public static string UnableToCreateIndividual(string constraints) => $"Could not create individuals with given constraint:\n{constraints}";
         public static string UnableToCreatePopulation(string constraints) => $"Could not create population with given constraint:\n{constraints}";
         public const string FactorShouldBeBiggerThanZero = "Factor should be bigger than 0.";
         public const string UnableToCreateInstanceOfShell = "Unable to create an instance of the shell.";
         public static string DistributionNotFound(string entityName, string data) => $"Cannot create distribution for '{entityName}' with the following data:\n{data}";
         public static string DistributionUnknown(string distribution) => $"Distribution '{distribution}' is unknown.";
         public const string NameIsRequired = "Name is required.";
         public const string MoleculeIsRequired = "Molecule is required.";
         public const string DataSourceIsRequired = "Data source is required.";
         public static string ProteinExpressionFactoryNotFound(string enzymeType) => $"Cannot retrieve enzyme expression factory for enzyme type '{enzymeType}'.";
         public const string RenameSameNameError = "The new name is the same as the original one.";
         public const string NoBuildingBlockTemplateSelected = "No template selected.";
         public static string CannotCreateContainerOfType(string type) => $"Cannot create container of type '{type}'.";
         public static string UnknownUsageInIndividualFlag(string flag) => $"'{flag}' is not valid flag for 'Usage in individual'.";
         public static string CompoundParameterSelectionNeededFor(string parameterName) => $"Compound parameter selection is required for '{parameterName}'.";
         public const string InvalidPartialStabiLink = "Invalid link for creating enzymatic stability partial process passed.";
         public const string ContainerPathIsEmpty = "Given container path is empty.";
         public const string CannotDeleteSchemaItem = "At least one schema item needs to be defined.";
         public const string CannotDeleteSimulationInterval = "At least one interval needs to be defined.";
         public const string CannotDeleteSchema = "At least one schema needs to be defined.";
         public const string CannotDeleteDefaultParameterAlternative = "The default parameter alternative cannot be deleted.";
         public const string CannotDeleteParameterAlternative = "At least one alternative needs to be defined.";
         public static string CouldNotFindAdvancedParameterContainerForParameter(string parameterName) => $"Could not find advanced parameter container for parameter '{parameterName}'.";
         public static string CouldNotFindAdvancedParameterInContainerForParameter(string containerName, string parameterName) => $"Could not find advanced parameter in container '{containerName}' for parameter '{parameterName}'.";
         public static string CompoundProcessParameterMappingNotAvailable(string process, string parameter) => $"No compound process parameter mapping found for process='{process}' and parameter ='{parameter}'.";
         public const string InvalidNumberOfBins = "Number of particle bins must be in [1..20].";
         public const string InvalidParticleSizeDistribution = "Unknown particles size distribution passed.";
         public const string FirstOrderActiveTransportsNotSupported = "First order active transports are not supported.";
         public const string CannotSwitchToAdvancedProtocolWhenUsingUserDefinedAppplication = "User defined administration cannot be used with an advanced administration protocol.";
         public const string MolWeightNotAvailable = "Molecular Weight not available.";
         public const string MolWeightNotAvailableForPopulationSimulationComparison = "Molecular Weight was not found or is not the same in all compared simulations.";
         public const string EventGroupSubContainerHasInvalidType = "Subcontainer of event group must be of type event or event group.";
         public static string NoStartTimeInEventBuilder(string eventName, string timeName) => $"Event group builder {eventName} does not contain parameter {timeName}.";
         public static string ModelNotAvailableForSpecies(string model, string species) => $"Model {model} is not available for species {species}.";
         public const string UnableToConverterIntestinalSecretion = "intestinal secretion directly into feces as a surrogate for luminal secretion and subsequent transport into faeces without re-absorption from the lumen is not supported any longer but can mechanistically be implemented.";
         public static string ExtendedNeighborhoodNotAllowed(string neighborhood) => $"Neighborhood {neighborhood} is marked as EXTENDED which is not allowed.";
         public const string FourCompModelCannotBeUsedWithLargeMolecule = "Model for small molecules cannot be used for a large molecule.\nPlease use the dedicated model for proteins and large molecules.";
         public const string ProjectNeedsToBeSavedFirst = "Project needs to be saved first.";
         public const string SimulationCloneOnlyAvailableWhenBuildingBlocksAreUptodate = "Cloning a simulation requires that all building blocks are consistent with the simulation (e.g. all have green checkmarks).";
         public const string NoReportTemplateDefined = "Could not find any report templates. Please check your installation.";
         public const string CompoundAndSimulationCannotShareTheSameName = "Compound and simulation cannot share the same name. Please rename your simulation.";
         public const string IndividualMoleculesAnSimulationCannotShareTheSameName = "The simulation cannot share a name with any molecules defined in the individual.";
         public const string ErrorWhileImportingPopulationAnalyses = "Some errors occured while importing the population analyses:";
         public const string DifferentVectorLengths = "Vectors have different length!";
         public const string MoBiNotFound = "MoBi was not found on current system. Please make sure that MoBi was installed using the provided setup. Alternatively, you can specify where MoBi is installed on your system under Utilities -> Options -> Application.";
         public const string CannotExportAnImportedSimulation = "An imported simulation (e.g. from MoBi or pkml Format) cannot be exported.";
         public const string AtLeastOneCompoundMustBeSelected = "At least one compound must be selected.";
         public const string AtLeastOneFileRequiredToStartPopulationImport = "At least one file is required to perform the population import.";
         public const string AtLeastOneProtocolRequiredToCreateSimulation = "Select at least one protocol for the administered compound(s).";
         public const string AProtocolCanOnlyBeUsedOnceInASimulation = "Each administered compound must have a unique administration protocol.";
         public const string CanOnlyCompareTwoObjectsAtATime = "Object comparison is only available for two objects at the same time.";
         public const string AdvancedCommitNotAvailable = "Advanced commit is not supported.";
         public const string KeywordsAndReplacementsSizeDiffer = "Keywords and replacementValues do not have the same length!";
         public const string GenderAndOrPopulationMissingFromFile = "Gender and/or Population are not defined in the file to import.";
         public const string FormulationShouldBeUsedAsTemplateOrAsSimulationBuildingBlock = "Formulation usage is inconsitent. Please use either the template formulation or the simulation formulation.";
         public const string AtLeastOneIndividualIdRequiredToPerformPopulationExtraction = "At least one valid individual id is required to perform the population extraction.";

         public static string DosePerBodySurfaceAreaProtocolCannotBeUsedWithSpeciesPopulation(string speciesPopulation) => $"Body surface area dosing cannot be used with species '{speciesPopulation}'.";

         public static string PregnantPopulationCanOnlyBeUsedWithMoBiModel(string speciesPopulation) => $"Population based on '{speciesPopulation}' can only be used with pregnancy models imported from MoBi.";

         public static string SimulationResultsFileDoesNotHaveTheExpectedFormat
         {
            get
            {
               var sb = new StringBuilder();
               sb.AppendLine("Simulation result files does not have the expected format:");
               sb.AppendLine(" - Column headers are required (e.g. IndividualId;Time;....)");
               sb.AppendLine(" - The 1st column represents the individual id");
               sb.AppendLine(" - The 2nd column represents the time values");
               return sb.ToString();
            }
         }

         public static string SimulationPKAnalysesFileDoesNotHaveTheExpectedFormat
         {
            get
            {
               var sb = new StringBuilder();
               sb.AppendLine("Simulation pk-Analyses files does not have the expected format:");
               sb.AppendLine(" - Column headers are required (e.g. IndividualId;Output;Parameter;Value;Unit)");
               sb.AppendLine(" - The 1st column represents the individual id");
               sb.AppendLine(" - The 2nd column represents the output for which the PK-Parameter was calculated");
               sb.AppendLine(" - The 3rd column represents the Name of the PK-Parameter");
               sb.AppendLine(" - The 4th column represents the Value of the PK-Parameter");
               sb.AppendLine(" - The 5th column represents the Unit in which the value in the 4th column is saved");
               return sb.ToString();
            }
         }

         public const string EventTemplateNotDefined = "Event template not defined.";

         public static string FormulationIsRequiredForType(string applicationType) => $"Formulation is requiered for type '{applicationType}'.";

         public static string BuildingBlockNotDefined(string buildingblock) => $"No {buildingblock} defined. Please use create.";

         public static string MissingColumnInView(string propertyName) => $"Property named {propertyName} not found.";

         public static string ProjectFileIsCorrupt(string productName) => $"Project file is corrupted and unreadable or this is not a {productName} project file.";

         public static string CannotCalculateDDIRatioFor(string parameterName) => $"Cannot calculate AUC Ratio: don't know how to handle parameter {parameterName}.";

         public static string NoBatchFileFoundIn(string inputFolder) => $"No simulation file found in '{inputFolder}'.";

         public static string TableFormulaWithOffsetMissingRefs(string rateKey, string ref1, string ref2) => $"Table formula with offset '{rateKey}' must contain references to '{ref1}' and '{ref2}'.";

         public static string ModelContainerNotAvailable(string containerName) => $"Model container '{containerName}' not available.";

         public static string ProjectFileIsReadOnlyAndCannotBeRead(string fileFullPath)
         {
            return $"The file '{fileFullPath}' is readonly and cannot be read by the application. Please make the project writable and try again.";
         }

         public static string ProjectVersionCannotBeLoaded(int projectVersion, int currentVersion, string dowloadUrl)
         {
            if (projectVersion > currentVersion)
               return $"The application is too old (compatible version {currentVersion}) and cannot load a project created with a newer version (project version {projectVersion}).\nVisit our download page at {dowloadUrl}.";

            return $"Work in progress.\nThis project file was created with a beta or RC version (version {projectVersion}) and cannot be loaded.\nSorry :-(";
         }

         public static string BuildingBlockVersionIsTooOld(int version)
         {
            return $"Work in progress.\nThe Building Block is too old (version {version}) and cannot be loaded.\nSorry :-(";
         }

         public static string CannotRenameCompoundUsedInSimulations(string compoundName, string simulationsList)
         {
            return $"{ObjectTypes.Compound} '{compoundName}' is used in simulations \n{simulationsList}\n and cannot be renamed.";
         }

         public static string CannotRenameCompoundUsedInSimulation(string buildingBlockName, string simulationName)
         {
            return $"{ObjectTypes.Compound} '{buildingBlockName}' is used in simulation '{simulationName}' and cannot be renamed.";
         }

         public static string CannotFindFormulationForMapping(string formulationId, string formulationKey, string simulationName)
         {
            return $"Cannot find formulation with id '{formulationId}' for formulation key '{formulationKey}' in simulation '{simulationName}'.";
         }

         public static string NoPartialTemplateProcessFound(string processType) => $"No templates for partial process {processType} found.";

         public static string BuildingBlockAlreadyExists(string buildingBlockType, string name) => $"{buildingBlockType} named '{name}' already exists in project.";

         public static string NameAlreadyExistsInContainerType(string name, string containerType) => $"'{name}' already exists in {containerType}.";

         public static string CompoundProcessDeclaredAsNotTemplate(string processName) => $"Compound process '{processName}' is not declared as a template.";

         public static string ProjectFileDoesNotExist(string projectFile) => $"Project file '{projectFile}' has been deleted or is not accessible.";

         public static string ProjectFileVersion4IsNotSupportedAnymore(string projectFile)
         {
            var sb = new StringBuilder();
            sb.AppendLine("Support for project file of version 4.2 and older has ended with version 5.6 of the software.");
            sb.AppendLine();
            sb.AppendLine($"In order to convert the file '{projectFile}' to the latest PK-Sim version please proceed as follows:");
            sb.AppendLine();
            sb.AppendLine("  1 - Install an earlier version of PK-Sim (version 5.5 or earlier) and open the file");
            sb.AppendLine("  2 - Save the file");
            sb.AppendLine("  3 - The file is now converted and can be open with any newer version of PK-Sim");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("Note: PK-Sim version 4.2 and 5.x or newer can be installed on the same computer without any issues.");
            return sb.ToString();
         }

         public static string FileIsNotAPKSimFile(string projectFile, string productName)
         {
            return $"File '{projectFile}' is not a {productName} project file.";
         }

         public static string FileIsNotASimulationFile(string simualtionFile, string productName)
         {
            return $"File '{simualtionFile}' is not a {productName} simulation file.";
         }

         public static string NoTemplateBuildingBlockAvailableForType(string buildingBlockType)
         {
            return $"No template '{buildingBlockType}' available in the template database.";
         }

         public static string UnableToUpdateParameterException(string parameterPath, string simulationName)
         {
            return $"Unable to update parameter.\nParameter with path '{parameterPath}' not found in simulation '{simulationName}'.";
         }

         public static string ConstantParameterAlreadyExistsInContainer(string containerName, string parameterName)
         {
            return $"Parameter '{parameterName}' already exists in '{containerName}' but is defined as a constant parameter.";
         }

         public static string FormulaParamterAlreadyExistsInContainerWithAnotherFormula(string containerName, string parameterName, string formulaString, string formulaStringToAdd)
         {
            return $"Parameter '{parameterName}' already exists in '{containerName}' with another formula:\nOld formula = '{formulaString}'\nNew formula = '{formulaStringToAdd}'.";
         }

         public static string FormulationCannotBeUsedWithRoute(string formulationName, string applicationRoute)
         {
            return $"Formulation '{formulationName}' cannot be used with route '{applicationRoute}.";
         }

         public static string NoFormulationFoundForRoute(string protocolName, string applicationRoute)
         {
            return $"No formulation found for route '{applicationRoute}' in administration protocol '{protocolName}'.";
         }

         public static string UnableToCreateSimulationWithMoleculesHavingSameName(string duplicateName)
         {
            return $"Simulation cannot be created. The selected configuration would result in having two molecules named '{duplicateName}' at the same place.";
         }

         public static string IntervalNotDefinedForParameter(string parameterName) => $"Interval not defined for parameter '{parameterName}'.";

         public static string NameCannotContainIllegalCharacters(IEnumerable<string> illegalCharacters)
         {
            return $"Name cannot contain any of the following characters:\n{illegalCharacters.ToString(", ", "'")}";
         }

         public static string MoleculeNameCannotBeUsedAsItWouldCreateDuplicateProcesses(string moleculeName)
         {
            return $"Molecule cannot be renamed to '{moleculeName}'. Two or more processes would have the same name.";
         }

         public static string CannotSelectTheSamePartialProcessMoreThanOnce(string processName)
         {
            return $"'{processName}' cannot be selected more than once.";
         }

         public static string CouldNotFindSpecies(string species, IEnumerable<string> availableSpecies)
         {
            return $"Could not find species '{species}'.\nAvailable species are:\n{availableSpecies.ToString("\n")}";
         }

         public static string CouldNotFindPopulationForSpecies(string population, string species, IEnumerable<string> availablePopulations)
         {
            return $"Could not find population '{population}' for species '{species}'.\nAvailable populations are:\n\t{availablePopulations.ToString("\n\t")}";
         }

         public static string CouldNotFindGenderForPopulation(string gender, string population, IEnumerable<string> availableGenders)
         {
            return $"Could not find gender '{gender}' for population '{population}'.\nAvailable genders  are:\n\t{availableGenders.ToString("\n\t")}";
         }

         public static string CouldNotFindCalculationMethodInCategory(string calculationMethod, string category, IEnumerable<string> availableCategories)
         {
            return $"Could not find calculation method '{calculationMethod}' in category '{category}'.\nAvailable calculation methods are:\n\t{availableCategories.ToString("\n\t")}";
         }

         public static string CalculationMethodNotDefinedForSpecies(string calculationMethod, string category, string species)
         {
            return $"Calculation method '{calculationMethod}' in category '{category}' is not defined for species '{species}'.";
         }

         public static string CalculationMethodNotFound(string calculationMethod) => $"Calculation method '{calculationMethod}' was not found.";

         public static string SimulationHasNoResultsAndCannotBeUsedInSummaryChart(string simulationName)
         {
            return $"Simulation '{simulationName}' needs to be run first before being used in a summary chart.";
         }

         public static string CouldNotCreatePartialProcessFor(string moleculeName, string processType)
         {
            return $"Could not create partial process '{processType}' for molecule '{moleculeName}'.";
         }

         public static string CannotExportResultsPleaseRunSimulation(string simulationName)
         {
            return $"Run simulation '{simulationName}' first before exporting the results to {UI.Excel}.";
         }

         public static string CannotExportPKAnalysesPleaseRunSimulation(string simulationName)
         {
            return $"Run simulation '{simulationName}' first before exporting the PK-Analyses to CSV.";
         }

         public static string ProjectWillBeOpenedAsReadOnly(string errorMessage)
         {
            return $"{errorMessage}\nAny change made to the project will not be saved.";
         }

         public static string CouldNotFindTransporterFor(string containerName, string membrane, string transportType)
         {
            return $"Could not find a transporter template for container '{containerName}' location '{membrane}' and type '{transportType}'.";
         }

         public static string CannotCreateAgingSimulationWithInvalidPercentile(string parameterPath, double percentile)
         {
            return $"Cannot create aging simulation: The percentile for parameter '{parameterPath}' is invalid. (percentile = {percentile}).";
         }

         public static string FileDoesNotExist(string fileFullPath) => $"File '{fileFullPath}' does not exist.";

         public static string NoDataFieldFoundFor(string name) => $"No data field found for '{name}'.";

         public static string DuplicatedIndividualResultsForId(int individualId)
         {
            return $"Individual results for individual with id '{individualId}' were defined more than once!";
         }

         public static string NumberOfIndividualsInResultsDoNotMatchPopulation(string populationName, int expectedCount, int importedCount)
         {
            return $"The simulation '{populationName}' has '{expectedCount}' individuals. The imported results however contain values for '{importedCount}' individuals.";
         }

         public static string TimeArrayLengthDoesNotMatchFirstIndividual(int id, int expectedLength, int currentLength)
         {
            return $"Time array for individual '{id}' does not have the expected length ({expectedLength} vs {currentLength}).";
         }

         public static string TimeArrayValuesDoesNotMatchFirstIndividual(int id, int index, float expectedValue, float currentValue)
         {
            return $"Time array for individual '{id}' does not have the expected value in row '{index}' ({expectedValue} vs {currentValue}).";
         }

         public static string IndividualResultsDoesNotHaveTheExpectedQuantity(int individualId, IReadOnlyList<string> expectedQuantities, IReadOnlyList<string> foundQuantities)
         {
            var sb = new StringBuilder();
            sb.AppendLine($"Individual results for individual '{individualId}' does not have the expected results:");
            sb.AppendLine($"Expected: {expectedQuantities.ToString(",")}");
            sb.AppendLine($"Found: {foundQuantities.ToString(",")}");
            return sb.ToString();
         }

         public static string CouldNotFindQuantityWithPath(string quantityPath) => $"Could not find quantity with path '{quantityPath}'.";

         public static string NotEnoughPKValuesForParameter(string parameterName, string quantityPath, int expectedValue, int currentValue)
         {
            var sb = new StringBuilder();
            sb.AppendLine($"Number of imported values for PK-Parameter '{parameterName}' in '{quantityPath}' does not have the expected length:");
            sb.AppendLine($"Expected: {expectedValue}");
            sb.AppendLine($"Found: {currentValue}");
            return sb.ToString();
         }

         public static string IndividualIdDoesNotMatchTheValueLength(int indiviudalId, int count)
         {
            return $"Individual Id '{indiviudalId}' does not match the expected number of individual '{count}'. A reason could be that the results were imported starting with an id of 1 instead of 0.";
         }

         public static string GroupingCannotBeCreatedForField(string fieldName)
         {
            return $"Not enough valid values for '{fieldName}' to create a grouping.";
         }

         public static string GroupingCannotBeUsedWithFieldOfType(Type fieldType, string grouping)
         {
            return $"Grouping '{grouping}' cannot be used with field of type '{fieldType}'.";
         }

         public static string QuantityNotFoundWillBeRemovedFromAnalysis(string quantityPath)
         {
            return $"Quantity '{quantityPath}' was not found and will be removed.";
         }

         public static string PKParameterWasNotCalculatedForQuantity(string pkParameter, string quantityPath)
         {
            return $"PK-Parameter '{pkParameter}' was not calculated for quantity '{quantityPath}' and will be removed.";
         }

         public static string ParameterNotFoundWillBeRemovedFromAnalysis(string parameterPath)
         {
            return $"Parameter '{parameterPath}' was not found and will be removed.";
         }

         public static string CovariateNotFoundWillBeRemovedFromAnalysis(string covariate)
         {
            return $"Covariate '{covariate}' was not found and will be removed.";
         }

         public static string OutputFieldCannotBeUsedInAnalysis(string outputName)
         {
            return $"Output '{outputName}' cannot be used in this analysis and will be removed.";
         }

         public static string InconsistentCurveData(int yValuesCount, int xValuesCount)
         {
            return $"Length of y values = {yValuesCount} != {xValuesCount} = length of x values.";
         }

         public static string InconsistentXValuesLength(int xValuesCount, int requiredXValuesCount)
         {
            return $"Length of x values = {xValuesCount} != {requiredXValuesCount}.";
         }

         public static string CouldNotFindDimensionWithUnit(string unit)
         {
            return $"Could not find dimension containing unit '{unit}'.";
         }

         public static string UnitIsNotDefinedInDimension(string unit, string dimension)
         {
            return $"Unit '{unit}' is not defined in dimension '{dimension}'."; 
         }

         public static string DerivedFieldCannotBeUsedForFieldOfType(string derivedField, string dataField, Type dataType)
         {
            return $"Grouping '{derivedField}' cannot be used for field '{dataField}' of type '{dataType.Name}'.";
         }

         public static string CouldNotLoadSimulationFromFile(string pkmlFileFullPath)
         {
            return $"Could not load simulation from file '{pkmlFileFullPath}'.";
         }

         public static string CannotAddOutputFieldBecauseOfDimensionMismatch(string outputName, IEnumerable<string> allowedDimensions, string currentDimension)
         {
            return cannotAddToAnalysisBecauseOfDimensionMismatch("output", outputName, allowedDimensions, new[] {currentDimension});
         }

         public static string CannotAddObservedDataBecauseOfDimensionMismatch(string observedDataName, IEnumerable<string> allowedDimensions, IEnumerable<string> usedDimensions)
         {
            return cannotAddToAnalysisBecauseOfDimensionMismatch(UI.ObservedData, observedDataName, allowedDimensions, usedDimensions);
         }

         private static string cannotAddToAnalysisBecauseOfDimensionMismatch(string objectType, string objectName, IEnumerable<string> allowedDimensions, IEnumerable<string> usedDimensions)
         {
            var sb = new StringBuilder();
            sb.AppendLine($"The {objectType} '{objectName}' cannot be added to the analysis.");
            sb.AppendLine();
            sb.AppendLine($"Its dimension '{usedDimensions.ToString(", ")}' is not in the list of possible dimension(s):");
            sb.Append(allowedDimensions.ToString("\n"));
            return sb.ToString();
         }

         public static string ComparisonWithTemplateNotSupportedForBuildingBlockOfType(string buildingBlockType)
         {
            return $"Comparison with template building block is not supported for {buildingBlockType}.";
         }

         public static string ComparisonBetweenBuildingBLocksNotSupportedForBuildingBlockOfType(string buildingBlockType)
         {
            return $"Comparison between building blocks is not supported for {buildingBlockType}.";
         }

         public static string CannotExtractIndividualFrom(string objectType) => $"Individual extraction is not available for '{objectType}'.";

         public static string SnapshotNotFoundFor(string modelTypeName) => $"Snapshot not found for '{modelTypeName}'.";

         public static string SnapshotParameterNotFoundInContainer(string parameterName, string container) => $"Snapshot parameter '{parameterName}' was not found in '{container}'.";

         public static string SnapshotParameterNotFound(string parameterName) => $"Snapshot parameter '{parameterName}' was not found.";

         public const string SnapshotIsOutdated = "Snapshot is outdated and cannot be loaded.";

         
         public static string MoleculeTypeNotSupported(string moleculeType) => $"Molecule type '{moleculeType}' not supported.";

         public static string RelativeExpressionContainerNotFound(string containerName) => $"Relative expression container '{containerName}' not found.";

         public static string SnapshotProcessNameNotFound(string processName) => $"Snapshot process '{processName}' not found in the PK-Sim database.";

         public static string MapToModelNotSupportedWithoutContext(string modelType, string contextType)
         {
            return $"{modelType} should not be created from snapshot directly. Instead use the overload with {contextType}.";
         }

         public static string MapToSnapshotNotSupportedWithoutContext(string snapshotType, string contextType)
         {
            return $"{snapshotType} should not be created from model directly. Instead use the overload with {contextType}.";
         }

         public const string PopulationSnapshotOnlySupportedForRandomPopulation = "Population snapshot can only be created for randomized population.";

         public const string SimulationSubjectUndefinedInSnapshot = "Simulation subject (Individual or Population) is not defined in snapshot.";

         public static string SimulationTemplateBuildingBlocktNotFoundInProject(string buildingBlockName, string buildingBlockType) => $"{buildingBlockType} '{buildingBlockName} not found in project.";

         public static string ProcessNotFoundInCompound(string processName, string compound) => $"Process '{processName}' was not found in compound '{compound}'";

         public static string OnlyPKSimSimulationCanBeExportedToSnapshot(string simulationName, string origin) => $"Snapshot export is not supported for {origin} simulation '{simulationName}'.";

         public static string SnapshotFileMismatch(string desiredType) => $"Snapshot file cannot be used to load a {desiredType.ToLowerInvariant()}.";
      }
   
      public static class Information
      {
         public static readonly string Formula = "Formula";
         public static readonly string ParameterDescription = "Description";
         public static readonly string Description = "Description";
         public static readonly string ParameterIsAFormulaWithOverridenValue = "Parameter value is a formula that was overwritten.";
         public static readonly string ParameterIsAFormula = "Parameter value is a formula. Overwriting the value might lead to cross-dependency loss.";
         public static readonly string ParameterIsDefinedIn = "Parameter is defined in";
         public static readonly string NoParametersInIndividualSelection = "<B>Note:</B> Default Anatomy and physiology for selected species is preset.\nTo change click on the desired item in the tree view (left part) and change the values in the appearing table in this window";
         public static readonly string NoParametersInSimulationSelection = "<B>Note:</B> Default Anatomy and physiology for selected species is preset.\nTo change click on the desired item in the tree view (left part) and change the values in the appearing table in this window";

         public static string IndividualExpressionInfo
         {
            get
            {
               var sb = new StringBuilder();
               sb.AppendLine("Enter all relevant enzyme, transport, and protein binding settings of the individual by a right click on the selected item in the tree view (left part)");
               sb.AppendLine();
               sb.AppendLine("   If a <I>metabolizing enzyme</I> is defined, the drug can react to its metabolism product within the specified localizations.");
               sb.AppendLine("   If a <I>transport protein</I> is defined, the drug can be transported between specified compartments in the specified directions.");
               sb.AppendLine("   If a <I>protein binding partner</I> is defined, the drug can reversibly bind to the partner within the specified localizations.");
               sb.AppendLine();
               sb.AppendLine("<B>Note:</B> The processes will be effective within a simulation, if the enzymes/transporters/binding partners defined for the individual are linked to the respective proteins defined for the biological properties of the compound. This linking is done while creating a simulation.");

               return sb.ToString();
            }
         }

         public static string SimulationComparisonInfo => "<B>Drag-and-drop the simulations to compare and select curves in data browser of chart editor.</B>";

         public static string NoParameterAvailableForScaling => "No parameter was changed in the base individual. Default Scaling configuration will be used.";

         public static string BuildingBlockSettingsDoNotMatchWithTemplate(string buildingBlockType)
         {
            return $"{buildingBlockType} settings were changed compared to template {buildingBlockType.ToLower()}";
         }

         public static string ProjectNeedsToBeConverted(string projectFile)
         {
            return $"This project was built with an old version of PK-Sim and needs to be converted.\nOne backup copy of the original project file will be created under {projectFile}_backup.";
         }

         public static string RenamingBuildingBlock(string buildingBlockType)
         {
            return $"Renaming {buildingBlockType.ToLower()}...";
         }

         public static string NoSystemicProcessDefinedInCompoundForType(string systemicProcessType)
         {
            return $"{systemicProcessType} is not defined in compound.";
         }

         public static string NoProcessDefinedInCompoudFor(string name)
         {
            return $"No process defined in compound for '{name}'";
         }

         public static string NewVersionIsAvailable(string newerVersion, string downloadPage)
         {
            return $"Version <b>{newerVersion}</b> is available for download. Visit our download page at <u>{downloadPage}</u>";
         }

         public static readonly string FollowingPKParametersWereSuccessfulyImported = $"Following {UI.PKParameters} were successfully imported:";

         public static string FollowingOutputsWereSuccessfulyImported(int numberOfIndividuals)
         {
            return $"Simulation results for {numberOfIndividuals} individuals were successfully imported for the following quantities:";
         }

         public static string PopulationSimulationSuccessfullyImported(string simulationName, int numberOfIndividuals)
         {
            return $"Population simulation '{simulationName}' was successfully created with '{numberOfIndividuals}' individuals";
         }

         public static readonly string CompoundProcessesInfo = "Enter all relevant properties of the compound by a right click on the selected item in the tree view (left part).\n<B>Note:</B> In the simulation process, these compound properties will be linked to enzymatic, transport, and binding settings defined for the selected individual/species";
         public static readonly string ObjectReferences = "References";
         public static readonly string DoNotShowVersionUpdate = "Ignore this update";

         public static string XAsTooltip(string x)
         {
            return $"X = {x}";
         }

         public static string BoxWhiskerYAsTooltip(string lowerWhisker, int lowerWiskerIndividualId, string lowerBox, int lowerBoxIndividualId, string median, int medianIndividualId, string upperBox, int upperboxIndividualId, string upperWhisker,int  upperWhiskerIndividualId,  string[] outliers, int[] outlierIndividualIds)
         {
            var sb = new StringBuilder();

            sb.AppendLine(percentilWithIndividualId("95", upperWhisker, upperWhiskerIndividualId));
            sb.AppendLine(percentilWithIndividualId("75", upperBox, upperboxIndividualId));
            sb.AppendLine(percentilWithIndividualId("50", median, medianIndividualId));
            sb.AppendLine(percentilWithIndividualId("25", lowerBox, lowerBoxIndividualId));
            sb.AppendLine(percentilWithIndividualId("5", lowerWhisker, lowerWiskerIndividualId));

            if (outliers.Length > 0 && outliers.Length == outlierIndividualIds.Length)
            {
               sb.AppendLine();
               sb.AppendLine("<b>Outliers</b>");
               outliers.Each((v, i) =>
               {
                  sb.AppendLine(valueWithIndividualId(v,outlierIndividualIds[i]));
               });
            }

            return sb.ToString();
         }

         private static string percentilWithIndividualId(string percentil, string value, int individualId) => valueWithIndividualId($"{percentil}% = {value}", individualId);

         private static string valueWithIndividualId(string value, int individualId) => $"{value}, IndividualId = {individualId}";

         public static string RangeXAsTooltip(string minimum, string value, string maximum, int numberOfIndividuals)
         {
            return $"Minimum= {minimum}\nValue = {value}\nMaximum= {maximum}\nNumber of Individuals={numberOfIndividuals}";
         }

         public static string RangeYAsTooltip(string lowerPercentile, string median, string upperPercentile)
         {
            return $"95% Percentile = {upperPercentile}\nMedian = {median}\n5% Percentile = {lowerPercentile}";
         }

         public static string ScatterYAsTooltip(string y)
         {
            return $"Y = {y}";
         }

         public static string TimeProfileYAsTooltip(string lowerValue, string upperValue)
         {
            return $"Upper = {upperValue}\nLower = {lowerValue}";
         }
         
         public static string ObservedDataYAsTooltip(string y, string lowerValue, string upperValue)
         {
            return $"{ObservedDataYAsTooltip(y)}\n{TimeProfileYAsTooltip(lowerValue, upperValue)}";
         }

         public static string ObservedDataYAsTooltip(string y) => ScatterYAsTooltip(y);

         public static string LoadingSnapshot(string snapshotFile, string type) => $"Loading {type} from snapshot file '{snapshotFile}'";

         public static string SnapshotLoaded(string typeToLoad) => $"{typeToLoad} loaded from snaphsot";
      }

      public static class MenuNames
      {
         public static readonly string LoadFromTemplate = UI.LoadFromTemplate;
         public static readonly string SaveAsTemplate = UI.SaveAsTemplate;
         public static readonly string SaveAsSytemTemplate = "Save as System Template...";
         public static readonly string Reset = "Reset";
         public static readonly string Undo = "Undo";
         public static readonly string Diff = "Show Differences...";
         public static readonly string Update = "Update from Building Block...";
         public static readonly string Commit = "Commit to Building Block...";
         public static readonly string File = "&File";
         public static readonly string RecentProjects = "&Recent Projects";
         public static readonly string EditQuery = "&Edit Database Query...";
         public static readonly string ExportToCSV = "E&xport to CSV...";
         public static readonly string ExportPopulationToCSV = "E&xport Population to CSV...";
         public static readonly string ExportPopulationToCSVMenu = "Population to CSV";
         public static readonly string ExportForClusterComputations = "Export for Cluster Computations...";
         public static readonly string ExportForClusterComputationsMenu = "Cluster Computations";
         public static readonly string ExportForMatlab = "Export for Matlab®/R...";
         public static readonly string Configure = "&Configure...";
         public static readonly string ConfigureShortMenu = "Configure";
         public static readonly string NewProject = "&New...";
         public static readonly string SaveProject = "&Save";
         public static readonly string CloseProject = "&Close";
         public static readonly string CompressProject = "Com&press";
         public static readonly string SaveProjectAs = "Save &As...";
         public static readonly string Exit = "E&xit";
         public static readonly string About = "&About...";
         public static readonly string OpenProject = "&Open...";
         public static readonly string ProjectDescription = "&Description...";
         public static readonly string ExportProjectToSnapshot = "Export to Snapshot";
         public static readonly string LoadProjectFromSnapshot = "Load from Snapshot";
         public static readonly string ExportProjectToSnapshotMenu = $"&{ExportProjectToSnapshot}...";
         public static readonly string LoadProjectFromSnapshotMenu = $"{LoadProjectFromSnapshot}...";
         public static readonly string Clone = "Clone...";
         public static readonly string CloneMenu = "Clone";
         public static readonly string ActivateSimulation = "Set as Active Simulation";
         public static readonly string Description = "Description...";
         public static readonly string Delete = OSPSuite.Assets.MenuNames.Delete;
         public static readonly string EditMetaData = "Edit Meta Data...";
         public static readonly string DeleteSelection = "Delete Selection...";
         public static readonly string DeleteAll = "Delete All...";
         public static readonly string Rename = "Rename...";
         public static readonly string AddLabel = "Add Label";
         public static readonly string NewIndividual = "Add &Individual...";
         public static readonly string NewPopulation = "Add &Population...";
         public static readonly string ImportPopulation = "Import &Population from File...";
         public static readonly string NewCompound = "Add &Compound...";
         public static readonly string NewSimulation = "Add &Simulation...";
         public static readonly string ImportIndividualSimulation = "&Individual Simulation";
         public static readonly string ImportPopulationSimulation = "&Population Simulation";
         public static readonly string CreateSimulation = "&Create";
         public static readonly string NewAdministrationProtocol = "Add &Administration Protocol...";
         public static readonly string NewSimulationSettings = "Add Si&mulation Settings...";
         public static readonly string NewFormulation = "Add &Formulation...";
         public static readonly string NewEvent = "Add &Event...";
         public static readonly string AddObservedData = "Add &Observed Data...";
         public static readonly string AddObservedDataFor = "Add Observed Data for";
         public static readonly string AddFractionData = "Add &Fraction Data...";
         public static readonly string SaveAs = "Save As...";
         public static readonly string Scale = "Scale...";
         public static readonly string Extras = "&Extras";
         public static readonly string LookAndFeel = "&Look And Feel";
         public static readonly string Options = "&Options";
         public static readonly string BuildingBlocks = "&Building Blocks";
         public static readonly string View = "&View";
         public static readonly string ExportToMoBi = "&Send to MoBi...";
         public static readonly string ExportToMoBiShortMEnu = "Send to &MoBi";
         public static readonly string ExportToPKMLShortMenu = "Pkml Format";
         public static readonly string SaveToMoBiSimulation = "&Save Simulation to MoBi pkml Format...";
         public static readonly string SaveToMoBiCompound = "&Save Compound to MoBi pkml Format...";
         public static readonly string Run = "Run";
         public static readonly string RunWithSettings = "Define Settings and Run";
         public static readonly string Stop = "Stop";
         public static readonly string BoxWhiskerAnalysis = UI.BoxWhisker;
         public static readonly string TimeProfileAnalysis = UI.TimeProfile;
         public static readonly string ScatterAnalysis = UI.Scatter;
         public static readonly string RangeAnalysis = UI.Range;
         public static readonly string ChartSettingsView = UI.ChartSettings;
         public static readonly string BuildingBlockExplorerView = UI.BuildingBlockExplorer;
         public static readonly string SimulationExplorerView = UI.SimulationExplorer;
         public static readonly string HistoryView = UI.History;
         public static readonly string ComparisonView = UI.Comparison;
         public static readonly string ExportToExcelMenu = $"Export to {UI.Excel}";
         public static readonly string ExportToPDFMenu = "Export to PDF";
         public static readonly string ExportToPKML = "Export to pkml...";
         public static readonly string ExportSimulationToPDFMenu = "PDF";
         public static readonly string ProjectReport = "Project Report";
         public static readonly string Report = "Report";
         public static readonly string Help = "Help";
         public static readonly string ExportSimulationResultsToExcel = $"Export Results to {UI.Excel}...";
         public static readonly string ExportSimulationResultsToExcelMenu = $"Results to {UI.Excel}";
         public static readonly string ExportSimulationResultsToCSV = $"Export Results to {"CSV"}...";
         public static readonly string ExportSimulationResultsToCSVMenu = $"Results to {"CSV"}";
         public static readonly string ImportSimulationResultsFromCSV = "Import Simulation Results...";
         public static readonly string ImportSimulationResultsFromCSVMenu = "Results";
         public static readonly string ImportPKAnalysesFromCSV = "Import PK-Analyses...";
         public static readonly string ImportPKAnalysesFromCSVMenu = "PK-Analyses";
         public static readonly string ExportPKAnalysesToCSV = $"Export PK-Analyses to {"CSV"}...";
         public static readonly string ExportPKAnalysesToCSVMenu = $"PK-Analyses to {"CSV"}";
         public static readonly string ExportSimulationModelToFile = "Export Simulation Structure to File...";
         public static readonly string GarbageCollection = "GC";
         public static readonly string GenerateCalculationMethods = "Generate Calculation Methods";
         public static readonly string GeneratePKMLTemplates = "Generate PKML Templates";
         public static readonly string GenerateGroupsTemplate = "Generate Groups Template";
         public static readonly string GeneratePretermsData = "Generate Preterms Data";
         public static readonly string Comparison = "Compare Results";
         public static readonly string IndividualSimulationComparison = "Individual Simulations";
         public static readonly string PopulationPopulationComparison = "Population Simulations";
         public static readonly string AddNewProcess = "Add new...";
         public static readonly string ExportHistory = "Export History";
         public static readonly string CreateDerivedField = $"{UI.CreateGrouping}...";
         public static readonly string LoadDerivedFieldFromTemplate = "Load Grouping...";
         public static readonly string SaveDerivedFieldToTemplate = "Save Grouping...";
         public static readonly string LoadPopulationAnalysisWorkflowFromTemplate = "Load Analyses from Template...";
         public static readonly string SavePopulationAnalysisWorkflowToTemplate = "Save Analyses to Template...";
         public static readonly string LoadPopulationAnalysisWorkflowFromTemplateMenu = "Load Analyses";
         public static readonly string SavePopulationAnalysisWorkflowToTemplateMenu = "Save Analyses";
         public static readonly string ExtractIndividualByPercentile = "Extract Individuals";
         public static readonly string ExtractIndividualsMenu = "Extract Individuals...";
         public static readonly string ExportSnapshot = "Save Snapshot...";
         public static readonly string LoadFromSnapshot = "Load from Snapshot...";

         public static string CompareBuildingBlocks(string buildingBlockType)
         {
            return $"Compare {buildingBlockType}s";  
         }

         public static string AddProteinDefault(string addProteinCaption)
         {
            return $"{addProteinCaption} (Default)";
         }

         public static string AddProteinQuery(string addProteinCaption, bool isDefined)
         {
            var hint = $"Database {(isDefined ? "query" : "not available")}";
            return $"{addProteinCaption} ({hint})";
         }

         public static string AddObservedDataToSimulation(string simulationName)
         {
            return $"Add to {ObjectTypes.Simulation} '{simulationName}'";
         }
      }

      public static class ObjectTypes
      {
         public static readonly string Transporter = "Transporter";
         public static readonly string ObservedData = "Observed Data";
         public static readonly string Template = "Template";
         public static readonly string AdvancedParameter = "User Defined Variability";
         public static readonly string Individual = "Individual";
         public static readonly string Population = "Population";
         public static readonly string Event = "Event";
         public static readonly string SimulationSettings = "Simulation Settings";
         public static readonly string Administration = "Administration";
         public static readonly string Formulation = "Formulation";
         public static readonly string Protein = "Protein";
         public static readonly string Enzyme = "Enzyme";
         public static readonly string Parameter = "Parameter";
         public static readonly string DistributedParameter = "Distributed Parameter";
         public static readonly string Compound = "Compound";
         public static readonly string ParameterGroupAlternative = "Alternative";
         public static readonly string ParameterGroup = "Group";
         public static readonly string SystemicProcess = "Systemic Process";
         public static readonly string Project = "Project";
         public static readonly string Simulation = "Simulation";
         public static readonly string AdministrationProtocol = "Administration Protocol";
         public static readonly string SimpleProtocol = "Simple Protocol";
         public static readonly string AdvancedProtocol = "Advanced Protocol";
         public static readonly string Schema = "Schema";
         public static readonly string SchemaItem = "SchemaItem";
         public static readonly string Compartment = "Compartment";
         public static readonly string Organ = "Organ";
         public static readonly string Zone = "Zone";
         public static readonly string Segment = "Segment";
         public static readonly string Organism = "Organism";
         public static readonly string Molecule = "Molecule";
         public static readonly string Unknown = "Unknown";
         public static readonly string Organs = "Organs";
         public static readonly string Mucosa = "Mucosa";
         public static readonly string MetabolizingEnzyme = "Metabolizing Enzyme";
         public static readonly string InhibitionProcess = "Inhibition";
         public static readonly string InductionProcess = "Induction";
         public static readonly string PartialProcess = "Partial Process";
         public static readonly string DerivedField = "Grouping";
         public static readonly string ExpressionField = "Expression";
         public static readonly string PopulationAnalysis = "Population Analysis";
         public static readonly string Chart = "Chart";
         public static readonly string PKParameterField = "PK-Parameter";
         public static readonly string ParameterField = "Parameter";
         public static readonly string OutputField = "Output";
         public static readonly string Ontogeny = "Ontogeny";
         public static readonly string Species = "Species";
         public static readonly string SimulationAnalysisWorkflow = "Analysis";
         public static readonly string IndividualOrPopulation = $"{Individual} or {Population}";
      }

      public static class ProteinExpressions
      {
         public static class ColumnCaptions
         {
            public static class ExpressionData
            {
               public static readonly string COL_CONTAINER = "PK-Sim Organ";
               public static readonly string COL_VARIANT_NAME = "Variant";
               public static readonly string COL_DATA_BASE = "Database";
               public static readonly string COL_DATA_BASE_REC_ID = "Database Record ID";
               public static readonly string COL_GENDER = "Gender";
               public static readonly string COL_TISSUE = "Tissue";
               public static readonly string COL_HEALTH_STATE = "Health State";
               public static readonly string COL_SAMPLE_SOURCE = "Sample Source";
               public static readonly string COL_AGE = "Age";
               public static readonly string COL_AGE_MIN = "Age (Minimum)";
               public static readonly string COL_AGE_MAX = "Age (Maximum)";
               public static readonly string COL_SAMPLE_COUNT = "Sample Count";
               public static readonly string COL_TOTAL_COUNT = "Total Count";
               public static readonly string COL_RATIO = "Ratio (Sample Count / Total Count)";
               public static readonly string COL_NORM_VALUE = "Expression Value";
               public static readonly string COL_UNIT = "Unit";
            }

            public static class ProteinSelection
            {
               public static readonly string COL_ID = "ID";
               public static readonly string COL_GENE_NAME = "Gene Name";
               public static readonly string COL_NAME_TYPE = "Name Type";
               public static readonly string COL_SYMBOL = "Symbol";
               public static readonly string COL_GENE_ID = "Gene ID";
               public static readonly string COL_OFFICIAL_FULL_NAME = "Official Full Name";
            }

            public static class Transfer
            {
               public static readonly string COL_CONTAINER = "PK-Sim Organ";
               public static readonly string COL_OLDVALUE = "Old Relative Expression Value";
               public static readonly string COL_NEWVALUE = "Relative Expression Value";
            }

            public static class Mapping
            {
               public static readonly string COL_CONTAINER = "PK-Sim Organ";
               public static readonly string COL_TISSUE = "Tissue in Database";
            }
         }

         public static class MainView
         {
            public static readonly string MainText = "Protein Expression Data";
            public static readonly string TabPageSelection = "Protein Selection";
            public static readonly string TabPageExpressionData = "Expression Data Analysis";
            public static readonly string TabPageTransfer = "Data Transfer Overview";
         }

         public static class MappingView
         {
            public static readonly string Caption = "Edit Mapping...";
         }

         public static class PageSelection
         {
            public static readonly string LabelSearchCriteria = "Search Criteria";
            public static readonly string ButtonSearch = "Search";
         }

         public static class PageTransfer
         {
            public static readonly string ButtonTransfer = "Transfer";
         }
      }

      public static class Ribbons
      {
         public static readonly string ObservedData = "&Observed Data";
         public static readonly string FractionData = "&Fraction Data";
         public static readonly string Individual = "&Individual";
         public static readonly string Population = "&Population";
         public static readonly string Compound = "&Compound";
         public static readonly string Simulation = "&Simulation";
         public static readonly string ImportSimulation = "&Import Simulation";
         public static readonly string Protocol = "&Administration Protocol";
         public static readonly string Event = "&Event";
         public static readonly string SimulationSettings = "Si&mulation Settings";
         public static readonly string Formulation = "&Formulation";
         public static readonly string Create = "Create";
         public static readonly string File = "File";
         public static readonly string LoadBuildingBlocks = "Load Building Blocks";
         public static readonly string History = "History";
         public static readonly string Tools = "Extras";
         public static readonly string Skins = "Skins";
         public static readonly string PanelView = "Show / Hide";
         public static readonly string Exit = "Exit";
         public static readonly string Import = "Import";
         public static readonly string Export = "Export";
         public static readonly string ExportProject = "Export Project";
         public static readonly string Admin = "Admin (Developer Only)";
         public static readonly string DisplayUnits = "Display Units";
         public static readonly string Analyses = "Analyses";
         public static readonly string Workflow = "Workflow";
         public static readonly string CompareResults = "Compare Results";
         public static readonly string WorkingJournal = "Journal";
         public static readonly string Favorites = "Favorites";
      }

      public static class RibbonPages
      {
         public static readonly string File = "File";
         public static readonly string WorkingJournal = "Working Journal";
         public static readonly string Modeling = "Modeling";
         public static readonly string Utilities = "Utilities";
         public static readonly string ImportExport = "Import/Export";
         public static readonly string Import = "Import";
         public static readonly string Export = "Export";
         public static readonly string RunSimulation = "Run & Analyze";
         public static readonly string Analyze = "Analyze";
         public static readonly string Views = "Views";
      }

      public static class RibbonCategories
      {
         public static readonly string IndividualSimulation = "Individual Simulation";
         public static readonly string PopulationSimulation = "Population Simulation";
         public static readonly string PopulationSimulationComparison = "Population Simulation Comparison";

         public static IEnumerable<string> AllDynamicCategories()
         {
            return new List<string>
            {
               IndividualSimulation,
               PopulationSimulation,
               PopulationSimulationComparison,
               OSPSuite.Assets.RibbonCategories.ParameterIdentification,
               OSPSuite.Assets.RibbonCategories.SensitivityAnalysis
            };
         }
      }

      public static class Rules
      {
         public static class Parameter
         {
            public static readonly string MinShouldBeDefined = "Minimum value should be defined.";
            public static readonly string MaxShouldBeDefined = "Maximum value should be defined.";
            public static readonly string MinLessThanMax = "Minimum value should be less than than maximum value.";
            public static readonly string MaxGreaterThanMin = "Maximum value should be greater than minimum value.";
            public static readonly string StartTimeLessThanOrEqualToEndTime = "Start time value should be less than end time value.";
            public static readonly string EndTimeGreaterThanOrEqualToStartTime = "End time value should be greater than start time value.";
            public static readonly string TimeValueShouldBeGreaterThanOrEqualToZero = "Time value should be greater than or equal to 0.";

            public static string MinGreaterThanDbMinValue(double? dbMinValue, string unit)
            {
               return $"Minimum value should be greater than or equal to {dbMinValue} {unit}.";
            }

            public static string MaxGreaterThanDbMinValue(double? dbMinValue, string unit)
            {
               return $"Maximum value should be greater than or equal to {dbMinValue} {unit}.";
            }

            public static string MaxLessThanDbMaxValue(double? dbMaxValue, string unit)
            {
               return $"Maximum value should be less than or equal to {dbMaxValue} {unit}.";
            }

            public static string MinLessThanDbMaxValue(double? dbMaxValue, string unit)
            {
               return $"Minimum value should be less than or equal to {dbMaxValue} {unit}.";
            }

            public static string ValueSmallerThanMax(string parameterName, string value, string unit)
            {
               return $"Value for {parameterName} should be less than or equal to {value} {unit}";
            }

            public static string ValueBiggerThanMin(string parameterName, string value, string unit)
            {
               return $"Value for {parameterName} should be greater than or equal to {value} {unit}";
            }

            public static string ValueStrictBiggerThanMin(string parameterName, string value, string unit)
            {
               return $"Value for {parameterName} should be strictly greater than {value} {unit}";
            }

            public static string ValueStrictSmallerThanMax(string parameterName, string value, string unit)
            {
               return $"Value for {parameterName} should be strictly less than {value} {unit}";
            }

            public static readonly string ProportionOfFemaleBetween0And100 = "Proportion of females should be between 0 and 100";
            public static readonly string NumberOfIndividualShouldBeBiggerThan2 = "Number of individuals should be greater than or equal to 2";
            public static readonly string NumberOfIndividualShouldBeSmallerThan10000 = "Number of individuals should be less than or equal to 10000";
         }

         public static string PatternShouldContainOneIterationPattern(string iterationPattern, string startPattern, string endPattern)
         {
            return $"Naming pattern should contain one of '{iterationPattern}', '{startPattern}' or '{endPattern}'.";
         }

         public static readonly string AtLeastTwoBinsRequired = "At least two bins are required";
         public static readonly string ExceededMaximumOfBins = "Maximum of bins exceeded";
      }

      public static class UI
      {
         public static readonly string Excel = "Excel®";
         public static readonly string SimulationInterval = "Simulation Interval";
         public static readonly string SimulationIntervalLowResolution = "Simulation interval low resolution";
         public static readonly string SimulationIntervalHighResolution = "Simulation interval high resolution";
         public static readonly string SchemaItem = "Schema Item";
         public static readonly string Schema = "Schema";
         public static readonly string Influx = "Influx";
         public static readonly string Efflux = "Efflux";
         public static readonly string PgpLike = "Pgp-Like";
         public static readonly string EmptyName = "Empty";
         public static readonly string EmptyDescription = "<Empty>";
         public static readonly string Discrete = "Constant";
         public static readonly string Normal = "Normal";
         public static readonly string Uniform = "Uniform";
         public static readonly string LogNormal = "Log Normal";
         public static readonly string Capsule = "Capsule";
         public static readonly string Solution = "Solution";
         public static readonly string Cream = "Cream";
         public static readonly string Infrastructure = "Infrastructure";
         public static readonly string Presentation = "Presentation";
         public static readonly string UserInterface = "User Interface";
         public static readonly string Commands = "Commands";
         public static readonly string Core = "Core";
         public static readonly string UpdatingSimulation = "Updating Simulation...";
         public static readonly string PerformingSimulationClone = "Cloning Simulation...";
         public static readonly string TransporterType = "Transporter Type";
         public static readonly string None = "<None>";
         public static readonly string Unknown = "Unknown";
         public static readonly string NoSystemicProcessAvailable = "Not available in compound";
         public static readonly string Female = "Female";
         public static readonly string Male = "Male";
         public static readonly string PlasmaClearanceType = "Plasma Clearance";
         public static readonly string ProcessInCompound = "Process in compound";
         public static readonly string AlternativeInCompound = "Alternative in compound";
         public static readonly string ParameterAlternatives = "Parameter Alternatives";
         public static readonly string PlasmaClearanceInCompound = "Plasma clearance process in compound";
         public static readonly string CreatingSimulation = "Creating...";
         public static readonly string Molecule = "Molecule";
         public static readonly string Observer = "Observer";
         public static readonly string Output = "Output";
         public static readonly string CurveName = "Curve Name";
         public static readonly string BuildingBlockExplorer = "Building Blocks";
         public static readonly string SimulationExplorer = "Simulations";
         public static readonly string ChartSettings = "Chart Options";
         public static readonly string Localization = "Localization";
         public const string Properties = "Properties";
         public const string Property = "Property";
         public static readonly string SaveProjectChanges = "Project has changed. Save changes?";
         public static readonly string SaveProjectTitle = "Save project...";
         public static readonly string OpenProjectTitle = "Open project...";
         public static readonly string CreateReportTitle = "Create Report...";
         public static readonly string NewProject = "New Project...";
         public static readonly string Name = "Name";
         public static readonly string Description = "Description";
         public static readonly string IndividualFolder = "Individuals";
         public static readonly string CompoundFolder = "Compounds";
         public static readonly string ObservedDataSettings = "Observed Data Settings";
         public static readonly string FormulationFolder = "Formulations";
         public static readonly string PopulationFolder = "Populations";
         public static readonly string Events = "Events";
         public static readonly string EventFolder = Events;
         public static readonly string AdministrationProtocolFolder = "Administration Protocols";
         public const string Value = "Value";
         public const string ValueDescription = "Value Description";
         public static readonly string Percentage = "Percentage";
         public static readonly string Container = "Container";
         public static readonly string Percentile = "Percentile";
         public static readonly string Distribution = "Distribution";
         public static readonly string MeanValues = "Mean";
         public static readonly string Unit = "Unit";
         public static readonly string History = "History";
         public static readonly string Comparison = "Comparison";
         public static readonly string ResetAll = "Reset";
         public static readonly string ScaleButton = "Scale";
         public static readonly string AllowsScientificNotation = "Allows scientific notation";
         public static readonly string ShouldRestoreWorkspaceLayout = "Restore opened view when loading project";
         public static readonly string ShowUpdateNotification = "Show software update notification if available";
         public static readonly string HideImmediatelyOnAutoHide = "Immediatly hide panel on auto hide (no animaton)";
         public static readonly string ActiveSkin = "Active skin";
         public static readonly string DecimalPlace = "Decimal place";
         public static readonly string IconSizes = "Icon Sizes";
         public static readonly string IconSizeTab = "Tabs";
         public static readonly string IconSizeTreeView = "Tree view";
         public static readonly string IconSizeContextMenu = "Context menu";
         public static readonly string MRUListItemCount = "Number of recent file items shown";
         public static readonly string NumericalProperties = "Numerical Properties";
         public static readonly string UIProperties = "Look and feel";
         public static readonly string TemplateDatabase = "Template Database";
         public static readonly string TemplateDatabasePath = "Template Database Path";
         public static readonly string User = "User";
         public static readonly string UserGeneral = "General";
         public static readonly string UserDisplayUnits = "Display Units";
         public static readonly string Application = "Application";
         public static readonly string Options = "Options";
         public static readonly string Override = "Override";
         public static readonly string Biometrics = "Biometrics";
         public static readonly string ScalePercentile = "Same percentile";
         public static readonly string KeepTarget = "Use default value";
         public static readonly string OverrideWithSource = "Use source value";
         public static readonly string Ratio = "Same ratio";
         public static readonly string SimulationSettings = "Settings";
         public static readonly string CompoundActiveProcessType = "Type";
         public static readonly string Kinetic = "Kinetic";
         public static readonly string SimulationModelConfiguration = "Model";
         public static readonly string SimulationCompoundsConfiguration = "Compounds";
         public static readonly string SimulationEventsConfiguration = "Events";
         public static readonly string SimulationApplicationConfiguration = "Administration";
         public static readonly string SimulationProcessDefinition = "Processes";
         public static readonly string NextButton = "&Next";
         public static readonly string PreviousButton = "&Previous";
         public static readonly string OKButton = "&OK";
         public static readonly string CancelButton = "&Cancel";
         public static readonly string UpgradeButton = "&Upgrade";
         public static readonly string OpenAnyway = "&Open Anyway";
         public static readonly string ImportAnyway = "&Import Anyway";
         public static readonly string Factor = "Factor";
         public static readonly string EmptyColumn = " ";
         public static readonly string QuantityName = "Name";
         public static readonly string Compartment = "Compartment";
         public static readonly string Organ = "Organ";
         public static readonly string Simulation = "Simulation";
         public static readonly string OrganType = "Organ Type";
         public static readonly string Calculating = "Calculating...";
         public static string CalculationPopulationSimulation(int number, int total) => $"Simulation {number}/{total}...";
         public static readonly string StartTime = "Start Time";
         public static readonly string EndTime = "End Time";
         public static readonly string NumberOfTimePoints = "Resolution";
         public static readonly string RelativeExpression = "Relative Expression";
         public static readonly string RelativeExpressionNorm = "Normalized Expression";
         public static readonly string ReferenceConcentration = "Reference concentration";
         public static readonly string HalfLife = "Half life";
         public static readonly string HalfLifeLiver = "Half life (liver)";
         public static readonly string HalfLifeIntestine = "Half life (intestine)";
         public static readonly string LocalizationInTissue = "Localization in tissue";
         public static readonly string LocalizationOnMembrane = "Localization on membrane";
         public static readonly string IntracellularVascularEndoLocation = "Localization in vasc. endothelium";
         public static readonly string LocationOnVascularEndo = "Localization on vasc. endothelium";
         public static readonly string OntogenyVariabilityLike = "Ontogeny/Variability like";
         public static readonly string Ontogeny = "Ontogeny";
         public static readonly string OntogenyFactor = "Ontogeny factor";
         public static readonly string Concentration = "Concentration";
         public static readonly string OpeningProjectDatabase = "Opening project database...";
         public static readonly string CreatingProjectDatabase = "Creating project database...";
         public static readonly string LoadingProject = "Loading project...";
         public static readonly string LoadingSnapshot = "Loading snapshot...";
         public static readonly string SnapshotFile = "Select snapshot file";
         public static readonly string SavingProject = "Saving project...";
         public static readonly string LoadingHistory = "Loading history...";
         public static readonly string LoadingMatlab = "Loading Matlab®...";
         public static readonly string LoadingLayout = "Loading layout...";
         public static readonly string LoadingWorkingJournal = "Loading journal...";
         public static readonly string SavingHistory = "Saving history...";
         public static readonly string SavingLayout = "Saving layout...";
         public static readonly string CompressionProject = "Compressing...";
         public static readonly string SystemicProcess = "Systemic process";
         public static readonly string ResetLayout = "Reset Layout...";
         public static readonly string Reset = "Reset";
         public static readonly string ChartLayout = "Layout";
         public static readonly string ExportPKAnalysesToExcelTitle = $"Export PK-Analyses to {Excel}";
         public static readonly string ExportSimulationToMoBiTitle = "Export simulation into MoBi pkml format";
         public static readonly string ExportCalculationMethodsToMoBiTitle = "Export calculation methods into MoBi pkml format";
         public static readonly string ExportSimulationModelToFileTitle = "Export model structure to text file";
         public static readonly string ExportCompoundToMoBiTitle = "Export compound into MoBi pkml format";
         public static readonly string ExportPopulationToCSVTitle = "Export Population to CSV File";
         public static readonly string ExportPKAnalysesToCSVTitle = $"Export PK-Analyses to {"CSV"}";
         public static readonly string ExportForClusterSimulationTitle = "Export for Cluster Simulation...";
         public static readonly string ExportPopulationForMatlabWrapper = "Select a folder where required files to run the matlab wrapper will be generated";
         public static readonly string UserTemplates = "User Templates";
         public static readonly string SystemTemplates = "Predefined Templates";
         public static readonly string EditDescription = "Edit Description";
         public static readonly string EditValueDescription = "Edit Value Description";
         public static readonly string ShowPKAnalysis = "Show PK-Analysis";
         public static readonly string ShowChart = "Show Chart";
         public static readonly string ImportObservedData = "Import Observed Data";
         public static readonly string ObservedData = "Observed Data";
         public static readonly string ApplyButton = "Apply";
         public static readonly string DisplayPathSeparator = " -> ";
         public static readonly string Curve = "Curve";
         public static readonly string Chart = "Chart";
         public static readonly string Charts = "Charts";
         public static readonly string All = "All";
         public static readonly string Colors = "Colors";
         public static readonly string Color = "Color";
         public static readonly string ChartBackColor = "Chart background";
         public static readonly string FormulaColor = "Formula parameter";
         public static readonly string ChartDiagramBackColor = "Chart diagram background";
         public static readonly string DisabledColor = "Disabled";
         public static readonly string ChangedColor = "Parameter changed";
         public static readonly string AvailableProteins = "Proteins";
         public static readonly string ModelSettings = "Model Settings";
         public static readonly string ModelParameters = "Model Parameters";
         public static readonly string DefaultSpecies = "Species";
         public static readonly string DefaultPopulation = "Population";
         public static readonly string DefaultParameterGroupLayout = "Parameter layout";
         public static readonly string DefaultLipophilicityName = "Lipophilicity";
         public static readonly string DefaultFractionUnboundName = "Fraction unbound";
         public static readonly string DefaultSolubilityName = "Solubility";
         public static readonly string DefaultPopulationAnalysisType = "Population analysis";
         public static readonly string PreferredViewLayout = "Preferred View Layout";
         public static readonly string Defaults = "Defaults";
         public static readonly string Default = "Default";
         public static readonly string Add = "Add";
         public static readonly string Edit = "Edit";
         public static readonly string Remove = "Remove";
         public static readonly string RemoveAll = "Remove All";
         public static readonly string RemoveAllButThis = "Remove All But This";
         public static readonly string AddNew = "Add new";
         public static readonly string Clone = "Clone";
         public static readonly string Rename = "Rename";
         public static readonly string CreateFormulation = "Create Formulation";
         public static readonly string CreateEvent = "Create Event";
         public static readonly string Expression = "Expression";
         public static readonly string Demographics = "Demographics";
         public static readonly string PopulationProperties = "Population Properties";
         public static readonly string Population = "Population";
         public static readonly string Age = "Age";
         public static readonly string PostnatalAge = "Postnatal age";
         public static readonly string PostMenstrualAge = "Post menstrual age";
         public static readonly string GestationalAge = "Gestational age";
         public static readonly string Weight = "Weight";
         public static readonly string Height = "Height";
         public static readonly string BMI = "BMI";
         public static readonly string Gender = "Gender";
         public static readonly string SubPopulation = "Sub Population";
         public static readonly string CalculationMethods = "Calculation methods";
         public static readonly string Category = "Category";
         public static readonly string CreateIndividual = "Create Individual";
         public static readonly string CreateSimulationSettings = "Create Simulation Settings";
         public static readonly string ShowAllParametersForScaling = "Show all parameters";
         public static readonly string AddNewEnzyme = "Add New Enzyme...";
         public static readonly string EditProteinExpression = "Edit Protein Expression...";
         public static readonly string IndividualScalingConfiguration = "Scaling Configuration";
         public static readonly string ConfigureScaling = "Configure Scaling >>";
         public static readonly string Protein = "Protein";
         public static readonly string Lipophilicity = "Lipophilicity";
         public static readonly string Permeability = "Permeability";
         public static readonly string FractionUnbound = "Fraction Unbound";
         public static readonly string FractionUnboundBindingType = "Binds to";
         public static readonly string Albumin = "Albumin";
         public static readonly string Glycoprotein = "α1-acid glycoprotein";
         public static readonly string Fraction = "Fraction";
         public static readonly string TubularSecretion = "Tubular Secretion";
         public static readonly string InVitroAssay = "In-Vitro Assay";
         public static readonly string GlomerularFiltration = "Glomerular Filtration";
         public static readonly string ShowCalculatedValues = "Show Values";
         public static readonly string ShowSolubilityPhChart = "Show Graph";
         public static readonly string CalculatedValue = "Calculated";
         public static readonly string PermeabilityCalculatedFromLipoAndMolWeight = "Permeability values calculated using liphophilicity";
         public static readonly string IsDefault = "Default";
         public static readonly string DefaultAlternative = "Measurement";
         public static readonly string CalculatedAlernative = "Calculated";
         public static readonly string CreateCompound = "Create Compound";
         public static readonly string DataSource = "Data Source";
         public static readonly string DataSourceColumn = "Data source";
         public static readonly string ProcessType = "Process Type";
         public static readonly string ProtocolType = "Protocol Type";
         public static readonly string ProcessTypeDescription = "Process Type Description";
         public static readonly string EnzymeFractionDescription = "Enzyme fraction";
         public static readonly string EnzymeFractionDisplayName = "Fraction";
         public static readonly string EnzymaticStabilityProcessType = "Process Type";
         public static readonly string EnzymaticStabilityRate = "Kinetic";
         public static readonly string BasicPharmacochemistry = "Basic Physico-chemistry";
         public static readonly string Halogens = "Halogens";
         public static readonly string DissociationConstants = "Dissociation Constants";
         public static readonly string AdvancedParameterTabCaption = "Advanced Parameters";
         public static readonly string CompoundParameterInSimulationSimple = BasicPharmacochemistry;
         public static readonly string CompoundParameterInSimulationAdvanced = AdvancedParameterTabCaption;
         public static readonly string ADME = "ADME";
         public static readonly string CompoundTypeAcid = "Acid";
         public static readonly string CompoundTypeBase = "Base";
         public static readonly string CompoundTypeNeutral = "Neutral";
         public static readonly string Species = "Species";
         public static readonly string ExpressionDatabasePath = "Expressions Database";
         public static readonly string SavingSettings = "Saving Settings...";
         public static readonly string SelectTemplateDatabasePath = "Select Template Database...";
         public static readonly string CreateTemplateDatabasePath = "Create Template Database...";
         public static readonly string MoBiPath = "MoBi Executable Path";
         public static readonly string BasedOnIndividual = "Based on individual";
         public static readonly string CurveSelection = "Curve Selection for Chart";
         public static readonly string ProportionOfFemales = "Proportion of Females [%]";
         public static readonly string PredefinedCurves = "Predefined Curves";
         public static readonly string Minimum = "Minimum";
         public static readonly string Maximum = "Maximum";
         public static readonly string MinValue = "Minimum Value";
         public static readonly string MaxValue = "Maximum Value";
         public static readonly string ArithmeticMean = "Arithmetic Mean";
         public static readonly string GeometricMean = "Geometric Mean";
         public static readonly string StandardDeviation = "Standard Deviation";
         public static readonly string Median = "Median";
         public static readonly string PercentilePercent = "% Percentile";
         public static readonly string AllGender = "All";
         public static readonly string CreatingPopulation = "Creating population...";
         public static readonly string ImportingResults = "Importing results...";
         public static readonly string UserDefinedVariability = "User Defined Variability";
         public static readonly string Generate = "Generate";
         public static readonly string NumberOfIndividuals = "Number of Individuals";
         public static readonly string NumberOfOutputs = "Number of Outputs";
         public static readonly string CreatePopulation = "Create Population";
         public static readonly string ImportPopulation = "Import Population";
         public static readonly string PopulationSettings = "Settings";
         public static readonly string PopulationParameterRanges = "Population Parameters Ranges";
         public static readonly string IndividualIsMeanOfPopulation = "<I>This individual will represent the mean individual of the created population</I>";
         public static readonly string GenderRatio = "Gender Ratio";
         public static readonly string AddButtonText = "Add";
         public static readonly string RemoveButtonText = "Remove";
         public static readonly string Parameters = "Parameters";
         public static readonly string PopulationParameters = "Population Parameters";
         public static readonly string Dosing = "Dosing";
         public static readonly string SpecificParameters = "Specific Parameters";
         public static readonly string IndividualParameters = "Individual Parameters";
         public static readonly string DistributionType = "Distribution";
         public static readonly string DoseUnit = "Dose Unit";
         public static readonly string TimeUnit = "Time Unit";
         public static readonly string CreateAdministrationProtocol = "Create Administration Protocol";
         public static readonly string Update = "Update";
         public static readonly string DefaultValue = "Default Value";
         public static readonly string SourceDefaultValue = "Source Default Value";
         public static readonly string SourceValue = "Source Value";
         public static readonly string TargetDefaultValue = "Default Value";
         public static readonly string TargetScaledValue = "Scaled Value";
         public static readonly string Parameter = "Parameter";
         public static readonly string ScalingMethod = "Scaling Method";
         public static readonly string SimulationSubjectSelection = "Select Simulation Subject";
         public static readonly string PopulationSimulationSettings = "Population Simulation Settings: Curve Selection";
         public static readonly string IndividualSimulationSettings = "Individual Simulation Settings: Curve Selection";
         public static readonly string SimulationSettingsDescription = "Select the curves that will be generated by the simulation.";
         public static string NumberOfGeneratedCurves(int number) => $"Number of curves to generate for this simulation : {number}";
         public static readonly string AtLeastOneMoleculeNeedsToBeSelected = "At least one molecule needs to be selected";
         public static readonly string GeneratePopulationSimulationReport = "Generate report";
         public static readonly string IndividualSimulation = "Individual";
         public static readonly string PopulationSimulation = "Population";
         public static readonly string CreateSimulation = "Create Simulation";
         public static readonly string CloneSimulation = "Clone Simulation";
         public static readonly string ConfigureSimulationDescription = "Configure Simulation";
         public static string EditIndividualSimulation(string simulationName) => $"Simulation: '{simulationName}'";
         public static string EditPopulationSimulation(string simulationName)  => $"Population Simulation : '{simulationName}'";
         public static readonly string RegisterAssembliesWithDefaultConvention = "Loading assemblies";
         public static readonly string RegisterCoreDependencies = "Registering core dependencies";
         public static readonly string RegisterORMDependencies = "Configuring database access";
         public static readonly string RegisterSerializationDependencies = "Loading serializer";
         public static readonly string StartingUserInterface = "Starting user interface";
         public static readonly string NumberOfRepetitions = "Number of Repetitions";
         public static readonly string TimeBetweenRepetitions = "Time Between Repetitions";
         public static readonly string ZeroOrder = "ZeroOrder";
         public static readonly string FirstOrder = "FirstOrder";
         public static readonly string Intravenous = "Intravenous Infusion";
         public static readonly string IntravenousBolus = "Intravenous Bolus";
         public static readonly string Oral = "Oral";
         public static readonly string SimpleProtocolMode = "Simple protocol";
         public static readonly string AdvancedProtocolMode = "Advanced protocol";
         public static readonly string Dose = "Dose";
         public static readonly string Time = "Time";
         public static readonly string ProtocolEndTime = "Protocol end time";
         public static readonly string DosingInterval = "Dosing interval";
         public static readonly string ApplicationType = "Administration type";
         public static readonly string TargetOrgan = "Target organ";
         public static readonly string TargetCompartment = "Target compartment";
         public static readonly string PlaceholderFormulation = "Placeholder for formulation";
         public static readonly string ProtocolProperties = "Protocol Properties";
         public static readonly string Dermal = "Dermal";
         public static readonly string Subcutaneous = "Subcutaneous";
         public static readonly string Single = "Single";
         public static readonly string DI_12_12 = "12 - 12 h (bi-daily)";
         public static readonly string DI_8_8_8 = "8 - 8 - 8 h";
         public static readonly string DI_24 = "24 h (once-daily)";
         public static readonly string DI_6_6_6_6 = "6 - 6 - 6 - 6 h";
         public static readonly string DI_6_6_12 = "6 - 6 - 12 h";
         public static readonly string CloseButton = "&Close";
         public static readonly string ExportActiveSimulationToMoBiDescription = "Export active simulation to MoBi";
         public static readonly string NaN = "<NaN>";
         public static readonly string NotAvailable = "n.a.";
         public static readonly string AboutThisApplication = "About this application...";
         public static readonly string NewProjectDescription = "Create a new project...";
         public static readonly string OpenProjectDescription = "Open an existing project...";
         public static readonly string ProjectDescriptionDescription = "Show or edit project description...";
         public static readonly string ExportProjectToSnapshotDescription = "Export project to snapshot...";
         public static readonly string LoadProjectFromSnapshotDescription = "Load project from snapshot...";
         public static readonly string CloseProjectDescription = "Close the project";
         public static readonly string NewSimulationDescription = "Create a new simulation...";
         public static readonly string ImportIndividualSimulationDescription = "Import an individual simulation from file...";
         public static readonly string ImportPopulationSimulationDescription = "Import a population simulation from file...";
         public static readonly string ImportPopulationSimulation = "Import Population Simulation";
         public static readonly string NewIndividualDescription = "Create a new individual...";
         public static readonly string NewPopulationDescription = "Create a new population...";
         public static readonly string ImportPopulationDescription = "Import a population from csv files...";
         public static readonly string NewProtocolDescription = "Create a new administration protocol...";
         public static readonly string NewFormulationDescription = "Create a new formulation...";
         public static readonly string NewEventDescription = "Create a new event...";
         public static readonly string NewCompoundDescription = "Create a new compound...";
         public static readonly string OptionsDescription = "Manage the options for the application and the current user...";
         public static readonly string ExitDescription = "Exit the application";
         public static readonly string RunDescription = "Simulate the active simulation";
         public static readonly string RunWithSettingsDescription = "Select simulation settings and simulate the active simulation";
         public static readonly string StopDescription = "Stop the current simulation";
         public static readonly string ShowIndividualResultsDescription = "Create a new chart displaying simulation results";
         public static readonly string StartPopulationAnalysisDescription = "Start a new population analysis";
         public static readonly string ScatterAnalysisDescription = "Start a new scatter analysis";
         public static readonly string RangeAnalysisDescription = "Start a new range analysis";
         public static readonly string TimeProfileAnalysisDescription = "Start a new time profile analysis";
         public static readonly string BoxWhiskerAnalysisDescription = "Start a new box whisker analysis";
         public static readonly string IndividualSimulationComparisonDescription = "Create a summary chart comparing the results of different individual simulations. Drag-and-drop the simulations to compare and select curves in data browser of chart editor.";
         public static readonly string PopulationSimulationComparisonDescription = "Create a summary chart comparing the results of different population simulations.";
         public static readonly string ChartSettingsViewDescription = "Show or hide the chart settings";
         public static readonly string HistoryViewDescription = "Show or hide the history";
         public static readonly string ComparisonViewDescription = "Show or hide the comparison";
         public static readonly string SimulationExplorerViewDescription = "Show or hide the simulation explorer";
         public static readonly string BuildingBlockExplorerViewDescription = "Show or hide the building block explorer";
         public static readonly string UndoDescription = "Undo the last action";
         public static readonly string AddObservedDataDescription = "Add observed data to the project...";
         public static readonly string AddFractionDataDescription = "Add fraction data to the project...";
         public static readonly string ExportHistoryToExcelDescription = "Export history to Excel file...";
         public static readonly string ExportHistoryToPDFDescription = "Export history to PDF file...";
         public static readonly string ProjectReportDescription = "Export project to a pdf file...";
         public static readonly string SaveProjectDescription = "Save the project...";
         public static readonly string SaveProjectAsDescription = "Save the project to a new file...";
         public static readonly string CompoundTwoPoreParametersNote = "These parameters are only used in a simulation where the model for proteins and large molecules was selected. Their values are irrelevant otherwise and can be left as is.";
         public static readonly string CompoundParticleDissolutionParametersNote = "These parameters are only used in a simulation where the particle dissolution function was selected. Their values are irrelevant otherwise and can be left as is.";
         public static readonly string ResetAllVisibleButtonToolTip = "Reset all visible parameters to default.";
         public static readonly string ResetParameterToolTip = "Reset parameter to default";
         public static readonly string ScaleButtonToolTip = "Scale all visible parameters with the given factor";
         public static readonly string Favorites = "Favorites";
         public static readonly string RefpH = "Ref-pH";
         public static readonly string SolubilityGainPerCharge = "Solubility gain per charge";
         public static readonly string RefSolubility = "Solubility at Ref-pH";
         public static readonly string pHDependentSolubility = "pH-dependent Solubility";
         public static readonly string MoleculeNameDescription = "Name of the molecule for which the data were measured";
         public static readonly string Undefined = "Undefined";
         public static readonly string ProjectConverter = "Project Converter";
         public static readonly string Info = "Info";
         public static readonly string Error = "Error";
         public static readonly string Warning = "Warning";
         public static readonly string ConversionLog = "Conversion Log";
         public static readonly string Upgrade = "Upgrade";
         public static readonly string Debug = "Debug";
         public static readonly string Monodisperse = "Monodisperse";
         public static readonly string Polydisperse = "Polydisperse";
         public static readonly string SinkCondition = "Sink Condition";
         public static readonly string NoSinkCondition = "No Sink Condition";
         public static readonly string Soluble = "Soluble";
         public static readonly string Insoluble = "Insoluble";
         public static readonly string AddEvent = "Add Event...";
         public static readonly string ExportLogToFile = "Export project conversion log to file...";
         public static readonly string SaveLog = "Save Log...";
         public static readonly string RelTol = "Relative tolerance";
         public static readonly string AbsTol = "Absolute tolerance";
         public static readonly string SimulationResults = "Simulation Results";
         public static readonly string No = "No";
         public static readonly string Yes = "Yes";
         public static readonly string ExportObservedDataToPkml = "Export observed data to pkml";
         public static readonly string ExportSimulationResultsToExcel = $"Export simulation results to {Excel}";
         public static readonly string ExportPopulationAnalysisToExcelTitle = $"Export analysis to {Excel}";
         public static readonly string ExportSimulationResultsToCSV = $"Export simulation results to {"CSV"}";
         public static readonly string ReallyCancel = "Do you really want to cancel?";
         public static readonly string BuildingBlockName = "Building Block Name";
         public static readonly string BuildingBlockType = "Building Block Type";
         public static readonly string MetabolizingEnzyme = "Metabolizing Enzyme";
         public static readonly string MetabolizingEnzymes = $"{MetabolizingEnzyme}s";
         public static readonly string TransportProtein = "Transport Protein";
         public static readonly string TransportProteins = $"{TransportProtein}s";
         public static readonly string ProteinBindingPartner = "Protein Binding Partner";
         public static readonly string ProteinBindingPartners = $"{ProteinBindingPartner}s";
         public static readonly string CreateMetabolizingEnzyme = $"Add {MetabolizingEnzyme}...";
         public static readonly string CreateProteinBindingPartner = $"Add {ProteinBindingPartner}...";
         public static readonly string CreateTransportProtein = $"Create {TransportProtein} ...";
         public static readonly string AddMetabolizingEnzyme = $"Add {MetabolizingEnzyme}...";
         public static readonly string AddTransportProtein = $"Add {TransportProtein}...";
         public static readonly string AddSpecificBindingPartner = $"Add {ProteinBindingPartner}...";
         public static readonly string SpecificBindingProcesses = "Specific Binding";
         public static readonly string TransportAndExcretionProcesses = "Transport & Excretion";
         public static readonly string BiliaryClearance = "Biliary Clearance";
         public static readonly string RenalClearance = "Renal Clearances";
         public static readonly string TotalHepaticClearance = "Total Hepatic Clearance";
         public static readonly string MetabolicProcesses = "Metabolism";
         public static readonly string SimulationMetabolism = MetabolicProcesses;
         public static readonly string SimulationSpecificBinding = SpecificBindingProcesses;
         public static readonly string SimulationTransportAndExcretion = "Transport && Excretion";
         public static readonly string ModelProperties = "Model Properties";
         public static readonly string CompoundProperties = "Compound Properties";
         public static readonly string EventProperties = "Event Properties";
         public static readonly string OriginData = "Origin Data";
         public static readonly string Origin = "Origin";
         public static readonly string CompoundProcessSpeciesDescription = "Species used in the experiment. This is not necessarily the species used in the simulation.";
         public static readonly string CompoundProcessDataSourceDescription = "Source of information (e.g. Lab, In-Vitro, Paper etc.)";
         public static readonly string ReallySwitchProtocolMode = "Do you really want to switch between advanced and simple? This action will reset the administration protocol.";
         public static readonly string UsedBuildingBlocks = "Used building blocks";
         public static readonly string UserDefined = "User Defined";
         public static readonly string MolWeight = "Molecular weight [g/mol]";
         public static readonly string CalculateBioavailability = "Calculate Bioavailability";
         public static readonly string CalculateDDIRatio = "Calculate DDI Ratio";
         public static readonly string ExportPKAnalysesToExcel = $"Export to {Excel}";
         public static readonly string AnatomyAndPhysiology = "Anatomy && Physiology";
         public static readonly string AnatomyAndPhysiologyText = "All anatomical and physiological parameters are set according to selected species, (population,) gender, age, weight and height except the manually changed parameters listed in {0}.";
         public static readonly string Experiment = "Experiment";
         public static readonly string ImportFormulation = "Import Formulation";
         public static readonly string AddPoint = "Add Point";
         public static readonly string Filter = "Filter";
         public static readonly string SaveSimulationToXmlFile = "Save Simulation to xml File (PKSim Format)";
         public static readonly string SaveSimulationParameterToCsvFile = "Save Simulation parameters to csv File (PKSim Format)";
         public static readonly string SaveSimulationToSimModelXmlFile = "Save Simulation to xml File (for use in Matlab or R)";
         public static readonly string Solubility = "Solubility";
         public static readonly string Analysis = "Analysis";
         public static readonly string pH = "pH";
         public static readonly string EnterAValue = "<enter a value>";
         public static readonly string DeleteEntry = "Delete entry";
         public static readonly string AddEntry = "Add entry";
         public static readonly string EditTable = "Edit Table";
         public static readonly string ShowTable = "Show Table";
         public static readonly string Show = "Show";
         public static readonly string SaveChartLayout = "Save Layout...";
         public static readonly string SaveChartLayoutToTemplateFile = "Save current layout to template file (Developer only)";
         public static readonly string CustomizeLayout = "Customize...";
         public static readonly string CustomizeLayoutToolTip = "Customize layout (Developer only)";
         public static readonly string SaveSettings = "Save Settings";
         public static readonly string SaveSimulationSettings = "Save Settings into";
         public static readonly string PopulationAnalysisSaveLoad = "Save/Load Analysis";
         public static readonly string SaveSimulationSettingsToProject = "Project";
         public static readonly string SaveSimulationSettingsToUserSettings = "User Profile";
         public static readonly string SaveChartSettingsToolTip = "Save current chart settings such as selected curves, colors etc.. as template into project. These settings will be used as default settings for each newly created chart.";
         public static readonly string SaveSimulationSettingsToolTip = "Save current simulations settings as template into project or user profile. These settings will be used as default settings for each newly created simulation.";
         public static readonly string SavePercentileSettingsToolTip = "Save current curve selection to the user profile. These settings will be used as default settings for each newly created chart.";
         public static readonly string PopulationAnalysisSaveLoadToolTip = "Save or load population analysis settings.";
         public static readonly string ObservedDataMappingDescription = "Which compound are the following observed data related to?";
         public static readonly string ObservedDataToCompoundMapping = "Observed Data Conversion";
         public static readonly string SaveAs = "Save As...";
         public static readonly string CompoundEnzymaticProcess = "Enzymatic process";
         public static readonly string CompoundTransportProcess = "Transport process";
         public static readonly string CompoundBindingProcess = "Binding process";
         public static readonly string FavoritesToolTip = "Add as favorite";
         public static readonly string NoParameter = "No parameter";
         public static readonly string TransporterTypeDescription = "<B>Note:</B> Always verify localization of the defined transporter in tissues displaying apico-basal polarity (liver, kidney, intestine, brain). Apical (or basolateral) localization is tissue dependent and may not be consistent in all polarized cell types.";
         public static readonly string VisitUs = "Visit us";
         public static readonly string FilePath = "File Path";
         public static readonly string ExportDirectory = "Export Directory";
         public static readonly string SimulationFilePath = "Simulation File Path";
         public static readonly string PopulationFile = "Population File";
         public static readonly string PopulationFilePath = "Population File Path";
         public static readonly string LoadingApplication = "Loading Application...";
         public static readonly string UpdateAvailable = "Update available...";
         public static readonly string ReportCreationStarted = "Report creation started...";
         public static readonly string ReportCreationFinished = "Report created!";
         public static readonly string CheckForUpdate = "Check for Update";
         public static readonly string Template = "Template";
         public static readonly string BuildingBlock = "Building Block";
         public static readonly string BuildingBlocks = "Building Blocks";
         public static readonly string AllowAging = "Allow aging";
         public static readonly string SelectObservedDataToDelete = "Select observed data to delete...";
         public static readonly string SelectSimulationPKMLFile = "Select Pkml File containing the simulation to import";
         public static readonly string SelectReportFile = "Select report file...";
         public static readonly string SelectPopulationFileToImport = "Select population file to import...";
         public static readonly string SelectSimulationResultsFileToImport = "Select simulation results file to import...";
         public static readonly string SelectPKAnalysesFileToImport = "Select PK-Analyses file to import...";
         public static readonly string ResetLayoutSettingsToolTip = "Reset the application layout saved in the current user settings. This is necessary if the layout becomes corrupted.";
         public static readonly string ReallyResetLayout = "Reseting the layout cannot be undone\nThe new layout will be implemented in the next PK-Sim start-up.";
         public static readonly string ShowOntogeny = "Show ontogeny data defined in the database...";
         public static readonly string ImportOntogeny = "Load ontogeny from file...";
         public static readonly string ShowingOntogenyData = "Ontogeny data...";
         public static readonly string DeselectAll = "Deselect All";
         public static readonly string GlobalPKAnalyses = "Global PK-Analyses";
         public static readonly string PKAnalyses = "PK-Analyses";
         public static readonly string Repository = "Repository";
         public static readonly string Stop = "Stop";
         public static readonly string CloseView = "Close";
         public static readonly string CloseAll = "Close All Documents";
         public static readonly string CloseAllButThis = "Close All But This";
         public static readonly string TargetBodyWeight = "Target Weight";
         public static readonly string NormalizedExpressionLevels = "Normalized Expression Levels";
         public static readonly string ModelStructure = "Model Structure";
         public static readonly string OntogenyDescription = "The shown variability consists of a combined reported variability in maturation as well as enzyme activity and is based on experimental data from the following tissues:";
         public static readonly string MoleculeNotDefined = "None";
         public static readonly string SimulationTime = "Simulation Time";
         public static readonly string AddFile = "Add File...";
         public static readonly string CreatedUsingFiles = "Created using files";
         public static readonly string TimeProfile = "Time Profile";
         public static readonly string BoxWhisker = "Box Whisker";
         public static readonly string Scatter = "Scatter";
         public static readonly string Range = "Range";
         public static readonly string ImportFolder = "Import all files from a specific folder";
         public static readonly string ImportFiles = "Import single files";
         public static readonly string StartImport = "Start Import";
         public static readonly string Browse = "Browse";
         public static readonly string SelectFolderContainingSimulationResults = "Select folder containing the results to import";
         public static readonly string FileSuccessfullyImported = "File successfully imported";
         public static readonly string ImportSimulationResults = "Import Simulation Results";
         public static readonly string ImportSimulationPKAnalyses = "Import Simulation PK-Analyses";
         public static readonly string PKParameter = "PK-Parameter";
         public static readonly string PKParameters = "PK-Parameters";
         public static readonly string Grouping = "Grouping";
         public static readonly string CreateGrouping = "Create Grouping";
         public static readonly string FixedLimitsGroupingDefinition = "User defined limits";
         public static readonly string NumberOfBinsGroupingDefinition = "User defined (equally populated) number of bins";
         public static readonly string ValueMappingGroupingDefinition = "Value mapping";
         public static readonly string Groupings = "Groupings";
         public static readonly string Label = "Label";
         public static readonly string LabelGeneration = "Label Generation";
         public static readonly string NumberOfBins = "Number Of Bins";
         public static readonly string NumberOfIndividualsPerBin = "Number Of Individuals Per Bins";
         public static readonly string NamingPattern = "Naming Pattern";
         public static readonly string NaimingPatternStrategy = "Template";
         public static readonly string GeneratedLabels = "Generated Labels";
         public static readonly string LabelGenerationNumeric = "Numeric (1, 2, 3, 4, ...)";
         public static readonly string LabelGenerationRoman = "Roman (I, II, III, IV, ...)";
         public static readonly string LabelGenerationAlpha = "Alpha (A, B, C, D, ...)";
         public static readonly string LoadFromTemplate = "Load from Template...";
         public static readonly string SaveAsTemplate = "Save as Template...";
         public static readonly string UsePopulationBuidlingBlock = "Use a population building block (typically for PK-Sim)";
         public static readonly string UsePopulationFileCSV = "Load a population from file (typically for MoBi)";
         public static readonly string NewPopulationFromSize = "Only allocate the number of individuals (typically for MoBi)";
         public static readonly string UseHistogramInReport = "Use in report";
         public static readonly string GridVisible = "Grid Visible";
         public static readonly string StartColor = "Start Color";
         public static readonly string EndColor = "End Color";
         public static readonly string Symbol = "Symbol";
         public static readonly string Symbols = "Symbols";
         public static readonly string LineStyle = "Line Style";
         public static readonly string EditPopulationAnalysisConfiguration = "Edit Analysis...";
         public static readonly string SelectSimulationForComparison = "Select simulations to use in comparison";
         public static readonly string SelectSimulationForComparisonDescription = "Select all population simulations that will be used in the comparison";
         public static readonly string BoxWhiskerAnalysis = analysisDisplay(BoxWhisker);
         public static readonly string ScatterAnalysis = analysisDisplay(Scatter);
         public static readonly string RangeAnalysis = analysisDisplay(Range);
         public static readonly string TimeProfileAnalysis = analysisDisplay(TimeProfile);
         public static readonly string SimulationComparison = "Simulation Comparison";
         public static readonly string StudyId = "Study Id";
         public static readonly string Route = "Route";
         public static readonly string PatientId = "Patient Id";
         public static readonly string MetaData = "Meta Data";
         public static readonly string AddDataPoint = "Add Data Point";
         public static readonly string AddMetaData = "Add Meta Data";
         public static readonly string LogScale = "Logarithmic Scale";
         public static readonly string LinearScale = "Linear Scale";
         public static readonly string Folder = "Folder";
         public static readonly string MoveUp = "Move Up";
         public static readonly string MoveDown = "Move Down";
         public static readonly string AvailableFields = "Available Parameters";
         public static readonly string Panes = "Panes";
         public static readonly string XGrouping = "X-Grouping";
         public static readonly string XField = "X =";
         public static readonly string YField = "Y = ";
         public static readonly string SelectedOutputs = "Selected Outputs";
         public static readonly string ReferenceSimulation = "Reference Simulation";
         public static readonly string ReferenceSimulationDescription = "Reference will not be used in grouping and splitting";
         public static readonly string Reference = "Reference";
         public static readonly string ImportPopulationSettings = "Imported Population";
         public static readonly string ImportPopulationSettingsDescription = "The base individual is used as template for the population: Parameters are replaced by values imported from file, if they exist. Otherwise, parameters are taken from the base individual.";
         public static readonly string SimulationsUsedInComparison= "Simulations used in comparison";
         public static readonly string UsedAsReferenceSimulation = "Used as Reference Simulation";
         public static readonly string ExportSimulationResultsToExcelDescription = ExportSimulationResultsToExcel;
         public static readonly string StatisticalOutputSelectionDescription = "Output: Select distribution statistics for display";
         public static readonly string ChooseFieldsToDisplay = chooseFieldsToDisplay("field");
         public static readonly string ChooseParametersToDisplay = "Add one or several parameters, change name or unit by clicking into the respective column";
         public static readonly string ChoosePKParametersToDisplay = "Add one or several PK-Parameters, change name or unit by clicking into the respective column";
         public static readonly string ChooseOutputsToDisplay = "Add one or several outputs here for display from left panel, select distribution statistics below";
         public static readonly string ApplyGroupingToObservedData = "Apply grouping to observed data";
         public static readonly string ApplyGroupingToObservedDataToolTip = "Uses the meta data to split the observed data according to grouping by panes and/or color";
         public static readonly string TabbedView = "Tabbed View";
         public static readonly string AccordionView = "Accordion View";
         public static readonly string CompoundsSelection = "Compounds Selection";
         public static readonly string Inhibition = "Inhibition";
         public static readonly string Induction = "Induction";
         public static readonly string InhibitionAndInduction = "Inhibition / Induction";
         public static readonly string DistributionCalculation = "Distribution Calculation";
         public static readonly string SpecificIntestinalPermeability = "Specific Intestinal Permeability";
         public static readonly string Absorption = "Absorption";
         public static readonly string Sink = "Sink";
         public static readonly string AddInhibitionProcess = "Add Inhibition Process...";
         public static readonly string AddInductionProcess = "Add Induction Process...";
         public static readonly string AddInteraction= "Add Interaction";
         public static readonly string AffectedEnzymeOrTransporter = "Affected enzyme / transporter";
         public static readonly string SpecificOrganPermeability = "Specific Organ Permeability";
         public static readonly string Metabolite = "Metabolite";
         public static readonly string InteractionProcessInCompound = "Interaction process in compound";
         public static readonly string ReactionDiagram = "Reaction Diagram";
         public static readonly string ShowDiagram = "Show Diagram";
         public static readonly string HidePKAnalysis = "Hide PK-Analysis";
         public static readonly string CalculateVSSValues = "Calculate VSS Values";
         public static readonly string PossibleVSSValuesForDefaultSpecies = "TODO PossibleVSSValuesForDefaultSpecies";
         public static readonly string DoYouWantToSaveCompoundMetaboliteAsTemplate = "Do you also want to save the metabolite(s) of this compound?";
         public static readonly string DoYouWantToLoadReferencedTemplateAsWell = "Do you also want to load the metabolite(s) of this compound?";
         public static readonly string LowerPercentile = "Lower Percentile";
         public static readonly string UpperPercentile = "Upper Percentile";
         public static readonly string LowerValue= "Lower Value";
         public static readonly string UpperValue= "Upper Value";
         public static readonly string ShowOutliers = "Show outliers";
         public static readonly string ShowOutliersToolTip = "Outliers will be shown on analysis and report when this option is selected";
         public static readonly string Solver = "Solver";
         public static readonly string LoadPopulationAnalysisWorkflowFromTemplateDescription = "Load population analysis from template";
         public static readonly string SavePopulationAnalysisWorkflowToTemplateDescription = "Save population analysis to template";
         public static readonly string IsSmallMolecule = "Is small molecule";
         public static readonly string ReallyRemoveObservedDataFromSimulation = string.Format("Really remove {0} from simulation?\nHint: {0} will not be deleted from project", ObjectTypes.ObservedData);
         public static readonly string ExportSettings = "Export Settings";
         public static readonly string Administration = "Administration";
         public static readonly string CompoundConfiguration = "Compound Configuration";
         public static readonly string Processes = "Processes";
         public static readonly string Protocol = "Protocol";
         public static readonly string SimulationProperties = "Simulation Properties";
         public static readonly string IndividualIds = "Individual Ids";
         public static readonly string IndividualIdsDescription = "Ids of individuals to extract separated with comma (e.g. 1, 4, 8)";
         public static readonly string UseWatermark = "Use watermark in charts when exporting to clipboard?";
         public static readonly string WatermarkText = "Text";
         public static readonly string WatermarkProperties = "Watermark Properties";

         public static string NumberOfIndividualsToExtract(int count, string populationName) => $"{count} {"individual".PluralizeIf(count)} will be extracted from population {populationName}.";

         public static string IndividualExtractionNamingPatternDescription(string populationNamePattern, string individualIdPattern)
         {
            var sb = new StringBuilder();
            sb.AppendLine("Automatically generates individual names replacing the occurence in the naming pattern of:");
            sb.AppendLine($" -   <b>{populationNamePattern}</b> with the name of the population");
            sb.AppendLine($" -   <b>{individualIdPattern}</b> with the id of the individual");
            return sb.ToString();
         }

         public static string ExtractIndividualFromPopulation(string populationName) => $"Extract Individuals from Population '{populationName}'";

         public static string ExtractIndividualPopulationDescription(string populationName, int numberOfIndividuals) => 
            $"Population '{populationName}' has {numberOfIndividuals} individuals. Individual Ids for this population are defined between 0 and {numberOfIndividuals-1}.";

         public static string GenderRationFor(string gender) => $"{gender} ratio";

         public static string AddPartialProcess(string partialProcessType) => $"Add {partialProcessType} Process...";

         private static string chooseFieldsToDisplay(string fieldType) => $"Choose {fieldType.ToLower()}s to display";

         public static string CreateAnalysis(string analysisType) => $"Create {analysisDisplay(analysisType)}";

         public static string EditAnalysis(string analysisType) => $"Edit {analysisDisplay(analysisType)}";

         private static string analysisDisplay(string analysisType) => $"{analysisType} {Analysis}";

         public static string NamingPatternDescription(string iterationPattern, string startPattern, string endPattern)
         {
            var sb = new StringBuilder();
            sb.AppendLine("Automatically generates labels replacing the occurence in the naming pattern of:");
            sb.AppendFormat(" -   <b>{0}</b> with an identifer generated using the selected template\n", iterationPattern);
            sb.AppendFormat(" -   <b>{0}</b> with a value representing the start of the interval and n the number of digits\n", startPattern);
            sb.AppendFormat(" -   <b>{0}</b> with a value representing the end of the interval and n the number of digits\n", endPattern);
            return sb.ToString();
         }

         public static string NamingPatternDescriptionToolTip(string iterationPattern, string startPattern, string endPattern)
         {
            var sb = new StringBuilder();
            sb.AppendLine("Naming Pattern Options:");
            sb.AppendFormat(" -   Use <b>{0}</b> for a unique identifier using the selected template.\n", iterationPattern);
            sb.AppendFormat(" -   Use <b>{0}</b> for the start of the interval.\n", startPattern);
            sb.AppendFormat(" -   Use <b>{0}</b> for the end of the interval.\n", endPattern);
            sb.AppendFormat(" -   Change <b>n</b> to specificy the number of digits to use, (i.e. n= 1, 2, 3 etc... ");
            sb.AppendLine("if n is not specified, the value defined in the user settings will be used.");
            sb.AppendLine();
            sb.AppendLine("<i>Examples:</i>");
            sb.AppendLine();
            sb.AppendLine("Consider two intervals, [1.12, 2.24] and [2.24, 3.36] and the following patterns");
            sb.AppendLine("<b>My Interval_#</b>: => My Interval_1 <i>and</i> My_Interval_2");
            sb.AppendLine("<b>From {start} to {end}</b>: => From 1.12 to 2.24 <i>and</i> From 2.24 to 3.36");
            sb.AppendLine("<b>From {start:1} to {end:1}</b>: => From 1.1 to 2.2 <i>and</i> From 2.2 to 3.4");
            sb.AppendLine("<b>From {start:3} to {end:3}</b>: => From 1.120 to 2.240 <i>and</i> From 2.240 to 3.360");
            return sb.ToString();
         }

         public static string ImportOntogenyToolTip
         {
            get
            {
               var sb = new StringBuilder();
               sb.AppendLine("The Excel table for ontogeny import should have a format of minimum <b>two colums</b>, plus a third optional error column.");
               sb.AppendLine(" -  The first column represents the <b>postmenstrual age</b> (unit Time e.g. in years).");
               sb.AppendLine(" -  The second column represents the respective <b>ontogeny factor</b>.");
               sb.AppendLine(" -  The optional third column represents the <b>variability</b> for the ontogeny factor if available (geometric SD)");
               return sb.ToString();
            }
         }

         public static string ImportFormulationDescription
         {
            get
            {
               var sb = new StringBuilder();
               sb.AppendLine("The Excel table for formulation import should have a format of two colums.");
               sb.AppendLine("The first column represents the Time (unit Time e.g. in years).");
               sb.AppendLine("The second column represents the fraction of the dose released at t.");
               return sb.ToString();               
            }
         } 
         
         public static string ListOf(string item) => $"List of {item}";

         public static string AddProteinExpression(string moleculeType) => $"Add {moleculeType} Expression...";

         public static string ReportEqual(string name, object value) => $"{name} = {value}";

         public static string ReportIs(string name, object value) => $"{name}: {value}";

         public static string ParametersDefinedIn(string container) => $"Parameters defined in {container}";

         public static string LoadingObject(string objectToLoad) => $"Loading '{objectToLoad}'...";

         public static string OntogenyFor(string moleculeName) => $"{Ontogeny} for {moleculeName}";

         public static string AddParameterAsFavorites(string parameterName) => $"Add '{parameterName}' as favorite";

         public static string ProductIsUptodate(string productName) => $"{productName} is up to date!";

         public static string ImportSimulationPKAnalysesDescription
         {
            get
            {
               var sb = new StringBuilder();
               sb.AppendLine("Import PK-Analysis from csv. The table should have <b>five columns</b>:");
               sb.AppendLine("(e.g. IndividualId, Output, Name, Value, Unit)");
               return sb.ToString();
            }
         }

         public static string ReallyRemoveFieldUsedInGrouping(string fieldName, IEnumerable<string> referencingFields )
         {
            return string.Format("Field '{0}' is used by {1}.\nRemoving '{0}' will also remove any fields depending on it. Do you want to continue?",
               fieldName, referencingFields.ToString(",", "'"));
         }

         public static string ReferenceConcentrationDescription(string moleculeType)
         {
            var sb = new StringBuilder();
            sb.AppendLine($"Reference concentration corresponds to 100% expression level of selected {moleculeType.ToLower()}.");
            sb.AppendLine();
            sb.AppendLine("CYP Form | [pmol/mg HLM] | [µmol CYP/l Liver tissue]");
            sb.AppendLine("----------------------------------------------------");
            sb.AppendLine("CYP1A2   | 45            | 1.8");
            sb.AppendLine("CYP2A6   | 68            | 2.72");
            sb.AppendLine("CYP2B6   | 39            | 1.56");
            sb.AppendLine("CYP2C18  | <2,5          | <0,1");
            sb.AppendLine("CYP2C19  | 19            | 0.76");
            sb.AppendLine("CYP2C8   | 64            | 2.56");
            sb.AppendLine("CYP2C9   | 96            | 3.84");
            sb.AppendLine("CYP2D6   | 10            | 0.4");
            sb.AppendLine("CYP2E1   | 49            | 1.96");
            sb.AppendLine("CYP3A4   | 108           | 4.32");
            sb.AppendLine("CYP3A5   | 1             | 0.04");
            sb.AppendLine();
            sb.AppendLine("Source: Rodrigues (1999), Table2");

            return sb.ToString();
         }

         public static string HalfLifeLiverDescription(string moleculeType) => $"Apparent half life liver of the affected {moleculeType.ToLower()}";

         public static string HalfLifeIntestineDescription(string moleculeType) => $"Apparent half life intestine of the affected {moleculeType.ToLower()}";

         public static string ConfigureSimulation(string simulatioName) => $"Configure Simulation: {simulatioName}";

         public static string CloningSimulation(string simulatioName) => $"Cloning Simulation: {simulatioName}";

         public static string CreateGroupParameterAlternativeCaption(string groupName) => $"Create new {groupName.ToLower()} value";

         public static string RenameGroupParameterAlternativeCaption(string groupName) => $"Rename {groupName.ToLower()} value";

         public static IReadOnlyList<string> PredefinedSolubilityAlternatives() => new []{ DefaultAlternative, "S_aq", "S_buffer", "S_FaSSIF", "S_FeSSIF", "S_FaSSGF" };

         public static IReadOnlyList<string> PredefinedFractionUnboundAlternatives() => new []{ DefaultAlternative, "fu_plasma", "fu_invitro" };

         public static IReadOnlyList<string> PredefinedLipophilicityAlternatives() => new []{ DefaultAlternative, "LogP", "LogMA", "LogD", "cLogP", "cLogMA", "cLogD" };

         public static string EditFormulation(string name) => $"Formulation: '{name}'";

         public static string EditEvent(string name) => $"Event: '{name}'";

         public static string ScaleIndividual(string name) => $"Scale Individual: '{name}'";

         public static string EditIndividual(string name) => $"Individual: '{name}'";

         public static string EditSimulationSettings(string name) => $"Simulation Settings: '{name}'";

         public static string EditCompound(string name) => $"Compound: '{name}'";

         public static string EditPopulation(string name) => $"Population: '{name}'";

         public static string EditProtocol(string name) => $"Administration Protocol: '{name}'";

         public static string ConvertingProject(string projectName) => $"Converting project '{projectName}'";

         public static string CreatingIndividualInPopulation(int currentIndividual, int numOfIndividual) => $"Creating individual {currentIndividual}/{numOfIndividual}...";

         public static string AboutProduct(string productName) => $"About {productName}";

         public static string SchemaItemDescription(string application, string formulation, string dose, string startTime)
         {
            var sb = new StringBuilder();
            sb.AppendLine($"{ObjectTypes.Administration}= {application}");
            if (!string.IsNullOrEmpty(formulation))
               sb.AppendLine($"{ObjectTypes.Formulation}= {formulation}");

            sb.AppendLine($"{Dose}= {dose}");
            sb.AppendLine($"{StartTime}= {startTime}");
            return sb.ToString();
         }

         public static readonly string SelectMoBiExecutablePath = "Select MoBi executable path";

         public static string SelectDatabasePathFor(string speciesDisplayName) => $"Select Expressions Database for {speciesDisplayName}";

         public static string CreateSystemicProcessInCompoundCaption(string systemicProcessType) => $"Create new {systemicProcessType} for compound";

         public static string EnterNameEntityCaption(string type) => $"Enter name for {type}";

         public static string RenameEntityCaption(string type, string name) => $"New name for {type} '{name}'";

         public static string RenameDataSourceCaption() => $"New name for {DataSource.ToLower()}";

         public static string RenamePartialProcessesMolecule(string moleculeType) => $"New name for {moleculeType.ToLower()}";

         public static string EditDescriptionEntityCaption(string type, string name) => $"Description for {type} '{name}'";

         public static string ReallyDeleteAlternative(string alternativeName) => ReallyDeleteObjectOfType(ObjectTypes.ParameterGroupAlternative, alternativeName);

         public static string ReallyDeleteSimulationComparisons(IReadOnlyList<string> simulationComparisons) => ReallyDeleteObjectOfType(SimulationComparison, simulationComparisons);

         public static string ReallyDeleteTemplate(string templateName) => ReallyDeleteObjectOfType(ObjectTypes.Template, templateName);

         public static string ReallyDeleteProtein(string proteinType, string proteinName) => ReallyDeleteObjectOfType(proteinType, proteinName);

         public static string ReallyDeleteProcess(string processType, string processName) => ReallyDeleteObjectOfType(processType, processName);

         public static string ReallyDeleteObjectOfType(string type, IReadOnlyList<string> names)
         {
            var displayNames = names.ToString(", ","'");
            var displayTypes = type.PluralizeIf(names);
            var message = $"Really delete {displayTypes.ToLowerInvariant()} {displayNames}?";

            if (string.Equals(type, ObjectTypes.Simulation))
               message = $"{message}\n{displayTypes} results will be permanently deleted.\nMoreover {displayTypes.ToLowerInvariant()} will be removed from all comparison charts, sensitivity analyses, and parameter identifications.";

            if (string.Equals(type, ObjectTypes.Template))
               message = $"{message}\nThis action is irreversible!";

            return message;
         }

         public static string ReallyDeleteObjectOfType(string type, params string[] names) => ReallyDeleteObjectOfType(type, names.ToList());

         public static string RegisterAssembly(string assemblyname) => $"Loading {assemblyname.ToLower()}";

         public static string CreateBuildingBlockHint(string buildingblockType) => $"Create new {buildingblockType}...";

         public static string LoadBuildingBlockHint(string buildingblockType) => $"Load {buildingblockType} from template...";

         public static string LoadBuildingBlockFromTemplate(string buildingBlock) => $"Load {buildingBlock} from template";

         public static string TemplateWithNameAlreadyExistsInTheDatabase(string name, string buildingBlockType) => $"A template for {buildingBlockType} named '{name}' already exists.";

         public static string MoleculeInIndividual(string molecule) => $"{molecule} in individual";

         public static string CreateSystemicProcess(string systemProcessDisplayName) => $"Add {systemProcessDisplayName} Process...";

         public static string ConvertingSimulation(int currentSimulation, int numberOfSimulations) => $"Converting simulation {currentSimulation}/{numberOfSimulations}...";

         public static string ContactSupport(string productDisplayName, string support) => $"For more information, please contact your {productDisplayName} support ({support})";

         public static string EditTableParameter(string parameter, bool editable)
         {
            return $"{(editable ? "Edit" : "Show")} table parameter '{parameter}'";
         }

         public static string ChartSettingsSaved() => "Chart settings were saved into project.";

         public static string SimulationSettingsSavedFor(string settingsType)
         {
            return $"Settings were saved into {settingsType.ToLowerInvariant()}.";
         }

         public static string TemplateSuccessfullySaved(string buildingBlockName, string buildingBlockType)
         {
            return $"{buildingBlockType} '{buildingBlockName}' successfully saved in the template database.";
         }

         public static string TemplatesSuccessfullySaved(IReadOnlyList<string> buildingBlockNames)
         {
            return $"{buildingBlockNames.ToString(", ", "'")} \nsuccessfully saved in the template database.";
         }

         public static string ReportCreationStartedMessage(string reportFullPath) => "This might take a while...";

         public static string ReportCreationFinishedMessage(string reportFullPath) => $"Report can be found at {reportFullPath}";

         public static string CreateGroupingForField(string fieldName) => $"Create Grouping for '{fieldName}'";

         public static string Interval(string min, string max) => $"{min}..{max}";

         public static string EditGroupingFor(string groupingName, string fieldName) => $"Edit Grouping '{groupingName}' for '{fieldName}'";

         public static string DeleteFilesIn(string populationFolder) => $"Directory '{populationFolder}' is not empty. All Files will be removed. Proceed?";

         public static string DragFieldMessage(string fieldType)
         {
            if (string.Equals(fieldType, Panes))
               return "Drag parameters here for separate panes";

            if (string.Equals(fieldType, Colors))
               return "Drag a parameter here for curve coloration";

            return $"Drag a field here to group by {fieldType}";
         }

         public static string FilterAreaDragFieldMessage() => "Drag a field here to remove grouping";

         public static readonly string ChartYScale = "Chart Y Scale";

         public static string  SelectSnapshotExportFile(string objectName, string ojectType) => $"Export snapshot for {ojectType.ToLowerInvariant()} '{objectName}'";

         public static string  LoadObjectFromSnapshot(string ojectType) => $"Load {ojectType.ToLowerInvariant()} from snapshot";

         public static string LoadFromSnapshot => "Load Snapshot";         
      }

      public static class Reporting
      {
         private static string listOfValuesDescriptionFor(string parameter)
         {
            return "{0} lists " + parameter + " values for compound {1}.";
         }

         public static readonly string LipophilicityDescription = listOfValuesDescriptionFor("lipophilicity");
         public static readonly string FractionUnboundDescription = listOfValuesDescriptionFor("fraction unbound");
         public static readonly string PermeabilityDescription = listOfValuesDescriptionFor("organ permeability");
         public static readonly string IntestinalPermeabilityDescription = listOfValuesDescriptionFor("intestinal permeability");
         public static readonly string SolubilityDescription = listOfValuesDescriptionFor("solubility");
      }

      public static class Classifications
      {
         public static readonly string Species = ObjectTypes.Species;
         public static readonly string Compound = ObjectTypes.Compound;
         public static readonly string AdministrationProtocol = ObjectTypes.AdministrationProtocol;
         public static readonly string Individual = ObjectTypes.Individual;
         public static readonly string Population = ObjectTypes.Population;
         public static readonly string SimulationType= "Simulation Type";
      }

      public class PKAnalysis
      {
         public static readonly string Compound = ObjectTypes.Compound;
         public static readonly string ParameterDisplayName = ObjectTypes.Parameter;
         public static readonly string CurveName = "CurveName";
         public static readonly string Value = UI.Value;
         public static readonly string ParameterName = "ParameterName";
         public static readonly string Unit = UI.Unit;
         public static readonly string Description = "Description";
         public static readonly string Warning = "Warning";
      }

      public class Comparison
      {
         public static readonly string RelativeTolerance = "Relative Tolerance";
         public static readonly string FormulaComparisonMode = "Formula Comparision";
         public static readonly string OnlyComputeModelRelevantProperties = "Do not compare descriptions";
         public static readonly string FormulaComparisonValue = "Compare values";
         public static readonly string FormulaComparisonFormula = "Compare Formulas";
         public static readonly string RunComparison = "Start";
         public static readonly string Left = "Left";
         public static readonly string Right = "Right";
         public static readonly string ComparisonResults = "Results";
         public static readonly string ComparisonSettings = "Settings";
         public static readonly string ExportToExcel = "Export to Excel";
         public static readonly string ShowSettings = "Settings";
         public static readonly string HideSettings = "Hide";
         public static readonly string Absent = "Absent";
         public static readonly string Present = "Present";
      }
   }
}
