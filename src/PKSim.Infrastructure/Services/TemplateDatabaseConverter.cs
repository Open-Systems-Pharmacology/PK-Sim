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
         if (needsConversionTo53Format(templateDatabase.DatabaseObject))
            convertDataBaseSchemaTo53(templateDatabase.DatabaseObject);

         if (needsConversionTo56Format(templateDatabase.DatabaseObject))
            convertDataBaseSchemaTo56(templateDatabase.DatabaseObject);
      }

      private void convertDataBaseSchemaTo56(DAS database)
      {
         //CREATION OF TABLE TAB_TEMPLATE_REFERENCES
         database.ExecuteSQL("CREATE TABLE TAB_TEMPLATE_REFERENCES (TEMPLATE_TYPE TEXT(50) NOT NULL, NAME TEXT(255) NOT NULL, REFERENCE_TEMPLATE_TYPE TEXT(50) NOT NULL, REFERENCE_NAME TEXT(255) NOT NULL , CONSTRAINT TEMPLATES_PK PRIMARY KEY(TEMPLATE_TYPE, NAME,REFERENCE_TEMPLATE_TYPE, REFERENCE_NAME))");

         //CREATION OF INDEXES
         //CREATION OF REFERENTIAL INTEGRITY
         database.ExecuteSQL("ALTER TABLE TAB_TEMPLATE_REFERENCES ADD CONSTRAINT RI_TEMPLATE_REFERENCES_1 FOREIGN KEY (TEMPLATE_TYPE,NAME) REFERENCES TAB_TEMPLATES (TEMPLATE_TYPE,NAME) ON UPDATE CASCADE ON DELETE CASCADE");
         database.ExecuteSQL("ALTER TABLE TAB_TEMPLATE_REFERENCES ADD CONSTRAINT RI_TEMPLATE_REFERENCES_2 FOREIGN KEY (REFERENCE_TEMPLATE_TYPE,REFERENCE_NAME) REFERENCES TAB_TEMPLATES (TEMPLATE_TYPE,NAME) ON UPDATE CASCADE ON DELETE CASCADE");
      }

      private void convertDataBaseSchemaTo53(DAS database)
      {
         //SAVE TEMPLATES
         database.ExecuteSQL("SELECT * INTO SAVED_TEMPLATES FROM TAB_TEMPLATES");

         //DROP OLD SCHEMA
         //DROP OF REFERENTIAL INTEGRITY
         //DROP OF CONSTRAINT RI_BUILDING_BLOCK_TYPE_METADATA_1
         database.ExecuteSQL("ALTER TABLE TAB_BUILDING_BLOCK_TYPE_METADATA DROP CONSTRAINT RI_BUILDING_BLOCK_TYPE_METADATA_1");
         //DROP OF CONSTRAINT RI_TEMPLATES_1
         database.ExecuteSQL("ALTER TABLE TAB_TEMPLATES DROP CONSTRAINT RI_TEMPLATES_1");
         //DROP OF CONSTRAINT RI_TEMPLATE_METADATA_1
         database.ExecuteSQL("ALTER TABLE TAB_TEMPLATE_METADATA DROP CONSTRAINT RI_TEMPLATE_METADATA_1");
         //DROP OF CONSTRAINT RI_TEMPLATE_METADATA_2
         database.ExecuteSQL("ALTER TABLE TAB_TEMPLATE_METADATA DROP CONSTRAINT RI_TEMPLATE_METADATA_2");

         //DROP OF CHECK CONSTRAINTS
         database.ExecuteSQL("ALTER TABLE TAB_TEMPLATE_METADATA DROP CONSTRAINT TEMPLATE_METADATA_C1");
         database.ExecuteSQL("ALTER TABLE TAB_TEMPLATES DROP CONSTRAINT TEMPLATES_C1");

         //DROP ALL TABLES
         database.ExecuteSQL("DROP TABLE TAB_BUILDING_BLOCK_TYPES");
         database.ExecuteSQL("DROP TABLE TAB_BUILDING_BLOCK_TYPE_METADATA");
         database.ExecuteSQL("DROP TABLE TAB_TEMPLATES");
         database.ExecuteSQL("DROP TABLE TAB_TEMPLATE_METADATA");

         //CREATE NEW SCHEMA
         //CREATION OF DATABASE SCHEMA TemplateDB
         //CREATION OF TABLES
         //CREATION OF TABLE TAB_OBJECT_TYPES
         database.ExecuteSQL("CREATE TABLE TAB_TEMPLATE_TYPES (TEMPLATE_TYPE TEXT(50) NOT NULL, CONSTRAINT TEMPLATE_TYPES_PK PRIMARY KEY (TEMPLATE_TYPE))");
         //CREATION OF TABLE TAB_TEMPLATES
         database.ExecuteSQL("CREATE TABLE TAB_TEMPLATES (TEMPLATE_TYPE TEXT(50) NOT NULL, NAME TEXT(255) NOT NULL, DESCRIPTION MEMO, XML MEMO NOT NULL, CONSTRAINT TEMPLATES_PK PRIMARY KEY(TEMPLATE_TYPE, NAME))");

         //CREATION OF INDEXES
         //CREATION OF REFERENTIAL INTEGRITY
         database.ExecuteSQL("ALTER TABLE TAB_TEMPLATES ADD CONSTRAINT RI_TEMPLATES_1 FOREIGN KEY (TEMPLATE_TYPE) REFERENCES TAB_TEMPLATE_TYPES (TEMPLATE_TYPE) ON UPDATE CASCADE");

         //RESTORE SAVED TEMPLATES
         database.ExecuteSQL("INSERT INTO TAB_TEMPLATE_TYPES (TEMPLATE_TYPE) SELECT DISTINCT s.BUILDING_BLOCK_TYPE FROM SAVED_TEMPLATES s");
         database.ExecuteSQL("INSERT INTO TAB_TEMPLATES (TEMPLATE_TYPE, NAME, DESCRIPTION, XML) SELECT s.BUILDING_BLOCK_TYPE, s.NAME, s.DESCRIPTION, s.XML FROM SAVED_TEMPLATES s");

         //DROP TEMPORARY TABLE
         database.ExecuteSQL("DROP TABLE SAVED_TEMPLATES");
      }

      private bool needsConversionTo56Format(DAS database)
      {
         return needsConverstion(database, "SELECT COUNT(*) FROM TAB_TEMPLATE_REFERENCES");
      }

      private bool needsConversionTo53Format(DAS database)
      {
         return needsConverstion(database, "SELECT COUNT(*) FROM TAB_TEMPLATE_TYPES");
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