using static OSPSuite.Core.Domain.Constants;

namespace PKSim.Core.Model
{
   public class CompoundGroupSelection
   {
      public string AlternativeName { get; set; }
      public string GroupName { get; set; }

      public override string ToString()
      {
         return CompositeNameFor(GroupName, AlternativeName);
      }
   }
}