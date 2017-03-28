namespace PKSim.Infrastructure.ORM.Core
{
   public interface ITemplateDatabase : IDatabase
   {
   }

   public class TemplateDatabase : AccessDatabase, ITemplateDatabase
   {
   }
}