using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Events;
using PKSim.Assets;
using PKSim.Core.Extensions;
using PKSim.Core.Snapshots.Services;
using PKSim.Presentation.DTO.Snapshots;
using PKSim.Presentation.Views.Snapshots;

namespace PKSim.Presentation.Presenters.Snapshots
{
   public interface ILoadFromSnapshotPresenter : IPresenter<ILoadFromSnapshotView>, IDisposablePresenter
   {
      void SelectFile();
      Task Start();
      bool ModelIsDefined { get; }
   }

   public interface ILoadFromSnapshotPresenter<T> : ILoadFromSnapshotPresenter where T : class, IObjectBase
   {
      IEnumerable<T> LoadModelFromSnapshot();
   }

   public class LoadFromSnapshotPresenter<T> : AbstractDisposablePresenter<ILoadFromSnapshotView, ILoadFromSnapshotPresenter>, ILoadFromSnapshotPresenter<T> where T : class, IObjectBase
   {
      protected readonly ISnapshotTask _snapshotTask;
      private readonly IDialogCreator _dialogCreator;
      private readonly IObjectTypeResolver _objectTypeResolver;
      private readonly ILogger _logger;
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
         ILogger logger,
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

      public IEnumerable<T> LoadModelFromSnapshot()
      {
         _view.Display();
         return _view.Canceled ? null : _model;
      }

      public void SelectFile()
      {
         var message = PKSimConstants.UI.LoadObjectFromSnapshot(typeToLoad);
         var fileName = _dialogCreator.AskForFileToOpen(message, Constants.Filter.JSON_FILE_FILTER, Constants.DirectoryKey.REPORT);
         if (string.IsNullOrEmpty(fileName))
            return;

         _loadFromSnapshotDTO.SnapshotFile = fileName;
      }

      private string typeToLoad => _objectTypeResolver.TypeFor<T>();

      public async Task Start()
      {
         try
         {
            _logPresenter.ClearLog();
            _view.EnableButtons(false);
            _model = null;
            _logger.AddInfo(PKSimConstants.Information.LoadingSnapshot(_loadFromSnapshotDTO.SnapshotFile, typeToLoad));
            await Task.Run(() => PerformLoad(_loadFromSnapshotDTO.SnapshotFile));
            _logger.AddInfo(PKSimConstants.Information.SnapshotLoaded(typeToLoad));
         }
         catch (Exception e)
         {
            _logger.AddException(e);
         }
         finally
         {
            _view.EnableButtons(cancelEnabled: true, okEnabled: ModelIsDefined, startEnabled: CanClose);
         }
      }

      public bool ModelIsDefined => _model != null;

      protected async Task PerformLoad(string snapshotFile)
      {
         _model = await LoadModelAsync(snapshotFile);
      }

      protected virtual Task<IEnumerable<T>> LoadModelAsync(string snapshotFile)
      {
         return _snapshotTask.LoadModelFromSnapshot<T>(snapshotFile);
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