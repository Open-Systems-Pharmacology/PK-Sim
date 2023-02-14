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

         if (concreteType.Name.EndsWith("CreateIndividualPresenterForMoBi"))
         {
            //Register using the first PK-Sim interface if available
            var pkSimInterface = concreteType
               .GetInterfaces()
               .Where(t => t.IsInterface)
               .FirstOrDefault(t => t.Namespace != null && t.Namespace.StartsWith("PKSim"));

            if (pkSimInterface != null)
               container.Register(pkSimInterface, concreteType, lifeStyle);

         }

         base.Process(concreteType, container, lifeStyle);
      }

      private bool shouldRegisterType(Type concreteType)
      {
         if (concreteType.Name.Contains("ExpressionProfile"))
            return true;

         if (concreteType.Name.Contains("Individual"))
            return true;

         if (concreteType.Name.Contains("Parameter"))
            return true;

         if (concreteType.Name.Contains("Mapper"))
            return true;

         if (concreteType.Name.Contains("Factory"))
            return true;

         return false;
      }
   }
}