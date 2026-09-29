using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Qualification;
using PKSim.CLI.Core.Services;
using PKSim.Core.Services;
using static PKSim.Starter.ExchangeSerializer;

namespace PKSim.Starter;

public static class SnapshotExchange
{
   public static object CreateIndividualBuildingBlock(string individualSnapshot)
   {
      var container = CLIApplicationStartup.Initialize();
      var mapper = container.Resolve<IIndividualSnapshotToIndividualBuildingBlockMapper>();
      var individualBuildingBlock = mapper.MapFrom(individualSnapshot);
      individualBuildingBlock.Snapshot = individualSnapshot;
      return Serialize(individualBuildingBlock, container);
   }

   public static object CreateExpressionProfileBuildingBlock(string expressionProfileSnapshot)
   {
      var container = CLIApplicationStartup.Initialize();
      var mapper = container.Resolve<IExpressionProfileSnapshotToExpressionProfileBuildingBlockMapper>();
      
      var expressionProfileBuildingBlock = mapper.MapFrom(expressionProfileSnapshot);
      expressionProfileBuildingBlock.Snapshot = expressionProfileSnapshot;
      return Serialize(expressionProfileBuildingBlock, container);
   }

   public static object CreateModule(string projectSnapshot)
   {
      var container = CLIApplicationStartup.Initialize();
      var projectSnapshotToModuleMapper = container.Resolve<IProjectSnapshotToModuleMapper>();

      var module = projectSnapshotToModuleMapper.MapFrom(projectSnapshot).module;
      container.Resolve<IRepresentationInfoUpdater>().UpdateRepresentationInfoIn(module);
      var objectIdResetter = container.Resolve<IObjectIdResetter>();
      objectIdResetter.ResetIdFor(module);
      return Serialize(module, container);
   }

   public static object CreateModuleAndExportInputs(string projectSnapshot, QualificationConfiguration qualificationConfiguration)
   {
      var container = CLIApplicationStartup.Initialize();
      var projectSnapshotToSimulationTransferMapper = container.Resolve<IProjectSnapshotToModuleMapper>();
      var qualificationInputTask = container.Resolve<IQualificationInputTask>();

      var (module, project) = projectSnapshotToSimulationTransferMapper.MapFrom(projectSnapshot);
      container.Resolve<IRepresentationInfoUpdater>().UpdateRepresentationInfoIn(module);
      var objectIdResetter = container.Resolve<IObjectIdResetter>();
      objectIdResetter.ResetIdFor(module);
      var inputMappings = qualificationInputTask.ExportInputs(project, qualificationConfiguration);

      return (Serialize(module, container), inputMappings);
   }
}