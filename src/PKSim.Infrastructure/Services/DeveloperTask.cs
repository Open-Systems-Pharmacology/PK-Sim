using System.Data.Common;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;
using OSPSuite.Infrastructure.Serialization.Services;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.Services
{
   public class DeveloperTask : IDeveloperTask
   {
      private readonly IProjectRetriever _projectRetriever;
      private readonly SQLiteProjectCommandExecuter _commandExecuter;

      public DeveloperTask(IProjectRetriever projectRetriever, SQLiteProjectCommandExecuter commandExecuter)
      {
         _projectRetriever = projectRetriever;
         _commandExecuter = commandExecuter;
      }

      public void ClearUnusedContent()
      {
         var projectPath = _projectRetriever.ProjectFullPath;
         if (string.IsNullOrEmpty(projectPath))
            return;

         _commandExecuter.ExecuteCommand(projectPath, clearUnusedContentCommand);
      }

      private void clearUnusedContentCommand(DbConnection connection)
      {
         connection.ExecuteNonQuery(
            @"PRAGMA foreign_keys = ON;
             DELETE FROM CONTENTS WHERE[Id] NOT IN(
               SELECT ContentId FROM SENSITIVITY_ANALYSES UNION 
               SELECT PropertiesId FROM SENSITIVITY_ANALYSES UNION 
               SELECT ContentId FROM DATA_REPOSITORIES UNION 
               SELECT ContentId FROM SIMULATION_CHARTS UNION 
               SELECT ContentId FROM SIMULATION_COMPARISONS UNION 
               SELECT ContentId FROM PROJECTS UNION 
               SELECT ContentId FROM BUILDING_BLOCKS UNION 
               SELECT PropertiesId FROM BUILDING_BLOCKS UNION 
               SELECT ContentId FROM SIMULATION_ANALYSES UNION 
               SELECT ContentId FROM WORKSPACE_LAYOUT UNION 
               SELECT ContentId FROM PARAMETER_IDENTIFICATIONS UNION 
               SELECT PropertiesId FROM PARAMETER_IDENTIFICATIONS
            );
            PRAGMA foreign_keys = OFF;");
      }
   }
}