using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public class Gender : ObjectBase
   {
      public virtual string DisplayName { get; set; }
      public virtual int Index { get; set; }
   }
}