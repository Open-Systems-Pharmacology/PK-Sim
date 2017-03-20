using System;
using System.Linq;
using PKSim.Core.Services;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Container.Conventions;
using OSPSuite.Utility.Extensions;

namespace PKSim.Presentation
{
   /// <summary>
   ///    Registers each implementation of IBuildingBlockTask[T] using the IBuildingBlockTask[T] interface
   /// </summary>
   public class BuildingBlockTaskRegistrationConvention : IRegistrationConvention
   {
      public void Process(Type concreteType, IContainer container, LifeStyle lifeStyle)
      {
         var typeForGenericTypes = concreteType.GetDeclaredTypesForGeneric(typeof(IBuildingBlockTask<>)).ToList();
         if (!typeForGenericTypes.Any())
            return;

         container.Register(typeForGenericTypes.Select(x => x.GenericType).ToList(), concreteType, lifeStyle);
      }
   }
}