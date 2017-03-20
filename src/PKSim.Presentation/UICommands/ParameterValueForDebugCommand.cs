using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.UICommands;

namespace PKSim.Presentation.UICommands
{
   public class ParameterValueForDebugCommand : ObjectUICommand<IPKSimBuildingBlock>
   {
      private readonly IApplicationController _applicationController;
      private readonly ILazyLoadTask _lazyLoadTask;

      public ParameterValueForDebugCommand(IApplicationController applicationController, ILazyLoadTask lazyLoadTask)
      {
         _applicationController = applicationController;
         _lazyLoadTask = lazyLoadTask;
      }

      protected override void PerformExecute()
      {
         _lazyLoadTask.Load(Subject);
         using (var presenter = _applicationController.Start<IParameterValuesDebugPresenter>())
         {
            presenter.ShowParametersFor(Subject);
         }
      }
   }
}