using System;
using System.Collections.Generic;
using System.Linq;
using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Services;
using OSPSuite.Utility;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Commands;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters;
using PKSim.Core.Snapshots.Services;
using PKSim.Presentation.Presenters.Snapshots;
using ILazyLoadTask = PKSim.Core.Services.ILazyLoadTask;

namespace PKSim.Presentation.Services
{
   public class BuildingBlockTask : IBuildingBlockTask
   {
      private readonly IApplicationController _applicationController;
      private readonly IDialogCreator _dialogCreator;
      private readonly IEntityTask _entityTask;
      private readonly IExecutionContext _executionContext;
      private readonly ITemplateTaskQuery _templateTaskQuery;
      private readonly ISingleStartPresenterTask _singleStartPresenterTask;
      private readonly IBuildingBlockRepository _buildingBlockRepository;
      private readonly ILazyLoadTask _lazyLoadTask;
      private readonly IPresentationSettingsTask _presentationSettingsTask;
      private readonly IBuildingBlockInSimulationManager _buildingBlockInSimulationManager;
      private readonly ISimulationReferenceUpdater _simulationReferenceUpdater;
      private readonly ISnapshotTask _snapshotTask;

      public BuildingBlockTask(IExecutionContext executionContext, 
         IApplicationController applicationController,
         IDialogCreator dialogCreator, 
         IBuildingBlockInSimulationManager buildingBlockInSimulationManager,
         IEntityTask entityTask, 
         ITemplateTaskQuery templateTaskQuery,
         ISingleStartPresenterTask singleStartPresenterTask, 
         IBuildingBlockRepository buildingBlockRepository,
         ILazyLoadTask lazyLoadTask, 
         IPresentationSettingsTask presentationSettingsTask, 
         ISimulationReferenceUpdater simulationReferenceUpdater,
         ISnapshotTask snapshotTask)
      {
         _executionContext = executionContext;
         _applicationController = applicationController;
         _dialogCreator = dialogCreator;
         _buildingBlockInSimulationManager = buildingBlockInSimulationManager;
         _entityTask = entityTask;
         _templateTaskQuery = templateTaskQuery;
         _singleStartPresenterTask = singleStartPresenterTask;
         _buildingBlockRepository = buildingBlockRepository;
         _lazyLoadTask = lazyLoadTask;
         _presentationSettingsTask = presentationSettingsTask;
         _simulationReferenceUpdater = simulationReferenceUpdater;
         _snapshotTask = snapshotTask;
      }

      public void AddCommandToHistory(ICommand command)
      {
         _executionContext.AddToHistory(command);
      }

      public void LoadResults<TBuildingBlock>(TBuildingBlock simulationToLoad) where TBuildingBlock : Simulation
      {
         Load(simulationToLoad);
         _lazyLoadTask.LoadResults(simulationToLoad);
      }

      public void Edit(IPKSimBuildingBlock buildingBlockToEdit)
      {
         if (buildingBlockToEdit == null) return;
         _singleStartPresenterTask.StartForSubject(buildingBlockToEdit);
      }

      public void Clone<TBuildingBlock>(TBuildingBlock buildingBlockToClone) where TBuildingBlock : class, IPKSimBuildingBlock
      {
         Load(buildingBlockToClone);
         using (var clonePresenter = _applicationController.Start<ICloneBuildingBlockPresenter>())
         {
            var clone = clonePresenter.CreateCloneFor(buildingBlockToClone);
            if (clone == null) return;

            clone.Creation.AsCloneOf(buildingBlockToClone);

            var addCommand = new AddBuildingBlockToProjectCommand(clone, _executionContext).Run(_executionContext);
            var entityType = _entityTask.TypeFor(buildingBlockToClone);
            addCommand.Description = PKSimConstants.Command.CloneEntity(entityType, buildingBlockToClone.Name, clone.Name);

            AddCommandToHistory(addCommand);
         }
      }

