using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Core.Qualification;
using OSPSuite.Core.Serialization.Exchange;
using OSPSuite.Core.Serialization.Xml;
using PKSim.CLI.Core.Services;
using PKSim.Core.Services;
using IContainer = OSPSuite.Utility.Container.IContainer;

namespace PKSim.Starter;

public static class SnapshotExchange
{
   public static object CreateIndividualBuildingBlock(string individualSnapshot)
   {
      var container = CLIApplicationStartup.Initialize();
      var mapper = container.Resolve<IIndividualSnapshotToIndividualBuildingBlockMapper>();

      var individualBuildingBlock = mapper.MapFrom(individualSnapshot);
      individualBuildingBlock.Snapshot = individualSnapshot;
      return serialize(individualBuildingBlock, container);
   }

   public static object CreateExpressionProfileBuildingBlock(string expressionProfileSnapshot)
   {
      var container = CLIApplicationStartup.Initialize();
      var mapper = container.Resolve<IExpressionProfileSnapshotToExpressionProfileBuildingBlockMapper>();
      var expressionProfileBuildingBlock = mapper.MapFrom(expressionProfileSnapshot);
      expressionProfileBuildingBlock.Snapshot = expressionProfileSnapshot;
      return serialize(expressionProfileBuildingBlock, container);
   }

   public static object CreateModule(string projectSnapshot)
   {
      var container = CLIApplicationStartup.Initialize();
      var projectSnapshotToModuleMapper = container.Resolve<IProjectSnapshotToModuleMapper>();

      var module = projectSnapshotToModuleMapper.MapFrom(projectSnapshot).module;
      var objectIdResetter = container.Resolve<IObjectIdResetter>();
      objectIdResetter.ResetIdFor(module);
      return serialize(module, container);
   }

   public static object CreateModuleAndExportInputs(string projectSnapshot, QualificationConfiguration qualificationConfiguration)
   {
      var container = CLIApplicationStartup.Initialize();
      var projectSnapshotToSimulationTransferMapper = container.Resolve<IProjectSnapshotToModuleMapper>();
      var qualificationInputTask = container.Resolve<IQualificationInputTask>();
      
      var (module, project) = projectSnapshotToSimulationTransferMapper.MapFrom(projectSnapshot);
      var objectIdResetter = container.Resolve<IObjectIdResetter>();
      objectIdResetter.ResetIdFor(module);
      var inputMappings = qualificationInputTask.ExportInputs(project, qualificationConfiguration);

      return (serialize(module, container), inputMappings);
   }

   // Serialize is required so that PKSimSpatialStructures are changed to MoBiSpatialStructures during exchange
   private static string serialize<T>(T itemToSerialize, IContainer container)
   {
      var pkmlPersistor = container.Resolve<IPKMLPersistor>();
      return pkmlPersistor.Serialize(itemToSerialize);
   }
}