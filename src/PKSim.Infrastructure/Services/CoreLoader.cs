using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Core.Journal;
using OSPSuite.Core.Serialization.Exchange;
using PKSim.Core.Services;

namespace PKSim.Infrastructure.Services
{
   public class CoreLoader : ICoreLoader
   {
      private readonly IDimensionFactory _dimensionFactory;
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly ISimulationPersistor _simulationPersister;
      private readonly IProjectRetriever _projectRetriever;
      private readonly IJournalTask _journalTask;
      private readonly ICloneManagerForModel _cloneManagerForModel;
      private readonly IObserverLoader _observerLoader;
      private readonly IObjectIdResetter _objectIdResetter;

      public CoreLoader(
         IDimensionFactory dimensionFactory,
         IObjectBaseFactory objectBaseFactory,
         ISimulationPersistor simulationPersister,
         IProjectRetriever projectRetriever,
         IJournalTask journalTask,
         ICloneManagerForModel cloneManagerForModel,
         IObserverLoader observerLoader,
         IObjectIdResetter objectIdResetter)
      {
         _dimensionFactory = dimensionFactory;
         _objectBaseFactory = objectBaseFactory;
         _simulationPersister = simulationPersister;
         _projectRetriever = projectRetriever;
         _journalTask = journalTask;
         _cloneManagerForModel = cloneManagerForModel;
         _observerLoader = observerLoader;
         _objectIdResetter = objectIdResetter;
      }

      public SimulationTransfer LoadSimulationTransfer(string pkmlFileFullPath)
      {
         var project = _projectRetriever.CurrentProject;

         //use new ObjectBaseRepository here as the resulting simulation will be registered later on when added to the project
         var simulationTransfer = _simulationPersister.Load(pkmlFileFullPath, _dimensionFactory, _objectBaseFactory, new WithIdRepository(), _cloneManagerForModel);

         project?.Favorites.AddFavorites(simulationTransfer.Favorites);

         if (shouldLoadJournal(project, simulationTransfer))
            _journalTask.LoadJournal(simulationTransfer.JournalPath, showJournal: false);

         return simulationTransfer;
      }

      public IObserverBuilder LoadObserver(string pkmlFileFullPath)
      {
         var observer = _observerLoader.Load(pkmlFileFullPath, _dimensionFactory, _objectBaseFactory, new WithIdRepository(), _cloneManagerForModel);
         _objectIdResetter.ResetIdFor(observer);
         return observer;
      }

      private static bool shouldLoadJournal(IProject project, SimulationTransfer simulationTransfer)
      {
         return project != null
                && string.IsNullOrEmpty(project.JournalPath)
                && !string.IsNullOrEmpty(simulationTransfer.JournalPath);
      }
   }
}