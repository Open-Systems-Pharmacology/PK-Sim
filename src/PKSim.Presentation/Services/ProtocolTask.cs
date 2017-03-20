using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Protocols;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Services
{
   public interface IProtocolTask : IBuildingBlockTask<Protocol>
   {
   }

   public class ProtocolTask : BuildingBlockTask<Protocol>, IProtocolTask
   {
      public ProtocolTask(IExecutionContext executionContext, IBuildingBlockTask buildingBlockTask, IApplicationController applicationController) :
         base(executionContext, buildingBlockTask, applicationController, PKSimBuildingBlockType.Protocol)
      {
      }

      public override Protocol AddToProject()
      {
         return AddToProject<ICreateProtocolPresenter>();
      }
   }
}