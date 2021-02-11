using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using OSPSuite.Assets;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Serialization;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Core.Services;
using OSPSuite.Infrastructure.Import.Core;
using OSPSuite.Utility.Extensions;
using OSPSuite.Infrastructure.Import.Services;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using IObservedDataTask = PKSim.Core.Services.IObservedDataTask;

namespace PKSim.Infrastructure.Services
{
   public class ImportObservedDataTask : IImportObservedDataTask, IObservedDataConfiguration
   {
      private readonly IDataImporter _dataImporter;
      private readonly IExecutionContext _executionContext;
      private readonly IDimensionRepository _dimensionRepository;
      private readonly IBuildingBlockRepository _buildingBlockRepository;
      private readonly ISpeciesRepository _speciesRepository;
      private readonly IDefaultIndividualRetriever _defaultIndividualRetriever;
      private readonly IRepresentationInfoRepository _representationInfoRepository;
      private readonly IObservedDataTask _observedDataTask;
      private readonly IParameterChangeUpdater _parameterChangeUpdater;
      private readonly IDialogCreator _dialogCreator;
      private readonly IContainer _container;
      private readonly IOSPSuiteXmlSerializerRepository _modelingXmlSerializerRepository;

      public ImportObservedDataTask(IDataImporter dataImporter, IExecutionContext executionContext,
         IDimensionRepository dimensionRepository, IBuildingBlockRepository buildingBlockRepository, ISpeciesRepository speciesRepository,
         IDefaultIndividualRetriever defaultIndividualRetriever, IRepresentationInfoRepository representationInfoRepository,
         IObservedDataTask observedDataTask, IParameterChangeUpdater parameterChangeUpdater, IDialogCreator dialogCreator, IContainer container,
         IOSPSuiteXmlSerializerRepository modelingXmlSerializerRepository)
      {
         _dataImporter = dataImporter;
         _executionContext = executionContext;
         _dimensionRepository = dimensionRepository;
         _buildingBlockRepository = buildingBlockRepository;
         _speciesRepository = speciesRepository;
         _defaultIndividualRetriever = defaultIndividualRetriever;
         _representationInfoRepository = representationInfoRepository;
         _observedDataTask = observedDataTask;
         _parameterChangeUpdater = parameterChangeUpdater;
         _dialogCreator = dialogCreator;
         _container = container;
         _modelingXmlSerializerRepository = modelingXmlSerializerRepository;
      }

      public void AddObservedDataToProject() => AddObservedDataToProjectForCompound(null);

      public void AddObservedDataToProjectForCompound(Compound compound)
      {
         _executionContext.Load(compound);
         addObservedData(importConfiguration, compound);
      }

      public void AddObservedDataFromXmlToProjectForCompound(Compound compound)
      {
         _executionContext.Load(compound);
         AddObservedDataFromXml(importConfiguration, compound);
      }

      public void AddObservedDataFromXml(Func<IReadOnlyList<ColumnInfo>> importConfiguration, Compound compound = null, bool allowCompoundNameEdit = false)
      {
         var dataImporterSettings = new DataImporterSettings { Caption = $"{CoreConstants.ProductDisplayName} - {PKSimConstants.UI.ImportObservedData}", IconName = ApplicationIcons.ObservedData.IconName };
         dataImporterSettings.AddNamingPatternMetaData(Constants.FILE);

         var metaDataCategories = defaultMetaDataCategories().ToList();
         metaDataCategories.Insert(0, compoundNameCategory(compound, allowCompoundNameEdit));


//----------------------------------------------
         using (var serializationContext = SerializationTransaction.Create(_container))
         {
            var configuration = new OSPSuite.Core.Import.ImporterConfiguration();

            var serializer = _modelingXmlSerializerRepository.SerializerFor(configuration);

            var fileName = _dialogCreator.AskForFileToOpen("Save configuration", "xml files (*.xml)|*.xml|All files (*.*)|*.*", Constants.DirectoryKey.PROJECT);

            var xel = XElement.Load(fileName);
            configuration = serializer.Deserialize<OSPSuite.Core.Import.ImporterConfiguration>(xel, serializationContext);
            //broadcast...
         }

         //this should probably be a seperate function
         

         
         var importedObservedData = _dataImporter.ImportFromXml(metaDataCategories, importConfiguration(), dataImporterSettings);
         foreach (var observedData in importedObservedData)
         {
            adjustMolWeight(observedData);
            _observedDataTask.AddObservedDataToProject(observedData);
            updateQuantityInfoInImportedColumns(observedData);
         }
      }

