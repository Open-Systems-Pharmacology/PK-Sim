using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;
using OSPSuite.Utility;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.Extensions;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.DAS;

namespace PKSim.Infrastructure.Services
{
   public class TemplateTaskQuery : ITemplateTaskQuery
   {
      private readonly IPKSimConfiguration _pkSimConfiguration;
      private readonly ICoreUserSettings _userSettings;
      private readonly ITemplateDatabaseConverter _templateDatabaseConverter;
      private readonly IObjectIdResetter _objectIdResetter;
      private readonly ITemplateDatabase _templateDatabase;
      private readonly IStringSerializer _stringSerializer;
      private const string _pName = "@PName";

      public TemplateTaskQuery(ITemplateDatabase templateDatabase, IStringSerializer stringSerializer,
         IObjectIdResetter objectIdResetter, IPKSimConfiguration pkSimConfiguration, ICoreUserSettings userSettings, ITemplateDatabaseConverter templateDatabaseConverter)
      {
         _objectIdResetter = objectIdResetter;
         _pkSimConfiguration = pkSimConfiguration;
         _userSettings = userSettings;
         _templateDatabaseConverter = templateDatabaseConverter;
         _templateDatabase = templateDatabase;
         _stringSerializer = stringSerializer;
      }

      private DAS databaseConnection()
      {
         return _templateDatabase.DatabaseObject;
      }

      public IEnumerable<Template> AllTemplatesFor(TemplateDatabaseType templateDatabaseType, TemplateType templateType)
      {
         var allTemplates = new List<Template>();
         using (establishConnection(templateDatabaseType))
         {
            var connection = databaseConnection();

            var sqlQuery = string.Format("SELECT t.{0}, t.{1} FROM {2} t WHERE t.{0} IN ({3})",
               TemplateTable.Columns.TEMPLATE_TYPE, TemplateTable.Columns.NAME, TemplateTable.NAME, typeFrom(templateType));

            foreach (DASDataRow row in connection.ExecuteQueryForDataTable(sqlQuery))
            {
               var template = loadTemplateBy(templateDatabaseType, row.StringAt(TemplateTable.Columns.NAME), row.StringAt(TemplateTable.Columns.TEMPLATE_TYPE), connection);
               addReferencesToTemplate(template, connection);
               allTemplates.Add(template);
            }
         }

         return allTemplates;
      }

      private void addReferencesToTemplate(Template template, DAS connection, List<string> loadedReferenceNames = null)
      {
         DASDataTable dataTable = null;
         loadedReferenceNames = loadedReferenceNames ?? new List<string>();

         try
         {
            addTemplateNameParameter(template.Name, connection);

            var sqlQuery = string.Format("SELECT ref.{2}, ref.{3}  FROM {4} ref WHERE ref.{0} = '{5}' AND ref.{1}={6}",
               TemplateReferenceTable.Columns.TEMPLATE_TYPE, TemplateReferenceTable.Columns.NAME,
               TemplateReferenceTable.Columns.REFERENCE_TEMPLATE_TYPE, TemplateReferenceTable.Columns.REFERENCE_NAME,
               TemplateReferenceTable.NAME, template.TemplateType, _pName);

            dataTable = connection.ExecuteQueryForDataTable(sqlQuery);
         }
         finally
         {
            removeNameParameter(connection);
         }

         foreach (DASDataRow row in dataTable)
         {
            var reference = loadTemplateBy(template.DatabaseType, row.StringAt(TemplateReferenceTable.Columns.REFERENCE_NAME), row.StringAt(TemplateReferenceTable.Columns.REFERENCE_TEMPLATE_TYPE), connection);
            template.References.Add(reference);

            if (loadedReferenceNames.Contains(reference.Name))
               continue;

            loadedReferenceNames.Add(reference.Name);

            addReferencesToTemplate(reference, connection, loadedReferenceNames);
         }
      }

      private Template loadTemplateBy(TemplateDatabaseType templateDatabaseType, string templateName, string templateType, DAS connection)
      {
         try
         {
            addTemplateNameParameter(templateName, connection);

            var sqlQuery = string.Format("SELECT t.{2} FROM {3} t WHERE t.{0} = '{4}' AND t.{1}={5}",
               TemplateTable.Columns.TEMPLATE_TYPE, TemplateTable.Columns.NAME,
               TemplateTable.Columns.DESCRIPTION, TemplateTable.NAME, templateType, _pName);


            var row = connection.ExecuteQueryForSingleRow(sqlQuery);
            return new Template
            {
               DatabaseType = templateDatabaseType,
               TemplateType = EnumHelper.ParseValue<TemplateType>(templateType),
               Name = templateName,
               Description = row.StringAt(TemplateTable.Columns.DESCRIPTION)
            };
         }
         finally
         {
            removeNameParameter(connection);
         }
      }