      public bool Delete<TBuildingBlock>(IReadOnlyList<TBuildingBlock> buildingBlocksToDelete) where TBuildingBlock : class, IPKSimBuildingBlock
      {
         if (!buildingBlocksToDelete.Any())
            return true;

         var buildingBlockType = _entityTask.TypeFor(buildingBlocksToDelete.First());

         foreach (var buildingBlockToDelete in buildingBlocksToDelete)
         {
            var simulationsUsingBuildingBlockToDelete = _buildingBlockInSimulationManager.SimulationsUsing(buildingBlockToDelete).ToList();
            if (simulationsUsingBuildingBlockToDelete.Any())
               throw new CannotDeleteBuildingBlockException(buildingBlockToDelete.Name, buildingBlockType, simulationsUsingBuildingBlockToDelete);
         }

         var viewResult = _dialogCreator.MessageBoxYesNo(PKSimConstants.UI.ReallyDeleteObjectOfType(buildingBlockType, buildingBlocksToDelete.AllNames().ToArray()));
         if (viewResult == ViewResult.No)
            return false;

         buildingBlocksToDelete.OfType<Simulation>().Each(simulation => _simulationReferenceUpdater.RemoveSimulationFromParameterIdentificationsAndSensitivityAnalyses(simulation));

         var macoCommand = new PKSimMacroCommand
         {
            CommandType = PKSimConstants.Command.CommandTypeDelete,
            ObjectType = buildingBlockType,
            BuildingBlockType = buildingBlockType,
            Description = PKSimConstants.Command.ObjectsDeletedFromProject(buildingBlockType),
         };

         buildingBlocksToDelete.Each(x => macoCommand.Add(DeleteCommand(x)));
         AddCommandToHistory(macoCommand.Run(_executionContext));
         buildingBlocksToDelete.Each(RemovePresenterSettings);

         return true;
      }

      public bool Delete<TBuildingBlock>(TBuildingBlock buildingBlockToDelete) where TBuildingBlock : class, IPKSimBuildingBlock
      {
         return Delete<TBuildingBlock>(new[] {buildingBlockToDelete});
      }

      public IPKSimCommand DeleteCommand<TBuildingBlock>(TBuildingBlock buildingBlockToDelete) where TBuildingBlock : class, IPKSimBuildingBlock
      {
         try
         {
            Load(buildingBlockToDelete);
            return new RemoveBuildingBlockFromProjectCommand(buildingBlockToDelete, _executionContext);
         }
         catch (Exception)
         {
            //could not load the building block while attempting to delete it. Let's be ok with that and delete
            buildingBlockToDelete.IsLoaded = true;
            return new RemoveBuildingBlockFromProjectIrreversibleCommand(buildingBlockToDelete, _executionContext);
         }
      }

      public void RemovePresenterSettings<TBuildingBlock>(TBuildingBlock buildingBlock) where TBuildingBlock : class, IPKSimBuildingBlock
      {
         _presentationSettingsTask.RemovePresentationSettingsFor(buildingBlock);
      }

      public void Rename<TBuildingBlock>(TBuildingBlock buildingBlockToRename) where TBuildingBlock : class, IPKSimBuildingBlock
      {
         if (buildingBlockToRename.IsAnImplementationOf<Compound>())
         {
            var simulationsUsingBuildingBlockToDelete = _buildingBlockInSimulationManager.SimulationsUsing(buildingBlockToRename).ToList();
            if (simulationsUsingBuildingBlockToDelete.Any())
               throw new CannotRenameCompoundException(buildingBlockToRename.Name, simulationsUsingBuildingBlockToDelete);
         }

         Load(buildingBlockToRename);
         AddCommandToHistory(_entityTask.Rename(buildingBlockToRename));
      }

