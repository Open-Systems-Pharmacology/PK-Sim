using System.Collections.Generic;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public class DynamicGroup : Group
   {
      public IEnumerable<IParameter> Parameters { get; private set; }

      public DynamicGroup(IEnumerable<IParameter> parameters)
      {
         Parameters = parameters;
      }
   }
}