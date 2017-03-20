using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using FakeItEasy;

namespace PKSim.Core
{
   public abstract class concern_for_BuildingBlockToUsedBuildingBlockMapper : ContextSpecification<IBuildingBlockToUsedBuildingBlockMapper>
   {
      protected IBuildingBlockRepository _buildingBlockRepository;
      protected ICloner _cloneManager;

      protected override void Context()
      {
         _buildingBlockRepository = A.Fake<IBuildingBlockRepository>();
         _cloneManager = A.Fake<ICloner>();
         sut = new BuildingBlockToUsedBuildingBlockMapper(_cloneManager, _buildingBlockRepository);
      }
   }

   
   public class When_mapping_a_building_block_defined_as_a_template_building_block_repository : concern_for_BuildingBlockToUsedBuildingBlockMapper
   {
      private IPKSimBuildingBlock _templateBuildingBlock;
      private UsedBuildingBlock _result;
      private IPKSimBuildingBlock _cloneBuildingBlock;
      private UsedBuildingBlock _oldUsedBuildingBlock;

      protected override void Context()
      {
         base.Context();
         _templateBuildingBlock =A.Fake<IPKSimBuildingBlock>();
         _cloneBuildingBlock = A.Fake<IPKSimBuildingBlock>();
         A.CallTo(() => _templateBuildingBlock.Id).Returns("Id");
         A.CallTo(() => _templateBuildingBlock.Version).Returns(5);
         A.CallTo(() => _templateBuildingBlock.StructureVersion).Returns(8);
         A.CallTo(() => _buildingBlockRepository.All()).Returns(new[] {_templateBuildingBlock,});
         A.CallTo(() => _cloneManager.Clone(_templateBuildingBlock)).Returns(_cloneBuildingBlock);
         _oldUsedBuildingBlock = null;
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_templateBuildingBlock,_oldUsedBuildingBlock);
      }

      [Observation]
      public void should_have_used_a_clone_of_the_template_building_block()
      {
         _result.BuildingBlock.ShouldBeEqualTo(_cloneBuildingBlock);
      }

      [Observation]
      public void should_have_set_the_altered_flag_to_false()
      {
         _result.Altered.ShouldBeFalse();
      }


      [Observation]
      public void should_have_set_the_version_and_structure_version_of_the_used_building_block_equals_to_the_one_defined_in_the_building_block()
      {
         _result.Version.ShouldBeEqualTo(_templateBuildingBlock.Version);
         _result.StructureVersion.ShouldBeEqualTo(_templateBuildingBlock.StructureVersion);
      }


   }

   
   public class When_mapping_a_building_block_defined_that_is_specific_to_a_given_simulation : concern_for_BuildingBlockToUsedBuildingBlockMapper
   {
      private IPKSimBuildingBlock _buildingBlockDefinedInSimulation;
      private UsedBuildingBlock _result;
      private UsedBuildingBlock _oldUsedBuildingBlock;

      protected override void Context()
      {
         base.Context();
         _buildingBlockDefinedInSimulation = A.Fake<IPKSimBuildingBlock>();
         A.CallTo(() => _buildingBlockDefinedInSimulation.Id).Returns("Id");
         A.CallTo(() => _buildingBlockDefinedInSimulation.Version).Returns(5);
         A.CallTo(() => _buildingBlockDefinedInSimulation.StructureVersion).Returns(8);
         A.CallTo(() => _buildingBlockRepository.All()).Returns(new List<IPKSimBuildingBlock>());
         _oldUsedBuildingBlock = A.Fake<UsedBuildingBlock>();
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_buildingBlockDefinedInSimulation, _oldUsedBuildingBlock);
      }

      [Observation]
      public void should_return_the_old_used_building_block()
      {
         _result.ShouldBeEqualTo(_oldUsedBuildingBlock);
      }

   }
}	