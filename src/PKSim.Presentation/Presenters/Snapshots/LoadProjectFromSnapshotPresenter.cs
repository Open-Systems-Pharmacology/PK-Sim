using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Presentation.Views;
using OSPSuite.Utility.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots;
using PKSim.Core.Snapshots.Services;
using QualificationPlan = OSPSuite.Core.Domain.QualificationPlan;

namespace PKSim.Presentation.Presenters.Snapshots
{
   public interface ILoadProjectFromSnapshotPresenter : ILoadFromSnapshotPresenter<PKSimProject>
   {
      /// <summary>
      ///    Project loaded from selected snapshot file. It is null if the user cancels the action or if the file was not loaded
      ///    properly
      /// </summary>
      PKSimProject LoadProject();
   }

   public class LoadProjectFromSnapshotPresenter : LoadProjectFromSnapshotPresenter<PKSimProject, Project>, ILoadProjectFromSnapshotPresenter
   {
      private readonly IRegistrationTask _registrationTask;

      public LoadProjectFromSnapshotPresenter(ILoadFromSnapshotView view,
         ILogPresenter logPresenter,
         ISnapshotTask snapshotTask,
         IDialogCreator dialogCreator,
         IObjectTypeResolver objectTypeResolver,
         IOSPSuiteLogger logger,
         IEventPublisher eventPublisher,
         IQualificationPlanRunner qualificationPlanRunner,
         IRegistrationTask registrationTask) : base(view, logPresenter, snapshotTask, dialogCreator, objectTypeResolver, logger, eventPublisher, qualificationPlanRunner)
      {
         _registrationTask = registrationTask;
      }

      protected override IReadOnlyList<QualificationPlan> AllQualificationPlansFrom(PKSimProject project)
      {
         return project.AllQualificationPlans.ToList();
      }

      protected override void RegisterProject(PKSimProject project)
      {
         _registrationTask.RegisterProject(project);
      }

      protected override void UnRegisterProjects(List<PKSimProject> projects)
      {
         _registrationTask.UnregisterProject(ProjectFrom(projects));
      }
   }
}