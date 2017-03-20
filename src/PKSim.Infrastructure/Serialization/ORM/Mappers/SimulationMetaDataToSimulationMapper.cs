using System;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization.ORM.MetaData;
using OSPSuite.Core.Domain;

namespace PKSim.Infrastructure.Serialization.ORM.Mappers
{
   public interface ISimulationMetaDataToSimulationMapper : IMapper<SimulationMetaData, Simulation>
   {
   }

   public class SimulationMetaDataToSimulationMapper : ISimulationMetaDataToSimulationMapper
   {
      private readonly ICompressedSerializationManager _serializationManager;

      public SimulationMetaDataToSimulationMapper(ICompressedSerializationManager serializationManager)
      {
         _serializationManager = serializationManager;
      }

      public Simulation MapFrom(SimulationMetaData simulationMetaData)
      {
         var simulation = simulationFrom(simulationMetaData.SimulationMode);
         simulationMetaData.BuildingBlocks.Each(bb => simulation.AddUsedBuildingBlock(mapFrom(bb)));
         //no lazy load need for the simulation properties
         simulation.Properties = _serializationManager.Deserialize<SimulationProperties>(simulationMetaData.Properties.Data);
         simulationMetaData.UsedObservedData.Each(o => simulation.AddUsedObservedData(mapFrom(o)));
         return simulation;
      }

      private UsedObservedData mapFrom(string observedDataId)
      {
         return new UsedObservedData {Id = observedDataId};
      }

      private Simulation simulationFrom(SimulationMode simulationMode)
      {
         switch (simulationMode)
         {
            case SimulationMode.Individual:
               return new IndividualSimulation();
            case SimulationMode.Population:
               return new PopulationSimulation();
            default:
               throw new ArgumentOutOfRangeException("simulationMode");
         }
      }

      private UsedBuildingBlock mapFrom(UsedBuildingBlockMetaData usedBuildingBlockMetaData)
      {
         return new UsedBuildingBlock(usedBuildingBlockMetaData.TemplateId, usedBuildingBlockMetaData.BuildingBlockType)
            {
               Name = usedBuildingBlockMetaData.Name,
               Altered = usedBuildingBlockMetaData.Altered,
               Version = usedBuildingBlockMetaData.Version,
               StructureVersion = usedBuildingBlockMetaData.StructureVersion,
               Id = usedBuildingBlockMetaData.Id,
            };
      }
   }
}