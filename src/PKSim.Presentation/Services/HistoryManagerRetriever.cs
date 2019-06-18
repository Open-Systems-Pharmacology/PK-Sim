using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain.Services;
using PKSim.Core;
using PKSim.Presentation.Core;

namespace PKSim.Presentation.Services
{
   public class HistoryManagerRetriever : IHistoryManagerRetriever
   {
      private readonly ICoreWorkspace _workspace;

      public HistoryManagerRetriever(ICoreWorkspace workspace )
      {
         _workspace = workspace;
      }

      public IHistoryManager Current => _workspace.HistoryManager;
   }
}