      private string typeFrom(TemplateType templateType)
      {
         var result = new List<string>();
         allPrimitiveTypes().Where(pt => templateType.Is(pt)).Each(t => result.Add(t.ToString()));
         return result.ToString(",", "'");
      }

      public bool IsPrimitiveType(TemplateType templateType)
      {
         return allPrimitiveTypes().Contains(templateType);
      }

      private IEnumerable<TemplateType> allPrimitiveTypes()
      {
         return EnumHelper.AllValuesFor<TemplateType>();
      }

      public void RenameTemplate(Template buildingBlockTemplate, string newName)
      {
         using (establishConnection(buildingBlockTemplate.DatabaseType))
         {
            //Is there an user template with that name for given building block type
            var template = createTemplateRow(buildingBlockTemplate);

            //should never be the case when renaming
            if (!template.ExistsInDB())
               return;

            template[TemplateTable.Columns.NAME] = newName;

            //could crash if already exists with the same name
            template.UpdateInDB();

            //it worked! rename the bb as well
            buildingBlockTemplate.Name = newName;
         }
      }

      public void DeleteTemplate(Template templateToDelete)
      {
         using (establishConnection(templateToDelete.DatabaseType))
         {
            //Is there an user template with that name for given building block type
            var template = createTemplateRow(templateToDelete);
            if (!template.ExistsInDB())
               return;

            template.Delete();
            template.DeleteFromDB();
         }
      }

      public IEnumerable<Template> AllTemplatesFor(TemplateType templateType)
      {
         var buildingBlockTemplates = new List<Template>();
         buildingBlockTemplates.AddRange(AllTemplatesFor(TemplateDatabaseType.System, templateType));
         buildingBlockTemplates.AddRange(AllTemplatesFor(TemplateDatabaseType.User, templateType));
         return buildingBlockTemplates;
      }

      public T LoadTemplate<T>(Template template)
      {
         using (establishConnection(template.DatabaseType))
         {
            var connection = databaseConnection();
            try
            {
               addTemplateNameParameter(template.Name, connection);

               var sqlQuery = $"SELECT t.{TemplateTable.Columns.XML} FROM {TemplateTable.NAME} t WHERE t.{TemplateTable.Columns.TEMPLATE_TYPE} IN ({typeFrom(template.TemplateType)}) AND t.{TemplateTable.Columns.NAME} = {_pName}";

               var query = new DASDataTable(connection);
               connection.FillDataTable(query, sqlQuery);

               if (query.Rows.Count() > 0)
               {
                  var serializationString = query.Rows.ItemByIndex(0)[TemplateTable.Columns.XML].ToString();
                  var objectFromTemplate = _stringSerializer.Deserialize<T>(serializationString);
                  _objectIdResetter.ResetIdFor(objectFromTemplate);

                  //Rename the template according to the template name as the template might have been renamed
                  var withName = objectFromTemplate as IWithName;
                  if (withName != null)
                     withName.Name = template.Name;

                  return objectFromTemplate;
               }
            }
            finally
            {
               removeNameParameter(connection);
            }

            return default(T);
         }
      }

      private static void addTemplateNameParameter(string templateName, DAS connection)
      {
         connection.AddParameter(_pName, templateName, DAS.ParameterModes.PARM_IN, DAS.ServerTypes.ST_VARCHAR2);
      }

      private static void removeNameParameter(DAS connection)
      {
         connection.RemoveParameter(_pName);
      }

      private DASDataRow createTemplateRow(Template buildingBlockTemplate)
      {
         return createTemplateRow(buildingBlockTemplate.TemplateType, buildingBlockTemplate.Name);
      }

      private DASDataRow createTemplateRow(TemplateType templateType, string name)
      {
         var keys = new Cache<string, string>
         {
            {TemplateTable.Columns.TEMPLATE_TYPE, templateType.ToString()},
            {TemplateTable.Columns.NAME, name},
         };

         var defaultValues = new Cache<string, string>
         {
            {TemplateTable.Columns.XML, string.Empty}
         };

         var templateRow = createNewRow(TemplateTable.NAME, keys, defaultValues);

         if (templateRow.ExistsInDB())
         {
            templateRow.SelectFromDB();
            templateRow.AcceptChanges();
         }

         return templateRow;
      }

