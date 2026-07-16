using PKSim.Infrastructure.ORM.Core;

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
   }
}