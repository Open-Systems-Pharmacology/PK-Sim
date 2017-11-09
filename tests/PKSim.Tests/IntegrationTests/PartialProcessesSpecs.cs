using System.Linq;
using System.Threading.Tasks;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using PKSim.Infrastructure.ProjectConverter;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Data;
using OSPSuite.Core.Domain.Services;
using SimulationRunOptions = PKSim.Core.Services.SimulationRunOptions;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_PartialProcesses : ContextForSimulationIntegration<Simulation>
   {
      protected ICompoundProcessRepository _compoundProcessRepository;
      protected ICloneManager _cloneManager;
      protected Individual _individual;
      protected Compound _compound;
      protected Protocol _protocol;
      protected IIndividualEnzymeFactory _enzymeFactory;
      protected IIndividualTransporterFactory _transporterFactory;
      protected double _relExpNormPls = 0.2;
      protected double _relExpNormBloodCells = 0.5;
      protected double _relExpVascEndo = 0.3;
      protected IndividualEnzyme _enzyme;
      protected double _hct = 0.6;
      protected PartialProcess _metabolizationProcess;
      protected IModelPropertiesTask _modelPropertiesTask;
      protected IModelConfigurationRepository _modelConfigurationRepository;
      protected SimulationRunOptions _simulationRunOptions;
      protected const double _relExpDuoNorm = 0.2;
      protected const double _relExpBoneNorm = 0.3;
      protected const double _relExpBone = 30;
      protected const double _relExpDuo = 40;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _compoundProcessRepository = IoC.Resolve<ICompoundProcessRepository>();
         _cloneManager = IoC.Resolve<ICloneManager>();
         _enzymeFactory = IoC.Resolve<IIndividualEnzymeFactory>();
         _transporterFactory = IoC.Resolve<IIndividualTransporterFactory>();
         _modelPropertiesTask = IoC.Resolve<IModelPropertiesTask>();
         _modelConfigurationRepository = IoC.Resolve<IModelConfigurationRepository>();
         _compound = DomainFactoryForSpecs.CreateStandardCompound();
         _individual = DomainFactoryForSpecs.CreateStandardIndividual();
         _protocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();
         _enzyme = _enzymeFactory.CreateFor(_individual).DowncastTo<IndividualEnzyme>().WithName("CYP");
         _enzyme.GetRelativeExpressionNormParameterFor(CoreConstants.Compartment.Plasma).Value = _relExpNormPls;
         _enzyme.GetRelativeExpressionNormParameterFor(CoreConstants.Compartment.BloodCells).Value = _relExpNormBloodCells;
         _enzyme.GetRelativeExpressionNormParameterFor(CoreConstants.Compartment.VascularEndothelium).Value = _relExpVascEndo;
         _individual.AddMolecule(_enzyme);
         _hct = _individual.Organism.Parameter(CoreConstants.Parameter.HCT).Value;
         _metabolizationProcess = _cloneManager.Clone(_compoundProcessRepository.ProcessByName(CoreConstantsForSpecs.Process.METABOLIZATION_SPECIFIC_FIRST_ORDER).DowncastTo<PartialProcess>());
         _metabolizationProcess.Name = "My Partial Process";
         _metabolizationProcess.Parameter(ConverterConstants.Parameter.CLspec).Value = 15;
         _compound.AddProcess(_metabolizationProcess);
         _simulationRunOptions = new SimulationRunOptions{RaiseEvents = false};
      }
   }

   public class When_creating_a_simulation_with_an_indivdual_containing_an_enzyme_localized_on_the_extracellular_membrane_apical_and_a_partial_process_in_compound : concern_for_PartialProcesses
   {
      public override void GlobalContext()
      {
         base.GlobalContext();

         _enzyme.TissueLocation = TissueLocation.ExtracellularMembrane;
         _enzyme.MembraneLocation = MembraneLocation.Apical;

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, _compound, _protocol)
            .DowncastTo<IndividualSimulation>();
         _simulation.CompoundPropertiesList.First()
            .Processes
            .MetabolizationSelection
            .AddPartialProcessSelection(new ProcessSelection {ProcessName = _metabolizationProcess.Name, MoleculeName = _enzyme.Name});

         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }

      [Observation]
      public void should_create_the_enzyme_only_in_plasma_and_interstial_compartments()
      {
         var allContainerWithEnzyme = _simulation.All<IMoleculeAmount>().Where(x => x.Name.Equals(_enzyme.Name)).Select(x => x.ParentContainer);
         allContainerWithEnzyme.Select(x => x.Name).Distinct().Contains(CoreConstants.Compartment.Intracellular).ShouldBeFalse();
         allContainerWithEnzyme.Select(x => x.Name).Distinct().Contains(CoreConstants.Compartment.BloodCells).ShouldBeFalse();
      }

      [Observation]
      public void the_formula_used_in_plasma_for_blood_organ_should_be_the_sum_of_the_value_in_plasma_and_the_value_in_blood_cells_scaled_with_the_hematocrit_ratio()
      {
         var allRelExpOutParameters = _simulation.All<IMoleculeAmount>()
            .Where(x => x.Name.Equals(_enzyme.Name))
            .Where(x => x.ParentContainer.Name.Equals(CoreConstants.Compartment.Plasma))
            .Where(x => x.ParentContainer.ParentContainer.IsBloodOrgan())
            .Select(x => x.Parameter(CoreConstants.Parameter.REL_EXP_OUT));

         foreach (var parameter in allRelExpOutParameters)
         {
            parameter.Value.ShouldBeEqualTo(_relExpNormPls + (_hct) / (1 - _hct) * _relExpNormBloodCells, 1e-6);
         }
      }

      [Observation]
      public void the_formula_used_in_plasma_for_tissue_organ_should_be_the_sum_of_the_value_in_plasma_with_the_value_in_blood_cells_scaled_with_the_hematocrit_ratio_and_the_value_in_vasc_endo_scaled_with_the_ration_of_volumina_()
      {
         var allEnzymeInTissuePlasma = _simulation.All<IMoleculeAmount>()
            .Where(x => x.Name.Equals(_enzyme.Name))
            .Where(x => x.ParentContainer.Name.Equals(CoreConstants.Compartment.Plasma))
            .Where(x => !x.ParentContainer.ParentContainer.IsBloodOrgan());


         foreach (var enzyme in allEnzymeInTissuePlasma)
         {
            var relExpOut = enzyme.Parameter(CoreConstants.Parameter.REL_EXP_OUT);
            var v_pls = enzyme.ParentContainer.Parameter(Constants.Parameters.VOLUME).Value;
            var v_vasend = enzyme.ParentContainer.ParentContainer.Parameter(ConverterConstants.Parameter.VolumeVascularEndothelium).Value;
            relExpOut.Value.ShouldBeEqualTo(_relExpNormPls + (_hct) / (1 - _hct) * _relExpNormBloodCells + v_vasend / v_pls * _relExpVascEndo, 1e-6);
         }
      }
   }

   public class When_creating_a_simulation_with_an_indivdual_containing_an_enzyme_localized_on_the_extracellular_membrane_basolateral_and_a_partial_process_in_compound : concern_for_PartialProcesses
   {
      public override void GlobalContext()
      {
         base.GlobalContext();

         _enzyme.TissueLocation = TissueLocation.ExtracellularMembrane;
         _enzyme.MembraneLocation = MembraneLocation.Basolateral;
         _enzyme.GetRelativeExpressionNormParameterFor(CoreConstants.Organ.Bone).Value = _relExpBoneNorm;
         _enzyme.GetRelativeExpressionParameterFor(CoreConstants.Organ.Bone).Value = _relExpBone;
         _enzyme.GetRelativeExpressionNormParameterFor(CoreConstants.Compartment.Duodenum).Value = _relExpDuoNorm;
         _enzyme.GetRelativeExpressionParameterFor(CoreConstants.Compartment.Duodenum).Value = _relExpDuo;

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, _compound, _protocol)
            .DowncastTo<IndividualSimulation>();
         _simulation.CompoundPropertiesList.First()
            .Processes
            .MetabolizationSelection
            .AddPartialProcessSelection(new ProcessSelection {ProcessName = _metabolizationProcess.Name, MoleculeName = _enzyme.Name});

         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }

      [Observation]
      public void should_create_the_enzyme_only_in_plasma_and_interstial_compartments()
      {
         var allContainerWithEnzyme = _simulation.All<IMoleculeAmount>().Where(x => x.Name.Equals(_enzyme.Name)).Select(x => x.ParentContainer);
         allContainerWithEnzyme.Select(x => x.Name).Distinct().Contains(CoreConstants.Compartment.Intracellular).ShouldBeFalse();
         allContainerWithEnzyme.Select(x => x.Name).Distinct().Contains(CoreConstants.Compartment.BloodCells).ShouldBeFalse();
      }

      [Observation]
      public void the_value_of_the_normalized_relative_expression_in_organ_should_have_been_set_to_the_value_defined_in_the_enzyme()
      {
         var allRelExpNorm = _simulation.All<IMoleculeAmount>()
            .Where(x => x.Name.Equals(_enzyme.Name))
            .Where(x => x.ParentContainer.Name.Equals(CoreConstants.Compartment.Interstitial))
            .Select(x => x.Parameter(CoreConstants.Parameter.REL_EXP_NORM));

         foreach (var parameter in allRelExpNorm)
         {
            var grandparent = parameter.ParentContainer.ParentContainer.ParentContainer;
            if (grandparent.Name.Equals(CoreConstants.Organ.Bone))
               parameter.Value.ShouldBeEqualTo(_relExpBoneNorm);

            if (grandparent.Name.Equals(CoreConstants.Compartment.Duodenum))
               parameter.Value.ShouldBeEqualTo(_relExpDuoNorm);
         }
      }

      [Observation]
      public void the_value_of_the_relative_expression_in_organ_should_have_been_set_to_the_value_defined_in_the_enzyme()
      {
         var allRelExp = _simulation.All<IMoleculeAmount>()
            .Where(x => x.Name.Equals(_enzyme.Name))
            .Where(x => x.ParentContainer.Name.Equals(CoreConstants.Compartment.Interstitial))
            .Select(x => x.Parameter(CoreConstants.Parameter.REL_EXP));

         foreach (var parameter in allRelExp)
         {
            var grandparent = parameter.ParentContainer.ParentContainer.ParentContainer;
            if (grandparent.Name.Equals(CoreConstants.Organ.Bone))
               parameter.Value.ShouldBeEqualTo(_relExpBone);

            if (grandparent.Name.Equals(CoreConstants.Compartment.Duodenum))
               parameter.Value.ShouldBeEqualTo(_relExpDuo);
         }
      }

      [Observation]
      public void the_formula_used_in_plasma_for_all_organs_should_be_the_sum_of_the_value_in_plasma_and_the_value_in_blood_cells_scaled_with_the_hematocrit_ratio()
      {
         var allRelExpOutParameters = _simulation.All<IMoleculeAmount>()
            .Where(x => x.Name.Equals(_enzyme.Name))
            .Where(x => x.ParentContainer.Name.Equals(CoreConstants.Compartment.Plasma))
            .Select(x => x.Parameter(CoreConstants.Parameter.REL_EXP_OUT));

         foreach (var parameter in allRelExpOutParameters)
         {
            parameter.Value.ShouldBeEqualTo(_relExpNormPls + (_hct) / (1 - _hct) * _relExpNormBloodCells, 1e-6);
         }
      }

      [Observation]
      public void the_formula_used_in_interstial_for_tissue_organ_should_be_the_sum_of_the_relexp_in_the_organ_scaled_with_the_organ_fraction_and_the_value_in_vasc_endo_scaled_with_the_ration_of_volumina_()
      {
         var allEnzymeInTissueInterstitial = _simulation.All<IMoleculeAmount>()
            .Where(x => x.Name.Equals(_enzyme.Name))
            .Where(x => x.ParentContainer.Name.Equals(CoreConstants.Compartment.Interstitial))
            .Where(x => !x.ParentContainer.ParentContainer.IsBloodOrgan());


         foreach (var enzyme in allEnzymeInTissueInterstitial)
         {
            var relExpOut = enzyme.Parameter(CoreConstants.Parameter.REL_EXP_OUT);
            var relExpNorm = enzyme.Parameter(CoreConstants.Parameter.REL_EXP_NORM).Value;
            var f_cell = enzyme.ParentContainer.ParentContainer.Parameter(CoreConstants.Parameter.FractionIntracellular).Value;
            var f_int = enzyme.ParentContainer.ParentContainer.Parameter(CoreConstants.Parameter.FractionInterstitial).Value;
            var v_int = enzyme.ParentContainer.Parameter(Constants.Parameters.VOLUME).Value;
            var v_vasend = enzyme.ParentContainer.ParentContainer.Parameter(ConverterConstants.Parameter.VolumeVascularEndothelium).Value;
            relExpOut.Value.ShouldBeEqualTo(relExpNorm * f_cell / f_int + v_vasend / v_int * _relExpVascEndo, 1e-6);
         }
      }

      [Observation]
      public void the_reference_concentration_parameter_should_be_marked_as_can_be_varied_in_the_simulation()
      {
         var refConc = _simulation.Model.Root.Container("CYP").Parameter(CoreConstants.Parameter.REFERENCE_CONCENTRATION);
         refConc.CanBeVaried.ShouldBeTrue();
         refConc.CanBeVariedInPopulation.ShouldBeTrue();
      }
   }

   public class When_creating_a_simulation_with_an_indivdual_containing_an_enzyme_localized_in_intracellular_with_location_in_vasc_endothelium_is_interstial_and_a_partial_process_in_compound : concern_for_PartialProcesses
   {
      public override void GlobalContext()
      {
         base.GlobalContext();

         _enzyme.TissueLocation = TissueLocation.Intracellular;
         _enzyme.IntracellularVascularEndoLocation = IntracellularVascularEndoLocation.Interstitial;
         _enzyme.GetRelativeExpressionNormParameterFor(CoreConstants.Organ.Bone).Value = _relExpBoneNorm;
         _enzyme.GetRelativeExpressionParameterFor(CoreConstants.Organ.Bone).Value = _relExpBone;
         _enzyme.GetRelativeExpressionNormParameterFor(CoreConstants.Compartment.Duodenum).Value = _relExpDuoNorm;
         _enzyme.GetRelativeExpressionParameterFor(CoreConstants.Compartment.Duodenum).Value = _relExpDuo;

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, _compound, _protocol)
            .DowncastTo<IndividualSimulation>();
         _simulation.CompoundPropertiesList.First()
            .Processes
            .MetabolizationSelection
            .AddPartialProcessSelection(new ProcessSelection {ProcessName = _metabolizationProcess.Name, MoleculeName = _enzyme.Name});

         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }

      [Observation]
      public void the_value_of_the_normalized_relative_expression_in_organ_should_have_been_set_to_the_value_defined_in_the_enzyme()
      {
         var allRelExpNorm = _simulation.All<IMoleculeAmount>()
            .Where(x => x.Name.Equals(_enzyme.Name))
            .Where(x => x.ParentContainer.Name.Equals(CoreConstants.Compartment.Intracellular))
            .Select(x => x.Parameter(CoreConstants.Parameter.REL_EXP_NORM));

         foreach (var parameter in allRelExpNorm)
         {
            var grandparent = parameter.ParentContainer.ParentContainer.ParentContainer;
            if (grandparent.Name.Equals(CoreConstants.Organ.Bone))
               parameter.Value.ShouldBeEqualTo(_relExpBoneNorm);

            if (grandparent.Name.Equals(CoreConstants.Compartment.Duodenum))
               parameter.Value.ShouldBeEqualTo(_relExpDuoNorm);
         }
      }

      [Observation]
      public void the_value_of_the_relative_expression_in_organ_should_have_been_set_to_the_value_defined_in_the_enzyme()
      {
         var allRelExp = _simulation.All<IMoleculeAmount>()
            .Where(x => x.Name.Equals(_enzyme.Name))
            .Where(x => x.ParentContainer.Name.Equals(CoreConstants.Compartment.Intracellular))
            .Select(x => x.Parameter(CoreConstants.Parameter.REL_EXP));

         foreach (var parameter in allRelExp)
         {
            var grandparent = parameter.ParentContainer.ParentContainer.ParentContainer;
            if (grandparent.Name.Equals(CoreConstants.Organ.Bone))
               parameter.Value.ShouldBeEqualTo(_relExpBone);

            if (grandparent.Name.Equals(CoreConstants.Compartment.Duodenum))
               parameter.Value.ShouldBeEqualTo(_relExpDuo);
         }
      }

      [Observation]
      public void the_formula_used_in_plasma_for_all_organs_should_be_the_the_value_defined_in_plasma()
      {
         var allRelExpOutParameters = _simulation.All<IMoleculeAmount>()
            .Where(x => x.Name.Equals(_enzyme.Name))
            .Where(x => x.ParentContainer.Name.Equals(CoreConstants.Compartment.Plasma))
            .Select(x => x.Parameter(CoreConstants.Parameter.REL_EXP_OUT));

         foreach (var parameter in allRelExpOutParameters)
         {
            parameter.Value.ShouldBeEqualTo(_relExpNormPls, 1e-6);
         }
      }

      [Observation]
      public void the_formula_used_in_blood_cells_for_all_organs_should_be_the_the_value_defined_in_blood_cells()
      {
         var allRelExpOutParameters = _simulation.All<IMoleculeAmount>()
            .Where(x => x.Name.Equals(_enzyme.Name))
            .Where(x => x.ParentContainer.Name.Equals(CoreConstants.Compartment.BloodCells))
            .Select(x => x.Parameter(CoreConstants.Parameter.REL_EXP_OUT));

         foreach (var parameter in allRelExpOutParameters)
         {
            parameter.Value.ShouldBeEqualTo(_relExpNormBloodCells, 1e-6);
         }
      }

      [Observation]
      public void the_formula_used_in_interstitial_for_all_organs_should_be_the_the_value_defined_in_the_vascular_endothelial_scaled_with_the_volume_ratio()
      {
         var allEnzymeInTissueInterstitial = _simulation.All<IMoleculeAmount>()
            .Where(x => x.Name.Equals(_enzyme.Name))
            .Where(x => x.ParentContainer.Name.Equals(CoreConstants.Compartment.Interstitial))
            .Where(x => !x.ParentContainer.ParentContainer.IsBloodOrgan());


         foreach (var enzyme in allEnzymeInTissueInterstitial)
         {
            var relExpOut = enzyme.Parameter(CoreConstants.Parameter.REL_EXP_OUT);
            var v_int = enzyme.ParentContainer.Parameter(Constants.Parameters.VOLUME).Value;
            var v_vasend = enzyme.ParentContainer.ParentContainer.Parameter(ConverterConstants.Parameter.VolumeVascularEndothelium).Value;
            relExpOut.Value.ShouldBeEqualTo(v_vasend / v_int * _relExpVascEndo, 1e-6);
         }
      }

      [Observation]
      public void the_formula_used_in_intracellular_for_all_organs_should_be_the_the_value_defined_in_organs()
      {
         var allRelExp = _simulation.All<IMoleculeAmount>()
            .Where(x => x.Name.Equals(_enzyme.Name))
            .Where(x => x.ParentContainer.Name.Equals(CoreConstants.Compartment.Intracellular))
            .Select(x => x.Parameter(CoreConstants.Parameter.REL_EXP_OUT));

         foreach (var parameter in allRelExp)
         {
            var grandparent = parameter.ParentContainer.ParentContainer.ParentContainer;
            if (grandparent.Name.Equals(CoreConstants.Organ.Bone))
               parameter.Value.ShouldBeEqualTo(_relExpBoneNorm);

            if (grandparent.Name.Equals(CoreConstants.Compartment.Duodenum))
               parameter.Value.ShouldBeEqualTo(_relExpDuoNorm);
         }
      }
   }

   public class When_creating_a_simulation_with_an_indivdual_containing_an_enzyme_localized_in_interstitial_and_a_partial_process_in_compound : concern_for_PartialProcesses
   {
      public override void GlobalContext()
      {
         base.GlobalContext();

         _enzyme.TissueLocation = TissueLocation.Interstitial;
         _enzyme.GetRelativeExpressionNormParameterFor(CoreConstants.Organ.Bone).Value = _relExpBoneNorm;
         _enzyme.GetRelativeExpressionParameterFor(CoreConstants.Organ.Bone).Value = _relExpBone;
         _enzyme.GetRelativeExpressionNormParameterFor(CoreConstants.Compartment.Duodenum).Value = _relExpDuoNorm;
         _enzyme.GetRelativeExpressionParameterFor(CoreConstants.Compartment.Duodenum).Value = _relExpDuo;

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, _compound, _protocol)
            .DowncastTo<IndividualSimulation>();

         _simulation.CompoundPropertiesList.First()
            .Processes
            .MetabolizationSelection
            .AddPartialProcessSelection(new ProcessSelection {ProcessName = _metabolizationProcess.Name, MoleculeName = _enzyme.Name});

         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }

      [Observation]
      public void should_not_create_the_enzyme_in_intracellular_compartments()
      {
         var allContainerWithEnzyme = _simulation.All<IMoleculeAmount>().Where(x => x.Name.Equals(_enzyme.Name)).Select(x => x.ParentContainer);
         allContainerWithEnzyme.Select(x => x.Name).Distinct().Contains(CoreConstants.Compartment.Intracellular).ShouldBeFalse();
      }

      [Observation]
      public void the_value_of_the_normalized_relative_expression_in_organ_should_have_been_set_to_the_value_defined_in_the_enzyme()
      {
         var allRelExpNorm = _simulation.All<IMoleculeAmount>()
            .Where(x => x.Name.Equals(_enzyme.Name))
            .Where(x => x.ParentContainer.Name.Equals(CoreConstants.Compartment.Intracellular))
            .Select(x => x.Parameter(CoreConstants.Parameter.REL_EXP_NORM));

         foreach (var parameter in allRelExpNorm)
         {
            var grandparent = parameter.ParentContainer.ParentContainer.ParentContainer;
            if (grandparent.Name.Equals(CoreConstants.Organ.Bone))
               parameter.Value.ShouldBeEqualTo(_relExpBoneNorm);

            if (grandparent.Name.Equals(CoreConstants.Compartment.Duodenum))
               parameter.Value.ShouldBeEqualTo(_relExpDuoNorm);
         }
      }

      [Observation]
      public void the_value_of_the_relative_expression_in_organ_should_have_been_set_to_the_value_defined_in_the_enzyme()
      {
         var allRelExp = _simulation.All<IMoleculeAmount>()
            .Where(x => x.Name.Equals(_enzyme.Name))
            .Where(x => x.ParentContainer.Name.Equals(CoreConstants.Compartment.Intracellular))
            .Select(x => x.Parameter(CoreConstants.Parameter.REL_EXP));

         foreach (var parameter in allRelExp)
         {
            var grandparent = parameter.ParentContainer.ParentContainer.ParentContainer;
            if (grandparent.Name.Equals(CoreConstants.Organ.Bone))
               parameter.Value.ShouldBeEqualTo(_relExpBone);

            if (grandparent.Name.Equals(CoreConstants.Compartment.Duodenum))
               parameter.Value.ShouldBeEqualTo(_relExpDuo);
         }
      }

      [Observation]
      public void the_formula_used_in_plasma_for_all_organs_should_be_the_the_value_defined_in_plasma()
      {
         var allRelExpOutParameters = _simulation.All<IMoleculeAmount>()
            .Where(x => x.Name.Equals(_enzyme.Name))
            .Where(x => x.ParentContainer.Name.Equals(CoreConstants.Compartment.Plasma))
            .Select(x => x.Parameter(CoreConstants.Parameter.REL_EXP_OUT));

         foreach (var parameter in allRelExpOutParameters)
         {
            parameter.Value.ShouldBeEqualTo(_relExpNormPls, 1e-6);
         }
      }

      [Observation]
      public void the_formula_used_in_blood_cells_for_all_organs_should_be_the_the_value_defined_in_blood_cells()
      {
         var allRelExpOutParameters = _simulation.All<IMoleculeAmount>()
            .Where(x => x.Name.Equals(_enzyme.Name))
            .Where(x => x.ParentContainer.Name.Equals(CoreConstants.Compartment.BloodCells))
            .Select(x => x.Parameter(CoreConstants.Parameter.REL_EXP_OUT));

         foreach (var parameter in allRelExpOutParameters)
         {
            parameter.Value.ShouldBeEqualTo(_relExpNormBloodCells, 1e-6);
         }
      }

      [Observation]
      public void the_formula_used_in_interstitial_for_all_organs_should_be_the_sum_of_the_value_defined_in_the_vascular_endothelial_scaled_with_the_volume_ratio_and_the_value_defined_in_the_organ_scaled_with_the_fraction_ratio()
      {
         var allEnzymeInTissueInterstitial = _simulation.All<IMoleculeAmount>()
            .Where(x => x.Name.Equals(_enzyme.Name))
            .Where(x => x.ParentContainer.Name.Equals(CoreConstants.Compartment.Interstitial))
            .Where(x => !x.ParentContainer.ParentContainer.IsBloodOrgan());


         foreach (var enzyme in allEnzymeInTissueInterstitial)
         {
            var relExpOut = enzyme.Parameter(CoreConstants.Parameter.REL_EXP_OUT);
            var relExpNorm = enzyme.Parameter(CoreConstants.Parameter.REL_EXP_NORM).Value;
            var v_int = enzyme.ParentContainer.Parameter(Constants.Parameters.VOLUME).Value;
            var v_vasend = enzyme.ParentContainer.ParentContainer.Parameter(ConverterConstants.Parameter.VolumeVascularEndothelium).Value;
            var f_cell = enzyme.ParentContainer.ParentContainer.Parameter(CoreConstants.Parameter.FractionIntracellular).Value;
            var f_int = enzyme.ParentContainer.ParentContainer.Parameter(CoreConstants.Parameter.FractionInterstitial).Value;

            relExpOut.Value.ShouldBeEqualTo(relExpNorm * f_cell / f_int + v_vasend / v_int * _relExpVascEndo, 1e-6);
         }
      }
   }

   public class When_creating_a_simulation_with_an_indivdual_containing_an_enzyme_with_intracellular_location_endosomal_with_the_two_pore_model : concern_for_PartialProcesses
   {
      public override void GlobalContext()
      {
         base.GlobalContext();

         _enzyme.TissueLocation = TissueLocation.Intracellular;
         _enzyme.IntracellularVascularEndoLocation = IntracellularVascularEndoLocation.Endosomal;

         var modelConfig = _modelConfigurationRepository.AllFor(_individual.Species).First(x => x.ModelName == CoreConstants.Model.TwoPores);
         var twoPoreModelProperties = _modelPropertiesTask.DefaultFor(modelConfig, _individual.OriginData);

         _compound.Parameter(CoreConstants.Parameter.IS_SMALL_MOLECULE).Value = 0;
         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, _compound, _protocol, twoPoreModelProperties)
            .DowncastTo<IndividualSimulation>();

         _simulation.CompoundPropertiesList.First()
            .Processes
            .MetabolizationSelection
            .AddPartialProcessSelection(new ProcessSelection {ProcessName = _metabolizationProcess.Name, MoleculeName = _enzyme.Name});

         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }

      [Observation]
      public void should_have_created_the_enzyme_in_all_endosome_compartment()
      {
         var allContainerWithEnzyme = _simulation.All<IMoleculeAmount>().Where(x => x.Name.Equals(_enzyme.Name)).Select(x => x.ParentContainer);
         allContainerWithEnzyme.Select(x => x.Name).Distinct().Contains(CoreConstants.Compartment.Endosome).ShouldBeTrue();
      }
   }

   public class When_creating_an_enzyme_with_localization_in_vascular_endo_set_to_interstitial_and_a_two_pore_model : concern_for_PartialProcesses
   {
      public override void GlobalContext()
      {
         base.GlobalContext();

         _enzyme.TissueLocation = TissueLocation.Intracellular;
         _enzyme.IntracellularVascularEndoLocation = IntracellularVascularEndoLocation.Interstitial;

         var modelConfig = _modelConfigurationRepository.AllFor(_individual.Species).First(x => x.ModelName == CoreConstants.Model.TwoPores);
         var twoPoreModelProperties = _modelPropertiesTask.DefaultFor(modelConfig, _individual.OriginData);
         _compound.Parameter(CoreConstants.Parameter.IS_SMALL_MOLECULE).Value = 0;

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, _compound, _protocol, twoPoreModelProperties)
            .DowncastTo<IndividualSimulation>();

         _simulation.CompoundPropertiesList.First()
            .Processes
            .MetabolizationSelection
            .AddPartialProcessSelection(new ProcessSelection {ProcessName = _metabolizationProcess.Name, MoleculeName = _enzyme.Name});

         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }

      [Observation]
      public void should_have_created_the_enzyme_in_all_interstitial_compartment()
      {
         var allContainerWithEnzyme = _simulation.All<IMoleculeAmount>().Where(x => x.Name.Equals(_enzyme.Name)).Select(x => x.ParentContainer);
         allContainerWithEnzyme.Select(x => x.Name).Distinct().Contains(CoreConstants.Compartment.Endosome).ShouldBeFalse();
      }
   }

   public class When_creating_a_transporter_for_brain_BBB_influx : concern_for_PartialProcesses
   {
      private IndividualTransporter _transporter;
      private ITransporterContainerTemplateRepository _transporterContainerTemplateRepository;
      private PartialProcess _transportProcess;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _transporterContainerTemplateRepository = IoC.Resolve<ITransporterContainerTemplateRepository>();
         var allTransporters = _transporterContainerTemplateRepository.TransportersFor(_individual.Species.Name, CoreConstants.Organ.Brain);

         var influxBBB = allTransporters.Where(x => x.MembraneLocation == MembraneLocation.BloodBrainBarrier)
            .FirstOrDefault(x => x.TransportType == TransportType.Influx);

         _transporter = _transporterFactory.CreateFor(_individual).DowncastTo<IndividualTransporter>().WithName("TRANS");
         var transportContainer = _transporter.ExpressionContainer(CoreConstants.Organ.Brain).DowncastTo<TransporterExpressionContainer>();
         transportContainer.UpdatePropertiesFrom(influxBBB);
         _individual.AddMolecule(_transporter);
         _transportProcess = _cloneManager.Clone(_compoundProcessRepository.ProcessByName(CoreConstantsForSpecs.Process.ACTIVE_TRANSPORT_SPECIFIC_MM)
            .DowncastTo<PartialProcess>());
         _transportProcess.Name = "My Transport Process";
         _compound.AddProcess(_transportProcess);

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, _compound, _protocol)
            .DowncastTo<IndividualSimulation>();

         _simulation.CompoundPropertiesList.First()
            .Processes
            .TransportAndExcretionSelection
            .AddPartialProcessSelection(new ProcessSelection {CompoundName = _compound.Name, ProcessName = _transportProcess.Name, MoleculeName = _transporter.Name});

         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }

      [Observation]
      public void should_have_created_a_simulation_and_a_transporter_in_brain_plasma()
      {
         var allContainerWithTransporter = _simulation.All<IMoleculeAmount>().Where(x => x.Name.Equals(_transporter.Name)).Select(x => x.ParentContainer);
         allContainerWithTransporter.Select(x => x.Name).Distinct().ShouldContain(CoreConstants.Compartment.Plasma);
      }
   }

   public class When_simulating_a_simulation_with_a_metabolization_process_in_liver : concern_for_PartialProcesses
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         _enzyme.GetRelativeExpressionNormParameterFor(CoreConstants.Compartment.Plasma).Value = 0;
         _enzyme.GetRelativeExpressionNormParameterFor(CoreConstants.Compartment.BloodCells).Value = 0;
         _enzyme.GetRelativeExpressionNormParameterFor(CoreConstants.Compartment.VascularEndothelium).Value = 0;
         _enzyme.GetRelativeExpressionNormParameterFor(CoreConstants.Compartment.Periportal).Value = 1;

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, _compound, _protocol)
            .DowncastTo<IndividualSimulation>();

         _simulation.CompoundPropertiesList.First()
            .Processes
            .MetabolizationSelection
            .AddPartialProcessSelection(new EnzymaticProcessSelection {CompoundName = _compound.Name, ProcessName = _metabolizationProcess.Name, MoleculeName = _enzyme.Name});

         DomainFactoryForSpecs.AddModelToSimulation(_simulation);

         var objectPatFactory = new ObjectPathFactoryForSpecs();
         var path = objectPatFactory.CreateObjectPathFrom(Constants.ORGANISM,
            CoreConstants.Organ.Liver,
            CoreConstants.Compartment.Pericentral,
            CoreConstants.Compartment.Intracellular,
            CoreConstants.Molecule.ProcessProductName(_compound.Name, _enzyme.Name, CoreConstants.Molecule.Metabolite),
            CoreConstants.Observer.ObserverNameFrom(CoreConstants.Observer.FRACTION_OF_DOSE, _compound.Name));

         var quantitySelection = new QuantitySelection(path.ToString(), QuantityType.Metabolite);
         _simulation.OutputSelections.AddOutput(quantitySelection);
      }

      [Observation]
      public async Task should_be_able_to_retrieve_the_fraction_metabolized_in_liver_intracellular()
      {
         var simulationEngine = IoC.Resolve<ISimulationEngine<IndividualSimulation>>();
         await simulationEngine.RunAsync(_simulation, _simulationRunOptions);

         _simulation.HasResults.ShouldBeTrue();

         var observerColumn = _simulation.DataRepository.Where(col => col.DataInfo.Origin == ColumnOrigins.Calculation)
            .Where(col => col.QuantityInfo.Type.Is(QuantityType.Metabolite))
            .Where(col => col.QuantityInfo.Path.Contains(CoreConstants.Organ.Liver))
            .FirstOrDefault(col => col.QuantityInfo.Path.Contains(CoreConstants.Compartment.Intracellular));

         observerColumn.ShouldNotBeNull();
      }
   }

   public class When_creating_a_simulation_with_two_compounds_using_a_partial_process_with_the_same_name : concern_for_PartialProcesses
   {
      private Compound _otherCompound;
      private Protocol _otherProtocol;

      public override void GlobalContext()
      {
         base.GlobalContext();


         _otherCompound = DomainFactoryForSpecs.CreateStandardCompound().WithName("OtherCompound");
         _otherProtocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol().WithName("OtherProtocol");
         _otherCompound.AddProcess(_cloneManager.Clone(_metabolizationProcess));

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, new[] {_compound, _otherCompound}, new[] {_protocol, _otherProtocol,})
            .DowncastTo<IndividualSimulation>();

         _simulation.CompoundPropertiesFor(_compound.Name)
            .Processes
            .MetabolizationSelection
            .AddPartialProcessSelection(new ProcessSelection {CompoundName = _compound.Name, ProcessName = _metabolizationProcess.Name, MoleculeName = _enzyme.Name});

         _simulation.CompoundPropertiesFor(_otherCompound.Name)
            .Processes
            .MetabolizationSelection
            .AddPartialProcessSelection(new ProcessSelection {CompoundName = _otherCompound.Name, ProcessName = _metabolizationProcess.Name, MoleculeName = _enzyme.Name});

         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }

      [Observation]
      public async Task should_be_able_to_create_and_run_the_simulation()
      {
         var simulationEngine = IoC.Resolve<ISimulationEngine<IndividualSimulation>>();
         await simulationEngine.RunAsync(_simulation, _simulationRunOptions);
         _simulation.HasResults.ShouldBeTrue();
      }
   }

   public class When_creating_a_simulation_using_an_irreversible_inhibition : concern_for_PartialProcesses
   {
      private InhibitionProcess _irreversibleProcess;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _irreversibleProcess = _cloneManager.Clone(_compoundProcessRepository.ProcessByName(CoreConstantsForSpecs.Process.IRREVERSIBLE_INHIBITION).DowncastTo<InhibitionProcess>());
         _irreversibleProcess.Name = "IrreversibleProcess";
         _irreversibleProcess.Parameter(CoreConstantsForSpecs.Parameter.KINACT).Value = 10;
         _compound.AddProcess(_irreversibleProcess);

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, new[] {_compound}, new[] {_protocol})
            .DowncastTo<IndividualSimulation>();

         _simulation.CompoundPropertiesFor(_compound.Name)
            .Processes
            .MetabolizationSelection
            .AddPartialProcessSelection(new ProcessSelection {CompoundName = _compound.Name, ProcessName = _metabolizationProcess.Name, MoleculeName = _enzyme.Name});


         _simulation.InteractionProperties.AddInteraction(new InteractionSelection {CompoundName = _compound.Name, MoleculeName = _enzyme.Name, ProcessName = _irreversibleProcess.Name});
         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }
       
      [Observation]
      public async Task should_be_able_to_create_and_run_the_simulation()
      {
         var simulationEngine = IoC.Resolve<ISimulationEngine<IndividualSimulation>>();
         await simulationEngine.RunAsync(_simulation, _simulationRunOptions);
         _simulation.HasResults.ShouldBeTrue();
      }
   }

   public class When_creating_a_simulation_using_an_induction_interaction : concern_for_PartialProcesses
   {
      private InductionProcess _induction;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _induction = _cloneManager.Clone(_compoundProcessRepository.ProcessByName(CoreConstantsForSpecs.Process.INDUCTION).DowncastTo<InductionProcess>());
         _induction.Name = "Induction";
         _induction.Parameter(CoreConstantsForSpecs.Parameter.EC50).Value = 10;
         _compound.AddProcess(_induction);

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, new[] { _compound }, new[] { _protocol })
            .DowncastTo<IndividualSimulation>();

         _simulation.CompoundPropertiesFor(_compound.Name)
            .Processes
            .MetabolizationSelection
            .AddPartialProcessSelection(new ProcessSelection { CompoundName = _compound.Name, ProcessName = _metabolizationProcess.Name, MoleculeName = _enzyme.Name });


         _simulation.InteractionProperties.AddInteraction(new InteractionSelection { CompoundName = _compound.Name, MoleculeName = _enzyme.Name, ProcessName = _induction.Name });
         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }

      [Observation]
      public async Task should_be_able_to_create_and_run_the_simulation()
      {
         var simulationEngine = IoC.Resolve<ISimulationEngine<IndividualSimulation>>();
         await simulationEngine.RunAsync(_simulation, _simulationRunOptions);
         _simulation.HasResults.ShouldBeTrue();
      }
   }

   public class When_creating_a_simulation_using_an_induction_and_an_irreversible_interaction : concern_for_PartialProcesses
   {
      private InductionProcess _induction;
      private InhibitionProcess _irreversibleInhibiton;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _induction = _cloneManager.Clone(_compoundProcessRepository.ProcessByName(CoreConstantsForSpecs.Process.INDUCTION).DowncastTo<InductionProcess>());
         _induction.Name = "Induction";
         _induction.Parameter(CoreConstantsForSpecs.Parameter.EC50).Value = 10;
         _compound.AddProcess(_induction);
  
         _irreversibleInhibiton = _cloneManager.Clone(_compoundProcessRepository.ProcessByName(CoreConstantsForSpecs.Process.IRREVERSIBLE_INHIBITION).DowncastTo<InhibitionProcess>());
         _irreversibleInhibiton.Name = "IrreversibleProcess";
         _irreversibleInhibiton.Parameter(CoreConstantsForSpecs.Parameter.KINACT).Value = 10;
         _compound.AddProcess(_irreversibleInhibiton);
 
         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, new[] { _compound }, new[] { _protocol })
            .DowncastTo<IndividualSimulation>();

         _simulation.CompoundPropertiesFor(_compound.Name)
            .Processes
            .MetabolizationSelection
            .AddPartialProcessSelection(new ProcessSelection { CompoundName = _compound.Name, ProcessName = _metabolizationProcess.Name, MoleculeName = _enzyme.Name });


         _simulation.InteractionProperties.AddInteraction(new InteractionSelection { CompoundName = _compound.Name, MoleculeName = _enzyme.Name, ProcessName = _induction.Name });
         _simulation.InteractionProperties.AddInteraction(new InteractionSelection { CompoundName = _compound.Name, MoleculeName = _enzyme.Name, ProcessName = _irreversibleInhibiton.Name });
         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }

      [Observation]
      public void should_only_create_one_turnover_reaction()
      { 
         _simulation.Reactions.Count().ShouldBeEqualTo(4); //Induction, turnover, irreversible and metabolization
      }
   }
}