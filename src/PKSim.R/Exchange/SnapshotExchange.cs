using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Serialization.Xml;
using PKSim.Core.Services;

namespace PKSim.R.Exchange;

/// <summary>
///    Headless counterpart to <c>PKSim.Starter.SnapshotExchange</c>. It is intended to be
///    dynamically loaded by MoBi from the shipped <c>PKSim.R.dll</c> which, unlike
///    <c>PKSim.Starter.dll</c>, carries no UI/Presentation dependencies and therefore ships
///    with the headless distributions (e.g. the R package).
/// </summary>
public static class SnapshotExchange
{
   public static string CreateModule(string projectSnapshot)
   {
      Api.InitializeOnce();
      var (projectSnapshotToModuleMapper, objectIdResetter, serializer) =
         Api.ResolveTasks<IProjectSnapshotToModuleMapper, IObjectIdResetter, IPKMLPersistor>();

      var module = projectSnapshotToModuleMapper.MapFrom(projectSnapshot).module;
      objectIdResetter.ResetIdFor(module);

      return serializer.Serialize(module);
   }

   public static string CreateIndividualBuildingBlock(string individualSnapshot)
   {
      Api.InitializeOnce();
      var (mapper, serializer) =
         Api.ResolveTasks<IIndividualSnapshotToIndividualBuildingBlockMapper, IPKMLPersistor>();

      var individualBuildingBlock = mapper.MapFrom(individualSnapshot);
      individualBuildingBlock.Snapshot = individualSnapshot;

      return serializer.Serialize(individualBuildingBlock);
   }

   public static string CreateExpressionProfileBuildingBlock(string expressionProfileSnapshot)
   {
      Api.InitializeOnce();
      var (mapper, serializer) =
         Api.ResolveTasks<IExpressionProfileSnapshotToExpressionProfileBuildingBlockMapper, IPKMLPersistor>();

      var expressionProfileBuildingBlock = mapper.MapFrom(expressionProfileSnapshot);
      expressionProfileBuildingBlock.Snapshot = expressionProfileSnapshot;

      return serializer.Serialize(expressionProfileBuildingBlock);
   }
}
