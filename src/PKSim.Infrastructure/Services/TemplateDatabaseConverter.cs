using System.Data.OleDb;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.DAS;

namespace PKSim.Infrastructure.Services
{
   public interface ITemplateDatabaseConverter
   {
      void Convert(ITemplateDatabase templateDatabase);
   }

   public class TemplateDatabaseConverter : ITemplateDatabaseConverter
   {
      public void Convert(ITemplateDatabase templateDatabase)
      {
         // Nothing to do starting 7.4
      }

      private bool needsConverstion(DAS database, string queryToExecute)
      {
         try
         {
            database.CreateAndFillDataTable(queryToExecute);
            return false;
         }
         catch (OleDbException)
         {
            return true;
         }
      }
   }
}