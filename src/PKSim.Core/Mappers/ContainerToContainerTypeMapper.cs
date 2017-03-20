using System;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Mappers
{
   public interface IContainerToContainerTypeMapper : IMapper<IContainer, PKSimContainerType>
   {
   }

   public class ContainerToContainerTypeMapper : IContainerToContainerTypeMapper
   {
      public PKSimContainerType MapFrom(IContainer container)
      {
         if (container.IsAnImplementationOf<Formulation>())
            return PKSimContainerType.Formulation;

         if (container.IsAnImplementationOf<Compartment>())
            return PKSimContainerType.Compartment;

         if (container.IsAnImplementationOf<Organ>())
            return PKSimContainerType.Organ;

         if (container.IsAnImplementationOf<Organism>())
            return PKSimContainerType.Organism;

         if (container.IsAnImplementationOf<Compound>())
            return PKSimContainerType.Compound;


         if (container.IsAnImplementationOf<Individual>())
            return PKSimContainerType.Root;

         throw new ArgumentException(string.Format("Container type not found for '{0}'", container.GetType()), "container");
      }
   }
}