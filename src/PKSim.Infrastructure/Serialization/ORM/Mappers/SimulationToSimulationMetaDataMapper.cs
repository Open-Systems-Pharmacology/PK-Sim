using OSPSuite.Core.Domain;
using OSPSuite.Utility;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Infrastructure.Serialization.ORM.MetaData;

namespace PKSim.Infrastructure.Serialization.ORM.Mappers
{
   public interface ISimulationToSimulationMetaDataMapper :
      IMapper<IndividualSimulation, SimulationMetaData>,
      IMapper<PopulationSimulation, SimulationMetaData>
   {
   }

   public class SimulationToSimulationMetaDataMapper : ISimulationToSimulationMetaDataMapper
   {
      private readonly ICompressedSerializationManager _serializationManager;
      private readonly ISimulationResultsCreator _simulationResultsCreator;

      public SimulationToSimulationMetaDataMapper(ICompressedSerializationManager serializationManager,
         ISimulationResultsCreator simulationResultsCreator)
      {
         _serializationManager = serializationManager;
         _simulationResultsCreator = simulationResultsCreator;
      }

      public SimulationMetaData MapFrom(PopulationSimulation populationSimulation)
      {
         var simMetaData = createFor(populationSimulation, SimulationMode.Population);

         simMetaData.SimulationAnalyses = mapFrom(populationSimulation.PKAnalyses);

         if (!resultsHaveChanged(populationSimulation))
            return simMetaData;

         simMetaData.SimulationResults = populationSimulation.Results;
         
         return simMetaData;
      }

      public SimulationMetaData MapFrom(IndividualSimulation simulation)
      {
         var simMetaData = createFor(simulation, SimulationMode.Individual);

         if (!resultsHaveChanged(simulation))
            return simMetaData;

         //simulation was loaded. It is necessary to update the results
         var results = _simulationResultsCreator.CreateResultsFrom(simulation.DataRepository);
         simMetaData.SimulationResults = results;
         return simMetaData;
      }

      private SimulationMetaData createFor(Simulation simulation, SimulationMode simulationMode)
      {
         var simDeta = new SimulationMetaData { SimulationMode = simulationMode };
         updateSimulationProperties(simulation, simDeta);
         return simDeta;
      }

      private bool resultsHaveChanged(Simulation simulation)
      {
         return simulation.IsLoaded && simulation.ResultsHaveChanged;
      }

      private void updateSimulationProperties(Simulation simulation, SimulationMetaData simMetaData)
      {
         simulation.UsedBuildingBlocks.Each(bb => simMetaData.AddBuildingBlock(mapFrom(bb)));
         simMetaData.Properties.Data = _serializationManager.Serialize(simulation.Properties);
         simulation.UsedObservedData.Each(x => simMetaData.AddObservedData(x.Id));
         simulation.Analyses.Each(chart => simMetaData.AddChart(mapFrom(chart)));
      }

      private SimulationAnalysesMetaData mapFrom(PopulationSimulationPKAnalyses populationPKAnalyses)
      {
         return new SimulationAnalysesMetaData
         {
            Content = { Data = _serializationManager.Serialize(populationPKAnalyses) }
         };
      }

      private SimulationChartMetaData mapFrom(ISimulationAnalysis chart)
      {
         return new SimulationChartMetaData
         {
            Id = chart.Id,
            Name = chart.Name,
            Description = chart.Description,
            Content = { Data = _serializationManager.Serialize(chart) }
         };
      }

      private UsedBuildingBlockMetaData mapFrom(UsedBuildingBlock usedBuildingBlock)
      {
         return new UsedBuildingBlockMetaData
         {
            Id = usedBuildingBlock.Id,
            Name = usedBuildingBlock.Name,
            TemplateId = usedBuildingBlock.TemplateId,
            Version = usedBuildingBlock.Version,
            BuildingBlockType = usedBuildingBlock.BuildingBlockType,
            StructureVersion = usedBuildingBlock.StructureVersion,
            Altered = usedBuildingBlock.Altered,
         };
      }
   }
}