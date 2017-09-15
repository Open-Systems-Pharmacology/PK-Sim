using System;
using OSPSuite.Utility.Visitor;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Model
{
   public class UsedBuildingBlock : IVersionable, IVisitable<IVisitor>, IWithName
   {
      private IPKSimBuildingBlock _buildingBlock;

      /// <summary>
      ///    Type of the building block
      /// </summary>
      public virtual PKSimBuildingBlockType BuildingBlockType { get; }

      /// <summary>
      ///    Id of the template building block used in the project (this is not the id of the buildingblock saved as member but
      ///    the id of the template
      ///    used to generate the building block (e.g. orign))
      /// </summary>
      public virtual string TemplateId { get; }

      public virtual int Version { get; set; }

      public virtual int StructureVersion { get; set; }

      /// <summary>
      ///    Was the original building block altered in the simulation
      /// </summary>
      public virtual bool Altered { get; set; }

      /// <summary>
      ///    Id of building block used in the simulation
      /// </summary>
      public string Id { get; set; }

      /// <summary>
      ///    Name of the building block (based on the tempalte building block). The name will be changed when the building
      ///    block in the simulation is changed as well. The name of the internal building block however remains unchanged
      /// </summary>
      public virtual string Name { get; set; }

      [Obsolete("For serialization")]
      public UsedBuildingBlock()
      {
      }

      public UsedBuildingBlock(string templateId, PKSimBuildingBlockType buildingBlockType)
      {
         TemplateId = templateId;
         BuildingBlockType = buildingBlockType;
      }

      public virtual UsedBuildingBlock Clone(ICloneManager cloneManager)
      {
         return new UsedBuildingBlock(TemplateId, BuildingBlockType)
         {
            Altered = Altered,
            Version = Version,
            StructureVersion = StructureVersion,
            BuildingBlock = cloneManager.Clone(BuildingBlock),
            Name = Name
         };
      }

      /// <summary>
      ///    Clone of the project building block used in the simulation
      /// </summary>
      public virtual IPKSimBuildingBlock BuildingBlock
      {
         get => _buildingBlock;
         set
         {
            _buildingBlock = value;
            updateBuildingBlockId();
         }
      }

      private void updateBuildingBlockId()
      {
         if (BuildingBlock != null)
            Id = BuildingBlock.Id;
      }

      /// <summary>
      ///    Update the version and structural version from the given building block, only if the building block is not loaded
      /// </summary>
      public virtual void UpdateVersionFrom(UsedBuildingBlock usedBuildingBlock)
      {
         if (usedBuildingBlock.BuildingBlock != null) return;

         Version = usedBuildingBlock.Version;
         StructureVersion = usedBuildingBlock.StructureVersion;
      }

      public virtual void AcceptVisitor(IVisitor visitor)
      {
         BuildingBlock?.AcceptVisitor(visitor);
         //Update the internal id as the visitor might have reset the id
         updateBuildingBlockId();
      }

      public virtual bool IsLoaded
      {
         get => BuildingBlock?.IsLoaded ?? false;
         set
         {
            if (BuildingBlock != null)
               BuildingBlock.IsLoaded = value;
         }
      }
   }
}