using System.Collections.Generic;
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

      protected override void Context()
      {
         _dataImporter = A.Fake<IDataImporter>();
         var executionContext = A.Fake<IExecutionContext>();
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
            _dataImporter, executionContext, buildingBlockRepository, speciesRepository,
            defaultIndividualRetriever, representationInfoRepository, observedDataTask,
            parameterChangeUpdater, _dialogCreator, container, modelingXmlSerializerRepository,
            eventPublisher, _parameterIdentificationTask, _coreWorkspace);
      }
   }

   public class When_adding_and_replacing_observed_data_from_configuration_to_project
      : concern_for_ImportObservedDataTask
   {
      private IReadOnlyList<DataRepository> _observedDataFromSameFile;

      protected override void Because()
      {
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

         sut.AddAndReplaceObservedDataFromConfigurationToProject(
            A.Fake<ImporterConfiguration>(), _observedDataFromSameFile);
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
}