using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters.Simulations;
using OSPSuite.Core.Services;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.UICommands;
using ILazyLoadTask = PKSim.Core.Services.ILazyLoadTask;

namespace PKSim.Presentation.UICommands
{
   public class ParameterExportForDebugCommand : ActiveObjectUICommand<Simulation>
   {
      private readonly IApplicationController _applicationController;
      private readonly ILazyLoadTask _lazyLoadTask;

      public ParameterExportForDebugCommand(IApplicationController applicationController, ILazyLoadTask lazyLoadTask, IActiveSubjectRetriever activeSubjectRetriever) : base(activeSubjectRetriever)
      {
         _applicationController = applicationController;
         _lazyLoadTask = lazyLoadTask;
      }

      protected override void PerformExecute()
      {
         _lazyLoadTask.Load(Subject);
         using (var presenter = _applicationController.Start<IParameterDebugPresenter>())
         {
            presenter.ShowParametersFor(Subject);
         }
      }
   }
}