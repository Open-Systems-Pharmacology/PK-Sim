using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;
using OSPSuite.Core.Domain;

namespace PKSim.Core
{
   public abstract class concern_for_CompoundPropertiesUpdater : ContextSpecification<ICompoundPropertiesUpdater>
   {
      private ICompoundToCompoundPropertiesMapper _compoundPropertiesMapper;
      protected Simulation _simulation;
      protected Compound _compound1;
      protected Compound _compound2;

      protected override void Context()
      {
         _compound1 = new Compound().WithId("1").WithName("Compound1");
         _compound2 = new Compound().WithId("2").WithName("Compound2");

         _simulation = new IndividualSimulation {Properties = new SimulationProperties()};
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("Temp1", PKSimBuildingBlockType.Compound) {BuildingBlock = _compound1});
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("Temp2", PKSimBuildingBlockType.Compound) {BuildingBlock = _compound2});

         _compoundPropertiesMapper = A.Fake<ICompoundToCompoundPropertiesMapper>();
         A.CallTo(() => _compoundPropertiesMapper.MapFrom(A<Compound>._)).ReturnsLazily(x => new CompoundProperties {Compound = x.GetArgument<Compound>(0)});
         sut = new CompoundPropertiesUpdater(_compoundPropertiesMapper);
      }
   }

   public class When_updating_the_compound_properties_of_a_simulation_with_brand_new_compounds : concern_for_CompoundPropertiesUpdater
   {
      protected override void Because()
      {
         sut.UpdateCompoundPropertiesIn(_simulation);
      }

      [Observation]
      public void should_add_one_compound_properties_per_compound()
      {
         _simulation.CompoundPropertiesFor(_compound1).ShouldNotBeNull();
         _simulation.CompoundPropertiesFor(_compound2).ShouldNotBeNull();
      }
   }

   public class When_updating_the_compound_properties_of_a_simulation_with_existing_compound_properties : concern_for_CompoundPropertiesUpdater
   {
      private CompoundProperties _compoundProperties;

      protected override void Context()
      {
         base.Context();
         _compoundProperties = new CompoundProperties {Compound = new Compound().WithName(_compound1.Name)};
         _simulation.Properties.AddCompoundProperties(_compoundProperties);
      }

      protected override void Because()
      {
         sut.UpdateCompoundPropertiesIn(_simulation);
      }

      [Observation]
      public void should_use_the_exsiting_one_instead()
      {
         _simulation.CompoundPropertiesFor(_compound1).ShouldNotBeNull();
      }

      [Observation]
      public void should_add_a_new_compound_properties_for_the_new_compound()
      {
         _simulation.CompoundPropertiesFor(_compound2).ShouldNotBeNull();
      }
   }

   public class When_updating_the_compound_properties_of_a_simulation_where_some_compounds_have_been_removed : concern_for_CompoundPropertiesUpdater
   {
      protected override void Context()
      {
         base.Context();
         _simulation.Properties.AddCompoundProperties(new CompoundProperties { Compound =  _compound1});
         _simulation.Properties.AddCompoundProperties(new CompoundProperties { Compound =  _compound2});
         _simulation.RemoveAllBuildingBlockOfType(PKSimBuildingBlockType.Compound);
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("Temp1", PKSimBuildingBlockType.Compound) { BuildingBlock = _compound1 });
      }

      protected override void Because()
      {
         sut.UpdateCompoundPropertiesIn(_simulation);
      }

      [Observation]
      public void should_use_the_exsiting_one()
      {
         _simulation.CompoundPropertiesFor(_compound1).ShouldNotBeNull();
      }

      [Observation]
      public void should_remove_the_compound_properties_correspinding_to_non_existing_compound()
      {
         _simulation.CompoundPropertiesFor(_compound2).ShouldBeNull();
      }
   }

   public class When_updating_the_compound_properties_of_a_simulation_where_one_compound_was_switch_for_another_compound : concern_for_CompoundPropertiesUpdater
   {
      private CompoundProperties _compoundProperties;
      private ProtocolProperties _protocolProperties;

      protected override void Context()
      {
         base.Context();
         _protocolProperties = new ProtocolProperties();
         _simulation.RemoveUsedBuildingBlock(_compound2);
         _compoundProperties = new CompoundProperties { Compound = new Compound().WithName(_compound2.Name), ProtocolProperties = _protocolProperties};
         _simulation.Properties.AddCompoundProperties(_compoundProperties);

      }
      protected override void Because()
      {
         sut.UpdateCompoundPropertiesIn(_simulation);
      }

      [Observation]
      public void should_use_the_existing_protocol_properties_instead_of_creating_a_new_one()
      {
         _simulation.CompoundPropertiesFor(_compound1).ProtocolProperties.ShouldBeEqualTo(_protocolProperties);
      }
   }
}