      private void addObservedData(Func<IReadOnlyList<ColumnInfo>> importConfiguration, Compound compound = null, bool allowCompoundNameEdit = false)
      {
         var dataImporterSettings = new DataImporterSettings {Caption = $"{CoreConstants.ProductDisplayName} - {PKSimConstants.UI.ImportObservedData}", IconName = ApplicationIcons.ObservedData.IconName};
         dataImporterSettings.AddNamingPatternMetaData(Constants.FILE);

         var metaDataCategories = defaultMetaDataCategories().ToList();
         metaDataCategories.Insert(0, compoundNameCategory(compound, allowCompoundNameEdit));

         var importedObservedData = _dataImporter.ImportDataSets(metaDataCategories, importConfiguration(), dataImporterSettings);
         foreach (var observedData in importedObservedData)
         {
            adjustMolWeight(observedData);
            _observedDataTask.AddObservedDataToProject(observedData);
            updateQuantityInfoInImportedColumns(observedData);
         }
      }

      private void updateQuantityInfoInImportedColumns(DataRepository observedData)
      {
         var moleculeName = observedData.ExtendedPropertyValueFor(ObservedData.MOLECULE);
         var organ = observedData.ExtendedPropertyValueFor(ObservedData.ORGAN);
         var compartment = observedData.ExtendedPropertyValueFor(ObservedData.COMPARTMENT);

         foreach (var col in observedData.AllButBaseGrid())
         {
            //needs to match the path in simulation ROOT\ORGANISM\ORGAN\COMPARTMENT\MOLECULE\Name
            var colName = col.Name.Replace(ObjectPath.PATH_DELIMITER, "\\");
            col.QuantityInfo = new QuantityInfo(col.Name, new[] {observedData.Name, CoreConstants.ContainerName.ObservedData, organ, compartment, moleculeName, colName}, QuantityType.Undefined);
         }

         var baseGrid = observedData.BaseGrid;
         var baseGridName = baseGrid.Name.Replace(ObjectPath.PATH_DELIMITER, "\\");
         baseGrid.QuantityInfo = new QuantityInfo(baseGrid.Name, new[] {observedData.Name, baseGridName}, QuantityType.Time);
      }

      private void adjustMolWeight(DataRepository observedData)
      {
         _parameterChangeUpdater.UpdateMolWeightIn(observedData);
      }


      private IReadOnlyList<ColumnInfo> importConfiguration()
      {
         var columns = new List<ColumnInfo>();
         var timeColumn = createTimeColumn();
         columns.Add(timeColumn);


         var supportedDimensions = new[] {
            _dimensionRepository.MolarConcentration,
            _dimensionRepository.MassConcentration,
            _dimensionRepository.Amount,
            _dimensionRepository.Mass,
            _dimensionRepository.NoDimension,
            _dimensionRepository.Fraction,
         };

         var measurementInfo = new ColumnInfo
         {
            Name = PKSimConstants.UI.Measurement,
            Description = PKSimConstants.UI.Measurement,
            DefaultDimension = supportedDimensions[0],
            IsMandatory = true,
            NullValuesHandling = NullValuesHandlingType.DeleteRow,
            BaseGridName = timeColumn.Name
         };

         addSupportedDimensionsTo(measurementInfo, supportedDimensions);

         columns.Add(measurementInfo);
         columns.Add(createErrorColumnInfo(measurementInfo, supportedDimensions));

         return columns;
      }

      private ColumnInfo createErrorColumnInfo(ColumnInfo mainColumnInfo, IEnumerable<IDimension> supportedDimensions)
      {
         var errorInfo = new ColumnInfo
         {
            DefaultDimension = mainColumnInfo.DefaultDimension,
            Name = PKSimConstants.UI.Error,
            Description = PKSimConstants.UI.Error,
            IsMandatory = false,
            NullValuesHandling = NullValuesHandlingType.Allowed,
            BaseGridName = mainColumnInfo.BaseGridName,
            RelatedColumnOf = mainColumnInfo.Name
         };

         addSupportedDimensionsTo(errorInfo, supportedDimensions);

         return errorInfo;
      }

