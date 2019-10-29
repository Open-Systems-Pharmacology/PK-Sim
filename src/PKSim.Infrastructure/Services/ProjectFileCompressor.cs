using System.Data.Common;
using OSPSuite.Core.Extensions;
using OSPSuite.Infrastructure.Serialization.Services;

namespace PKSim.Infrastructure.Services
{
   public interface IProjectFileCompressor
   {
      void Compress(string projectFile);
   }

   public class ProjectFileCompressor : IProjectFileCompressor
   {
      private readonly SQLiteProjectCommandExecuter _commandExecuter;

      public ProjectFileCompressor(SQLiteProjectCommandExecuter commandExecuter)
      {
         _commandExecuter = commandExecuter;
      }

      public void Compress(string projectFile)
      {
         _commandExecuter.ExecuteCommand(projectFile, vacuum);
      }

      private static void vacuum(DbConnection sqlLite)
      {
         sqlLite.ExecuteNonQuery("vacuum;");
      }
   }
}