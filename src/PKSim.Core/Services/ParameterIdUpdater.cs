using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Services;

namespace PKSim.Core.Services
{
   public interface IParameterIdUpdater
   {
      /// <summary>
      /// Resets the parameter origin in all <paramref name="parameters"/>
      /// </summary>
      void ResetParameterOrigin(IEnumerable<IParameter> parameters);


      /// <summary>
      /// Resets the <paramref name="parameterOrigin"/>
      /// </summary>
      void ResetParameterOrigin(ParameterOrigin parameterOrigin);

      /// <summary>
      ///    Update the origin parameter id of the target parameter with the id of the source parameter
      /// </summary>
      void UpdateParameterId(IParameter sourceParameter, IParameter targetParameter);

      /// <summary>
      ///    Update the origin parameter ids of a parameter in the target container with the id of the same parameter
      ///    in the source container, if available.
      ///    <remarks>Same is defined as "having the same name"</remarks>
      /// </summary>
      void UpdateParameterIds(IContainer sourceContainer, IContainer targetContainer);

      /// <summary>
      ///    Update the origin parameter ids of a parameter in the target enumeration with the id of the same parameter
      ///    in the source container, if available.
      ///    <remarks>Same is defined as "having the same name"</remarks>
      /// </summary>
      void UpdateParameterIds(IContainer sourceContainer, IEnumerable<IParameter> targetParameters);

      /// <summary>
      ///    Update the building block if of all parameters to the id of the building block given as parameter
      /// </summary>
      void UpdateBuildingBlockId(IEnumerable<IParameter> allParameters, IPKSimBuildingBlock buildingBlock);

      /// <summary>
      ///    Update the building block if of all parameters defined in the container and its sub container to the id of the building block given as parameter
      /// </summary>
      void UpdateBuildingBlockId(IContainer container, IPKSimBuildingBlock buildingBlock);

      void UpdateSimulationId(Simulation simulation);
   }

   public class ParameterIdUpdater : IParameterIdUpdater
   {
      private readonly ISimulationParameterOriginIdUpdater _simulationParameterOriginIdUpdater;

      public ParameterIdUpdater(ISimulationParameterOriginIdUpdater simulationParameterOriginIdUpdater)
      {
         _simulationParameterOriginIdUpdater = simulationParameterOriginIdUpdater;
      }

      public void UpdateParameterId(IParameter sourceParameter, IParameter targetParameter)
      {
         string originalId = targetParameter.Origin.ParameterId;
         if (string.IsNullOrEmpty(originalId))
            targetParameter.Origin.ParameterId = sourceParameter.Id;
      }



      public void UpdateParameterIds(IContainer sourceContainer, IContainer targetContainer)
      {
         if (sourceContainer == null || targetContainer == null) return;
         UpdateParameterIds(sourceContainer, targetContainer.AllParameters());
      }

      public void ResetParameterOrigin(IEnumerable<IParameter> parameters)
      {
         parameters.Each(p => ResetParameterOrigin(p.Origin));
      }

      public void ResetParameterOrigin(ParameterOrigin parameterOrigin)
      {
         parameterOrigin.BuilingBlockId = string.Empty;
         parameterOrigin.SimulationId = string.Empty;
         parameterOrigin.ParameterId = string.Empty;
      }

      public void UpdateParameterIds(IContainer sourceContainer, IEnumerable<IParameter> targetParameters)
      {
         var allTargetParameters = targetParameters.ToList();
         foreach (var sourceParameter in sourceContainer.AllParameters())
         {
            var targetParameter = allTargetParameters.FindByName(sourceParameter.Name);
            if (targetParameter == null) continue;

            UpdateParameterId(sourceParameter, targetParameter);
         }
      }

      public void UpdateBuildingBlockId(IEnumerable<IParameter> allParameters, IPKSimBuildingBlock buildingBlock)
      {
         allParameters.Where(p => p.BuildingBlockType == buildingBlock.BuildingBlockType)
            .Each(p => p.Origin.BuilingBlockId = buildingBlock.Id);
      }

      public void UpdateBuildingBlockId(IContainer container, IPKSimBuildingBlock buildingBlock)
      {
         UpdateBuildingBlockId(container.GetAllChildren<IParameter>(), buildingBlock);
      }

      public void UpdateSimulationId(Simulation simulation)
      {
         _simulationParameterOriginIdUpdater.UpdateSimulationId(simulation);
      }
   }
}