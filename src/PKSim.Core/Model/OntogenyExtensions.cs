using OSPSuite.Utility.Extensions;

namespace PKSim.Core.Model
{
   public static class OntogenyExtensions
   {
      public static bool IsDefined(this Ontogeny ontogeny)
      {
         return !ontogeny.IsUndefined();
      }

      public static bool IsUserDefined(this Ontogeny ontogeny)
      {
         return ontogeny.IsAnImplementationOf<UserDefinedOntogeny>();
      }

      public static bool IsUndefined(this Ontogeny ontogeny)
      {
         return ontogeny == null || ontogeny.IsAnImplementationOf<NullOntogeny>();
      }
   }
}