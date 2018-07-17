namespace PKSim.Infrastructure.ORM.Core
{
   public interface IGeneExpressionDatabase : IDatabase
   {
   }

   public class GeneExpressionDatabase : SQLiteDatabase, IGeneExpressionDatabase
   {
   }
}