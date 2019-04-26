using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.Services
{
   public class PKSimProjectRetriever : IPKSimProjectRetriever
   {
      private readonly ICoreWorkspace _workspace;

      public PKSimProjectRetriever(ICoreWorkspace workspace)
      {
         _workspace = workspace;
      }

      public IProject CurrentProject => Current;

      public string ProjectName => Current?.Name;

      public string ProjectFullPath => Current?.FilePath;

      public PKSimProject Current => _workspace.Project;

      public void AddToHistory(ICommand command)
      {
         _workspace.AddCommand(command);
      }
   }
}