      public void SaveAsTemplate(ICache<IPKSimBuildingBlock, IReadOnlyList<IPKSimBuildingBlock>> buildingBlocksWithReferenceToSave, TemplateDatabaseType templateDatabaseType)
      {
         var templates = new Cache<IPKSimBuildingBlock, Template>();

         var allBuildingBlocksToSave = buildingBlocksWithReferenceToSave.Keys.Union(buildingBlocksWithReferenceToSave.SelectMany(x => x)).ToList();
         //First pass. Save the template item corresponding to the specific building blocks
         foreach (var buildingBlockToSave in allBuildingBlocksToSave)
         {
            var templateItem = templateItemFor(buildingBlockToSave, templateDatabaseType);
            //user cancels saving action for one building block. Cancel the whole process
            if (templateItem == null)
               return;

            templates.Add(buildingBlockToSave, templateItem);
         }

         //then update the template references that will be save to the template database
         foreach (var keyValue in buildingBlocksWithReferenceToSave.KeyValues)
         {
            var templateItem = templates[keyValue.Key];
            keyValue.Value.Each(reference => templateItem.References.Add(templates[reference]));
         }

         _templateTaskQuery.SaveToTemplate(templates.ToList());
         _dialogCreator.MessageBoxInfo(PKSimConstants.UI.TemplatesSuccessfullySaved(templates.Select(x => x.Name).ToList()));
      }

      private Template templateItemFor(IPKSimBuildingBlock buildingBlockToSave, TemplateDatabaseType templateDatabaseType)
      {
         string buildingBlockName = buildingBlockToSave.Name;
         string buildingBlockType = TypeFor(buildingBlockToSave);
         var templateType = buildingBlockToSave.BuildingBlockType.AsTemplateType();

         if (_templateTaskQuery.Exists(templateDatabaseType, buildingBlockName, templateType))
         {
            var result = _dialogCreator.MessageBoxYesNoCancel(PKSimConstants.UI.TemplateWithNameAlreadyExistsInTheDatabase(buildingBlockToSave.Name, buildingBlockType),
               PKSimConstants.UI.Override, PKSimConstants.UI.SaveAs, PKSimConstants.UI.CancelButton);

            //user does not want to override
            if (result == ViewResult.Cancel)
               return null;

            //retrieve a new name
            if (result == ViewResult.No)
            {
               var allTemplateNames = _templateTaskQuery.AllTemplatesFor(templateDatabaseType, templateType).Select(x => x.Name);
               buildingBlockName = _entityTask.NewNameFor(buildingBlockToSave, allTemplateNames);
               if (string.IsNullOrEmpty(buildingBlockName))
                  return null;
            }
         }
         Load(buildingBlockToSave);
         return new Template
         {
            Name = buildingBlockName,
            Description = buildingBlockToSave.Description,
            Object = buildingBlockToSave,
            TemplateType = templateType,
            DatabaseType = templateDatabaseType
         };
      }

      public void EditDescription(IPKSimBuildingBlock buildingBlock)
      {
         Load(buildingBlock);
         AddCommandToHistory(_entityTask.EditDescription(buildingBlock));
      }

      public string TypeFor<TBuildingBlock>(TBuildingBlock buildingBlock) where TBuildingBlock : class, IPKSimBuildingBlock
      {
         return _entityTask.TypeFor(buildingBlock);
      }

      public IEnumerable<TBuildingBlock> All<TBuildingBlock>() where TBuildingBlock : class, IPKSimBuildingBlock
      {
         return _buildingBlockRepository.All<TBuildingBlock>();
      }

      public IPKSimCommand AddToProject<TBuildingBlock>(TBuildingBlock buildingBlock, bool editBuildingBlock = false, bool addToHistory = true) where TBuildingBlock : class, IPKSimBuildingBlock
      {
         if (!RenameBuildingBlockIfAlreadyUsed(buildingBlock))
            return new PKSimEmptyCommand();

         var addToProjectCommand = new AddBuildingBlockToProjectCommand(buildingBlock, _executionContext).Run(_executionContext);
         addToProjectCommand.ExtendedDescription = _executionContext.ReportFor(buildingBlock);

         if (addToHistory)
            AddCommandToHistory(addToProjectCommand);

         if (editBuildingBlock)
            Edit(buildingBlock);

         return addToProjectCommand;
      }

      public void Load<TBuildingBlock>(TBuildingBlock buildingBlockToLoad) where TBuildingBlock : class, IPKSimBuildingBlock
      {
         _executionContext.Load(buildingBlockToLoad);
      }

