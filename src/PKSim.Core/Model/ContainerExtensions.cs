using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using PKSim.Core.Extensions;

namespace PKSim.Core.Model
{
   public static class ContainerExtensions
   {
      /// <summary>
      ///    Returns all direct visible parameters (not in sub-containers)
      /// </summary>
      public static IEnumerable<IParameter> AllVisibleParameters(this IContainer container) => container.AllParameters(x => x.Visible);

      /// <summary>
      ///    Returns all user defined parameters (direct children and in sub-containers)
      /// </summary>
      public static IEnumerable<IParameter> AllUserDefinedParameters(this IContainer container) => container.GetAllChildren<IParameter>(x => !x.IsDefault);

      public static IEnumerable<string> AllParameterNames(this IContainer container) => container.AllParameters().Select(param => param.Name);

      /// <summary>
      ///    Returns true if the container is representing a generic compartment such as blood cells, plasma or endosome
      /// </summary>
      public static bool IsSurrogate(this IContainer container) => container.Name.IsSurrogate();

      /// <summary>
      ///    Returns true if the container is representing an undefined molecule.
      /// </summary>
      public static bool IsUndefinedMolecule(this IContainer container) => container.Name.IsUndefinedMolecule();

      public static bool IsEndosome(this IContainer container) => container.Name.IsEndosome();

      public static bool IsVascularEndothelium(this IContainer container) => container.Name.IsVascularEndothelium();

      public static bool IsBloodCell(this IContainer container) => container.Name.IsBloodCells();

      public static bool IsPlasma(this IContainer container) => container.Name.IsPlasma();

      public static bool IsLiver(this IContainer container) => container.Name.IsLiver();
      
      public static bool IsKidney(this IContainer container) => container.Name.IsKidney();
      
      public static bool IsBrain(this IContainer container) => container.Name.IsBrain();

      public static bool IsLumen(this IContainer container) => container.Name.IsLumen();

      public static bool IsBloodOrgan(this IContainer container)
      {
         return string.Equals(container.Name, CoreConstants.Organ.VENOUS_BLOOD) ||
                string.Equals(container.Name, CoreConstants.Organ.ARTERIAL_BLOOD) ||
                string.Equals(container.Name, CoreConstants.Organ.PORTAL_VEIN);
      }

      public static bool IsInterstitial(this IContainer container) => string.Equals(container.Name, CoreConstants.Compartment.INTERSTITIAL);

      public static bool IsMucosa(this IContainer container) => string.Equals(container.Name, CoreConstants.Compartment.MUCOSA);

      public static bool IsLumenOrMucosa(this IContainer container) => container.IsLumen() || container.IsMucosa();

      public static bool IsSegment(this IContainer container)
      {
         if (container.ContainerType != ContainerType.Compartment || container.ParentContainer == null)
            return false;

         return container.ParentContainer.IsLumenOrMucosa();
      }

      public static bool IsLiverZone(this IContainer container)
      {
         if (container.ContainerType != ContainerType.Compartment || container.ParentContainer == null)
            return false;

         return container.ParentContainer.IsLiver() && container.NameIsOneOf(CoreConstants.Compartment.LiverZones);
      }

      /// <summary>
      /// Returns <c>True</c> of the container is an organ with lumen, otherwise <c>False</c>.
      /// Typically, Kidney, Liver and all mucosal tissues
      /// </summary>
      public static bool IsOrganWithLumen(this IContainer container)
      {
         if (container.IsKidney())
            return true;

         // We use parent here as liver is split into zones
         if (container.ParentContainer.NameIsOneOf(CoreConstants.Organ.LIVER, CoreConstants.Compartment.MUCOSA))
            return true;

         return false;
      }
   }
}