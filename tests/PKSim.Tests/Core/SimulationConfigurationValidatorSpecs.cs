using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.UnitSystem;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core
{
   public abstract class concern_for_SimulationConfigurationValidator : ContextSpecification<ISimulationConfigurationValidator>
   {
      protected Simulation _simulation;
      protected Individual _individual;
      protected Compound _compound;
      protected Protocol _protocol;
      protected IProtocolToSchemaItemsMapper _protocolToProtocolSchemaItemMapper;
      protected SpeciesPopulation _speciesPopulation;
      private SchemaItem _schemaItem;
      protected Unit _doseUnit;

      protected override void Context()
      {
         _simulation = new IndividualSimulation
         {
            Properties = new SimulationProperties(),
            SimulationSettings = new SimulationSettings(),
            ModelConfiguration = new ModelConfiguration()
         };
         _individual = new Individual().WithName("MyIndividuyal");
         _speciesPopulation = new SpeciesPopulation();

         _individual.OriginData = new OriginData {SpeciesPopulation = _speciesPopulation};
         _compound = A.Fake<Compound>().WithName("MyCompound");
         _protocol = A.Fake<Protocol>().WithName("MyProtocol");

         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("Individual", PKSimBuildingBlockType.Individual) { BuildingBlock = _individual });
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("Compound", PKSimBuildingBlockType.Compound) { BuildingBlock = _compound });
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("Protocol", PKSimBuildingBlockType.Protocol) { BuildingBlock = _protocol });

         _protocolToProtocolSchemaItemMapper = A.Fake<IProtocolToSchemaItemsMapper>();
         sut = new SimulationConfigurationValidator(_protocolToProtocolSchemaItemMapper);

         _speciesPopulation.IsHeightDependent = false;
         _schemaItem= A.Fake<SchemaItem>();
         _doseUnit= A.Fake<Unit>();
         _schemaItem.Dose.DisplayUnit = _doseUnit;
         A.CallTo(() => _protocolToProtocolSchemaItemMapper.MapFrom(_protocol)).Returns(new []{_schemaItem});
      }
   }

   public class When_validating_the_configuration_of_a_valid_simulation_using_the_two_pore_model : concern_for_SimulationConfigurationValidator
   {
      protected override void Context()
      {
         base.Context();
         _simulation.ModelConfiguration.ModelName = CoreConstants.Model.TwoPores;
         A.CallTo(() => _compound.IsSmallMolecule).Returns(false);
      }

      [Observation]
      public void should_not_throw_an_exception()
      {
         sut.ValidateConfigurationFor(_simulation);
      }
   }

   public class When_validating_the_configuration_of_a_valid_simulation_using_the_four_comp_model : concern_for_SimulationConfigurationValidator
   {
      protected override void Context()
      {
         base.Context();
         _simulation.ModelConfiguration.ModelName = CoreConstants.Model.FourComp;
         A.CallTo(() => _compound.IsSmallMolecule).Returns(true);
      }

      [Observation]
      public void should_not_throw_an_exception()
      {
         sut.ValidateConfigurationFor(_simulation);
      }
   }

   public class When_validating_the_configuration_of_a_simulation_using_a_small_molecule_with_the_two_pores_model : concern_for_SimulationConfigurationValidator
   {
      protected override void Context()
      {
         base.Context();
         _simulation.ModelConfiguration.ModelName = CoreConstants.Model.TwoPores;
         A.CallTo(() => _compound.IsSmallMolecule).Returns(true);
      }

      [Observation]
      public void should_not_throw_an_exception()
      {
         sut.ValidateConfigurationFor(_simulation);
      }
   }

   public class When_validating_the_configuration_of_a_simulation_using_a_large_molecule_with_the_4comp_model : concern_for_SimulationConfigurationValidator
   {
      protected override void Context()
      {
         base.Context();
         _simulation.ModelConfiguration.ModelName = CoreConstants.Model.FourComp;
         A.CallTo(() => _compound.IsSmallMolecule).Returns(false);
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.ValidateConfigurationFor(_simulation)).ShouldThrowAn<InvalidSimulationConfigurationException>();
      }
   }

   public class When_validating_the_configuration_of_a_simulation_using_a_compound_with_the_same_name_as_the_simulation : concern_for_SimulationConfigurationValidator
   {
      protected override void Context()
      {
         base.Context();
         _compound.Name = "TOTO";
         _simulation.Name = "TOTO";
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.ValidateConfigurationFor(_simulation)).ShouldThrowAn<InvalidSimulationConfigurationException>();
      }
   }

   public class When_validating_the_configuration_of_a_simulation_using_an_individudal_molecule_with_the_same_name_as_the_simulation : concern_for_SimulationConfigurationValidator
   {
      protected override void Context()
      {
         base.Context();
         _individual.AddMolecule(new IndividualTransporter().WithName("TOTO"));
         _simulation.Name = "TOTO";
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.ValidateConfigurationFor(_simulation)).ShouldThrowAn<InvalidSimulationConfigurationException>();
      }
   }

   public class When_validating_the_configuration_of_a_simulation_using_a_simulation_that_is_not_surface_area_dependent_using_a_protocol_in_body_surface_area_dosing : concern_for_SimulationConfigurationValidator
   {
      protected override void Context()
      {
         base.Context();
         _speciesPopulation.IsHeightDependent = false;
         A.CallTo(() => _doseUnit.Name).Returns(CoreConstants.Units.MgPerM2);
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.ValidateConfigurationFor(_simulation)).ShouldThrowAn<InvalidSimulationConfigurationException>();
      }
   }

   public class When_validating_the_configuration_of_a_simulation_using_an_individual_or_population_based_on_the_pregnant_population: concern_for_SimulationConfigurationValidator
   {
      protected override void Context()
      {
         base.Context();
         _speciesPopulation.Name = CoreConstants.Population.Pregnant;
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.ValidateConfigurationFor(_simulation)).ShouldThrowAn<InvalidSimulationConfigurationException>();
      }
   }

   public class When_validating_the_configuration_of_a_simulation_using_a_simulation_that_is_not_surface_area_dependent_using_a_protocol_in_body_weight_dosing : concern_for_SimulationConfigurationValidator
   {
      protected override void Context()
      {
         base.Context();
         _speciesPopulation.IsHeightDependent = false;
         A.CallTo(() => _doseUnit.Name).Returns(CoreConstants.Units.MgPerKg);
      }

      [Observation]
      public void should_not_throw_an_exception()
      {
         sut.ValidateConfigurationFor(_simulation);
      }
   }


   public class When_validating_the_configuration_of_a_simulation_using_a_simulation_that_is_surface_area_dependent_using_a_protocol_in_body_surface_area_dosing : concern_for_SimulationConfigurationValidator
   {
      protected override void Context()
      {
         base.Context();
         _speciesPopulation.IsHeightDependent = true;
         A.CallTo(() => _doseUnit.Name).Returns(CoreConstants.Units.MgPerM2);
      }

      [Observation]
      public void should_not_throw_an_exception()
      {
         sut.ValidateConfigurationFor(_simulation);
      }
   }
}	
