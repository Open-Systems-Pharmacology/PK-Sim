using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Presenters;
using OSPSuite.Utility.Events;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Core.Snapshots.Services;
using PKSim.Presentation.DTO.Snapshots;
using PKSim.Presentation.Views.Snapshots;

namespace PKSim.Presentation.Presenters.Snapshots
{
   public interface ILoadProjectFromSnapshotPresenter : ILoadFromSnapshotPresenter<PKSimProject>
   {
      /// <summary>
      /// Project loaded from selected snapshot file. It is null if the user cancels the action or if the file was not loaded properly
      /// </summary>
      PKSimProject LoadProject();

   }

   public class LoadProjectFromSnapshotPresenter : LoadFromSnapshotPresenter<PKSimProject>, ILoadProjectFromSnapshotPresenter
   {
      private readonly IQualiticationPlanRunner _qualificationPlanRunner;
      private readonly IRegistrationTask _registrationTask;

      public LoadProjectFromSnapshotPresenter(ILoadFromSnapshotView view,
         ILogPresenter logPresenter,
         ISnapshotTask snapshotTask,
         IDialogCreator dialogCreator,
         IObjectTypeResolver objectTypeResolver,
         IOSPSuiteLogger logger,
         IEventPublisher eventPublisher,
         IQualiticationPlanRunner qualificationPlanRunner,
         IRegistrationTask registrationTask) : base(view, logPresenter, snapshotTask, dialogCreator, objectTypeResolver, logger, eventPublisher)
      {
         _qualificationPlanRunner = qualificationPlanRunner;
         _registrationTask = registrationTask;
      }

      public PKSimProject LoadProject()
      {
         var models = LoadModelFromSnapshot();
         return models?.FirstOrDefault();
      }

      protected override async Task<IEnumerable<PKSimProject>> LoadModelAsync(LoadFromSnapshotDTO loadFromSnapshotDTO)
      {
         var project = await _snapshotTask.LoadProjectFromSnapshotFileAsync(loadFromSnapshotDTO.SnapshotFile, loadFromSnapshotDTO.RunSimulations);
         _registrationTask.RegisterProject(project);
         await runQualificationPlans(project);
         return new[] { project };
      }

      protected override void ClearModel(IEnumerable<PKSimProject> model)
      {
         var projects = model?.ToList();
         base.ClearModel(projects);
         _registrationTask.UnregisterProject(projectFrom(projects));
      }

      private PKSimProject projectFrom(IEnumerable<PKSimProject> projects) => projects?.FirstOrDefault();

      private async Task runQualificationPlans(PKSimProject project)
      {
         //needs to be done sequentially
         foreach (var qualificationPlan in project.AllQualificationPlans)
         {
            await _qualificationPlanRunner.RunAsync(qualificationPlan);
         }
      }
   }
}