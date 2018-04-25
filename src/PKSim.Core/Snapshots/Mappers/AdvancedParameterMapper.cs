using System.Threading.Tasks;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Model;
using ModelAdvancedParameter = PKSim.Core.Model.AdvancedParameter;
using SnapshotAdvancedParameter = PKSim.Core.Snapshots.AdvancedParameter;

namespace PKSim.Core.Snapshots.Mappers
{
   public class AdvancedParameterMapper : ParameterContainerSnapshotMapperBase<ModelAdvancedParameter, SnapshotAdvancedParameter, PathCache<IParameter>>
   {
      private readonly IAdvancedParameterFactory _advancedParameterFactory;
      private readonly IEntityPathResolver _entityPathResolver;
      private readonly ILogger _logger;

      public AdvancedParameterMapper(
         ParameterMapper parameterMapper, 
         IAdvancedParameterFactory advancedParameterFactory, 
         IEntityPathResolver entityPathResolver, 
         ILogger logger) : base(parameterMapper)
      {
         _advancedParameterFactory = advancedParameterFactory;
         _entityPathResolver = entityPathResolver;
         _logger = logger;
      }

      public override Task<SnapshotAdvancedParameter> MapToSnapshot(ModelAdvancedParameter advancedParameter)
      {
         return SnapshotFrom(advancedParameter, snapshot =>
         {
            //the parameter path is what identified the advanced parameter. The name is not used anywhere.
            snapshot.Name = advancedParameter.ParameterPath;
            snapshot.DistributionType = advancedParameter.DistributionType.Id;
         });
      }

      protected override Task AddModelParametersToSnapshot(ModelAdvancedParameter model, SnapshotAdvancedParameter snapshot)
      {
         return AddParametersToSnapshot(model.AllParameters, snapshot);
      }

      public override async Task<ModelAdvancedParameter> MapToModel(SnapshotAdvancedParameter snapshot, PathCache<IParameter> allParameters)
      {
         var parameter = allParameters[snapshot.Name];
         if (parameter == null)
         {
            _logger.AddWarning(PKSimConstants.Error.SnapshotParameterNotFound(snapshot.Name));
            return null;
         }

         var advancedParameter = _advancedParameterFactory.Create(parameter, DistributionTypes.ById(snapshot.DistributionType));
         await UpdateParametersFromSnapshot(snapshot, advancedParameter.DistributedParameter);
         return advancedParameter;
      }

      public virtual async Task MapToModel(SnapshotAdvancedParameter[] snapshotAdvancedParameters, IAdvancedParameterContainer advancedParameterContainer)
      {
         if (snapshotAdvancedParameters == null || advancedParameterContainer == null)
            return;

         advancedParameterContainer.RemoveAllAdvancedParameters();
         var parameterCache = advancedParameterContainer.AllParameters(_entityPathResolver);

         var advancedParameters = await this.MapToModels(snapshotAdvancedParameters, parameterCache);

         advancedParameters.Each(x => advancedParameterContainer.AddAdvancedParameter(x, generateRandomValues: true));
      }
   }
}