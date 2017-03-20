using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter;
using PKSim.Infrastructure.ProjectConverter.v5_6;
using PKSim.IntegrationTests;
using OSPSuite.Core.Domain;

namespace PKSim.ProjectConverter.v5_6
{
   public class When_converting_the_SimpleSimulationIV_551_project : ContextWithLoadedProject<Converter552To561>
   {
      private Simulation _simulationPKSim;
      private Compound _C1;
      private Simulation _simulationRR;
      private Compound _C2;
      private Simulation _simulationBER;
      private Individual _individual;
      private Simulation _simulationOral;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("SimpleSimulationIV_551");
         _individual = First<Individual>();
         _simulationPKSim = FindByName<IndividualSimulation>("Simple_C1_PKSim");
         _simulationRR = FindByName<IndividualSimulation>("Simple_C1_RR");
         _simulationBER = FindByName<IndividualSimulation>("Simple_C2_BER");
         _simulationOral = FindByName<IndividualSimulation>("Simple_C1_ORAL");
         _C1 = FindByName<Compound>("C1");
         _C2 = FindByName<Compound>("C2");
      }

      [Observation]
      public void should_have_set_the_calculation_methods_of_compounds_to_the_default_of_the_pksim_database()
      {
         validateCalculationMethodInCache(_C1, CoreConstants.Category.DistributionCellular, ConverterConstants.CalculationMethod.PKSim);
         validateCalculationMethodInCache(_C2, CoreConstants.Category.DistributionCellular, ConverterConstants.CalculationMethod.PKSim);
         validateCalculationMethodInCache(_simulationBER.Compounds.First(), CoreConstants.Category.DistributionCellular, ConverterConstants.CalculationMethod.PKSim);
         validateCalculationMethodInCache(_simulationPKSim.Compounds.First(), CoreConstants.Category.DistributionCellular, ConverterConstants.CalculationMethod.PKSim);
         validateCalculationMethodInCache(_simulationRR.Compounds.First(), CoreConstants.Category.DistributionCellular, ConverterConstants.CalculationMethod.PKSim);
      }

      [Observation]
      public void should_have_updated_the_protocol_properties_to_reflect_the_new_structure()
      {
         validateReferenceToProtocol(_simulationPKSim);
         validateReferenceToProtocol(_simulationRR);
         validateReferenceToProtocol(_simulationBER);
      }

      [Observation]
      public void should_have_updated_the_protocol_mapping_of_simulations_with_formulations()
      {
         var compoundProperties = compoundPropertiesFor(_simulationOral);
         compoundProperties.ProtocolProperties.FormulationMappings.ShouldNotBeEmpty();
      }

      private void validateReferenceToProtocol(Simulation simulation)
      {
         var compoundProperties = compoundPropertiesFor(simulation);
         var protocol = simulation.AllBuildingBlocks<Protocol>().First();
         compoundProperties.ProtocolProperties.Protocol.ShouldBeEqualTo(protocol);
      }

      [Observation]
      public void should_have_converted_all_available_processes_in_the_simulation()
      {
         var compoundProperties = compoundPropertiesFor(_simulationPKSim);
         compoundProperties.Processes.MetabolizationSelection.AllEnabledProcesses().ShouldNotBeEmpty();
      }

      [Observation]
      public void should_have_created_a_reaction_building_block()
      {
         _simulationPKSim.Reactions.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_created_a_model_diagram_for_the_reaction()
      {
         _simulationPKSim.ReactionDiagramModel.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_removed_the_volume_plasma_parameter_from_the_individual()
      {
         _individual.Organism.Parameter(CoreConstants.Parameter.VOLUME_PLASMA).ShouldBeNull();
      }

      [Observation]
      public void should_have_added_the_compound_name_to_all_partial_processes()
      {
         var compound = _simulationPKSim.Compounds.First();
         _simulationPKSim.CompoundPropertiesFor(compound).Processes.MetabolizationSelection.AllPartialProcesses()
            .Each(p => p.CompoundName.ShouldBeEqualTo(compound.Name));
      }

      [Observation]
      public void should_have_set_the_calculation_methods_of_used_in_the_simulation_according_to_the_values_in_the_original_simulations()
      {
         validateSimulationDistributionCalculationMethod(_simulationPKSim, ConverterConstants.CalculationMethod.PKSim);
         validateSimulationDistributionCalculationMethod(_simulationBER, ConverterConstants.CalculationMethod.BER);
         validateSimulationDistributionCalculationMethod(_simulationRR, ConverterConstants.CalculationMethod.RR);
      }

      [Observation]
      public void should_have_removed_calculation_methods_related_to_molecule_from_model_properties()
      {
         _simulationPKSim.ModelProperties.CalculationMethodFor(CoreConstants.Category.DistributionCellular).ShouldBeNull();
         _simulationPKSim.ModelProperties.CalculationMethodFor(CoreConstants.Category.DiffusionIntCell).ShouldBeNull();
         _simulationPKSim.ModelProperties.CalculationMethodFor(CoreConstants.Category.DistributionInterstitial).ShouldBeNull();
         _simulationPKSim.ModelProperties.CalculationMethodFor(CoreConstants.Category.IntestinalPermeability).ShouldBeNull();
      }

      [Observation]
      public void should_have_added_the_calculation_methods_related_to_molecule_to_the_compound_properties()
      {
         var compoundProperties = compoundPropertiesFor(_simulationPKSim);
         compoundProperties.CalculationMethodFor(CoreConstants.Category.DistributionCellular).ShouldNotBeNull();
         compoundProperties.CalculationMethodFor(CoreConstants.Category.DiffusionIntCell).ShouldNotBeNull();
         compoundProperties.CalculationMethodFor(CoreConstants.Category.DistributionInterstitial).ShouldNotBeNull();
         compoundProperties.CalculationMethodFor(CoreConstants.Category.IntestinalPermeability).ShouldNotBeNull();
      }

      [Observation]
      public void should_have_loaded_the_calculation_methods_defined_in_the_origin_data_of_the_individual()
      {
         _individual.OriginData.CalculationMethodCache.All().ShouldNotBeEmpty();
      }

      [Observation]
      public void should_have_moved_the_fraction_unbound_parameter_under_the_fraction_unbound_parameter_group()
      {
         var compoundName = _simulationPKSim.CompoundNames.First();
         var parameter= _simulationPKSim.Model.Root.EntityAt<IParameter>(compoundName, ConverterConstants.Parameter.FractionUnboundPlasma);
         parameter.GroupName.ShouldBeEqualTo(CoreConstants.Groups.FRACTION_UNBOUND_PLASMA);
      }

      private void validateSimulationDistributionCalculationMethod(Simulation simulation, string expectedCalculationMethod)
      {
         validateSimulationCalculationMethod(simulation, CoreConstants.Category.DistributionCellular, expectedCalculationMethod);
      }

      private void validateSimulationCalculationMethod(Simulation simulation, string category, string expectedCalculationMethod)
      {
         validateCalculationMethodInCache(simulation.CompoundPropertiesFor(simulation.Compounds.First()), category, expectedCalculationMethod);
      }

      private void validateCalculationMethodInCache(IWithCalculationMethods withCalculationMethods, string category, string expectedCalculationMethod)
      {
         withCalculationMethods.CalculationMethodFor(category).Name.ShouldBeEqualTo(expectedCalculationMethod);
      }

    
      private CompoundProperties compoundPropertiesFor(Simulation simulation)
      {
         var compound = simulation.Compounds.First();
         return simulation.CompoundPropertiesFor(compound);
  
      }
   }
}