      private void addSupportedDimensionsTo(ColumnInfo column, IEnumerable<IDimension> supportedDimensions)
      {
         var mainDimension = column.DefaultDimension;
         supportedDimensions.Select(dim => new DimensionInfo { Dimension = dim, IsMainDimension = Equals(dim, mainDimension)}).Each(column.DimensionInfos.Add);
      }

      private ColumnInfo createTimeColumn()
      {
         var timeColumn = new ColumnInfo
         {
            DefaultDimension = _dimensionRepository.Time,
            Name = PKSimConstants.UI.Time,
            Description = PKSimConstants.UI.Time,
            IsMandatory = true,
            NullValuesHandling = NullValuesHandlingType.DeleteRow,
         };

         timeColumn.DimensionInfos.Add(new DimensionInfo {Dimension = _dimensionRepository.Time, IsMainDimension = true});
         return timeColumn;
      }

      public IEnumerable<string> PredefinedValuesFor(string name)
      {
         if (string.Equals(name, ObservedData.ORGAN))
            return predefinedOrgans;

         if (string.Equals(name, ObservedData.COMPARTMENT))
            return predefinedCompartments;

         if (string.Equals(name, CoreConstants.ObservedData.SPECIES))
            return predefinedSpecies;

         if (string.Equals(name, CoreConstants.ObservedData.GENDER))
            return predefinedGenders;

         return Enumerable.Empty<string>();
      }

      public IReadOnlyList<string> DefaultMetaDataCategories => CoreConstants.ObservedData.DefaultProperties;

      public IReadOnlyList<string> ReadOnlyMetaDataCategories => new List<string> {ObservedData.MOLECULE};

      public bool MolWeightEditable => false;

      public bool MolWeightVisible => true;

      private IEnumerable<string> predefinedGenders => predefinedValuesFor(addPredefinedGenderValues);

      private IEnumerable<string> predefinedSpecies => predefinedValuesFor(addPredefinedSpeciesValues);

      private IEnumerable<string> predefinedCompartments => predefinedValuesFor(addPredefinedCompartmentValues);

      private IEnumerable<string> predefinedOrgans => predefinedValuesFor(addPredefinedOrganValues);

      private IEnumerable<string> predefinedValuesFor(Action<MetaDataCategory> predefinedValuesRetriever)
      {
         var category = new MetaDataCategory();
         predefinedValuesRetriever(category);
         return category.ListOfValues.Values;
      }

      private IEnumerable<MetaDataCategory> defaultMetaDataCategories()
      {
         var categories = new List<MetaDataCategory>();

         var speciesCategory = createMetaDataCategory<string>(CoreConstants.ObservedData.SPECIES, isMandatory: true, isListOfValuesFixed: true, fixedValuesRetriever: addPredefinedSpeciesValues);
         categories.Add(speciesCategory);

         var organCategory = createMetaDataCategory<string>(ObservedData.ORGAN, isMandatory: true, isListOfValuesFixed: true, fixedValuesRetriever: addPredefinedOrganValues);
         organCategory.Description = ObservedData.ObservedDataOrganDescription;
         categories.Add(organCategory);

         var compCategory = createMetaDataCategory<string>(ObservedData.COMPARTMENT, isMandatory: true, isListOfValuesFixed: true, fixedValuesRetriever: addPredefinedCompartmentValues);
         compCategory.Description = ObservedData.ObservedDataCompartmentDescription;
         categories.Add(compCategory);

         // Add non-mandatory metadata categories
         categories.Add(createMetaDataCategory<string>(PKSimConstants.UI.StudyId));
         categories.Add(createMetaDataCategory<string>(PKSimConstants.UI.Gender, isListOfValuesFixed: true, fixedValuesRetriever: addPredefinedGenderValues));
         categories.Add(createMetaDataCategory<string>(PKSimConstants.UI.Dose));
         categories.Add(createMetaDataCategory<string>(PKSimConstants.UI.Route));
         categories.Add(createMetaDataCategory<string>(PKSimConstants.UI.PatientId));

         return categories;
      }

      private void addPredefinedGenderValues(MetaDataCategory genderMetaData)
      {
         addUndefinedValueTo(genderMetaData);
         var defaultIndividual = _defaultIndividualRetriever.DefaultIndividual();
         foreach (var gender in defaultIndividual.AvailableGenders())
         {
            addInfoToCategory(genderMetaData, gender);
         }
      }

