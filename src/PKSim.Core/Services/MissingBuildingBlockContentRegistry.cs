using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Services
{
   /// <summary>
   ///    Describes a building block whose content is missing from the project file (the CONTENTS row referenced by the
   ///    building block holds no data). Such a building block cannot be deserialized and is skipped when the project is
   ///    loaded. The properties kept here are the ones still available in the project file.
   /// </summary>
   public class BuildingBlockWithMissingContent
   {
      public string Id { get; set; }
      public string Name { get; set; }
      public string Description { get; set; }
      public int Version { get; set; }
      public int StructureVersion { get; set; }
      public PKSimBuildingBlockType BuildingBlockType { get; set; }

      /// <summary>
      ///    Ids of the simulation subjects (individuals or populations) referencing the building block. Only relevant for
      ///    expression profiles
      /// </summary>
      public IReadOnlyList<string> SimulationSubjectIds { get; set; } = new List<string>();
   }

   /// <summary>
   ///    Collects the building blocks skipped while loading a project because their content is missing from the project
   ///    file, so that they can be recovered once the project is loaded (see
   ///    <see cref="IMissingBuildingBlockContentRecoveryTask" />)
   /// </summary>
   public interface IMissingBuildingBlockContentRegistry
   {
      void Add(BuildingBlockWithMissingContent buildingBlockWithMissingContent);
      IReadOnlyList<BuildingBlockWithMissingContent> All();
      void Clear();
   }

   public class MissingBuildingBlockContentRegistry : IMissingBuildingBlockContentRegistry
   {
      private readonly List<BuildingBlockWithMissingContent> _allBuildingBlocksWithMissingContent = new List<BuildingBlockWithMissingContent>();

      public void Add(BuildingBlockWithMissingContent buildingBlockWithMissingContent) => _allBuildingBlocksWithMissingContent.Add(buildingBlockWithMissingContent);

      public IReadOnlyList<BuildingBlockWithMissingContent> All() => _allBuildingBlocksWithMissingContent.ToList();

      public void Clear() => _allBuildingBlocksWithMissingContent.Clear();
   }
}
