using OSPSuite.Utility.Validation;
using PKSim.Core.Model;
using PKSim.Presentation.DTO;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;

namespace PKSim.Presentation
{
    public abstract class concern_for_BuildingBlockSelectionDTO : ContextSpecification<BuildingBlockSelectionDTO>
    {
        protected override void Context()
        {
            sut = new BuildingBlockSelectionDTO();
        }
    }

    
    public class When_validating_a_building_block_dto_for_which_a_building_block_has_been_set : concern_for_BuildingBlockSelectionDTO
    {
        protected override void Context()
        {
            base.Context();
            sut.BuildingBlock = A.Fake<IPKSimBuildingBlock>();
        }
        [Observation]
        public void should_return_a_valide_state()
        {
            sut.IsValid().ShouldBeTrue();
        }
    }

    
    public class When_validating_a_building_block_dto_for_which_a_building_block_has_not_been_set : concern_for_BuildingBlockSelectionDTO
    {
        [Observation]
        public void should_return_a_valide_state()
        {
            sut.IsValid().ShouldBeFalse();
        }
    }
}	