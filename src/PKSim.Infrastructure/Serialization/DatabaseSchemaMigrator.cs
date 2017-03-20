using System;
using System.Data;
using System.Data.SQLite;
using OSPSuite.Core.Extensions;

namespace PKSim.Infrastructure.Serialization
{
   /// <summary>
   ///    handles all sql conversion that should be done before loading a project (schema changes etc that NHinbernate does
   ///    not support out of the box)
   /// </summary>
   public interface IDatabaseSchemaMigrator
   {
      void MigrateSchema(string fileFullPath);
   }

   public class DatabaseSchemaMigrator : IDatabaseSchemaMigrator
   {
      public void MigrateSchema(string fileFullPath)
      {
         var path = fileFullPath.ToUNCPath();
         using (var sqlLite = new SQLiteConnection($"Data Source={path}"))
         {
            sqlLite.Open();
            migrateTo5_3(sqlLite);
            migrateTo6_2(sqlLite);
         }
      }

      //change from 5.2 to 5.3
      private void migrateTo5_3(SQLiteConnection sqlite)
      {
         if (!needsConversionTo5_3(sqlite)) return;

         //rename table SUMMARY_CHART to SIMULATION_COMPARISONS
         sqlite.ExecuteNonQuery("ALTER TABLE SUMMARY_CHART RENAME TO SIMULATION_COMPARISONS");

         //create new table for individual sim comparisons (rest will be done automatically by NHibernate schema update)
         var query = @"CREATE TABLE INDIVIDUAL_SIMULATION_COMPARISONS (
                        Id TEXT NOT NULL,
                        PRIMARY KEY (Id),
                        CONSTRAINT fk_SimulationComparision_IndividualSimulationComparison FOREIGN KEY (Id) REFERENCES SIMULATION_COMPARISONS)";

         sqlite.ExecuteNonQuery(query);

         //last: Copy all previous data from SimulationComparisons into INDIVIDUAL_SIMULATION_COMPARISONS

         query = "SELECT Id FROM SIMULATION_COMPARISONS";
         foreach (DataRow allSummaryChartIds in sqlite.ExecuteQueryForDataTable(query).Rows)
         {
            sqlite.ExecuteNonQuery($"INSERT INTO INDIVIDUAL_SIMULATION_COMPARISONS (Id) VALUES ('{allSummaryChartIds.StringAt("Id")}')");
         }
      }

      //change from 6.1 to 6.2
      private void migrateTo6_2(SQLiteConnection sqlite)
      {
         if (!needsConversionTo6_2(sqlite)) return;

         //rename table SUMMARY_CHART to SIMULATION_COMPARISONS
         sqlite.ExecuteNonQuery("ALTER TABLE OBSERVED_DATA RENAME TO USED_OBSERVED_DATA");
      }

      private bool needsConversionTo6_2(SQLiteConnection sqlite)
      {
         //USED_OBSERVED_DATA table was added in 6.2
         return !hasTable(sqlite, "USED_OBSERVED_DATA");
      }

      private bool needsConversionTo5_3(SQLiteConnection sqlLite)
      {
         //SIMULATION_COMPARISONS table was added in 5.3
         return !hasTable(sqlLite, "SIMULATION_COMPARISONS");
      }

      private static bool hasTable(SQLiteConnection sqlLite, string tableName)
      {
         try
         {
            sqlLite.ExecuteQueryForSingleRow($"SELECT COUNT(*) FROM {tableName}");
            return true;
         }
         catch (Exception)
         {
            return false;
         }
      }
   }
}