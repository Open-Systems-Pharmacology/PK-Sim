using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Core;
using PKSim.Presentation.DTO.Mappers;
using FakeItEasy;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation
{
    public abstract class concern_for_BuildingBlockPropertiesMapper : ContextSpecification<IBuildingBlockPropertiesMapper>
    {
        protected override void Context()
        {
            sut = new BuildingBlockPropertiesMapper();
        }
    }

    
    public class When_mapping_a_building_block_properties_dto_to_a_building_block : concern_for_BuildingBlockPropertiesMapper
    {
        private IPKSimBuildingBlock _buildingBlock;
        private ObjectBaseDTO _buildingBlockDTO;

        protected override void Context()
        {
            base.Context();
           _buildingBlockDTO = new ObjectBaseDTO();
           _buildingBlock = A.Fake<IPKSimBuildingBlock>();
            _buildingBlockDTO.Name="Name";
            _buildingBlockDTO.Description = "Description";
        }

        protected override void Because()
        {
            sut.MapProperties(_buildingBlockDTO, _buildingBlock);
        }

        [Observation]
        public void should_update_the_name_of_the_building_block()
        {
            _buildingBlock.Name.ShouldBeEqualTo(_buildingBlockDTO.Name);
        }

        [Observation]
        public void should_update_the_desciption_of_the_building_block()
        {
            _buildingBlock.Description.ShouldBeEqualTo(_buildingBlockDTO.Description);
        }
    }
}