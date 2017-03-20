using OSPSuite.Utility.Visitor;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization.ORM.Queries;
using OSPSuite.Core.Domain.Data;

namespace PKSim.Infrastructure.Services
{
   public class SimulationResultsLoader : ISimulationResultsLoader, IVisitor<PopulationSimulation>, IVisitor<IndividualSimulation>, IStrictVisitor
   {
      private readonly ISimulationResultsQuery _simulationResultsQuery;
      private readonly IDataRepositoryQuery _dataRepositoryQuery;
      private readonly ICompressedSerializationManager _serializationManager;
      private readonly ISimulationResultsCreator _simulationResultsCreator;
      private readonly IDataRepositoryFromResultsCreator _dataRepositoryFromResultsCreator;

      public SimulationResultsLoader(ISimulationResultsQuery simulationResultsQuery, IDataRepositoryQuery dataRepositoryQuery,
         ICompressedSerializationManager serializationManager, ISimulationResultsCreator simulationResultsCreator, IDataRepositoryFromResultsCreator dataRepositoryFromResultsCreator)
      {
         _simulationResultsQuery = simulationResultsQuery;
         _dataRepositoryQuery = dataRepositoryQuery;
         _serializationManager = serializationManager;
         _simulationResultsCreator = simulationResultsCreator;
         _dataRepositoryFromResultsCreator = dataRepositoryFromResultsCreator;
      }

      public virtual void LoadResultsFor(Simulation simulation)
      {
         if (simulation.HasResults) return;
         int resultVersion = simulation.ResultsVersion;
         try
         {
            this.Visit(simulation);
         }
         finally
         {
            //results version are set when setting results. The version should not be changed when loading the results
            simulation.ResultsVersion = resultVersion;
         }
      }

      private void loadResultsFor(IndividualSimulation simulation)
      {
         var simulationResults = _simulationResultsQuery.ResultFor(simulation.Id);
         if (simulationResults == null)
         {
            //this is an old simulation for which results were saved in data repository format
            simulation.DataRepository = dataRepositoryFor(simulation);
            simulation.Results = _simulationResultsCreator.CreateResultsFrom(simulation.DataRepository);
         }
         else
         {
            simulation.Results = simulationResults;
            simulation.DataRepository = _dataRepositoryFromResultsCreator.CreateResultsFor(simulation);
         }

         simulation.ResultsHaveChanged = false;
      }

      private void loadResultsFor(PopulationSimulation populationSimulation)
      {
         var simulationResults = _simulationResultsQuery.ResultFor(populationSimulation.Id);
         if (simulationResults == null)
            populationSimulation.Results = _simulationResultsCreator.CreateResultsFrom(dataRepositoryFor(populationSimulation));
         else
            populationSimulation.Results = simulationResults;

         populationSimulation.ResultsHaveChanged = false;
      }

      private DataRepository dataRepositoryFor(Simulation simulation)
      {
         var dataRepositoryMetaData = _dataRepositoryQuery.ResultFor(simulation.Id);
         if (dataRepositoryMetaData == null)
            return new NullDataRepository();
         return _serializationManager.Deserialize<DataRepository>(dataRepositoryMetaData.Content.Data);
      }

      public void Visit(PopulationSimulation populationSimulation)
      {
         loadResultsFor(populationSimulation);
      }

      public void Visit(IndividualSimulation individualSimulation)
      {
         loadResultsFor(individualSimulation);
      }
   }
}