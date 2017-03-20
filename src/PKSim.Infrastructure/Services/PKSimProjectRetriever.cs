using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Core;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.Services
{
   public class PKSimProjectRetriever : IPKSimProjectRetriever
   {
      private readonly IWorkspace _workspace;

      public PKSimProjectRetriever(IWorkspace workspace)
      {
         _workspace = workspace;
      }

      public IProject CurrentProject
      {
         get { return Current; }
      }

      public string ProjectName
      {
         get { return Current.Name; }
      }

      public string ProjectFullPath
      {
         get { return Current.FilePath; }
      }

      public IPKSimProject Current
      {
         get { return _workspace.Project; }
      }

      public void AddToHistory(ICommand command)
      {
         _workspace.AddCommand(command);
      }
   }
}