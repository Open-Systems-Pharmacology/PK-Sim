using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Services
{
   public interface ISimulationParametersUpdater
   {
      /// <summary>
      ///    Updates all simulation parameter values from the source simulation in the target simulation. Returns a <see cref="ValidationResult"/> containing
      /// one message for each parameter that was changed in the <paramref name="sourceSimulation "/> and that does not exist in the <paramref name="targetSimulation"/> anymore.
      /// </summary>
      /// <param name="sourceSimulation">Simulation with the original values</param>
      /// <param name="targetSimulation">Simulation that will be updated</param>
      /// <param name="buildingBlockType">Type of parameters to be synchronized between simulations. By default, only simulation parameters will be updated</param>
      ValidationResult ReconciliateSimulationParametersBetween(Simulation sourceSimulation, Simulation targetSimulation, PKSimBuildingBlockType buildingBlockType = PKSimBuildingBlockType.Simulation);
   }

   public class SimulationParametersUpdater : ISimulationParametersUpdater
   {
      private readonly IParameterSetUpdater _parameterSetUpdater;
      private readonly IEntityPathResolver _entityPathResolver;

      public SimulationParametersUpdater(IParameterSetUpdater parameterSetUpdater, IEntityPathResolver entityPathResolver)
      {
         _parameterSetUpdater = parameterSetUpdater;
         _entityPathResolver = entityPathResolver;
      }

      public ValidationResult ReconciliateSimulationParametersBetween(Simulation sourceSimulation, Simulation targetSimulation, PKSimBuildingBlockType buildingBlockType = PKSimBuildingBlockType.Simulation)
      {
         var sourceParameters = simulationParametersCacheFor(sourceSimulation, buildingBlockType);
         var targetParameters = simulationParametersCacheFor(targetSimulation, buildingBlockType);
         _parameterSetUpdater.UpdateValues(sourceParameters, targetParameters);
         return validateParameterUpdates(sourceParameters, targetParameters);
      }

      private ValidationResult validateParameterUpdates(PathCache<IParameter> sourceParameters, PathCache<IParameter> targetParameters)
      {
         var validationResult = new ValidationResult();
         foreach (var sourceParameterKeyValue in sourceParameters.KeyValues)
         {
            if (targetParameters.Contains(sourceParameterKeyValue.Key))
               continue;

            var sourceParameter = sourceParameterKeyValue.Value;
            if (!sourceParameter.ValueDiffersFromDefault())
               continue;

            validationResult.AddMessage(NotificationType.Warning, sourceParameter, sourceParameterKeyValue.Key);
         }

         return validationResult;
      }

      private PathCache<IParameter> simulationParametersCacheFor(Simulation simulation, PKSimBuildingBlockType buildingBlockType)
      {
         return new PathCache<IParameter>(_entityPathResolver).For(simulation.ParametersOfType(buildingBlockType));
      }
   }
}