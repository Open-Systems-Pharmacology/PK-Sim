using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services.ParameterIdentifications;
using OSPSuite.Core.Serialization.Xml;
using OSPSuite.Core.Services;
using OSPSuite.Infrastructure.Import.Core;
using OSPSuite.Infrastructure.Import.Services;
using OSPSuite.Utility.Events;
using PKSim.Core;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure.Services;
using IObservedDataTask = PKSim.Core.Services.IObservedDataTask;
using IContainer = OSPSuite.Utility.Container.IContainer;
using ImporterConfiguration = OSPSuite.Core.Import.ImporterConfiguration;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_ImportObservedDataTask : ContextSpecification<IImportObservedDataTask>
   {
      protected IParameterIdentificationTask _parameterIdentificationTask;
      protected ICoreWorkspace _coreWorkspace;

      protected IDataImporter _dataImporter;
      protected IDialogCreator _dialogCreator;
      protected IExecutionContext _executionContext;

      protected override void Context()
      {
         _dataImporter = A.Fake<IDataImporter>();
         _executionContext = A.Fake<IExecutionContext>();
         var buildingBlockRepository = A.Fake<IBuildingBlockRepository>();
         var speciesRepository = A.Fake<ISpeciesRepository>();
         var defaultIndividualRetriever = A.Fake<IDefaultIndividualRetriever>();
         var representationInfoRepository = A.Fake<IRepresentationInfoRepository>();
         var observedDataTask = A.Fake<IObservedDataTask>();
         var parameterChangeUpdater = A.Fake<IParameterChangeUpdater>();
         _dialogCreator = A.Fake<IDialogCreator>();
         var container = A.Fake<IContainer>();
         var modelingXmlSerializerRepository = A.Fake<IOSPSuiteXmlSerializerRepository>();
         var eventPublisher = A.Fake<IEventPublisher>();
         _coreWorkspace = A.Fake<ICoreWorkspace>();
         _parameterIdentificationTask = A.Fake<IParameterIdentificationTask>();
         A.CallTo(() => _dialogCreator.AskForFileToOpen(
               A<string>._,
               A<string>._,
               A<string>._,
               A<string>._,
               A<string>._
            ))
            .Returns("dummy.csv");


         A.CallTo(() => _dataImporter.ImportFromConfiguration(
               A<ImporterConfiguration>._,
               A<IReadOnlyList<MetaDataCategory>>._,
               A<IReadOnlyList<ColumnInfo>>._,
               A<DataImporterSettings>._,
               A<string>._))
            .Returns(new List<DataRepository>()); 

         sut = new ImportObservedDataTask(
            _dataImporter, _executionContext, buildingBlockRepository, speciesRepository,
            defaultIndividualRetriever, representationInfoRepository, observedDataTask,
            parameterChangeUpdater, _dialogCreator, container, modelingXmlSerializerRepository,
            eventPublisher, _parameterIdentificationTask, _coreWorkspace);
      }
   }

   public class When_adding_and_replacing_observed_data_from_configuration_to_project_and_the_configuration_is_not_found
   : concern_for_ImportObservedDataTask
   {
      private IReadOnlyList<DataRepository> _observedDataFromSameFile;
      private readonly string _configurationId = "configurationId";

      protected override void Context()
      {
         base.Context();
         _observedDataFromSameFile = new List<DataRepository>();
         A.CallTo(() => _executionContext.Project.ImporterConfigurationBy(_configurationId)).Returns(null);
      }

      protected override void Because()
      {
         sut.AddAndReplaceObservedDataFromConfigurationToProject(_configurationId, _observedDataFromSameFile);
      }

      [Observation]
      public void the_dialog_creator_indicates_error()
      {
         A.CallTo(() => _dialogCreator.MessageBoxError(Assets.PKSimConstants.Error.ImporterConfigurationNotFoundInProject)).MustHaveHappened();
      }

      [Observation]
      public void should_set_has_not_changed_to_true()
      {
         _coreWorkspace.Project.HasChanged.ShouldBeEqualTo(false);
      }
   }

   public class When_adding_and_replacing_observed_data_from_configuration_to_project : concern_for_ImportObservedDataTask
   {
      private IReadOnlyList<DataRepository> _observedDataFromSameFile;
      private readonly string _configurationId = "configurationId";

      protected override void Context()
      {
         base.Context();
         var overwrittenDataSet = A.Fake<DataRepository>();
         var fakeCol = A.Fake<DataColumn>();

         A.CallTo(() => overwrittenDataSet.Columns).Returns(new List<DataColumn> { fakeCol });

         var existingDataSet = A.Fake<DataRepository>();

         var existingBaseGrid = A.Fake<BaseGrid>();

         A.CallTo(() => existingBaseGrid.Values).Returns(new List<float> { 0 });

         A.CallTo(() => existingDataSet.BaseGrid).Returns(existingBaseGrid);

         A.CallTo(() => _dataImporter.AreFromSameMetaDataCombination(existingDataSet, overwrittenDataSet))
            .Returns(true);

         _observedDataFromSameFile = new List<DataRepository> { existingDataSet };

         A.CallTo(() => _dataImporter.CalculateReloadDataSetsFromConfiguration(
               A<IReadOnlyList<DataRepository>>._,
               A<IReadOnlyList<DataRepository>>._))
            .Returns(new ReloadDataSets(
               new List<DataRepository>(),
               new List<DataRepository> { overwrittenDataSet },
               new List<DataRepository>()
            ));

         A.CallTo(() => _dialogCreator.AskForFileToOpen(
               A<string>._, A<string>._, A<string>._, A<string>._, A<string>._))
            .Returns("dummy.csv");

         A.CallTo(() => _executionContext.Project.ImporterConfigurationBy(_configurationId)).Returns(A.Fake<ImporterConfiguration>());
      }

      protected override void Because()
      {
         sut.AddAndReplaceObservedDataFromConfigurationToProject(_configurationId, _observedDataFromSameFile);
      }

      [Observation]
      public void should_call_update_parameter_identification_Using()
      {
         A.CallTo(() => _parameterIdentificationTask.UpdateParameterIdentificationsUsing(_observedDataFromSameFile))
            .MustHaveHappened();
      }

      [Observation]
      public void should_set_has_changed_to_true()
      {
         _coreWorkspace.Project.HasChanged.ShouldBeEqualTo(true);
      }
   }

   public abstract class concern_for_reloading_data_whose_error_type_changed : concern_for_ImportObservedDataTask
   {
      protected const string ERROR = "Error";
      protected const string MASS_CONCENTRATION_UNIT = "g/l";
      protected const float RELOADED_ERROR_VALUE = 2.0f;
      protected const double MOL_WEIGHT = 300;
      private const string CONFIGURATION_ID = "configurationId";
      protected DataRepository _existingDataSet;
      protected DataColumn _existingErrorColumn;
      private IReadOnlyList<DataRepository> _observedDataFromSameFile;

      protected abstract AuxiliaryType existingErrorType { get; }
      protected abstract AuxiliaryType reloadedErrorType { get; }

      protected override void Context()
      {
         base.Context();
         _existingDataSet = new DataRepository("existing");
         _existingErrorColumn = addMeasurementWithError(_existingDataSet, existingErrorType, 1.0f);
         _existingErrorColumn.DataInfo.MolWeight = MOL_WEIGHT;
         _observedDataFromSameFile = new List<DataRepository> { _existingDataSet };

         var reloadedDataSet = new DataRepository("reloaded");
         addMeasurementWithError(reloadedDataSet, reloadedErrorType, RELOADED_ERROR_VALUE);

         A.CallTo(() => _executionContext.Project.ImporterConfigurationBy(CONFIGURATION_ID)).Returns(A.Fake<ImporterConfiguration>());
         A.CallTo(() => _dataImporter.AreFromSameMetaDataCombination(_existingDataSet, reloadedDataSet)).Returns(true);
         A.CallTo(() => _dataImporter.CalculateReloadDataSetsFromConfiguration(A<IReadOnlyList<DataRepository>>._, A<IReadOnlyList<DataRepository>>._))
            .Returns(new ReloadDataSets(new List<DataRepository>(), new List<DataRepository> { reloadedDataSet }, new List<DataRepository>()));
      }

      private static DataColumn addMeasurementWithError(DataRepository dataRepository, AuxiliaryType errorType, float errorValue)
      {
         var baseGrid = new BaseGrid("Time", DomainHelperForSpecs.TimeDimensionForSpecs()) { Values = new[] { 0.0f } };
         var measurement = new DataColumn("Measurement", DomainHelperForSpecs.MassConcentrationDimensionForSpecs(), baseGrid) { Values = new[] { 10.0f } };
         measurement.DataInfo.DisplayUnitName = MASS_CONCENTRATION_UNIT;

         var isGeometric = errorType == AuxiliaryType.GeometricStdDev;
         var errorDimension = isGeometric ? DomainHelperForSpecs.NoDimension() : DomainHelperForSpecs.MassConcentrationDimensionForSpecs();
         var error = new DataColumn(ERROR, errorDimension, baseGrid) { Values = new[] { errorValue } };
         error.DataInfo.AuxiliaryType = errorType;
         error.DataInfo.DisplayUnitName = isGeometric ? errorDimension.DefaultUnitName : MASS_CONCENTRATION_UNIT;
         measurement.AddRelatedColumn(error);

         dataRepository.Add(baseGrid);
         dataRepository.Add(measurement);
         dataRepository.Add(error);
         return error;
      }

      protected override void Because()
      {
         sut.AddAndReplaceObservedDataFromConfigurationToProject(CONFIGURATION_ID, _observedDataFromSameFile);
      }

      [Observation]
      public void should_keep_the_existing_error_column()
      {
         _existingDataSet.AllButBaseGrid().Single(x => x.Name == ERROR).ShouldBeEqualTo(_existingErrorColumn);
      }

      [Observation]
      public void should_update_the_error_values()
      {
         _existingErrorColumn.Values.ShouldOnlyContain(RELOADED_ERROR_VALUE);
      }

      [Observation]
      public void should_keep_the_molecular_weight_of_the_error_column()
      {
         _existingErrorColumn.DataInfo.MolWeight.ShouldBeEqualTo(MOL_WEIGHT);
      }
   }

   public class When_reloading_data_whose_error_type_changed_from_arithmetic_to_geometric : concern_for_reloading_data_whose_error_type_changed
   {
      protected override AuxiliaryType existingErrorType => AuxiliaryType.ArithmeticStdDev;
      protected override AuxiliaryType reloadedErrorType => AuxiliaryType.GeometricStdDev;

      [Observation]
      public void the_error_column_should_be_a_dimensionless_geometric_standard_deviation()
      {
         _existingErrorColumn.DataInfo.AuxiliaryType.ShouldBeEqualTo(AuxiliaryType.GeometricStdDev);
         _existingErrorColumn.Dimension.ShouldBeEqualTo(DomainHelperForSpecs.NoDimension());
         _existingErrorColumn.DataInfo.DisplayUnitName.ShouldBeEqualTo(DomainHelperForSpecs.NoDimension().DefaultUnitName);
      }
   }

   public class When_reloading_data_whose_error_type_changed_from_geometric_to_arithmetic : concern_for_reloading_data_whose_error_type_changed
   {
      protected override AuxiliaryType existingErrorType => AuxiliaryType.GeometricStdDev;
      protected override AuxiliaryType reloadedErrorType => AuxiliaryType.ArithmeticStdDev;

      [Observation]
      public void the_error_column_should_be_an_arithmetic_standard_deviation_in_the_dimension_of_the_measurement()
      {
         _existingErrorColumn.DataInfo.AuxiliaryType.ShouldBeEqualTo(AuxiliaryType.ArithmeticStdDev);
         _existingErrorColumn.Dimension.ShouldBeEqualTo(DomainHelperForSpecs.MassConcentrationDimensionForSpecs());
         _existingErrorColumn.DataInfo.DisplayUnitName.ShouldBeEqualTo(MASS_CONCENTRATION_UNIT);
      }
   }
}