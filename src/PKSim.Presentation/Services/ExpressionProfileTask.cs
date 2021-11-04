using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.ExpressionProfiles;

namespace PKSim.Presentation.Services
{
   public class ExpressionProfileTask: BuildingBlockTask<ExpressionProfile>, IExpressionProfileTask
   {
      public ExpressionProfileTask(IExecutionContext executionContext, IBuildingBlockTask buildingBlockTask, IApplicationController applicationController) :
         base(executionContext, buildingBlockTask, applicationController, PKSimBuildingBlockType.ExpressionProfile)
      {
      }

      public override ExpressionProfile AddToProject() => AddToProject<ICreateExpressionProfilePresenter>();
   }
}