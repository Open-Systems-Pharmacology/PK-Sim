using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Journal;
using OSPSuite.Core.Serialization.Exchange;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.Services
{
   public class SimulationTransferLoader : ISimulationTransferLoader
   {
      private readonly ISimulationPersistor _simulationPersister;
      private readonly IProjectRetriever _projectRetriever;
      private readonly IJournalTask _journalTask;

      public SimulationTransferLoader(
         ISimulationPersistor simulationPersister,
         IProjectRetriever projectRetriever,
         IJournalTask journalTask
      )
      {
         _simulationPersister = simulationPersister;
         _projectRetriever = projectRetriever;
         _journalTask = journalTask;
      }

      public SimulationTransfer Load(string pkmlFileFullPath)
      {
         var project = _projectRetriever.CurrentProject;

         //use new ObjectBaseRepository here as the resulting simulation will be registered later on when added to the project
         var simulationTransfer = _simulationPersister.Load(pkmlFileFullPath, new WithIdRepository());

         project?.Favorites.AddFavorites(simulationTransfer.Favorites);

         if (shouldLoadJournal(project, simulationTransfer))
            _journalTask.LoadJournal(simulationTransfer.JournalPath, showJournal: false);

         return simulationTransfer;
      }

      private static bool shouldLoadJournal(IProject project, SimulationTransfer simulationTransfer)
      {
         return project != null
                && string.IsNullOrEmpty(project.JournalPath)
                && !string.IsNullOrEmpty(simulationTransfer.JournalPath);
      }
   }
}