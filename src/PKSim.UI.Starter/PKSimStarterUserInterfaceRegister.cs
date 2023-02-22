using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Presentation.UICommands;
using PKSim.UI.UICommands;
using System;

namespace PKSim.UI.Starter
{
   public class PKSimStarterUserInterfaceRegister : Register
   {
      public override void RegisterInContainer(IContainer container)
      {
         container.AddScanner(scan =>
         {
            scan.AssemblyContainingType<UserInterfaceRegister>();
            scan.WithConvention<PKSimRegistrationConvention>();
            scan.Include(shouldRegisterType);
         });
         container.Register<OSPSuite.UI.Services.IToolTipCreator, IToolTipCreator, ToolTipCreator>(LifeStyle.Transient);
         container.Register<IExitCommand, ExitCommand>();
      }

      private bool shouldRegisterType(Type concreteType)
      {
         if (concreteType.FullName == null)
            return false;

         if (concreteType.FullName.Contains("ExpressionProfile"))
            return true;

         if (concreteType.FullName.Contains("Individual"))
            return true;

         if (concreteType.FullName.Contains("Parameter"))
            return true;

         if (concreteType.FullName.Contains("Mapper"))
            return true;

         if (concreteType.FullName.Contains("Factory"))
            return true;

         if (concreteType.FullName.Contains("Tooltip"))
            return true;

         return false;
      }
   }
}