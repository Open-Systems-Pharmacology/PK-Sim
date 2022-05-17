using System;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_FormulationFromMappingRetriever : ContextSpecification<IFormulationFromMappingRetriever>
   {
      protected Simulation _simulation;
      protected IBuildingBlockInProjectManager _buildingBlockInSmulationManager;
      protected IBuildingBlockRepository _buildingBlockRepository;

      protected override void Context()
      {
         _simulation = new IndividualSimulation();
         _buildingBlockInSmulationManager = A.Fake<IBuildingBlockInProjectManager>();
         _buildingBlockRepository= A.Fake<IBuildingBlockRepository>();
         sut = new FormulationFromMappingRetriever(_buildingBlockInSmulationManager,_buildingBlockRepository);
      }
   }

   public class When_retrieving_a_formulation_used_for_a_mapping : concern_for_FormulationFromMappingRetriever
   {
      private Formulation _formulation1;
      private Formulation _formulation2;
      private Formulation _templateFormulation;

      protected override void Context()
      {
         base.Context();
         _templateFormulation= A.Fake<Formulation>();
         _formulation1 = A.Fake<Formulation>().WithName("f1");
         _formulation1.Id = "1";
         _formulation2 = A.Fake<Formulation>().WithName("f2");
         _formulation2.Id = "2";
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock(_formulation1.Id, PKSimBuildingBlockType.Formulation) {BuildingBlock = _formulation1});
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock(_formulation2.Id, PKSimBuildingBlockType.Formulation) {BuildingBlock = _formulation2});
         A.CallTo(() => _buildingBlockRepository.ById<Formulation>("template")).Returns(_templateFormulation);
         A.CallTo(() => _buildingBlockRepository.ById<Formulation>("does not exist")).Returns(null);
      }

      [Observation]
      public void should_return_null_if_the_given_mapping_is_not_well_defined()
      {
         sut.FormulationUsedBy(_simulation, null).ShouldBeNull();
      }

      [Observation]
      public void should_return_null_if_the_mapping_does_not_use_any_formulation()
      {
         sut.FormulationUsedBy(_simulation, new FormulationMapping()).ShouldBeNull();
      }

      [Observation]
      public void should_return_a_formulation_if_the_mapping_is_well_defined()
      {
         sut.FormulationUsedBy(_simulation, new FormulationMapping {TemplateFormulationId = _formulation1.Id}).ShouldBeEqualTo(_formulation1);
      }

      [Observation]
      public void should_return_null_if_the_id_is_not_well_defined()
      {
         sut.FormulationUsedBy(_simulation, new FormulationMapping {TemplateFormulationId = "does not exist"}).ShouldBeNull();
      }

      [Observation]
      public void should_return_the_template_building_block_if_the_simulation_was_not_initialized()
      {
         sut.FormulationUsedBy(_simulation, new FormulationMapping { TemplateFormulationId = "template" }).ShouldBeEqualTo(_templateFormulation);
      }
   }

   public class When_retrieving_the_template_formulation_used_for_a_mapping_in_a_simulation : concern_for_FormulationFromMappingRetriever
   {
      private Formulation _templateFormulation;
      private Formulation _localFormulation;
      private Formulation _result;

      protected override void Context()
      {
         base.Context();
         _templateFormulation = new Formulation().WithId("f1");
         _localFormulation = new Formulation().WithId("f2");
         var usedBuildingBlock = new UsedBuildingBlock("f1", PKSimBuildingBlockType.Formulation) {BuildingBlock = _localFormulation};
         _simulation.AddUsedBuildingBlock(usedBuildingBlock);
         A.CallTo(() => _buildingBlockInSmulationManager.TemplateBuildingBlockUsedBy<Formulation>(usedBuildingBlock)).Returns(_localFormulation);
      }

      protected override void Because()
      {
         _result = sut.TemplateFormulationUsedBy(_simulation, new FormulationMapping {TemplateFormulationId = _templateFormulation.Id});
      }

      [Observation]
      public void should_return_the_actual_building_block()
      {
         _result.ShouldBeEqualTo(_localFormulation);
      }
   }
}