using System;
using System.Collections.Generic;

namespace PKSim.Core.Extensions
{
   public static class ExceptionExtensions
   {
      /// <summary>
      ///    Returns <c>true</c> when <paramref name="exception" /> is or wraps an <see cref="OutOfMemoryException" />, e.g.
      ///    inside an <see cref="AggregateException" /> from a blocking wait or an inner exception from a reflection
      ///    invocation. An out-of-memory failure must fail the operation rather than degrade it silently.
      /// </summary>
      public static bool IsOutOfMemory(this Exception exception)
      {
         //iterative with a visited set instead of recursive: exception chains from interop or serialization can be
         //self-referencing, and this is called from exception filters where a stack overflow is unrecoverable
         var pending = new Stack<Exception>();
         var visited = new HashSet<Exception>();
         pending.Push(exception);

         while (pending.Count > 0)
         {
            var current = pending.Pop();
            if (current == null || !visited.Add(current))
               continue;

            switch (current)
            {
               case OutOfMemoryException:
                  return true;
               case AggregateException aggregateException:
                  foreach (var innerException in aggregateException.InnerExceptions)
                  {
                     pending.Push(innerException);
                  }

                  break;
               default:
                  pending.Push(current.InnerException);
                  break;
            }
         }

         return false;
      }
   }
}
