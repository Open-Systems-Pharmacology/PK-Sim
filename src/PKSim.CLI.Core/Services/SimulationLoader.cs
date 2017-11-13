using System.IO;
using Newtonsoft.Json;
using PKSim.Core.Batch;
using PKSim.Core.Batch.Mapper;
using BatchSimulation = PKSim.Core.Batch.Simulation;

namespace PKSim.CLI.Core.Services
{
   internal interface ISimulationLoader
   {
      /// <summary>
      ///    Load the simulation that will be run from the json file
      /// </summary>
      /// <param name="jsonFile"> Full path of the json file containing the simulation to be run </param>
      /// <returns> an individual simulation that will be run</returns>
      SimulationForBatch LoadSimulationFrom(string jsonFile);
   }

   internal class SimulationLoader : ISimulationLoader
   {
      private readonly ISimulationMapper _simulationMapper;

      public SimulationLoader(ISimulationMapper simulationMapper)
      {
         _simulationMapper = simulationMapper;
      }

      public SimulationForBatch LoadSimulationFrom(string jsonFile)
      {
         var serializer = new JsonSerializer();

         using (var sr = new StreamReader(jsonFile))
         using (var reader = new JsonTextReader(sr))
         {
            var simulation = serializer.Deserialize<BatchSimulation>(reader);
            return _simulationMapper.MapFrom(simulation);
         }
      }
   }
}