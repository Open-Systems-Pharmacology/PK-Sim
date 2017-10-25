using System;
using PKSim.Assets;
using PKSim.Presentation.Core;
using OSPSuite.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Events;
using OSPSuite.Core.Journal;
using OSPSuite.Presentation.Events;
using OSPSuite.Presentation.MenuAndBars;
using OSPSuite.Presentation.Presenters.Main;
using OSPSuite.Presentation.Views;
using OSPSuite.Assets;
using OSPSuite.TeXReporting.Events;
using OSPSuite.Utility;
using OSPSuite.Utility.Events;
using OSPSuite.Utility.Extensions;

namespace PKSim.Presentation.Presenters.Main
{
   public interface IStatusBarPresenter : IMainViewItemPresenter,
      IListener<ProjectCreatedEvent>,
      IListener<ProjectLoadedEvent>,
      IListener<ProjectLoadingEvent>,
      IListener<ProjectSavingEvent>,
      IListener<ProjectSavedEvent>,
      IListener<ProjectClosedEvent>,
      IListener<ProgressDoneEvent>,
      IListener<ProgressInitEvent>,
      IListener<ProgressingEvent>,
      IListener<StatusMessageEvent>,
      IListener<ApplicationInitializedEvent>,
      IListener<JournalLoadedEvent>,
      IListener<JournalClosedEvent>,
      IListener<ReportCreationStartedEvent>,
      IListener<ReportCreationFinishedEvent>

   {
   }

   public class StatusBarPresenter : IStatusBarPresenter
   {
      private readonly IStatusBarView _view;
      private readonly IApplicationConfiguration _applicationConfiguration;
      private int _numberOfReportsBeingCreated;
      public event EventHandler StatusChanged = delegate { };

      public StatusBarPresenter(IStatusBarView view, IApplicationConfiguration applicationConfiguration)
      {
         _view = view;
         _applicationConfiguration = applicationConfiguration;
      }

      public void Initialize()
      {
         StatusBarElements.All().Each(_view.AddItem);
      }

      public void ToggleVisibility()
      {
         //nothing to do
      }

      public void ViewChanged()
      {
         //nothing to do here
      }

      public IView BaseView => null;

      public void Handle(ProjectCreatedEvent eventToHandle)
      {
         updateProjectInfo(eventToHandle.Project, enabled: true);
         updateUndefinedJournalInfo();
      }

      public void Handle(ProjectLoadedEvent eventToHandle)
      {
         updateProjectInfo(eventToHandle.Project, enabled: true);
      }

      public void Handle(ProjectLoadingEvent eventToHandle)
      {
         update(StatusBarElements.Status).WithCaption("Loading project...");
      }

      public void Handle(ProjectSavingEvent eventToHandle)
      {
         update(StatusBarElements.Status).WithCaption("Saving project...");
      }

      public void Handle(ProjectSavedEvent eventToHandle)
      {
         updateProjectInfo(eventToHandle.Project, enabled: true);
      }

      public void Handle(ProjectClosedEvent eventToHandle)
      {
         updateProjectInfo(Captions.None, Captions.None, enabled: false);
      }

      private IStatusBarElementExpression update(StatusBarElement statusBarElement)
      {
         return _view.BarElementExpressionFor(statusBarElement);
      }

      private void updateProjectInfo(string projectName, string projectPath, bool enabled)
      {
         update(StatusBarElements.Status)
            .WithCaption(string.Empty);

         update(StatusBarElements.ProjectName)
            .WithCaption($"Project: {projectName}")
            .And.ToolTipText($"Project: {projectName}")
            .And.Enabled(enabled);

         update(StatusBarElements.ProjectPath)
            .WithCaption(projectPath)
            .And.ToolTipText($"Project File: {projectPath}")
            .And.Enabled(enabled);
      }

      private void updateProjectInfo(IProject project, bool enabled)
      {
         updateProjectInfo(project.Name, project.FilePath, enabled);
      }

      public void Handle(ProgressInitEvent eventToHandle)
      {
         update(StatusBarElements.ProgressBar)
            .WithValue(0)
            .And.Visible(true);

         update(StatusBarElements.ProgressStatus)
            .WithCaption(eventToHandle.Message)
            .And.Visible(true);
      }

      public void Handle(ProgressingEvent eventToHandle)
      {
         update(StatusBarElements.ProgressBar)
            .WithValue(eventToHandle.ProgressPercent);

         update(StatusBarElements.ProgressStatus)
            .WithCaption(eventToHandle.Message);
      }

      public void Handle(ProgressDoneEvent eventToHandle)
      {
         hideProgressBar();
      }

      public void Handle(ApplicationInitializedEvent eventToHandle)
      {
         updateProjectInfo(Captions.None, Captions.None, false);
         update(StatusBarElements.Version)
            .WithCaption(_applicationConfiguration.FullVersion);

         hideProgressBar();
      }

      public void Handle(JournalLoadedEvent journalLoadedEvent)
      {
         var name = FileHelper.FileNameFromFileFullPath(journalLoadedEvent.Journal.FullPath);
         updateJournalInfo(name, journalLoadedEvent.Journal.FullPath, enabled: true);
      }

      public void Handle(JournalClosedEvent eventToHandle)
      {
         updateUndefinedJournalInfo();
      }

      private void updateUndefinedJournalInfo()
      {
         updateJournalInfo(PKSimConstants.UI.Undefined, string.Empty, enabled: false);
      }

      private void updateJournalInfo(string name, string path, bool enabled)
      {
         update(StatusBarElements.Journal)
            .WithCaption($"Journal: {name}")
            .And.ToolTipText($"Journal File: {path}")
            .And.Enabled(enabled);
      }

      private void updateReportInfo()
      {
         string caption = "";
         if (_numberOfReportsBeingCreated == 1)
            caption = "1 report is being created...";

         else if (_numberOfReportsBeingCreated > 1)
            caption = $"{_numberOfReportsBeingCreated} reports are being created...";

         update(StatusBarElements.Report)
            .WithCaption(caption);
      }

      private void hideProgressBar()
      {
         update(StatusBarElements.ProgressBar)
            .Visible(false)
            .And.WithValue(0);

         update(StatusBarElements.ProgressStatus)
            .WithCaption(string.Empty)
            .And.Visible(false);
      }

      public void Handle(StatusMessageEvent eventToHandle)
      {
         update(StatusBarElements.Status)
            .WithCaption(eventToHandle.Message)
            .And.ToolTipText(eventToHandle.Message);
      }

      public bool CanClose => true;

      public string ErrorMessage => string.Empty;

      public void ReleaseFrom(IEventPublisher eventPublisher)
      {
         eventPublisher.RemoveListener(this);
      }

      public void Handle(ReportCreationStartedEvent eventToHandle)
      {
         _numberOfReportsBeingCreated++;
         updateReportInfo();
      }

      public void Handle(ReportCreationFinishedEvent eventToHandle)
      {
         _numberOfReportsBeingCreated--;
         updateReportInfo();
      }
   }
}