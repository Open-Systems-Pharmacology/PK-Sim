using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Services;
using OSPSuite.Core.Snapshots.Mappers;
using OSPSuite.Utility;
using PKSim.Core.Mappers;
using PKSim.Core.Snapshots;

namespace PKSim.Core.Services;

public interface IExpressionProfileSnapshotToExpressionProfileBuildingBlockMapper : IMapper<string, ExpressionProfileBuildingBlock>;

public class ExpressionProfileSnapshotToExpressionProfileBuildingBlockMapper(
   IJsonSerializer jsonSerializer,
   ISnapshotMapper snapshotMapper,
   IExpressionProfileToExpressionProfileBuildingBlockMapper expressionProfileToExpressionProfileBuildingBlockMapper) :
   SnapshotExchangeMapper<ExpressionProfileBuildingBlock, ExpressionProfile, Model.ExpressionProfile>(jsonSerializer, snapshotMapper), IExpressionProfileSnapshotToExpressionProfileBuildingBlockMapper
{
   public override ExpressionProfileBuildingBlock MapFrom(string individualSnapshotString)
   {
      var model = PKSimModelFor(individualSnapshotString);
      return expressionProfileToExpressionProfileBuildingBlockMapper.MapFrom(model);
   }
}