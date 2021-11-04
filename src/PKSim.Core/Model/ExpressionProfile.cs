using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public class ExpressionProfile : PKSimBuildingBlock
   {
      public virtual Species Species { get; set; }

      public ExpressionProfile() : base(PKSimBuildingBlockType.ExpressionProfile)
      {
      }
   }
}