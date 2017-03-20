using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization.ORM.MetaData;
using PKSim.Infrastructure.Serialization.ORM.Queries;
using PKSim.Presentation.Services;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.Services
{
   public class SimulationChartsLoader : ISimulationChartsLoader
   {
      private readonly ISimulationChartsQuery _simulationChartsQuery;
      private readonly ICompressedSerializationManager _serializationManager;
      private readonly IChartTask _chartTask;

      public SimulationChartsLoader(ISimulationChartsQuery simulationChartsQuery, ICompressedSerializationManager serializationManager, IChartTask chartTask)
      {
         _simulationChartsQuery = simulationChartsQuery;
         _serializationManager = serializationManager;
         _chartTask = chartTask;
      }

      public void LoadChartsFor(Simulation simulation)
      {
         var simulationChartsMetaData = _simulationChartsQuery.ResultFor(simulation.Id);
         simulationChartsMetaData.Each(chart => simulation.AddAnalysis(mapFrom(chart)));
         _chartTask.UpdateObservedDataInChartsFor(simulation);
      }

      private ISimulationAnalysis mapFrom(SimulationChartMetaData chart)
      {
         return _serializationManager.Deserialize<ISimulationAnalysis>(chart.Content.Data);
      }
   }
}