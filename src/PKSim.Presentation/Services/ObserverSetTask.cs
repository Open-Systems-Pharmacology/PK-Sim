using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Observers;

namespace PKSim.Presentation.Services
{
   public class ObserverSetTask : BuildingBlockTask<ObserverSet>, IObserverSetTask
   {
      public ObserverSetTask(
         IExecutionContext executionContext,
         IBuildingBlockTask buildingBlockTask,
         IApplicationController applicationController
      ) :
         base(executionContext, buildingBlockTask, applicationController, PKSimBuildingBlockType.ObserverSet)
      {
      }

      public override ObserverSet AddToProject()
      {
         return AddToProject<ICreateObserverSetPresenter>();
      }

      public ObserverSetMapping CreateObserverSetMapping() => CreateObserverSetMapping(All().FirstOrDefault());

      public ObserverSetMapping CreateObserverSetMapping(ObserverSet observerSet) =>
         new ObserverSetMapping {TemplateObserverSetId = observerSet?.Id ?? string.Empty};
   }
}