using OSPSuite.Core.Domain;

namespace PKSim.Core.Extensions
{
   //TODO MOVE TO CORE
   public static class ValueOriginExtensions
   {
      public static bool DiffersFrom(this ValueOrigin valueOrigin, ValueOrigin otherValueOrigin)
      {
         return !IsIdenticalTo(valueOrigin, otherValueOrigin);
      }

      public static bool IsIdenticalTo(this ValueOrigin valueOrigin, ValueOrigin otherValueOrigin)
      {
         return valueOrigin.Default == otherValueOrigin.Default &&
                valueOrigin.Source == otherValueOrigin.Source &&
                valueOrigin.Method == otherValueOrigin.Method &&
                string.Equals(valueOrigin.Description, otherValueOrigin.Description);
      }
   }
}