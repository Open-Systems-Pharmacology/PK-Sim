using PKSim.Core;

namespace PKSim.Infrastructure.ORM.Core
{
   public interface IProteinExpressionDatabase : IDatabase
   {
   }

   public class ProteinExpressionDatabase : AccessDatabase, IProteinExpressionDatabase
   {
      public ProteinExpressionDatabase(): base(CoreConstants.ExpressionPassword)
      {
      }
   }
}