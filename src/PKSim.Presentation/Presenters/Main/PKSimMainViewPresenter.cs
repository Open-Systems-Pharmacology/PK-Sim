using OSPSuite.Presentation.Events;
using OSPSuite.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Presenters.Main;
using OSPSuite.Presentation.UICommands;
using OSPSuite.TeXReporting.Events;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core;
using PKSim.Core.Events;
using PKSim.Core.Services;
using PKSim.Presentation.Events;
using PKSim.Presentation.Services;
using PKSim.Presentation.UICommands;
using PKSim.Presentation.Views.Main;

namespace PKSim.Presentation.Presenters.Main
{
   public interface IPKSimMainViewPresenter : IMainViewPresenter,
      IListener<SimulationRunStartedEvent>,
      IListener<SimulationRunFinishedEvent>,
      IListener<ShowNotificationEvent>,
      IListener<ReportCreationStartedEvent>,
      IListener<ReportCreationFinishedEvent>


   {
      StartOptions StartOptions { get; set; }
   }

   public class PKSimMainViewPresenter : AbstractMainViewPresenter<IPKSimMainView, IPKSimMainViewPresenter>, IPKSimMainViewPresenter
   {
      private readonly IRepository<IMainViewItemPresenter> _presenterRepository;
      private readonly IExitCommand _exitCommand;
      private readonly IUserSettings _userSettings;
      private readonly IProjectTask _projectTask;
      private readonly IPKSimConfiguration _configuration;
      private readonly IPostLaunchChecker _postLaunchChecker;
      private readonly IVersionChecker _versionChecker;
      public StartOptions StartOptions { get; set; }

      public PKSimMainViewPresenter(IPKSimMainView mainView,
         IEventPublisher eventPublisher,
         ITabbedMdiChildViewContextMenuFactory contextMenuFactory,
         IRepository<IMainViewItemPresenter> presenterRepository,
         IExitCommand exitCommand,
         IUserSettings userSettings,
         IProjectTask projectTask,
         IPKSimConfiguration configuration,
         IPostLaunchChecker postLaunchChecker,
         IVersionChecker versionChecker
      )
         : base(mainView, eventPublisher, contextMenuFactory)
      {
         _presenterRepository = presenterRepository;
         _exitCommand = exitCommand;
         _userSettings = userSettings;
         _projectTask = projectTask;
         _configuration = configuration;
         _postLaunchChecker = postLaunchChecker;
         _versionChecker = versionChecker;
      }

      public override void Initialize()
      {
         View.Initialize();
         View.Caption = _configuration.ProductDisplayName;

         //intialize all sub presenter defined in the user interface
         _presenterRepository.All().Each(presenter => presenter.Initialize());

         //set the action to be performed when closing the main form
         View.Closing += (o, e) =>
         {
            _exitCommand.ExecuteWithinExceptionHandler(_eventPublisher, this);
            e.Cancel = _exitCommand.Canceled;
         };

         View.Loading += () => _userSettings.RestoreLayout();

         _eventPublisher.PublishEvent(new ApplicationInitializedEvent());
      }

      public override void Run()
      {
         _projectTask.Run(StartOptions);

         _postLaunchChecker.PerformPostLaunchCheckAsync();
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

      public void Handle(ReportCreationStartedEvent reportStartedEvent)
      {
         _view.DisplayNotification(PKSimConstants.UI.ReportCreationStarted,
            PKSimConstants.UI.ReportCreationStartedMessage(reportStartedEvent.ReportFullPath), string.Empty);
      }

      public void Handle(ReportCreationFinishedEvent reportFinishedEvent)
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

      public void Handle(ShowNotificationEvent eventToHandle)
      {
         showNotification(eventToHandle.Caption, eventToHandle.Notification, eventToHandle.Url);
      }

      private void showNotification(string caption, string notification, string url)
      {
         _view.DisplayNotification(caption, notification, url);
      }
   }
}