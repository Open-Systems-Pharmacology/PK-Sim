﻿using System;
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
using IContainer = OSPSuite.Utility.Container.IContainer;
using IObservedDataTask = PKSim.Core.Services.IObservedDataTask;
using ImporterConfiguration = OSPSuite.Core.Import.ImporterConfiguration;
using OSPSuite.Core.Events;
using OSPSuite.Utility.Events;

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
      private readonly IEventPublisher _eventPublisher;

      public ImportObservedDataTask(IDataImporter dataImporter, IExecutionContext executionContext,
         IDimensionRepository dimensionRepository, IBuildingBlockRepository buildingBlockRepository, ISpeciesRepository speciesRepository,
         IDefaultIndividualRetriever defaultIndividualRetriever, IRepresentationInfoRepository representationInfoRepository,
         IObservedDataTask observedDataTask, IParameterChangeUpdater parameterChangeUpdater, IDialogCreator dialogCreator, IContainer container,
         IOSPSuiteXmlSerializerRepository modelingXmlSerializerRepository, IEventPublisher eventPublisher)
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
         _eventPublisher = eventPublisher;
      }

      public void AddObservedDataToProject() => AddObservedDataToProjectForCompound(null);

      public void AddObservedDataFromConfigurationToProject(ImporterConfiguration configuration) => AddObservedDataFromConfigurationToProjectForCompound(null, configuration, null);

      public void AddObservedDataFromConfigurationToProjectForDataRepository(ImporterConfiguration configuration, string dataRepositoryName) => AddObservedDataFromConfigurationToProjectForCompound(null, configuration, dataRepositoryName);

      public ImporterConfiguration OpenXmlConfiguration()
      {
         using (var serializationContext = SerializationTransaction.Create(_container))
         {
            var serializer = _modelingXmlSerializerRepository.SerializerFor<ImporterConfiguration>();

            var fileName = _dialogCreator.AskForFileToOpen("Open configuration", "xml files (*.xml)|*.xml|All files (*.*)|*.*", //move to constants and use filter
               Constants.DirectoryKey.PROJECT);

            if (fileName.IsNullOrEmpty()) return null;

            var xel = XElement.Load(fileName); // We have to correctly handle the case of cancellation
            return serializer.Deserialize<ImporterConfiguration>(xel, serializationContext);
         }
      }

      public void AddAndReplaceObservedDataFromConfigurationToProject(ImporterConfiguration configuration, IEnumerable<DataRepository> observedDataFromSameFile)
      {
         var importedObservedData = getObservedDataFromImporter(configuration, columnInfoConfiguration, null, false, false);
         var reloadDataSets = _dataImporter.CalculateReloadDataSetsFromConfiguration(importedObservedData.ToList(), observedDataFromSameFile.ToList());

         if (reloadDataSets == null) return;

         foreach (var dataSet in reloadDataSets.NewDataSets)
         {
            adjustMolWeight(dataSet);
            _observedDataTask.AddObservedDataToProject(dataSet);
            updateQuantityInfoInImportedColumns(dataSet);
         }

         foreach (var dataSet in reloadDataSets.DataSetsToBeDeleted.ToArray())//toDo it should be checked if to array solves the deleting problem
         {
            _observedDataTask.Delete(dataSet);
         }

         foreach (var dataSet in reloadDataSets.OverwrittenDataSets)
         {
            //TODO this here should be tested
            var existingDataSet = findDataRepositoryInList(observedDataFromSameFile, dataSet);

            foreach (var column in dataSet.Columns)
            {
               var datacolumn = new DataColumn(column.Id, column.Name, column.Dimension, column.BaseGrid)
               {
                  QuantityInfo = column.QuantityInfo,
                  DataInfo = column.DataInfo,
                  IsInternal = column.IsInternal,
                  Values = column.Values
               };

               if (column.IsBaseGrid())
               {
                  existingDataSet.BaseGrid.Values = datacolumn.Values;
               }
               else
               {
                  var existingColumn = existingDataSet.FirstOrDefault(x => x.Name == column.Name);
                  if (existingColumn == null)
                     existingDataSet.Add(column);
                  else
                     existingColumn.Values = column.Values;
               }
            }
            _eventPublisher.PublishEvent(new ObservedDataValueChangedEvent(existingDataSet));
         }
      }

      public void AddObservedDataToProjectForCompound(Compound compound)
      {
         _executionContext.Load(compound);
         addObservedData(columnInfoConfiguration, compound);
      }

      public void AddObservedDataFromConfigurationToProjectForCompound(Compound compound, ImporterConfiguration configuration)
      {
         _executionContext.Load(compound);
         AddObservedDataFromConfiguration(configuration, columnInfoConfiguration, compound, null, false, true);
      }

      private void AddObservedDataFromConfigurationToProjectForCompound(Compound compound, ImporterConfiguration configuration, string dataRepositoryName)
      {
         _executionContext.Load(compound);
         AddObservedDataFromConfiguration(configuration, columnInfoConfiguration, compound, dataRepositoryName);
      }

      private void AddObservedDataFromConfiguration(ImporterConfiguration configuration, Func<IReadOnlyList<ColumnInfo>> importConfiguration, Compound compound = null, string dataRepositoryName = null, bool allowCompoundNameEdit = false, bool propmtUser = false)
      {
         var importedObservedData = getObservedDataFromImporter(configuration, importConfiguration, compound, propmtUser, allowCompoundNameEdit);
         if (importedObservedData == null) return;

         foreach (var observedData in string.IsNullOrEmpty(dataRepositoryName) ? importedObservedData : importedObservedData.Where(r => r.Name == dataRepositoryName))
         {
            adjustMolWeight(observedData);
            _observedDataTask.AddObservedDataToProject(observedData);
            updateQuantityInfoInImportedColumns(observedData);
         }
      }

      private DataRepository findDataRepositoryInList(IEnumerable<DataRepository> dataRepositoryList, DataRepository targetDataRepository)
      {
         return (from dataRepo in dataRepositoryList let result = _dataImporter.AreFromSameMetaDataCombination(dataRepo, targetDataRepository) where result select dataRepo).FirstOrDefault();
      }

      private void addNamingPatterns(DataImporterSettings dataImporterSettings)
      {
         dataImporterSettings.AddNamingPatternMetaData(
            Constants.FILE
         );

         dataImporterSettings.AddNamingPatternMetaData(
            Constants.FILE,
            Constants.SHEET
         );

         dataImporterSettings.AddNamingPatternMetaData(
            Constants.ObservedData.MOLECULE,
            Constants.ObservedData.SPECIES,
            Constants.ObservedData.ORGAN,
            Constants.ObservedData.COMPARTMENT
         );

         dataImporterSettings.AddNamingPatternMetaData(
            Constants.ObservedData.MOLECULE,
            Constants.ObservedData.SPECIES,
            Constants.ObservedData.ORGAN,
            Constants.ObservedData.COMPARTMENT,
            Constants.ObservedData.STUDY_ID,
            Constants.ObservedData.GENDER,
            Constants.ObservedData.DOSE,
            Constants.ObservedData.ROUTE,
            Constants.ObservedData.SUBJECT_ID
         );
      }

      private IEnumerable<DataRepository> getObservedDataFromImporter(ImporterConfiguration configuration, Func<IReadOnlyList<ColumnInfo>> importConfiguration, Compound compound, bool propmtUser,
         bool allowCompoundNameEdit)
      {
         var metaDataCategories = _dataImporter.DefaultMetaDataCategories();
         populateMetaDataLists(metaDataCategories, compound, allowCompoundNameEdit);

         var dataImporterSettings = new DataImporterSettings
         { Caption = $"{CoreConstants.ProductDisplayName} - {PKSimConstants.UI.ImportObservedData}", IconName = ApplicationIcons.ObservedData.IconName };
         addNamingPatterns(dataImporterSettings);
         dataImporterSettings.NameOfMetaDataHoldingMoleculeInformation = Constants.ObservedData.MOLECULE;
         dataImporterSettings.NameOfMetaDataHoldingMolecularWeightInformation = Constants.ObservedData.MOLECULARWEIGHT;
         dataImporterSettings.PromptForConfirmation = propmtUser;

         var importedObservedData = _dataImporter.ImportFromConfiguration(
            configuration, 
            (IReadOnlyList<MetaDataCategory>)metaDataCategories, 
            importConfiguration(), 
            dataImporterSettings,
            _dialogCreator.AskForFileToOpen(Captions.Importer.OpenFile, Captions.Importer.ImportFileFilter, Constants.DirectoryKey.OBSERVED_DATA)
         );
         return importedObservedData;
      }

      private void addObservedData(Func<IReadOnlyList<ColumnInfo>> importConfiguration, Compound compound = null, bool allowCompoundNameEdit = false)
      {
         var dataImporterSettings = new DataImporterSettings { Caption = $"{CoreConstants.ProductDisplayName} - {PKSimConstants.UI.ImportObservedData}", IconName = ApplicationIcons.ObservedData.IconName };
         addNamingPatterns(dataImporterSettings);
         dataImporterSettings.NameOfMetaDataHoldingMoleculeInformation = Constants.ObservedData.MOLECULE;
         dataImporterSettings.NameOfMetaDataHoldingMolecularWeightInformation = Constants.ObservedData.MOLECULARWEIGHT;

         var metaDataCategories = _dataImporter.DefaultMetaDataCategories();
         populateMetaDataLists(metaDataCategories, compound, allowCompoundNameEdit);

         var importedObservedData = _dataImporter.ImportDataSets(
            (IReadOnlyList<MetaDataCategory>)metaDataCategories, 
            importConfiguration(), 
            dataImporterSettings,
            _dialogCreator.AskForFileToOpen(Captions.Importer.OpenFile, Captions.Importer.ImportFileFilter, Constants.DirectoryKey.OBSERVED_DATA)
         );

         if (importedObservedData.DataRepositories == null || importedObservedData.Configuration == null) return;

         _observedDataTask.AddImporterConfigurationToProject(importedObservedData.Configuration);
         foreach (var observedData in importedObservedData.DataRepositories)
         {
            adjustMolWeight(observedData);
            _observedDataTask.AddObservedDataToProject(observedData);
            updateQuantityInfoInImportedColumns(observedData);
         }
      }

      private void updateQuantityInfoInImportedColumns(DataRepository observedData)
      {
         var moleculeName = observedData.ExtendedPropertyValueFor(Constants.ObservedData.MOLECULE);
         var organ = observedData.ExtendedPropertyValueFor(Constants.ObservedData.ORGAN);
         var compartment = observedData.ExtendedPropertyValueFor(Constants.ObservedData.COMPARTMENT);

         foreach (var col in observedData.AllButBaseGrid())
         {
            //needs to match the path in simulation ROOT\ORGANISM\ORGAN\COMPARTMENT\MOLECULE\Name
            var colName = col.Name.Replace(ObjectPath.PATH_DELIMITER, "\\");
            col.QuantityInfo = new QuantityInfo(col.Name, new[] { observedData.Name, CoreConstants.ContainerName.ObservedData, organ, compartment, moleculeName, colName }, QuantityType.Undefined);
         }

         var baseGrid = observedData.BaseGrid;
         var baseGridName = baseGrid.Name.Replace(ObjectPath.PATH_DELIMITER, "\\");
         baseGrid.QuantityInfo = new QuantityInfo(baseGrid.Name, new[] { observedData.Name, baseGridName }, QuantityType.Time);
      }

      private void adjustMolWeight(DataRepository observedData)
      {
         _parameterChangeUpdater.UpdateMolWeightIn(observedData);
      }


      private IReadOnlyList<ColumnInfo> columnInfoConfiguration()
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
            DefaultDimension = supportedDimensions[0],
            IsMandatory = true,
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
            IsMandatory = false,
            BaseGridName = mainColumnInfo.BaseGridName,
            RelatedColumnOf = mainColumnInfo.Name
         };

         addSupportedDimensionsTo(errorInfo, supportedDimensions);

         return errorInfo;
      }

      private void addSupportedDimensionsTo(ColumnInfo column, IEnumerable<IDimension> supportedDimensions)
      {
         supportedDimensions.Each(column.SupportedDimensions.Add);
      }

      private ColumnInfo createTimeColumn()
      {
         var timeColumn = new ColumnInfo
         {
            DefaultDimension = _dimensionRepository.Time,
            Name = PKSimConstants.UI.Time,
            IsMandatory = true,
         };

         timeColumn.SupportedDimensions.Add(_dimensionRepository.Time);
         return timeColumn;
      }

      public IEnumerable<string> PredefinedValuesFor(string name)
      {
         if (string.Equals(name, Constants.ObservedData.ORGAN))
            return predefinedOrgans;

         if (string.Equals(name, Constants.ObservedData.COMPARTMENT))
            return predefinedCompartments;

         if (string.Equals(name, Constants.ObservedData.SPECIES))
            return predefinedSpecies;

         if (string.Equals(name, Constants.ObservedData.GENDER))
            return predefinedGenders;

         if (string.Equals(name, Constants.ObservedData.MOLECULE))
            return predefinedMolecules;

         return Enumerable.Empty<string>();
      }

      public IReadOnlyList<string> DefaultMetaDataCategories => CoreConstants.ObservedData.DefaultProperties;

      public IReadOnlyList<string> ReadOnlyMetaDataCategories => new List<string> { };

      public bool MolWeightEditable => false;

      public bool MolWeightVisible => true;

      private IEnumerable<string> predefinedGenders => predefinedValuesFor(addPredefinedGenderValues);

      private IEnumerable<string> predefinedSpecies => predefinedValuesFor(addPredefinedSpeciesValues);

      private IEnumerable<string> predefinedCompartments => predefinedValuesFor(addPredefinedCompartmentValues);

      private IEnumerable<string> predefinedOrgans => predefinedValuesFor(addPredefinedOrganValues);

      private IEnumerable<string> predefinedMolecules => predefinedValuesFor(addPredefinedMoleculeValues);

      private IEnumerable<string> predefinedValuesFor(Action<MetaDataCategory> predefinedValuesRetriever)
      {
         var category = new MetaDataCategory();
         predefinedValuesRetriever(category);
         return category.ListOfValues.Values;
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

      private void addPredefinedMoleculeValues(MetaDataCategory moleculeCategory)
      {
         addUndefinedValueTo(moleculeCategory);

         foreach (var existingCompound in _buildingBlockRepository.All<Compound>())
         {
            addInfoToCategory(moleculeCategory, existingCompound);
         }
      }

      private void addPredefinedSpeciesValues(MetaDataCategory speciesCategory)
      {
         foreach (var species in _speciesRepository.All().OrderBy(x => x.Name))
         {
            addInfoToCategory(speciesCategory, species);
         }
      }

      private void populateMetaDataLists(IList<MetaDataCategory> metaDataCategories, Compound compound, bool allowCompoundNameEdit)
      {
         compoundNameCategory(metaDataCategories.FirstOrDefault(md => md.Name == Constants.ObservedData.MOLECULE), compound, allowCompoundNameEdit);
         organNameCategory(metaDataCategories.FirstOrDefault(md => md.Name == Constants.ObservedData.ORGAN));
         genderNameCategory(metaDataCategories.FirstOrDefault(md => md.Name == Constants.ObservedData.GENDER));
         compartmentNameCategory(metaDataCategories.FirstOrDefault(md => md.Name == Constants.ObservedData.COMPARTMENT));
         speciesNameCategory(metaDataCategories.FirstOrDefault(md => md.Name == Constants.ObservedData.SPECIES));
      }

      private void compoundNameCategory(MetaDataCategory nameCategory, Compound compound, bool canEditName)
      {
         if (nameCategory == null) return;
         nameCategory.IsListOfValuesFixed = !canEditName;
         nameCategory.ListOfValues.Clear();

         foreach (var existingCompound in _buildingBlockRepository.All<Compound>())
         {
            nameCategory.ListOfValues.Add(existingCompound.Name, existingCompound.MolWeight.ToString());
         }

         if (canEditName)
            nameCategory.ListOfValues.Add(PKSimConstants.UI.Undefined, PKSimConstants.UI.Undefined);

         if (compound != null)
         {
            nameCategory.DefaultValue = compound.Name;
            nameCategory.SelectDefaultValue = true;
         }

         nameCategory.ShouldListOfValuesBeIncluded = true;
      }

      private void organNameCategory(MetaDataCategory nameCategory)
      {
         if (nameCategory == null) return;
         nameCategory.ListOfValues.Clear();

         var defaultIndividual = _defaultIndividualRetriever.DefaultIndividual();
         var organism = defaultIndividual.Organism;
         addInfoToCategory(nameCategory, organism.Organ(CoreConstants.Organ.PERIPHERAL_VENOUS_BLOOD));
         foreach (var organ in organism.OrgansByType(OrganType.VascularSystem | OrganType.Tissue | OrganType.Lumen))
         {
            nameCategory.ListOfValues.Add(organ.Name, organ.Name);
         }

         nameCategory.ShouldListOfValuesBeIncluded = true;
      }

      private void genderNameCategory(MetaDataCategory nameCategory)
      {
         if (nameCategory == null) return;
         nameCategory.ListOfValues.Clear();

         var defaultIndividual = _defaultIndividualRetriever.DefaultIndividual();
         foreach (var gender in defaultIndividual.AvailableGenders())
         {
            nameCategory.ListOfValues.Add(gender.Name, gender.Name);
         }

         nameCategory.ShouldListOfValuesBeIncluded = true;
      }

      private void speciesNameCategory(MetaDataCategory nameCategory)
      {
         if (nameCategory == null) return;
         nameCategory.ListOfValues.Clear();

         foreach (var species in _speciesRepository.All().OrderBy(x => x.Name))
         {
            nameCategory.ListOfValues.Add(species.Name, species.Name);
         }

         nameCategory.ShouldListOfValuesBeIncluded = true;
      }

      private void compartmentNameCategory(MetaDataCategory nameCategory)
      {
         if (nameCategory == null) return;
         nameCategory.ListOfValues.Clear();

         var defaultIndividual = _defaultIndividualRetriever.DefaultIndividual();

         foreach (var compartment in defaultIndividual.Organism.Organ(CoreConstants.Organ.MUSCLE).Compartments.Where(x => x.Visible))
         {
            nameCategory.ListOfValues.Add(compartment.Name, compartment.Name);
         }
         nameCategory.ListOfValues.Add(CoreConstants.Observer.TISSUE, CoreConstants.Observer.TISSUE);
         nameCategory.ListOfValues.Add(CoreConstants.Observer.INTERSTITIAL_UNBOUND, CoreConstants.Observer.INTERSTITIAL_UNBOUND);
         nameCategory.ListOfValues.Add(CoreConstants.Observer.INTRACELLULAR_UNBOUND, CoreConstants.Observer.INTRACELLULAR_UNBOUND);
         nameCategory.ListOfValues.Add(CoreConstants.Observer.WHOLE_BLOOD, CoreConstants.Observer.WHOLE_BLOOD);
         nameCategory.ListOfValues.Add(CoreConstants.Compartment.URINE, CoreConstants.Compartment.URINE);
         nameCategory.ListOfValues.Add(CoreConstants.Compartment.FECES, CoreConstants.Compartment.FECES);

         nameCategory.ShouldListOfValuesBeIncluded = true;
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