using System;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public class RootContainer : Container, IRootContainer
   {
      public RootContainer()
      {
         Name = Constants.ROOT;
         Mode = ContainerMode.Logical;
         Id = Guid.NewGuid().ToString();
      }
   }
}