using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Presentation.UICommands;
using PKSim.UI.UICommands;

namespace PKSim.UI.Starter
{
   public class PKSimStarterUserInterfaceRegister : Register
   {
      public override void RegisterInContainer(IContainer container)
      {
         container.AddScanner(scan =>
         {
            scan.AssemblyContainingType<UserInterfaceRegister>();
            scan.WithConvention<PKSimStarterRegistrationConvention>();
         });
         container.Register<OSPSuite.UI.Services.IToolTipCreator, IToolTipCreator, ToolTipCreator>(LifeStyle.Transient);
         container.Register<IExitCommand, ExitCommand>();
      }
   }
}