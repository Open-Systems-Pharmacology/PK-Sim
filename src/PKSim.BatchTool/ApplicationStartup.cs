using DevExpress.LookAndFeel;
using Microsoft.Extensions.Logging;
using OSPSuite.Presentation;
using OSPSuite.Presentation.Services;
using OSPSuite.Utility.Container;
using PKSim.CLI.Core;
using PKSim.Core;
using PKSim.Infrastructure;
using PresenterRegister = PKSim.Presentation.PresenterRegister;

namespace PKSim.BatchTool
{
   public static class ApplicationStartup
   {
      public static void Start()
      {
         UI.BootStrapping.ApplicationStartup.Initialize(LogLevel.Debug);
         var container = IoC.Container;
         using (container.OptimizeDependencyResolution())
         {
            container.AddRegister(x => x.FromType<CoreRegister>());
            container.AddRegister(x => x.FromType<PresenterRegister>());
            container.AddRegister(x => x.FromType<InfrastructureRegister>());
            container.AddRegister(x => x.FromType<BatchRegister>());
            container.AddRegister(x => x.FromType<CLIRegister>());

            InfrastructureRegister.RegisterSerializationDependencies();
            InfrastructureRegister.RegisterWorkspace();
            UI.BootStrapping.ApplicationStartup.RegisterCommands(container);
            container.RegisterImplementationOf(new DefaultLookAndFeel().LookAndFeel);
         }

         var skinManager = container.Resolve<ISkinManager>();
         var userSettings = container.Resolve<IPresentationUserSettings>();
         skinManager.ActivateSkin(userSettings, Constants.DEFAULT_SKIN);
      }
   }
}