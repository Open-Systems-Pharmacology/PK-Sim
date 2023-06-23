using OSPSuite.Core.Domain;
using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Model.Extensions
{
   public static class EntityExtensions
   {
      public static bool IsInNeighborhood(this IEntity entity)
      {
         if (entity == null)
            return false;

         if (entity.IsAnImplementationOf<Neighborhood>())
            return true;

         return IsInNeighborhood(entity.ParentContainer);
      }

      public static bool IsInMucosa(this IEntity entity)
      {
         if (entity.HasAncestorNamed(CoreConstants.Compartment.MUCOSA))
            return true;

         var neighborhood = entity.NeighborhoodAncestor();
         if (neighborhood == null)
            return false;

         return
            IsInMucosa(neighborhood.FirstNeighbor) ||
            IsInMucosa(neighborhood.SecondNeighbor);
      }

      public static bool IsInLumen(this IEntity entity)
      {
         if (entity.HasAncestorNamed(CoreConstants.Organ.LUMEN))
            return true;

         var neighborhood = entity.NeighborhoodAncestor();
         if (neighborhood == null)
            return false;

         return
            IsInLumen(neighborhood.FirstNeighbor) ||
            IsInLumen(neighborhood.SecondNeighbor);
      }

      public static bool IsInLiverZone(this IEntity entity)
      {
         var parentContainer = entity.ParentContainer;
         if (parentContainer == null)
            return false;

         return parentContainer.IsLiverZone() || IsInLiverZone(parentContainer);
      }

      public static bool IsInLumenOrMucosa(this IEntity entity)
      {
         return entity.IsInLumen() || entity.IsInMucosa();
      }

      public static Neighborhood NeighborhoodAncestor(this IEntity entity)
      {
         if (entity == null) return null;
         if (entity.IsAnImplementationOf<Neighborhood>())
            return entity.DowncastTo<Neighborhood>();

         return NeighborhoodAncestor(entity.ParentContainer);
      }
   }
}