      private static void addUndefinedValueTo(MetaDataCategory metaDataCategory)
      {
         metaDataCategory.ListOfValues.Add(PKSimConstants.UI.Undefined, PKSimConstants.UI.Undefined);
      }

      private void addPredefinedCompartmentValues(MetaDataCategory compCategory)
      {
         var defaultIndividual = _defaultIndividualRetriever.DefaultIndividual();
         addUndefinedValueTo(compCategory);
         foreach (var compartment in defaultIndividual.Organism.Organ(CoreConstants.Organ.MUSCLE).Compartments.Where(x => x.Visible))
         {
            addInfoToCategory(compCategory, compartment);
         }

         addInfoToCategory(compCategory, new Observer().WithName(CoreConstants.Observer.TISSUE));
         addInfoToCategory(compCategory, new Observer().WithName(CoreConstants.Observer.INTERSTITIAL_UNBOUND));
         addInfoToCategory(compCategory, new Observer().WithName(CoreConstants.Observer.INTRACELLULAR_UNBOUND));
         addInfoToCategory(compCategory, new Observer().WithName(CoreConstants.Observer.WHOLE_BLOOD));
         addInfoToCategory(compCategory, new Observer().WithName(CoreConstants.Compartment.URINE));
         addInfoToCategory(compCategory, new Observer().WithName(CoreConstants.Compartment.FECES));
      }

      private void addPredefinedOrganValues(MetaDataCategory organCategory)
      {
         var defaultIndividual = _defaultIndividualRetriever.DefaultIndividual();
         var organism = defaultIndividual.Organism;
         addUndefinedValueTo(organCategory);
         addInfoToCategory(organCategory, organism.Organ(CoreConstants.Organ.PERIPHERAL_VENOUS_BLOOD));
         foreach (var organ in organism.OrgansByType(OrganType.VascularSystem | OrganType.Tissue | OrganType.Lumen))
         {
            addInfoToCategory(organCategory, organ);
         }
      }

      private void addPredefinedSpeciesValues(MetaDataCategory speciesCategory)
      {
         foreach (var species in _speciesRepository.All().OrderBy(x => x.Name))
         {
            addInfoToCategory(speciesCategory, species);
         }
      }

      private static MetaDataCategory createMetaDataCategory<T>(string descriptiveName, bool isMandatory = false, bool isListOfValuesFixed = false, Action<MetaDataCategory> fixedValuesRetriever = null)
      {
         var category = new MetaDataCategory
         {
            Name = descriptiveName,
            DisplayName = descriptiveName,
            Description = descriptiveName,
            MetaDataType = typeof(T),
            IsMandatory = isMandatory,
            IsListOfValuesFixed = isListOfValuesFixed
         };

         fixedValuesRetriever?.Invoke(category);

         return category;
      }

      private MetaDataCategory compoundNameCategory(Compound compound, bool canEditName)
      {
         var nameCategory = new MetaDataCategory
         {
            Name = ObservedData.MOLECULE,
            DisplayName = PKSimConstants.UI.Molecule,
            Description = PKSimConstants.UI.MoleculeNameDescription,
            MetaDataType = typeof(string),
            IsListOfValuesFixed = !canEditName,
            IsMandatory = true,
         };

         foreach (var existingCompound in _buildingBlockRepository.All<Compound>())
         {
            nameCategory.ListOfValues.Add(existingCompound.Name, existingCompound.Name);
         }

         if (canEditName)
            nameCategory.ListOfValues.Add(PKSimConstants.UI.Undefined, PKSimConstants.UI.Undefined);

         if (compound != null)
            nameCategory.DefaultValue = compound.Name;

         return nameCategory;
      }

      private void addInfoToCategory(MetaDataCategory metaDataCategory, IObjectBase objectBase)
      {
         var info = _representationInfoRepository.InfoFor(objectBase);
         if (info == null)
         {
            metaDataCategory.ListOfValues.Add(objectBase.Name, objectBase.Name);
            return;
         }

         //only add with display name as information will be used in data repository as is
         metaDataCategory.ListOfValues.Add(info.DisplayName, info.DisplayName);
         var icon = ApplicationIcons.IconByName(info.IconName);
         if (icon != ApplicationIcons.EmptyIcon)
            metaDataCategory.ListOfImages.Add(info.DisplayName, icon.IconName);
      }
   }
}