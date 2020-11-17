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

      public static bool IsBloodCell(this IContainer container) => container.Name.IsBloodCell();

      public static bool IsPlasma(this IContainer container) => container.Name.IsPlasma();

      public static bool IsLiver(this IContainer container) => container.Name.IsLiver();

      public static bool IsLumen(this IContainer container) => container.Name.IsLumen();

      public static bool IsBloodOrgan(this IContainer container)
      {
         return string.Equals(container.Name, CoreConstants.Organ.VenousBlood) ||
                string.Equals(container.Name, CoreConstants.Organ.ArterialBlood) ||
                string.Equals(container.Name, CoreConstants.Organ.PortalVein);
      }

      public static bool IsInterstitial(this IContainer container) => string.Equals(container.Name, CoreConstants.Compartment.Interstitial);

      public static bool IsMucosa(this IContainer container) => string.Equals(container.Name, CoreConstants.Compartment.Mucosa);

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
         if (container.IsNamed(CoreConstants.Organ.Kidney))
            return true;

         if (container.ParentContainer.NameIsOneOf(CoreConstants.Organ.Liver, CoreConstants.Compartment.Mucosa))
            return true;

         return false;
      }
   }
}