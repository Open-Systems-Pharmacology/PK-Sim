using System;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Events;
using PKSim.Core.Services;
using PKSim.Presentation.Services;
using PKSim.Presentation.UICommands;
using PKSim.Presentation.Views.Main;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Events;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Presenters.Main;
using OSPSuite.Presentation.UICommands;
using OSPSuite.TeXReporting.Events;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;

namespace PKSim.Presentation.Presenters.Main
{
   public interface IPKSimMainViewPresenter : IMainViewPresenter,
      IListener<SimulationRunStartedEvent>,
      IListener<SimulationRunFinishedEvent>

   {
      StartOptions StartOptions { get; set; }
   }

   public class PKSimMainViewPresenter : AbstractMainViewPresenter<IPKSimMainView, IPKSimMainViewPresenter>, IPKSimMainViewPresenter
   {
      private readonly IRepository<IMainViewItemPresenter> _presenterRepository;
      private readonly IExitCommand _exitCommand;
      private readonly IUserSettings _userSettings;
      private readonly IProjectTask _projectTask;
      private readonly IVersionChecker _versionChecker;
      public StartOptions StartOptions { get; set; }

      public PKSimMainViewPresenter(IPKSimMainView mainView, IRepository<IMainViewItemPresenter> presenterRepository,
         IExitCommand exitCommand, IEventPublisher eventPublisher, IUserSettings userSettings,
         IProjectTask projectTask, IVersionChecker versionChecker, ITabbedMdiChildViewContextMenuFactory contextMenuFactory)
         : base(mainView, eventPublisher, contextMenuFactory)
      {
         _presenterRepository = presenterRepository;
         _exitCommand = exitCommand;
         _userSettings = userSettings;
         _projectTask = projectTask;
         _versionChecker = versionChecker;
      }

      public override void Initialize()
      {
         View.Initialize();
         View.Caption = CoreConstants.ProductDisplayName;

         //intialize all sub presenter defined in the user interface
         _presenterRepository.All().Each(presenter => presenter.Initialize());

         //set the action to be performed when closing the main form
         View.Closing += (o, e) =>
         {
            _exitCommand.ShouldCloseApplication = false;
            _exitCommand.ExecuteWithinExceptionHandler(_eventPublisher, this);
            e.Cancel = _exitCommand.Canceled || !CanClose;
         };

         View.Loading += () => _userSettings.RestoreLayout();

         _eventPublisher.PublishEvent(new ApplicationInitializedEvent());
      }

      public override void Run()
      {
         _projectTask.Run(StartOptions);
         showUpdateNotification();
      }

      private async void showUpdateNotification()
      {
         try
         {
            var hasNewVersion = await _versionChecker.NewVersionIsAvailableAsync();
            if (!hasNewVersion) return;
         }
         catch (Exception)
         {
            //no need to do anything if version cannot be returned
            return;
         }

         if (!_userSettings.ShowUpdateNotification)
            return;

         if (string.Equals(_versionChecker.LatestVersion, _userSettings.LastIgnoredVersion))
            return;

         _view.DisplayNotification(PKSimConstants.UI.UpdateAvailable,
            PKSimConstants.Information.NewVersionIsAvailable(_versionChecker.LatestVersion, Constants.PRODUCT_SITE),
            Constants.PRODUCT_SITE_DOWNLOAD);
      }

      public override void RemoveAlert()
      {
         var latestVersion = _versionChecker.LatestVersion;
         if (latestVersion == null) return;
         _userSettings.LastIgnoredVersion = latestVersion.Version;
      }

      public override void OpenFile(string fileName)
      {
         _projectTask.OpenProjectFrom(fileName);
      }

      public override void Handle(ReportCreationStartedEvent reportStartedEvent)
      {
         _view.DisplayNotification(PKSimConstants.UI.ReportCreationStarted,
            PKSimConstants.UI.ReportCreationStartedMessage(reportStartedEvent.ReportFullPath), string.Empty);
      }

      public override void Handle(ReportCreationFinishedEvent reportFinishedEvent)
      {
         _view.DisplayNotification(PKSimConstants.UI.ReportCreationFinished,
            PKSimConstants.UI.ReportCreationFinishedMessage(reportFinishedEvent.ReportFullPath), reportFinishedEvent.ReportFullPath);
      }

      public void Handle(SimulationRunStartedEvent eventToHandle)
      {
         View.AllowChildActivation = false;
      }

      public void Handle(SimulationRunFinishedEvent eventToHandle)
      {
         View.AllowChildActivation = true;
      }
   }
}