using System.Collections.Generic;
using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
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
      protected SimpleProtocol _protocol;
      protected IProtocolToSchemaItemsMapper _protocolToProtocolSchemaItemMapper;
      protected SpeciesPopulation _speciesPopulation;
      protected SchemaItem _schemaItem;
      protected Unit _doseUnit;
      protected Formulation _formulation;
      protected ProtocolProperties _protocolProperties;
      protected FormulationMapping _formulationMapping;

      protected override void Context()
      {
         _simulation = new IndividualSimulation
         {
            Properties = new SimulationProperties(),
            SimulationSettings = new SimulationSettings(),
            ModelConfiguration = new ModelConfiguration()
         };
         _individual = new Individual().WithName("MyIndividual");
         _speciesPopulation = new SpeciesPopulation();

         _individual.OriginData = new OriginData {SpeciesPopulation = _speciesPopulation};
         _compound = A.Fake<Compound>().WithName("MyCompound");
         _protocol = A.Fake<SimpleProtocol>().WithName("MyProtocol");
         _formulation = A.Fake<Formulation>().WithName("Formulation");

         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("Individual", PKSimBuildingBlockType.Individual) { BuildingBlock = _individual });
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("Compound", PKSimBuildingBlockType.Compound) { BuildingBlock = _compound });
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("Protocol", PKSimBuildingBlockType.Protocol) { BuildingBlock = _protocol });
         _simulation.AddUsedBuildingBlock(new UsedBuildingBlock("Formulation", PKSimBuildingBlockType.Formulation) { BuildingBlock = _formulation });

         _protocolToProtocolSchemaItemMapper = A.Fake<IProtocolToSchemaItemsMapper>();
         sut = new SimulationConfigurationValidator(_protocolToProtocolSchemaItemMapper);

         _speciesPopulation.IsHeightDependent = false;
         _schemaItem= A.Fake<SchemaItem>();
         _doseUnit= A.Fake<Unit>();
         _schemaItem.Dose.DisplayUnit = _doseUnit;

         _protocolProperties = new ProtocolProperties();
         _formulationMapping = new FormulationMapping {FormulationKey = "F1", Formulation = _formulation};
         _simulation.Properties.AddCompoundProperties(new CompoundProperties
         {
            Compound = _compound,
            ProtocolProperties = _protocolProperties
         });
         A.CallTo(() => _protocolToProtocolSchemaItemMapper.MapFrom(_protocol)).Returns(new []{_schemaItem});
      }
   }

   public class When_validating_the_configuration_of_a_valid_simulation_using_the_two_pore_model : concern_for_SimulationConfigurationValidator
   {
      protected override void Context()
      {
         base.Context();
         _simulation.ModelConfiguration.ModelName = CoreConstants.Model.TwoPores;
         _compound.IsSmallMolecule = false;
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
         _compound.IsSmallMolecule = true;
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
         _compound.IsSmallMolecule = true;
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
         _compound.IsSmallMolecule = false;
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.ValidateConfigurationFor(_simulation)).ShouldThrowAn<InvalidSimulationConfigurationException>();
      }
   }

   public class When_validating_the_configuration_of_a_simulation_using_a_table_formula_without_any_point : concern_for_SimulationConfigurationValidator
   {
      protected override void Context()
      {
         base.Context();
         _formulation.FormulationType = CoreConstants.Formulation.TABLE;
         var tableParameter = new PKSimParameter().WithName(CoreConstants.Parameters.FRACTION_DOSE).WithFormula(new TableFormula());
         _formulation.Add(tableParameter);
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
         _speciesPopulation.Name = CoreConstants.Population.PREGNANT;
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

   public class When_validating_the_configuration_of_a_simulation_using_a_compound_with_an_oral_application_super_saturation_and_not_particle_dissolution : concern_for_SimulationConfigurationValidator
   {
      protected override void Context()
      {
         base.Context();
         _compound.SupersaturationEnabled = true;
         _schemaItem.ApplicationType = ApplicationTypes.Oral;
         _schemaItem.FormulationKey = _formulationMapping.FormulationKey;
         _formulation.FormulationType = CoreConstants.Formulation.FIRST_ORDER;
         _protocolProperties.Protocol = _protocol;
         _protocolProperties.AddFormulationMapping(_formulationMapping);

      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.ValidateConfigurationFor(_simulation)).ShouldThrowAn<InvalidSimulationConfigurationException>();
      }
   }

   public class When_validating_the_configuration_of_a_simulation_using_a_compound_with_a_user_defined_in_lumen_application_and_super_saturation : concern_for_SimulationConfigurationValidator
   {
      protected override void Context()
      {
         base.Context();
         _compound.SupersaturationEnabled = true;
         _schemaItem.ApplicationType = ApplicationTypes.UserDefined;
         _schemaItem.FormulationKey = _formulationMapping.FormulationKey;
         _schemaItem.TargetOrgan = CoreConstants.Organ.Lumen;
         _formulation.FormulationType = CoreConstants.Formulation.LINT80;
         _protocolProperties.Protocol = _protocol;
         _protocolProperties.AddFormulationMapping(_formulationMapping);
      }

      [Observation]
      public void should_throw_an_exception()
      {
         The.Action(() => sut.ValidateConfigurationFor(_simulation)).ShouldThrowAn<InvalidSimulationConfigurationException>();
      }
   }

   public class When_validating_the_configuration_of_a_simulation_using_a_compound_with_a_user_defined_in_lumen_application_without_super_saturation : concern_for_SimulationConfigurationValidator
   {
      protected override void Context()
      {
         base.Context();
         _compound.SupersaturationEnabled = false;
         _schemaItem.ApplicationType = ApplicationTypes.UserDefined;
         _schemaItem.FormulationKey = _formulationMapping.FormulationKey;
         _schemaItem.TargetOrgan = CoreConstants.Organ.Lumen;
         _formulation.FormulationType = CoreConstants.Formulation.LINT80;
         _protocolProperties.Protocol = _protocol;
         _protocolProperties.AddFormulationMapping(_formulationMapping);
      }

      [Observation]
      public void should_not_throw_an_exception()
      {
         sut.ValidateConfigurationFor(_simulation);
      }
   }

   public class When_validating_the_configuration_of_a_simulation_using_a_compound_with_a_user_defined_not_in_lumen_application_with_super_saturation : concern_for_SimulationConfigurationValidator
   {
      protected override void Context()
      {
         base.Context();
         _compound.SupersaturationEnabled = true;
         _schemaItem.ApplicationType = ApplicationTypes.UserDefined;
         _schemaItem.FormulationKey = _formulationMapping.FormulationKey;
         _schemaItem.TargetOrgan = CoreConstants.Organ.Bone;
         _formulation.FormulationType = CoreConstants.Formulation.LINT80;
         _protocolProperties.Protocol = _protocol;
         _protocolProperties.AddFormulationMapping(_formulationMapping);
      }

      [Observation]
      public void should_not_throw_an_exception()
      {
         sut.ValidateConfigurationFor(_simulation);
      }
   }

   public class When_validating_the_configuration_of_a_simulation_using_a_compound_with_an_oral_application_super_saturation_and_particle_dissolution : concern_for_SimulationConfigurationValidator
   {
      protected override void Context()
      {
         base.Context();
         _compound.SupersaturationEnabled = true;
         _schemaItem.ApplicationType = ApplicationTypes.Oral;
         _schemaItem.FormulationKey = _formulationMapping.FormulationKey;
         _protocolProperties.Protocol = _protocol;
         _protocolProperties.AddFormulationMapping(_formulationMapping);
         _formulation.FormulationType = CoreConstants.Formulation.PARTICLES;
      }


      [Observation]
      public void should_not_throw_an_exception()
      {
         sut.ValidateConfigurationFor(_simulation);
      }
   }

   public class When_validating_the_configuration_of_a_simulation_using_a_compound_with_an_oral_application_and_not_user_super_saturation_and_not_particle_dissolution : concern_for_SimulationConfigurationValidator
   {
      protected override void Context()
      {
         base.Context();
         _compound.SupersaturationEnabled = false;
         _schemaItem.ApplicationType = ApplicationTypes.Oral;
         _schemaItem.FormulationKey = _formulationMapping.FormulationKey;
         _protocolProperties.Protocol = _protocol;
         _protocolProperties.AddFormulationMapping(_formulationMapping);
         _formulation.FormulationType = CoreConstants.Formulation.FIRST_ORDER;
      }

      [Observation]
      public void should_not_throw_an_exception()
      {
         sut.ValidateConfigurationFor(_simulation);
      }
   }
}	
