using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters.Nodes;
using PKSim.Presentation.Mappers;
using PKSim.Presentation.Nodes;

namespace PKSim.Presentation;

public abstract class concern_for_PKSimClassificationTypeToRootNodeTypeMapper : ContextSpecification<PKSimClassificationTypeToRootNodeTypeMapper>
{
   protected override void Context()
   {
      sut = new PKSimClassificationTypeToRootNodeTypeMapper();
   }
}

public class When_mapping_a_building_block_classification_type_to_a_root_node : concern_for_PKSimClassificationTypeToRootNodeTypeMapper
{
   [Observation]
   public void should_map_each_building_block_classification_type_to_its_folder()
   {
      sut.MapFrom(ClassificationType.Compound).ShouldBeEqualTo(PKSimRootNodeTypes.CompoundFolder);
      sut.MapFrom(ClassificationType.Formulation).ShouldBeEqualTo(PKSimRootNodeTypes.FormulationFolder);
      sut.MapFrom(ClassificationType.Individual).ShouldBeEqualTo(PKSimRootNodeTypes.IndividualFolder);
      sut.MapFrom(ClassificationType.Population).ShouldBeEqualTo(PKSimRootNodeTypes.PopulationFolder);
      sut.MapFrom(ClassificationType.Protocol).ShouldBeEqualTo(PKSimRootNodeTypes.ProtocolFolder);
      sut.MapFrom(ClassificationType.Event).ShouldBeEqualTo(PKSimRootNodeTypes.EventFolder);
      sut.MapFrom(ClassificationType.ObserverSet).ShouldBeEqualTo(PKSimRootNodeTypes.ObserverSetFolder);
      sut.MapFrom(ClassificationType.ExpressionProfile).ShouldBeEqualTo(PKSimRootNodeTypes.ExpressionProfileFolder);
   }

   [Observation]
   public void should_delegate_non_building_block_types_to_the_default_core_mapper()
   {
      sut.MapFrom(ClassificationType.ObservedData).ShouldBeEqualTo(RootNodeTypes.ObservedDataFolder);
      sut.MapFrom(ClassificationType.Simulation).ShouldBeEqualTo(RootNodeTypes.SimulationFolder);
   }
}
