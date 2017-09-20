using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_SimulationBuildingBlockUpdater : ContextSpecification<ISimulationBuildingBlockUpdater>
   {
      protected IBuildingBlockToUsedBuildingBlockMapper _buildingBlockMapper;

      protected override void Context()
      {
         _buildingBlockMapper = A.Fake<IBuildingBlockToUsedBuildingBlockMapper>();
         sut = new SimulationBuildingBlockUpdater(_buildingBlockMapper);
      }
   }

   public class When_updating_the_building_block_used_in_a_simulation : concern_for_SimulationBuildingBlockUpdater
   {
      private IPKSimBuildingBlock _templateBuildingBlock;
      private Simulation _simulation;
      private UsedBuildingBlock _oldUsedBuildingBlock;
      private UsedBuildingBlock _newUsedBuildingBlock;

      protected override void Context()
      {
         base.Context();
         _templateBuildingBlock = A.Fake<IPKSimBuildingBlock>();
         _templateBuildingBlock.Id = "toto";
         _simulation = A.Fake<Simulation>();
         _oldUsedBuildingBlock = A.Fake<UsedBuildingBlock>();
         _newUsedBuildingBlock = A.Fake<UsedBuildingBlock>();
         _newUsedBuildingBlock.Id = "tralala";
         A.CallTo(() => _newUsedBuildingBlock.TemplateId).Returns(_templateBuildingBlock.Id);
         A.CallTo(() => _simulation.UsedBuildingBlockInSimulation(PKSimBuildingBlockType.SimulationSubject)).Returns(_oldUsedBuildingBlock);
         A.CallTo(() => _buildingBlockMapper.MapFrom(_templateBuildingBlock, _oldUsedBuildingBlock)).Returns(_newUsedBuildingBlock);
      }

      protected override void Because()
      {
         sut.UpdateUsedBuildingBlockInSimulationFromTemplate(_simulation, _templateBuildingBlock, PKSimBuildingBlockType.SimulationSubject);
      }

      [Observation]
      public void should_add_the_new_building_block_to_the_simulation()
      {
         A.CallTo(() => _simulation.AddUsedBuildingBlock(_newUsedBuildingBlock)).MustHaveHappened();
      }

      [Observation]
      public void should_remove_the_old_building_used_for_the_given_type()
      {
         A.CallTo(() => _simulation.RemoveUsedBuildingBlock(_oldUsedBuildingBlock)).MustHaveHappened();
      }
   }

   public class When_asked_if_a_quick_update_possible_between_a_building_block_and_a_used_building_block : concern_for_SimulationBuildingBlockUpdater
   {
      private IPKSimBuildingBlock _templateBuildingBlock;
      private UsedBuildingBlock _sameIdAndStructVersionUsedBuildingBlock;
      private UsedBuildingBlock _sameIdAndOtherStructVersionUsedBuildingBlock;
      private UsedBuildingBlock _otherIdAndSameStructVersionUsedBuildingBlock;
      private UsedBuildingBlock _otherIdAndOtherSameStructVersionUsedBuildingBlock;
      private Protocol _templateProtocol;
      private Population _templatePopulation;
      private UsedBuildingBlock _usedProtocolBuildingBlock;
      private UsedBuildingBlock _usedPopulationBuildingBlock;

      protected override void Context()
      {
         base.Context();
         _templateBuildingBlock = A.Fake<IPKSimBuildingBlock>();
         A.CallTo(() => _templateBuildingBlock.StructureVersion).Returns(5);
         A.CallTo(() => _templateBuildingBlock.Id).Returns("templateId");
         _sameIdAndStructVersionUsedBuildingBlock = new UsedBuildingBlock(_templateBuildingBlock.Id, PKSimBuildingBlockType.Compound);
         _sameIdAndStructVersionUsedBuildingBlock.StructureVersion = _templateBuildingBlock.StructureVersion;
         _sameIdAndOtherStructVersionUsedBuildingBlock = new UsedBuildingBlock(_templateBuildingBlock.Id, PKSimBuildingBlockType.Compound);
         _sameIdAndOtherStructVersionUsedBuildingBlock.StructureVersion = 2;
         _otherIdAndSameStructVersionUsedBuildingBlock = new UsedBuildingBlock("another id", PKSimBuildingBlockType.Compound);
         _otherIdAndSameStructVersionUsedBuildingBlock.StructureVersion = _templateBuildingBlock.StructureVersion;
         _otherIdAndOtherSameStructVersionUsedBuildingBlock = new UsedBuildingBlock("another id", PKSimBuildingBlockType.Compound);
         _otherIdAndOtherSameStructVersionUsedBuildingBlock.StructureVersion = 3;
         _templateProtocol = new SimpleProtocol().WithId("_templateProtocol");
         _templateProtocol.StructureVersion = 4;
         _templatePopulation = new RandomPopulation().WithId("_templatePopulation");
         _templatePopulation.StructureVersion = 4;

         _usedProtocolBuildingBlock = new UsedBuildingBlock(_templateProtocol.Id, PKSimBuildingBlockType.Protocol);
         _usedProtocolBuildingBlock.StructureVersion = _templateProtocol.StructureVersion;

         _usedPopulationBuildingBlock = new UsedBuildingBlock(_templatePopulation.Id, PKSimBuildingBlockType.Population);
         _usedPopulationBuildingBlock.StructureVersion = _templatePopulation.StructureVersion;
      }

      [Observation]
      public void should_return_true_if_the_used_building_block_came_from_the_tempalte_building_block_and_share_the_same_structural_version()
      {
         sut.QuickUpdatePossibleFor(_templateBuildingBlock, _sameIdAndStructVersionUsedBuildingBlock).ShouldBeTrue();
      }

      [Observation]
      public void should_return_false_if_the_used_building_block_does_not_come_from_the_building_block()
      {
         sut.QuickUpdatePossibleFor(_templateBuildingBlock, _sameIdAndOtherStructVersionUsedBuildingBlock).ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_if_the_used_building_block_comes_from_the_building_block_but_does_not_share_the_same_structural_version()
      {
         sut.QuickUpdatePossibleFor(_templateBuildingBlock, _otherIdAndSameStructVersionUsedBuildingBlock).ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_if_the_used_building_block_does_not_come_from_the_building_block_and_does_not_share_the_same_structural_version()
      {
         sut.QuickUpdatePossibleFor(_templateBuildingBlock, _otherIdAndOtherSameStructVersionUsedBuildingBlock).ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_if_the_building_block_being_updated_is_a_protocol_building_block_even_is_the_structure_version_are_the_same()
      {
         sut.QuickUpdatePossibleFor(_templateProtocol, _usedProtocolBuildingBlock).ShouldBeFalse();
      }

      [Observation]
      public void should_return_false_if_the_building_block_being_updated_is_a_population_building_block_even_is_the_structure_version_are_the_same()
      {
         sut.QuickUpdatePossibleFor(_templatePopulation, _usedPopulationBuildingBlock).ShouldBeFalse();
      }
   }

   public class When_updating_the_formulation_defined_in_a_simulation_according_to_the_formulation_mapping : concern_for_SimulationBuildingBlockUpdater
   {
      private Simulation _simulation;
      private Formulation _formulation;
      private UsedBuildingBlock _usedBuildingBlockFormulation;
      private IPKSimBuildingBlock _formulationUsedInSimulation;

      protected override void Context()
      {
         base.Context();
         _formulation = new Formulation();
         _simulation = new IndividualSimulation {Properties = new SimulationProperties()};
         _formulationUsedInSimulation = new Formulation();
         _usedBuildingBlockFormulation = new UsedBuildingBlock("template", PKSimBuildingBlockType.Formulation) {BuildingBlock = _formulationUsedInSimulation};
         var compoundProperties = new CompoundProperties();
         var formulationMapping = new FormulationMapping {Formulation = _formulation};
         compoundProperties.ProtocolProperties.AddFormulationMapping(formulationMapping);
         _simulation.Properties.AddCompoundProperties(compoundProperties);
         A.CallTo(() => _buildingBlockMapper.MapFrom(_formulation, null)).Returns(_usedBuildingBlockFormulation);
      }

      protected override void Because()
      {
         sut.UpdateFormulationsInSimulation(_simulation);
      }

      [Observation]
      public void should_add_one_instance_of_each_used_formulation_to_the_simulation()
      {
         _simulation.AllBuildingBlocks<Formulation>().ShouldOnlyContain(_formulationUsedInSimulation);
      }
   }

   public class When_updating_the_formulation_defined_in_a_simulation_according_to_the_formulation_mapping_using_both_a_template_and_a_simulation_formulation_for_the_same_formulation : concern_for_SimulationBuildingBlockUpdater
   {
      private Simulation _simulation;
 
      protected override void Context()
      {
         base.Context();
         var formulation = new Formulation().WithName("F");
         _simulation = new IndividualSimulation { Properties = new SimulationProperties() };
         var formulationUsedInSimulation = new Formulation().WithName("F");
         var compoundProperties = new CompoundProperties();
         compoundProperties.ProtocolProperties.AddFormulationMapping(new FormulationMapping { Formulation = formulation });
         compoundProperties.ProtocolProperties.AddFormulationMapping(new FormulationMapping { Formulation = formulationUsedInSimulation });
         _simulation.Properties.AddCompoundProperties(compoundProperties);
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(()=>sut.UpdateFormulationsInSimulation(_simulation)).ShouldThrowAn<PKSimException>();
      }
   }

   public class When_updating_the_protocol_used_in_a_simulation_according_to_the_protocol_properties : concern_for_SimulationBuildingBlockUpdater
   {
      private Simulation _simulation;
      private Protocol _protocolUsedInSimulation;
      private UsedBuildingBlock _usedBuildingBlockProtocol;
      private Protocol _templateProtocol;
      private CompoundProperties _compoundProperties;

      protected override void Context()
      {
         base.Context();
         _protocolUsedInSimulation = new SimpleProtocol().WithName("P");
         _templateProtocol = new SimpleProtocol().WithName("P");
         _simulation = new IndividualSimulation {Properties = new SimulationProperties()};
         _usedBuildingBlockProtocol = new UsedBuildingBlock("template", PKSimBuildingBlockType.Protocol) {BuildingBlock = _protocolUsedInSimulation};
         _compoundProperties = new CompoundProperties {ProtocolProperties = {Protocol = _templateProtocol}};
         _simulation.Properties.AddCompoundProperties(_compoundProperties);
         A.CallTo(() => _buildingBlockMapper.MapFrom(_templateProtocol, null)).Returns(_usedBuildingBlockProtocol);
      }

      protected override void Because()
      {
         sut.UpdateProtocolsInSimulation(_simulation);
      }

      [Observation]
      public void should_add_one_instance_protocol_for_each_well_defined_protocol()
      {
         _simulation.AllBuildingBlocks<Protocol>().ShouldOnlyContain(_protocolUsedInSimulation);
      }

      [Observation]
      public void should_update_the_reference_to_the_protocol_to_the_one_used_in_the_simulation()
      {
         _compoundProperties.ProtocolProperties.Protocol.ShouldBeEqualTo(_protocolUsedInSimulation);
      }
   }
}