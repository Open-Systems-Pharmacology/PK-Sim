﻿using System.Data.SQLite;
using PKSim.Core.Extensions;
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
         using (var sqlLite = new SQLiteConnection(string.Format("Data Source={0}", file)))
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