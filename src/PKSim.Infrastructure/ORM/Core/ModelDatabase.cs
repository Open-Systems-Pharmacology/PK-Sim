namespace PKSim.Infrastructure.ORM.Core
{
   public interface IModelDatabase : IDatabase
   {
   }

   public class ModelDatabase : AccessDatabase, IModelDatabase
   {
   }
}