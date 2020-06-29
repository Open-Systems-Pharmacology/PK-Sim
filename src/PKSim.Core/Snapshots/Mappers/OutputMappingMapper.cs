using System.Linq;
using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.ParameterIdentifications;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Extensions;
using OSPSuite.Core.Services;
using PKSim.Assets;
using PKSim.Core.Model;
using ModelOutputMapping = OSPSuite.Core.Domain.ParameterIdentifications.OutputMapping;
using SnapshotOutputMapping = PKSim.Core.Snapshots.OutputMapping;

namespace PKSim.Core.Snapshots.Mappers
{
   public class OutputMappingMapper : SnapshotMapperBase<ModelOutputMapping, SnapshotOutputMapping, ParameterIdentificationContext>
   {
      private readonly IOSPLogger _logger;

      public OutputMappingMapper(IOSPLogger logger)
      {
         _logger = logger;
      }

      public override Task<SnapshotOutputMapping> MapToSnapshot(ModelOutputMapping outputMapping)
      {
         return SnapshotFrom(outputMapping, x =>
         {
            x.Scaling = outputMapping.Scaling;
            x.Weight = SnapshotValueFor(outputMapping.Weight, Constants.DEFAULT_WEIGHT);
            x.Path = outputMapping.FullOutputPath;
            x.ObservedData = outputMapping.WeightedObservedData?.Name;
            x.Weights = weightsFrom(outputMapping.WeightedObservedData);
         });
      }

      private float[] weightsFrom(WeightedObservedData weightedObservedData)
      {
         var weights = weightedObservedData?.Weights;
         if (weights == null)
            return null;

         if (weights.All(x => ValueComparer.AreValuesEqual(x, Constants.DEFAULT_WEIGHT)))
            return null;

         return weights;
      }

      public override Task<ModelOutputMapping> MapToModel(SnapshotOutputMapping snapshot, ParameterIdentificationContext context)
      {
         var outputSelection = outputSelectionFrom(snapshot.Path, context.Project);
         if (outputSelection == null)
            return Task.FromResult<ModelOutputMapping>(null);

         var observedData = context.Project.AllObservedData.FindByName(snapshot.ObservedData);
         var weightedObservedData = new WeightedObservedData(observedData);
         if (snapshot.Weights != null)
            updateWeights(weightedObservedData.Weights, snapshot.Weights);

         var outputMapping = new ModelOutputMapping
         {
            Scaling = snapshot.Scaling,
            Weight = ModelValueFor(snapshot.Weight, Constants.DEFAULT_WEIGHT),
            OutputSelection = outputSelection,
            WeightedObservedData = weightedObservedData
         };

         return Task.FromResult(outputMapping);
      }

      //TODO Define in core
      private void updateWeights(float[] weights, float[] snapshotWeights)
      {
         for (int index = 0; index < weights.Length; ++index)
            weights[index] = snapshotWeights[index];
      }

      private SimulationQuantitySelection outputSelectionFrom(string outputFullPath, PKSimProject project)
      {
         var outputPath = new ObjectPath(outputFullPath.ToPathArray());
         if (outputPath.Count == 0)
            return null;

         var simulationName = outputPath[0];
         var simulation = project.All<Model.Simulation>().FindByName(simulationName);
         if (simulation == null)
         {
            _logger.AddWarning(PKSimConstants.Error.CouldNotFindSimulation(simulationName));
            return null;
         }

         outputPath.RemoveAt(0);
         var output = simulation.Model.Root.EntityAt<IQuantity>(outputPath.ToArray());
         if(output==null)
         {
            _logger.AddWarning(PKSimConstants.Error.CouldNotFindOutputInSimulation(outputPath,simulationName));
            return null;
         }


         return new SimulationQuantitySelection(simulation, new QuantitySelection(outputPath, output.QuantityType));
      }
   }
}