using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;

namespace PKSim.Core.Mappers
{
   public interface IBuildingBlockToUsedBuildingBlockMapper
   {
      UsedBuildingBlock MapFrom(IPKSimBuildingBlock buildingBlock, UsedBuildingBlock previousUsedBuildingBlock);
   }

   public class BuildingBlockToUsedBuildingBlockMapper : IBuildingBlockToUsedBuildingBlockMapper
   {
      private readonly ICloner _cloner;
      private readonly IBuildingBlockRepository _buildingBlockRepository;

      public BuildingBlockToUsedBuildingBlockMapper(ICloner cloner, IBuildingBlockRepository buildingBlockRepository)
      {
         _cloner = cloner;
         _buildingBlockRepository = buildingBlockRepository;
      }

      public UsedBuildingBlock MapFrom(IPKSimBuildingBlock buildingBlock, UsedBuildingBlock previousUsedBuildingBlock)
      {
         //check if the given building block is a real template building block. 
         //If not, this is a modified building block in a simulation that should be used as is
         bool isTemplate = (_buildingBlockRepository.FindById(buildingBlock.Id) != null || previousUsedBuildingBlock == null);

         if (isTemplate)
         {
            return new UsedBuildingBlock(buildingBlock.Id, buildingBlock.BuildingBlockType)
               {
                  BuildingBlock = usedCloneInSimulationFrom(buildingBlock),
                  Version = buildingBlock.Version,
                  StructureVersion = buildingBlock.StructureVersion,
                  Name = buildingBlock.Name,
               };
         }

         //the building block is not a template building block. Return the previously used building block
         return previousUsedBuildingBlock;
      }

      private IPKSimBuildingBlock usedCloneInSimulationFrom(IPKSimBuildingBlock templateBuildingBlock)
      {
         return _cloner.Clone(templateBuildingBlock);
      }
   }
}