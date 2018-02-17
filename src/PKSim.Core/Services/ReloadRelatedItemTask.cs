using System;
using OSPSuite.Utility.Visitor;
using PKSim.Core.Model;
using OSPSuite.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.ParameterIdentifications;
using OSPSuite.Core.Domain.SensitivityAnalyses;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Journal;
using OSPSuite.Core.Services;
using OSPSuite.Core.Domain.Services.ParameterIdentifications;
using OSPSuite.Core.Domain.Services.SensitivityAnalyses;
using OSPSuite.Utility.Exceptions;
using PKSim.Assets;

namespace PKSim.Core.Services
{
   public class ReloadRelatedItemTask : OSPSuite.Core.Journal.ReloadRelatedItemTask,
      IVisitor<DataRepository>,
      IVisitor<IPKSimBuildingBlock>,
      IVisitor<ParameterIdentification>,
      IVisitor<SensitivityAnalysis>
   {
      private readonly IMoBiExportTask _moBiExportTask;
      private readonly IRelatedItemSerializer _relatedItemSerializer;
      private readonly IObservedDataTask _observedDataTask;
      private readonly IBuildingBlockTask _buildingBlockTask;
      private readonly IObjectIdResetter _objectIdResetter;
      private readonly IWithIdRepository _withIdRepository;
      private readonly IParameterIdentificationTask _parameterIdentificationTask;
      private readonly ISensitivityAnalysisTask _sensitivityAnalysisTask;

      public ReloadRelatedItemTask(IApplicationConfiguration applicationConfiguration, OSPSuite.Core.Journal.IContentLoader contentLoader, IDialogCreator dialogCreator,
         IMoBiExportTask moBiExportTask, IRelatedItemSerializer relatedItemSerializer, IObservedDataTask observedDataTask,
         IBuildingBlockTask buildingBlockTask, IObjectIdResetter objectIdResetter, IWithIdRepository withIdRepository, 
         IParameterIdentificationTask parameterIdentificationTask, ISensitivityAnalysisTask sensitivityAnalysisTask) :
            base(applicationConfiguration, contentLoader, dialogCreator)
      {
         _moBiExportTask = moBiExportTask;
         _relatedItemSerializer = relatedItemSerializer;
         _observedDataTask = observedDataTask;
         _buildingBlockTask = buildingBlockTask;
         _objectIdResetter = objectIdResetter;
         _withIdRepository = withIdRepository;
         _parameterIdentificationTask = parameterIdentificationTask;
         _sensitivityAnalysisTask = sensitivityAnalysisTask;
      }

      protected override void StartSisterApplicationWithContentFile(string contentFile)
      {
         _moBiExportTask.StartWithContentFile(contentFile);
      }

      protected override void LoadOwnContent(RelatedItem relatedItem)
      {
         try
         {
            var relatedItemObject = _relatedItemSerializer.Deserialize(relatedItem);
            resetObjectIdIfRequired(relatedItemObject);
            this.Visit(relatedItemObject);
         }
         catch (NotUniqueIdException)
         {
            //Probably trying to load an object that was already loaded. Show a message to the user
            throw new OSPSuiteException(PKSimConstants.Error.CannotLoadRelatedItemAsObjectAlreadyExistInProject(relatedItem.ItemType, relatedItem.Name));

         }
      }

      protected override bool RelatedItemCanBeLaunchedBySisterApplication(RelatedItem relatedItem)
      {
         return relatedItem.Origin == Origins.MoBi;
      }

      public void Visit(DataRepository dataRepository)
      {
         _observedDataTask.AddObservedDataToProject(dataRepository);
      }

      public void Visit(IPKSimBuildingBlock buildingBlock)
      {
         _buildingBlockTask.AddToProject(buildingBlock, editBuildingBlock: true);
      }

      private void resetObjectIdIfRequired(IWithId withId)
      {
         if (_withIdRepository.ContainsObjectWithId(withId.Id))
            _objectIdResetter.ResetIdFor(withId);
      }

      public void Visit(ParameterIdentification parameterIdentification)
      {
         _parameterIdentificationTask.AddToProject(parameterIdentification);
      }

      public void Visit(SensitivityAnalysis sensitivityAnalysis)
      {
         _sensitivityAnalysisTask.AddToProject(sensitivityAnalysis);
      }
   }
}