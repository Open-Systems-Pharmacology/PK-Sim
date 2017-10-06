using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core
{
   /// <summary>
   ///    Register components in the container using following logic
   ///    1- if an interface IMyType for type MyType is found, the component is registerd with the given interface
   ///    2- if an interface starting with I in the PKSim namespace was defined for type MyType, and the flag
   ///    "registerWithBasedInterface" is true (default),
   ///    the component is registerd with the given interface
   ///    3- Otherwise, the component is registered as is
   /// </summary>
   public class PKSimRegistrationConvention : OSPSuiteRegistrationConvention
   {
      public override void Process(Type concreteType, IContainer container, LifeStyle lifeStyle)
      {
         if (concreteType.IsNested)
            return;

         if (Register(concreteType, container, lifeStyle))
            return;

         if (!shouldRegisterType(concreteType))
            return;

         var services = new HashSet<Type>();

         //Register using the first PK-Sim interface if available
         var pkSimInterface = concreteType
            .GetInterfaces()
            .Where(t => t.IsInterface)
            .FirstOrDefault(t => t.Namespace != null && t.Namespace.StartsWith("PKSim"));

         if (pkSimInterface != null)
            services.Add(pkSimInterface);

         services.Add(concreteType);

         if (concreteType.IsAnImplementationOf<IStartable>())
            services.Add(typeof(IStartable));

         container.Register(services.ToList(), concreteType, lifeStyle);
      }

      private bool shouldRegisterType(Type concreteType)
      {
         //do not register MetaData type
         if (concreteType.Name.EndsWith("MetaData"))
            return false;

         //do not register events
         if (concreteType.Namespace != null && concreteType.Namespace.StartsWith("PKSim.Core.Events"))
            return false;

         //do not register command
         if (concreteType.IsAnImplementationOf<ICommand>())
            return false;

         //do not register exception
         if (concreteType.IsAnImplementationOf<Exception>())
            return false;

         return true;
      }
   }
}