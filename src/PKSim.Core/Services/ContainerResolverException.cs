using System;

namespace PKSim.Core.Services
{
   public class ContainerResolverException : Exception
   {
      public ContainerResolverException(string message) : base(message)
      {
      }
   }
}