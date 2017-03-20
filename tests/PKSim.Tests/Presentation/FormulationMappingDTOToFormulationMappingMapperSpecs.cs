using System.Collections.Generic;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Presentation.DTO.Simulations;
using OSPSuite.Core.Domain;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Presentation.DTO.Mappers;


namespace PKSim.Presentation
{
   public abstract class concern_for_FormulationMappingDTOToFormulationMappingMapper : ContextSpecification<IFormulationMappingDTOToFormulationMappingMapper>
   {
      private IBuildingBlockRepository _buildingBlockRepository;
      protected IList<IPKSimBuildingBlock> _allBuildingBlocks;

      protected override void Context()
      {
         _allBuildingBlocks = new List<IPKSimBuildingBlock>();
         _buildingBlockRepository = A.Fake<IBuildingBlockRepository>();
         A.CallTo(() => _buildingBlockRepository.All()).Returns(_allBuildingBlocks);
         sut = new FormulationMappingDTOToFormulationMappingMapper(_buildingBlockRepository);
      }
   }

   public class When_mapping_an_application_formulation_mapping_dto_to_a_formulation_mapping_using_a_template_formulation : concern_for_FormulationMappingDTOToFormulationMappingMapper
   {
      private FormulationMappingDTO _formulationMappingDTO;
      private Formulation _formulation;
      private FormulationMapping _result;
      private Simulation _simulation;

      protected override void Context()
      {
         base.Context();
         _formulation = A.Fake<Formulation>();
         _formulation.Id = "templateId";
         _allBuildingBlocks.Add(_formulation);
         _simulation = new IndividualSimulation();
         _formulationMappingDTO = new FormulationMappingDTO {ApplicationType = ApplicationTypes.Oral, Selection = new FormulationSelectionDTO {BuildingBlock = _formulation} , FormulationKey = "tata"};
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_formulationMappingDTO, _simulation);
      }

      [Observation]
      public void should_return_a_formulation_mapping_whose_formulation_key_was_set_to_the_key_used_in_the_dto()
      {
         _result.FormulationKey.ShouldBeEqualTo(_formulationMappingDTO.FormulationKey);
      }

      [Observation]
      public void should_return_a_formulation_mapping_whose_formulation_id_was_set_to_the_id_of_the_formulation_used_for_the_mapping()
      {
         _result.TemplateFormulationId.ShouldBeEqualTo(_formulation.Id);
      }


      [Observation]
      public void should_save_a_reference_to_the_formulation_used()
      {
         _result.Formulation.ShouldBeEqualTo(_formulation);
      }
   }

   public class When_mapping_an_application_formulation_mapping_dto_to_a_formulation_mapping_using_a_formulation_building_block_defined_in_a_simulation : concern_for_FormulationMappingDTOToFormulationMappingMapper
   {
      private FormulationMappingDTO _formulationMappingDTO;
      private Formulation _formulation;
      private FormulationMapping _result;
      private Simulation _simulation;

      protected override void Context()
      {
         base.Context();
         _formulation = A.Fake<Formulation>();
         _formulation.Id = "simFormulation";
         _simulation = new IndividualSimulation();
         var usedBuildingBlock = new UsedBuildingBlock("templateId", PKSimBuildingBlockType.Formulation);
         _simulation.AddUsedBuildingBlock(usedBuildingBlock);
         usedBuildingBlock.BuildingBlock = _formulation;
         _formulationMappingDTO = new FormulationMappingDTO {ApplicationType = ApplicationTypes.Oral, Selection = new FormulationSelectionDTO {BuildingBlock = _formulation}, FormulationKey = "tata"};
      }

      protected override void Because()
      {
         _result = sut.MapFrom(_formulationMappingDTO, _simulation);
      }

      [Observation]
      public void should_return_a_formulation_mapping_whose_formulation_key_was_set_to_the_key_used_in_the_dto()
      {
         _result.FormulationKey.ShouldBeEqualTo(_formulationMappingDTO.FormulationKey);
      }

      [Observation]
      public void should_return_a_formulation_mapping_whose_formulation_id_was_set_to_the_id_of_the_template_formulation()
      {
         _result.TemplateFormulationId.ShouldBeEqualTo("templateId");
      }
   }
}