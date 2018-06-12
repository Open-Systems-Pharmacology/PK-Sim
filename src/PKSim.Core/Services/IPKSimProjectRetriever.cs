using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Services
{
   public interface IPKSimProjectRetriever : IProjectRetriever
   {
      PKSimProject Current { get; }
      void AddToHistory(ICommand command);
   }

}