using PKSim.Core.Model;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Mappers
{
    public interface IBuildingBlockPropertiesMapper
    {
       void MapProperties(ObjectBaseDTO buildingBlockDTO, IPKSimBuildingBlock buildingBlockToMap);
    }

    public class BuildingBlockPropertiesMapper : IBuildingBlockPropertiesMapper
    {
       public void MapProperties(ObjectBaseDTO buildingBlockDTO, IPKSimBuildingBlock buildingBlockToMap)
        {
            buildingBlockToMap.Name = buildingBlockDTO.Name;
            buildingBlockToMap.Description = buildingBlockDTO.Description;
        }
    }
}