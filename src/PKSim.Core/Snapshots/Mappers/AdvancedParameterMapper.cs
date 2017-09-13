using System;
using ModelAdvancedParameter = PKSim.Core.Model.AdvancedParameter;
using SnapshotAdvancedParameter = PKSim.Core.Snapshots.AdvancedParameter;

namespace PKSim.Core.Snapshots.Mappers
{
   public class AdvancedParameterMapper : ParameterContainerSnapshotMapperBase<ModelAdvancedParameter, SnapshotAdvancedParameter>
   {
      public AdvancedParameterMapper(ParameterMapper parameterMapper) : base(parameterMapper)
      {
      }

      public override SnapshotAdvancedParameter MapToSnapshot(ModelAdvancedParameter advancedParameter)
      {
         return SnapshotFrom(advancedParameter, snapshot =>
         {
            //the parameter path is what identified the advanced parameter. The name is not used anywhere. We will use the path as key
            snapshot.Name = advancedParameter.ParameterPath;   
            snapshot.DistributionType = advancedParameter.DistributionType.Id;
         });
      }

      protected override void AddModelParametersToSnapshot(ModelAdvancedParameter model, SnapshotAdvancedParameter snapshot)
      {
         AddParametersToSnapshot(model.AllParameters, snapshot);
      }

      public override ModelAdvancedParameter MapToModel(SnapshotAdvancedParameter snapshot)
      {
         throw new NotImplementedException();
      }
   }
}