      private DASDataRow createReferenceRow(Template templateItem, Template reference)
      {
         var keys = new Cache<string, string>
         {
            {TemplateReferenceTable.Columns.TEMPLATE_TYPE, templateItem.TemplateType.ToString()},
            {TemplateReferenceTable.Columns.NAME, templateItem.Name},
            {TemplateReferenceTable.Columns.REFERENCE_TEMPLATE_TYPE, reference.TemplateType.ToString()},
            {TemplateReferenceTable.Columns.REFERENCE_NAME, reference.Name},
         };
         return createNewRow(TemplateReferenceTable.NAME, keys);
      }

      private void createBuildingBlockTypeIfMissing(TemplateType templateType)
      {
         var keys = new Cache<string, string> {{TemplateTypeTable.Columns.TEMPLATE_TYPE, templateType.ToString()}};
         var newRow = createNewRow(TemplateTypeTable.NAME, keys);
         if (newRow.ExistsInDB()) return;
         newRow.InsertIntoDB();
      }

      private DASDataRow createNewRow(string tableName, ICache<string, string> primaryKeys, ICache<string, string> defaultValues = null)
      {
         var dataTable = databaseConnection().CreateDASDataTable(tableName);
         dataTable.PrimaryKey = primaryKeys.Keys.Select(x => dataTable.Columns.ItemByName(x)).ToArray();
         var row = dataTable.NewRow().DowncastTo<DASDataRow>();
         primaryKeys.KeyValues.Each(kv => { row[kv.Key] = kv.Value; });
         defaultValues?.KeyValues.Each(kv => { row[kv.Key] = kv.Value; });
         dataTable.Rows.Add(row);
         return row;
      }

      public void SaveToTemplate(IReadOnlyList<Template> templateItems)
      {
         if (!templateItems.Any()) return;
         var templateDatabaseType = templateItems[0].DatabaseType;

         using (establishConnection(templateDatabaseType))
         {
            foreach (var templateItem in templateItems)
            {
               createBuildingBlockTypeIfMissing(templateItem.TemplateType);

               //Is there a user template with that name for given building block type
               var newTemplate = createTemplateRow(templateItem.TemplateType, templateItem.Name);
               var xml = _stringSerializer.Serialize(templateItem.Object);
               newTemplate[TemplateTable.Columns.DESCRIPTION] = templateItem.Description;
               newTemplate[TemplateTable.Columns.XML] = xml;

               if (newTemplate.ExistsInDB())
                  newTemplate.UpdateInDB();
               else
                  newTemplate.InsertIntoDB();
            }

            foreach (var templateItem in templateItems)
            {
               foreach (var reference in templateItem.References)
               {
                  var newReference = createReferenceRow(templateItem, reference);
                  if (newReference.ExistsInDB())
                     continue;

                  newReference.InsertIntoDB();
               }
            }
         }
      }

      public void SaveToTemplate(Template templateItem)
      {
         SaveToTemplate(new[] {templateItem});
      }

      public bool Exists(TemplateDatabaseType templateDatabaseType, string name, TemplateType templateType)
      {
         using (establishConnection(templateDatabaseType))
         {
            var newTemplate = createTemplateRow(templateType, name);
            return newTemplate.ExistsInDB();
         }
      }

      private IDisposable establishConnection(TemplateDatabaseType templateDatabaseType)
      {
         var database = templateDatabaseType == TemplateDatabaseType.User
            ? new DatabaseDisposer(_templateDatabase, _userSettings.TemplateDatabasePath)
            : new DatabaseDisposer(_templateDatabase, _pkSimConfiguration.TemplateSystemDatabasePath);

         _templateDatabaseConverter.Convert(_templateDatabase);
         return database;
      }

      private static class TemplateTable
      {
         public const string NAME = "TAB_TEMPLATES";

         public static class Columns
         {
            public const string TEMPLATE_TYPE = "TEMPLATE_TYPE";
            public const string NAME = "NAME";
            public const string DESCRIPTION = "DESCRIPTION";
            public const string XML = "XML";
         }
      }

      private static class TemplateTypeTable
      {
         public const string NAME = "TAB_TEMPLATE_TYPES";

         public static class Columns
         {
            public const string TEMPLATE_TYPE = "TEMPLATE_TYPE";
         }
      }

      private static class TemplateReferenceTable
      {
         public const string NAME = "TAB_TEMPLATE_REFERENCES";

         public static class Columns
         {
            public const string TEMPLATE_TYPE = "TEMPLATE_TYPE";
            public const string NAME = "NAME";
            public const string REFERENCE_TEMPLATE_TYPE = "REFERENCE_TEMPLATE_TYPE";
            public const string REFERENCE_NAME = "REFERENCE_NAME";
         }
      }
   }
}