using System.Collections.Generic;
using System.Linq;
using PKSim.Core.Extensions;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public static class ContainerExtensions
   {
      public static IEnumerable<IParameter> AllVisibleParameters(this IContainer container) => container.AllParameters(x => x.Visible);

      public static IEnumerable<IParameter> AllUserDefinedParameters(this IContainer container) => container.AllParameters(x => !x.IsDefault);

      public static IEnumerable<string> AllParameterNames(this IContainer container)
      {
         return container.AllParameters().Select(param => param.Name);
      }

      /// <summary>
      ///    Returns true if the container is representing a generic compartment such as blood cells, plasma or endosome
      /// </summary>
      public static bool IsSurrogate(this IContainer container)
      {
         return container.Name.IsSurrogate();
      }

      /// <summary>
      ///    Returns true if the container is representing an undefined molecule.
      /// </summary>
      public static bool IsUndefinedMolecule(this IContainer container)
      {
         return container.Name.IsUndefinedMolecule();
      }

      public static bool IsEndosome(this IContainer container)
      {
         return container.Name.IsEndosome();
      }

      public static bool IsVascularEndothelium(this IContainer container)
      {
         return container.Name.IsVascularEndothelium();
      }

      public static bool IsBloodCell(this IContainer container)
      {
         return container.Name.IsBloodCell();
      }

      public static bool IsPlasma(this IContainer container)
      {
         return container.Name.IsPlasma();
      }

      public static bool IsLiver(this IContainer container)
      {
         return container.Name.IsLiver();
      }

      public static bool IsLumen(this IContainer container)
      {
         return container.Name.IsLumen();
      }

      public static bool IsBloodOrgan(this IContainer container)
      {
         return string.Equals(container.Name, CoreConstants.Organ.VenousBlood) ||
                string.Equals(container.Name, CoreConstants.Organ.ArterialBlood) ||
                string.Equals(container.Name, CoreConstants.Organ.PortalVein);
      }

      public static bool IsInterstitial(this IContainer container)
      {
         return string.Equals(container.Name, CoreConstants.Compartment.Interstitial);
      }

      public static bool IsMucosa(this IContainer container)
      {
         return string.Equals(container.Name, CoreConstants.Compartment.Mucosa);
      }

      public static bool IsLumenOrMucosa(this IContainer container)
      {
         return container.IsLumen() || container.IsMucosa();
      }

      public static bool IsSegment(this IContainer container)
      {
         if (container.ContainerType!=ContainerType.Compartment || container.ParentContainer == null)
            return false;

         return container.ParentContainer.IsLumenOrMucosa();
      }

      public static bool IsLiverZone(this IContainer container)
      {
         if (container.ContainerType != ContainerType.Compartment || container.ParentContainer == null)
            return false;

         return container.ParentContainer.IsLiver() && container.NameIsOneOf(CoreConstants.Compartment.LiverZones);
      }

   }
}