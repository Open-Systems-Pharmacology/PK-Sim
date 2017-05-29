using System;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Services;
using OSPSuite.Utility;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.ParameterIdentifications;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Journal;
using OSPSuite.Core.Domain.Services.ParameterIdentifications;
using OSPSuite.Core.Domain.Services.SensitivityAnalyses;
using IContentLoader = OSPSuite.Core.Journal.IContentLoader;
using IObservedDataTask = PKSim.Core.Services.IObservedDataTask;
using ReloadRelatedItemTask = PKSim.Core.Services.ReloadRelatedItemTask;

namespace PKSim.Core
{
   public abstract class concern_for_ReloadRelatedItemTask : ContextSpecification<IReloadRelatedItemTask>
   {
      protected IDialogCreator _dialogCreator;
      protected IApplicationConfiguration _applicationConfiguration;
      protected IContentLoader _contentLoader;
      protected IMoBiExportTask _mobiExportTask;
      protected IRelatedItemSerializer _relatedItemSerializer;
      protected IObservedDataTask _observedDataTask;
      protected IBuildingBlockTask _buildingBlockTask;
      protected IObjectIdResetter _objectIdResetter;
      protected IWithIdRepository _withIdRepository;
      protected RelatedItem _relatedItem;
      private Func<string> _oldTempFile;
      protected string _tempFile;
      private string _fullPath;
      protected IParameterIdentificationTask _parameterIdentificationTask;
      protected ISensitivityAnalysisTask _sensitivityAnalysisTask;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _tempFile = FileHelper.GenerateTemporaryFileName();
         _fullPath = FileHelper.GenerateTemporaryFileName();
         _oldTempFile = FileHelper.GenerateTemporaryFileName;
         FileHelper.GenerateTemporaryFileName = () => _tempFile;
      }

      protected override void Context()
      {
         _dialogCreator = A.Fake<IDialogCreator>();
         _applicationConfiguration = A.Fake<IApplicationConfiguration>();
         _contentLoader = A.Fake<IContentLoader>();
         _mobiExportTask = A.Fake<IMoBiExportTask>();
         _relatedItemSerializer = A.Fake<IRelatedItemSerializer>();
         _observedDataTask = A.Fake<IObservedDataTask>();
         _buildingBlockTask = A.Fake<IBuildingBlockTask>();
         _objectIdResetter = A.Fake<IObjectIdResetter>();
         _withIdRepository = A.Fake<IWithIdRepository>();
         _parameterIdentificationTask = A.Fake<IParameterIdentificationTask>();
         _sensitivityAnalysisTask= A.Fake<ISensitivityAnalysisTask>();

         sut = new ReloadRelatedItemTask(_applicationConfiguration, _contentLoader, _dialogCreator,
            _mobiExportTask, _relatedItemSerializer, _observedDataTask, _buildingBlockTask, _objectIdResetter, _withIdRepository, _parameterIdentificationTask,_sensitivityAnalysisTask);

         _relatedItem = new RelatedItem {FullPath = _fullPath, Content = new Content {Data = new byte[] {10, 20}}};

         A.CallTo(() => _applicationConfiguration.Product).Returns(Origins.PKSim);
      }

      protected override void Because()
      {
         sut.Load(_relatedItem);
      }

      public override void GlobalCleanup()
      {
         base.GlobalCleanup();
         FileHelper.GenerateTemporaryFileName = _oldTempFile;
         FileHelper.DeleteFile(_tempFile);
      }
   }

   public class When_loading_a_related_item_created_by_mobi : concern_for_ReloadRelatedItemTask
   {
      protected override void Context()
      {
         base.Context();
         _relatedItem.Origin = Origins.MoBi;
      }

      [Observation]
      public void should_dump_the_content_to_file_and_launch_mobi_with_the_given_file()
      {
         A.CallTo(() => _mobiExportTask.StartWithContentFile(_tempFile)).MustHaveHappened();
      }
   }

   public class When_loading_a_related_item_that_was_imported_as_an_external_file : concern_for_ReloadRelatedItemTask
   {
      protected override void Context()
      {
         base.Context();
         _relatedItem.Origin = Origins.Other;
         A.CallTo(_dialogCreator).WithReturnType<string>().Returns(_tempFile);
      }

      [Observation]
      public void should_export_the_file_to_a_location_specified_by_the_user()
      {
         FileHelper.FileExists(_tempFile).ShouldBeTrue();
      }
   }

   public class When_loading_a_parameter_identification : concern_for_ReloadRelatedItemTask
   {
      private ParameterIdentification _relatedObject;

      protected override void Context()
      {
         base.Context();
         _relatedObject = A.Fake<ParameterIdentification>().WithId("XX");
         _relatedItem.Origin = Origins.PKSim;
         A.CallTo(() => _relatedItemSerializer.Deserialize(_relatedItem)).Returns(_relatedObject);
      }

      [Observation]
      public void the_parameter_identification_task_must_be_used_to_reload_the_identification()
      {
         A.CallTo(() => _parameterIdentificationTask.AddToProject(_relatedObject)).MustHaveHappened();  
      }
   }

   public class When_loading_a_related_item_containing_a_PKSim_building_block : concern_for_ReloadRelatedItemTask
   {
      private IPKSimBuildingBlock _relatedObject;

      protected override void Context()
      {
         base.Context();
         _relatedObject = A.Fake<IPKSimBuildingBlock>().WithId("XX");
         _relatedItem.Origin = Origins.PKSim;
         A.CallTo(() => _relatedItemSerializer.Deserialize(_relatedItem)).Returns(_relatedObject);
      }

      [Observation]
      public void should_add_the_building_block_to_the_project()
      {
         A.CallTo(() => _buildingBlockTask.AddToProject(_relatedObject, true, true)).MustHaveHappened();
      }
   }

   public class When_loading_a_related_item_containing_a_PKSim_building_block_that_already_exists_in_the_project : concern_for_ReloadRelatedItemTask
   {
      private IPKSimBuildingBlock _relatedObject;

      protected override void Context()
      {
         base.Context();
         _relatedObject = A.Fake<IPKSimBuildingBlock>().WithId("XX");
         _relatedItem.Origin = Origins.PKSim;
         A.CallTo(() => _withIdRepository.ContainsObjectWithId(_relatedObject.Id)).Returns(true);
         A.CallTo(() => _relatedItemSerializer.Deserialize(_relatedItem)).Returns(_relatedObject);
      }

      [Observation]
      public void should_reset_the_ids_in_the_loaded_building_block()
      {
         A.CallTo(() => _objectIdResetter.ResetIdFor(_relatedObject)).MustHaveHappened();
      }

      [Observation]
      public void should_add_the_building_block_to_the_project()
      {
         A.CallTo(() => _buildingBlockTask.AddToProject(_relatedObject, true, true)).MustHaveHappened();
      }
   }

   public class When_loading_a_related_item_containing_observed_data : concern_for_ReloadRelatedItemTask
   {
      private DataRepository _relatedObject;

      protected override void Context()
      {
         base.Context();
         _relatedObject = new DataRepository().WithId("XX");
         _relatedItem.Origin = Origins.PKSim;
         A.CallTo(() => _relatedItemSerializer.Deserialize(_relatedItem)).Returns(_relatedObject);
      }

      [Observation]
      public void should_add_the_observed_data_to_the_project()
      {
         A.CallTo(() => _observedDataTask.AddObservedDataToProject(_relatedObject)).MustHaveHappened();
      }
   }
}