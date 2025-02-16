﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Assets;
using OSPSuite.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Events;
using PKSim.Assets;
using PKSim.Core.Snapshots.Services;
using PKSim.Presentation.DTO.Snapshots;
using PKSim.Presentation.Views.Snapshots;

namespace PKSim.Presentation.Presenters.Snapshots
{
   public interface ILoadFromSnapshotPresenter : IPresenter<ILoadFromSnapshotView>, IDisposablePresenter
   {
      /// <summary>
      ///    Starts the snapshot file selection process. Returns <c>true</c> if a file was selected of <c>false</c> if the
      ///    selection was canceled
      /// </summary>
      bool SelectFile();

      Task StartAsync();

      bool ModelIsDefined { get; }

      /// <summary>
      ///    File selected by the user containing the snapshot being loaded.
      /// </summary>
      string SnapshotFile { get; }
   }

   public interface ILoadFromSnapshotPresenter<T> : ILoadFromSnapshotPresenter where T : class, IObjectBase
   {
      IReadOnlyList<T> LoadModelFromSnapshot();
   }

   public class LoadFromSnapshotPresenter<T> : AbstractDisposablePresenter<ILoadFromSnapshotView, ILoadFromSnapshotPresenter>, ILoadFromSnapshotPresenter<T> where T : class, IObjectBase
   {
      protected readonly ISnapshotTask _snapshotTask;
      private readonly IDialogCreator _dialogCreator;
      private readonly IObjectTypeResolver _objectTypeResolver;
      private readonly IOSPSuiteLogger _logger;
      private readonly IEventPublisher _eventPublisher;
      private readonly ILogPresenter _logPresenter;
      private readonly LoadFromSnapshotDTO _loadFromSnapshotDTO = new LoadFromSnapshotDTO();
      private IEnumerable<T> _model;

      public LoadFromSnapshotPresenter(
         ILoadFromSnapshotView view,
         ILogPresenter logPresenter,
         ISnapshotTask snapshotTask,
         IDialogCreator dialogCreator,
         IObjectTypeResolver objectTypeResolver,
         IOSPSuiteLogger logger,
         IEventPublisher eventPublisher) : base(view)
      {
         _snapshotTask = snapshotTask;
         _dialogCreator = dialogCreator;
         _objectTypeResolver = objectTypeResolver;
         _logger = logger;
         _logPresenter = logPresenter;
         _eventPublisher = eventPublisher;
         AddSubPresenters(_logPresenter);
         _view.Caption = PKSimConstants.UI.LoadObjectFromSnapshot(typeToLoad);
         _view.AddLogView(_logPresenter.BaseView);
         _view.BindTo(_loadFromSnapshotDTO);
      }

      public override bool ShouldClose
      {
         get
         {
            if (!ModelIsDefined)
               return true;

            var shouldCancel = _dialogCreator.MessageBoxYesNo(Captions.ReallyCancel);
            return shouldCancel == ViewResult.Yes;
         }
      }

      public IReadOnlyList<T> LoadModelFromSnapshot()
      {
         if (!SelectFile())
            return null;

         _view.Display();
         ClearModel(_model);
         return _view.Canceled ? null : _model.ToList();
      }

      public bool SelectFile()
      {
         var fileName = selectSnapshotFile();
         if (string.IsNullOrEmpty(fileName))
            return false;

         ClearModel();

         _loadFromSnapshotDTO.SnapshotFile = fileName;
         return true;
      }

      private string selectSnapshotFile()
      {
         var message = PKSimConstants.UI.LoadObjectFromSnapshot(typeToLoad);
         return _dialogCreator.AskForFileToOpen(message, Constants.Filter.JSON_FILE_FILTER, Constants.DirectoryKey.REPORT);
      }

      private string typeToLoad => _objectTypeResolver.TypeFor<T>();

      public async Task StartAsync()
      {
         try
         {
            _logPresenter.ClearLog();
            _view.EnableButtons(false);
            _logger.AddInfo(PKSimConstants.Information.LoadingSnapshot(_loadFromSnapshotDTO.SnapshotFile, typeToLoad));
            await Task.Run(performLoadAsync);
            _logger.AddInfo(PKSimConstants.Information.SnapshotLoaded(typeToLoad));
         }
         catch (Exception e)
         {
            _logger.AddException(e);
         }
         finally
         {
            //Start is disabled if the model was loaded successfully
            _view.EnableButtons(cancelEnabled: true, okEnabled: ModelIsDefined, startEnabled: !ModelIsDefined);
         }
      }

      public bool ModelIsDefined => _model != null;

      public string SnapshotFile => _loadFromSnapshotDTO.SnapshotFile;

      private async Task performLoadAsync()
      {
         ClearModel();
         _model = await LoadModelAsync(_loadFromSnapshotDTO);
      }

      protected virtual void ClearModel()
      {
         ClearModel(_model);
         _model = null;
      }

      protected virtual void ClearModel(IEnumerable<T> model)
      {
         /*override if something needs to be done for specific loading case*/
      }

      protected virtual Task<IEnumerable<T>> LoadModelAsync(LoadFromSnapshotDTO loadFromSnapshotDTO)
      {
         return _snapshotTask.LoadModelsFromSnapshotFileAsync<T>(loadFromSnapshotDTO.SnapshotFile);
      }

      protected override void Cleanup()
      {
         try
         {
            ReleaseFrom(_eventPublisher);
         }
         finally
         {
            base.Cleanup();
         }
      }
   }
}