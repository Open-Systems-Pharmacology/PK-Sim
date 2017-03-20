using OSPSuite.Utility;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Services
{
   
 
   public class SimpleIdGenerator : IIdGenerator
   {
      private int _counter;

      public string NewId()
      {
         return _counter++.ToString();
      }
   }
}