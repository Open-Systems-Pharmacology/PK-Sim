using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain.Services;
using PKSim.Presentation.Core;

namespace PKSim.Presentation.Services
{
   public class HistoryManagerRetriever : IHistoryManagerRetriever
   {
      private readonly IWorkspace _workspace;

      public HistoryManagerRetriever(IWorkspace workspace )
      {
         _workspace = workspace;
      }

      public IHistoryManager Current => _workspace.HistoryManager;
   }
}