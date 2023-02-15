using System;
using OSPSuite.Utility.Container;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core
{
   /// <summary>
   ///    Register components in the container that are relevant for the PKSimStarter
   /// </summary>
   public class PKSimStarterRegistrationConvention : PKSimRegistrationConvention
   {
      public override void Process(Type concreteType, IContainer container, LifeStyle lifeStyle)
      {
         if (!shouldRegisterType(concreteType))
            return;

         base.Process(concreteType, container, lifeStyle);
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