      public IReadOnlyList<TBuildingBlock> LoadFromTemplate<TBuildingBlock>(PKSimBuildingBlockType buildingBlockType) where TBuildingBlock : class, IPKSimBuildingBlock
      {
         using (var presenter = _applicationController.Start<ITemplatePresenter>())
         {
            var buildingBlocks = presenter.LoadFromTemplate<TBuildingBlock>(typeFrom(buildingBlockType));

            return addBuildingBlocksToProject(buildingBlocks).ToList();
         }
      }

      public  IReadOnlyList<TBuildingBlock> LoadFromSnapshot<TBuildingBlock>(PKSimBuildingBlockType buildingBlockType) where TBuildingBlock : class, IPKSimBuildingBlock
      {
         using (var presenter = _applicationController.Start<ILoadFromSnapshotPresenter<TBuildingBlock>>())
         {
            var buildingBlocks = presenter.LoadModelFromSnapshot();
            return addBuildingBlocksToProject(buildingBlocks).ToList();

         }
      }

      private IEnumerable<TBuildingBlock> addBuildingBlocksToProject<TBuildingBlock>(IEnumerable<TBuildingBlock> buildingBlocks) where TBuildingBlock : class, IPKSimBuildingBlock
      {
         if (buildingBlocks == null)
            return Enumerable.Empty<TBuildingBlock>();

         return from bb in buildingBlocks
            let command = AddToProject(bb, editBuildingBlock: false)
            where !command.IsEmpty()
            select bb;
      }

      public bool BuildingBlockNameIsAlreadyUsed(IPKSimBuildingBlock buildingBlock)
      {
         var allBuildingBlockNames = _executionContext.CurrentProject.All(buildingBlock.BuildingBlockType).Select(x => x.Name.ToUpperInvariant());
         return allBuildingBlockNames.Contains(buildingBlock.Name.ToUpperInvariant());
      }

      public bool RenameBuildingBlockIfAlreadyUsed(IPKSimBuildingBlock buildingBlock)
      {
         if (!BuildingBlockNameIsAlreadyUsed(buildingBlock))
            return true;

         var command = _entityTask.Rename(buildingBlock);

         //User canceled rename
         return !command.IsEmpty();
      }

      private TemplateType typeFrom(PKSimBuildingBlockType buildingBlockType)
      {
         if (buildingBlockType == PKSimBuildingBlockType.SimulationSubject)
            return TemplateType.Individual | TemplateType.Population;

         return EnumHelper.ParseValue<TemplateType>(buildingBlockType.ToString());
      }
   }

   public abstract class BuildingBlockTask<TBuildingBlock> : IBuildingBlockTask<TBuildingBlock> where TBuildingBlock : class, IPKSimBuildingBlock
   {
      protected readonly IBuildingBlockTask _buildingBlockTask;
      protected readonly IApplicationController _applicationController;
      private readonly PKSimBuildingBlockType _buildingBlockType;
      protected readonly IExecutionContext _executionContext;

      protected BuildingBlockTask(IExecutionContext executionContext, IBuildingBlockTask buildingBlockTask, IApplicationController applicationController, PKSimBuildingBlockType buildingBlockType)
      {
         _executionContext = executionContext;
         _buildingBlockTask = buildingBlockTask;
         _applicationController = applicationController;
         _buildingBlockType = buildingBlockType;
      }

      public abstract TBuildingBlock AddToProject();

      public TBuildingBlock LoadSingleFromTemplate()
      {
         return LoadFromTemplate().FirstOrDefault();
      }

      public IReadOnlyList<TBuildingBlock> LoadFromTemplate()
      {
         return LoadFromTemplate(_buildingBlockType);
      }

      public IReadOnlyList<TBuildingBlock> LoadFromSnapshot()
      {
         return LoadFromSnapshot(_buildingBlockType);
      }

      public void Load(TBuildingBlock buildingBlockToLoad)
      {
         _buildingBlockTask.Load(buildingBlockToLoad);
      }

      public IEnumerable<TBuildingBlock> All()
      {
         return _buildingBlockTask.All<TBuildingBlock>();
      }

      public void SaveAsTemplate(TBuildingBlock buildingBlockToSave)
      {
         SaveAsTemplate(buildingBlockToSave, TemplateDatabaseType.User);
      }

