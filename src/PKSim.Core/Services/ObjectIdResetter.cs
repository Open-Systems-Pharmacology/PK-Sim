using System.Linq;
using OSPSuite.Utility.Collections;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Services
{
   public class ObjectIdResetter : OSPSuite.Core.Domain.Services.ObjectIdResetter
   {
      private readonly IParameterIdUpdater _parameterIdUpdater;
      private readonly Cache<string, UsedBuildingBlock> _usedBuildingBlockCache = new Cache<string, UsedBuildingBlock>(x => x.Id, x => null);
      private readonly Cache<string, IParameter> _buildingBlockParameterCache = new Cache<string, IParameter>(x => x.Id, x => null);

      public ObjectIdResetter(IIdGenerator idGenerator, IParameterIdUpdater parameterIdUpdater) : base(idGenerator)
      {
         _parameterIdUpdater = parameterIdUpdater;
      }

      public override void ResetIdFor(object objectWithId)
      {
         try
         {
            var simulation = objectWithId as Simulation;
            if (simulation != null)
               cacheUsedBuildingBlockAndParametersFromSimulation(simulation);

            base.ResetIdFor(objectWithId);

            if (simulation != null)
               updateParameterOrginInSimulation(simulation);
         }
         finally
         {
            _usedBuildingBlockCache.Clear();
            _buildingBlockParameterCache.Clear();
         }
      }

      private void updateParameterOrginInSimulation(Simulation simulation)
      {
         var allParameters = simulation.Model.Root.GetAllChildren<IParameter>()
            .Where(x => !string.IsNullOrEmpty(x.Origin.BuilingBlockId)).ToList();

         var allParametersGroupByBuildingBlockId = allParameters.GroupBy(x => x.Origin.BuilingBlockId);

         foreach (var parametersByBuildingBlockId in allParametersGroupByBuildingBlockId)
         {
            var usedBuildingBlock = _usedBuildingBlockCache[parametersByBuildingBlockId.Key];
            if (usedBuildingBlock != null)
               _parameterIdUpdater.UpdateBuildingBlockId(parametersByBuildingBlockId, usedBuildingBlock.BuildingBlock);
            else
               _parameterIdUpdater.ResetParameterOrigin(parametersByBuildingBlockId);
         }

         allParameters.Where(p => !string.IsNullOrEmpty(p.Origin.ParameterId))
            .Each(updateParameterOrigin);

         _parameterIdUpdater.UpdateSimulationId(simulation);
      }

      private void updateParameterOrigin(IParameter parameter)
      {
         var buildingBlockParameter = _buildingBlockParameterCache[parameter.Origin.ParameterId];
         if (buildingBlockParameter != null)
            parameter.Origin.ParameterId = buildingBlockParameter.Id;
      }

      private void cacheUsedBuildingBlockAndParametersFromSimulation(Simulation simulation)
      {
         simulation.UsedBuildingBlocks.Each(x =>
         {
            _usedBuildingBlockCache.Add(x);
            _buildingBlockParameterCache.AddRange(x.BuildingBlock.GetAllChildren<IParameter>());
         });
      }
   }
}