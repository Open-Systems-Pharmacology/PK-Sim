using PKSim.Core;
using PKSim.Infrastructure;
using PKSim.Presentation.Core;
using PKSim.UI.BootStrapping;
using OSPSuite.Presentation.Services;
using OSPSuite.Utility.Container;
using PresenterRegister = PKSim.Presentation.PresenterRegister;

namespace PKSim.BatchTool
{
   public static class BatchStarter
   {
      public static void Start()
      {
         ApplicationStartup.Initialize();
         var container = IoC.Container;
         using (container.OptimizeDependencyResolution())
         {
            container.AddRegister(x => x.FromType<CoreRegister>());
            container.AddRegister(x => x.FromType<PresenterRegister>());
            container.AddRegister(x => x.FromType<InfrastructureRegister>());
            container.AddRegister(x => x.FromType<BatchRegister>());

            InfrastructureRegister.RegisterSerializationDependencies();
            ApplicationStartup.RegisterCommands(container);

            var workspace = container.Resolve<IWorkspace>();
            container.RegisterImplementationOf<IWithWorkspaceLayout>(workspace);
            container.RegisterImplementationOf<OSPSuite.Core.IWorkspace>(workspace);
         }
      }
   }
}