      public void SaveAsSystemTemplate(TBuildingBlock buildingBlockToSave)
      {
         SaveAsTemplate(buildingBlockToSave, TemplateDatabaseType.System);
      }

      protected virtual void SaveAsTemplate(TBuildingBlock buildingBlockToSave, TemplateDatabaseType templateDatabaseType)
      {
         var cache = new Cache<IPKSimBuildingBlock, IReadOnlyList<IPKSimBuildingBlock>> {[buildingBlockToSave] = new List<IPKSimBuildingBlock>()};
         _buildingBlockTask.SaveAsTemplate(cache, templateDatabaseType);
      }

      public virtual void Edit(TBuildingBlock buildingBlockToEdit)
      {
         _buildingBlockTask.Edit(buildingBlockToEdit);
      }

      /// <summary>
      ///    Launch the create builing block presenter implementing TCreatePresenter and start the creation process
      /// </summary>
      /// <typeparam name="TCreatePresenter">Type of presenter that will be started to create the new building block</typeparam>
      /// <returns>The created building block, if the action was confirmed by the user or cancel otherwise</returns>
      protected TBuildingBlock AddToProject<TCreatePresenter>() where TCreatePresenter : ICreateBuildingBlockPresenter<TBuildingBlock>
      {
         return AddToProject<TCreatePresenter>(x => x.Create());
      }

      /// <summary>
      ///    Launch the create builing block presenter implementing TCreatePresenter and start the creation process
      /// </summary>
      /// <typeparam name="TCreatePresenter">Type of presenter that will be started to create the new building block</typeparam>
      /// <param name="createFunction">Function of the presenter that will be called to launch the creation function</param>
      /// <returns>The created building block, if the action was confirmed by the user or cancel otherwise</returns>
      protected TBuildingBlock AddToProject<TCreatePresenter>(Func<TCreatePresenter, IPKSimCommand> createFunction) where TCreatePresenter : ICreateBuildingBlockPresenter<TBuildingBlock>
      {
         using (var presenter = _applicationController.Start<TCreatePresenter>())
         {
            var createCommand = createFunction(presenter);

            //User cancel action. return
            if (createCommand.IsEmpty())
               return null;

            //Add first the building block to the project. Orders matters since the createCommand might contain references to objects 
            //that need to be loaded ahead of time when performing an undo
            AddToProject(presenter.BuildingBlock);

            var macroCommand = createCommand as IPKSimMacroCommand;
            if (macroCommand != null)
            {
               macroCommand.ReplaceNameTemplateWithName(presenter.BuildingBlock.Name);
               macroCommand.ReplaceTypeTemplateWithType(_executionContext.TypeFor(presenter.BuildingBlock));
               macroCommand.All().Each(_buildingBlockTask.AddCommandToHistory);
            }

            //required since some subpresenter might still be registered and need to be released from event publisher
            _applicationController.ReleasePresenter(presenter);
            return presenter.BuildingBlock;
         }
      }

      /// <summary>
      ///    Add the building block to the project using a command
      /// </summary>
      /// <param name="buildingBlock">building block to add</param>
      /// <param name="editBuildingBlock">If set to <c>true</c>, the edit workflow is started automatically. Default is true</param>
      /// <param name="addToHistory">If set to <c>true</c>, the command is added to the history. Default is true</param>
      public IPKSimCommand AddToProject(TBuildingBlock buildingBlock, bool editBuildingBlock = true, bool addToHistory = true)
      {
         return _buildingBlockTask.AddToProject(buildingBlock, editBuildingBlock, addToHistory);
      }

      protected virtual IReadOnlyList<TBuildingBlock> LoadFromTemplate(PKSimBuildingBlockType buildingBlockType)
      {
         return _buildingBlockTask.LoadFromTemplate<TBuildingBlock>(buildingBlockType);
      }

      protected virtual IReadOnlyList<TBuildingBlock> LoadFromSnapshot(PKSimBuildingBlockType buildingBlockType)
      {
         return _buildingBlockTask.LoadFromSnapshot<TBuildingBlock>(buildingBlockType);
      }
   }
}