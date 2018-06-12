using System.Data.SQLite;
using OSPSuite.Core.Extensions;

namespace PKSim.Infrastructure.Services
{
   public interface IProjectFileCompressor
   {
      void Compress(string projectFile);
   }

   public class ProjectFileCompressor : IProjectFileCompressor
   {
      public void Compress(string projectFile)
      {
         string file = projectFile.ToUNCPath();
         using (var sqlLite = new SQLiteConnection($"Data Source={file}"))
         {
            sqlLite.Open();
            vacuum(sqlLite);
         }
      }

      private static void vacuum(SQLiteConnection sqlLite)
      {
         sqlLite.ExecuteNonQuery("vacuum;");
      }
   }
}