using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Services;
using OSPSuite.Core.Snapshots.Mappers;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using SnapshotExpressionProfile = PKSim.Core.Snapshots.ExpressionProfile;
using SnapshotIndividual = PKSim.Core.Snapshots.Individual;

namespace PKSim.Core.Services;

public interface IBuildingBlockSnapshotUpdater
{
   void AddSnapshotTo(IndividualBuildingBlock individualBuildingBlock, Individual individual);
   void AddSnapshotTo(ExpressionProfileBuildingBlock expressionProfileBuildingBlock, ExpressionProfile expressionProfile);
   void UpdateSnapshotFromQuery(ExpressionProfileBuildingBlock expressionProfileBuildingBlock, QueryExpressionResults queryResults);
}

public class BuildingBlockSnapshotUpdater(
   ISnapshotMapper snapshotMapper,
   IJsonSerializer jsonSerializer,
   IExpressionProfileUpdater expressionProfileUpdater) : IBuildingBlockSnapshotUpdater
{
   public void AddSnapshotTo(IndividualBuildingBlock individualBuildingBlock, Individual individual)
   {
      var snapshot = snapshotFor<SnapshotIndividual>(individual);
      snapshot.ExpressionProfiles = null;
      individualBuildingBlock.Snapshot = jsonSerializer.SerializeToBase64String(snapshot);
   }

   public void AddSnapshotTo(ExpressionProfileBuildingBlock expressionProfileBuildingBlock, ExpressionProfile expressionProfile) =>
      expressionProfileBuildingBlock.Snapshot = jsonSerializer.SerializeToBase64String(snapshotFor<SnapshotExpressionProfile>(expressionProfile));

   public void UpdateSnapshotFromQuery(ExpressionProfileBuildingBlock expressionProfileBuildingBlock, QueryExpressionResults queryResults)
   {
      if (!expressionProfileBuildingBlock.HasSnapshot)
         return;

      var expressionProfile = expressionProfileFrom(expressionProfileBuildingBlock.Snapshot);
      expressionProfileUpdater.UpdateExpressionFromQuery(expressionProfile, queryResults);
      AddSnapshotTo(expressionProfileBuildingBlock, expressionProfile);
   }

   private TSnapshot snapshotFor<TSnapshot>(object model) where TSnapshot : class => snapshotMapper.MapToSnapshot(model).Result.DowncastTo<TSnapshot>();

   private ExpressionProfile expressionProfileFrom(string base64Snapshot)
   {
      var snapshot = jsonSerializer.DeserializeFromBase64String<SnapshotExpressionProfile>(base64Snapshot).Result;
      return snapshotMapper.MapToModel(snapshot, new ProjectContext(new PKSimProject(), runSimulations: false)).Result.DowncastTo<ExpressionProfile>();
   }
}
