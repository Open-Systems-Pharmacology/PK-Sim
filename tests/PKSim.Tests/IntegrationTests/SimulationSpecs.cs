using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Model.Extensions;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using PKSim.Infrastructure.ProjectConverter;
using IContainer = OSPSuite.Core.Domain.IContainer;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;
using System;
using SimulationRunOptions = PKSim.Core.Services.SimulationRunOptions;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_Simulation<T> : ContextForSimulationIntegration<Simulation, T> where T : Simulation
   {
      protected Compound _compound;
      protected Individual _individual;
      protected Protocol _protocol;
      protected IGlobalPKAnalysisTask _globalPKAnalysisTask;
      protected SimulationRunOptions _simulationRunOptions;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _compound = DomainFactoryForSpecs.CreateStandardCompound();
         _individual = DomainFactoryForSpecs.CreateStandardIndividual();
         _protocol = DomainFactoryForSpecs.CreateStandardIVBolusProtocol();
         _globalPKAnalysisTask = IoC.Resolve<IGlobalPKAnalysisTask>();
         _simulationRunOptions = new SimulationRunOptions();
      }
   }

   public abstract class concern_for_IndividualSimulation : concern_for_Simulation<IndividualSimulation>
   {
   }

   public abstract class concern_for_PopulationSimulation : concern_for_Simulation<PopulationSimulation>
   {
   }

   public class when_retrieving_body_weight_from_a_simulation : concern_for_IndividualSimulation
   {
      private double _bodyWeight;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _simulation = DomainFactoryForSpecs.CreateSimulationWith(_individual, _compound, _protocol) as IndividualSimulation;
         _bodyWeight = _simulation.Model.Root.EntityAt<IParameter>(Constants.ORGANISM, CoreConstants.Parameter.WEIGHT).Value;
         _simulation.Individual.WeightParameter.Value = 0.0;
      }

      [Observation]
      public void should_retrieve_body_weight_from_model_not_individual()
      {
         _simulation.BodyWeight.Value.ShouldBeEqualTo(_bodyWeight);
      }
   }

   public class When_creating_an_individual_simulation_with_the_standard_building_block : concern_for_IndividualSimulation
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         _simulation = DomainFactoryForSpecs.CreateSimulationWith(_individual, _compound, _protocol) as IndividualSimulation;
      }

      [Observation]
      public void should_be_able_to_create_the_simulation()
      {
         _simulation.ShouldNotBeNull();
      }

      [Observation]
      public async Task should_be_able_to_simulate_the_simulation()
      {
         var simulationEngine = IoC.Resolve<ISimulationEngine<IndividualSimulation>>();
         await simulationEngine.RunAsync(_simulation, _simulationRunOptions);
         _simulation.HasResults.ShouldBeTrue();
      }

      [Observation]
      public void all_parmaeters_defined_in_a_template_building_blocks_should_have_an_origin_parameter_id_set()
      {
         var allParameters = _simulation.ParametersOfType(PKSimBuildingBlockType.Template)
            .Where(x => string.IsNullOrEmpty(x.Origin.ParameterId));
         allParameters.Count().ShouldBeEqualTo(0);
      }

      [Observation]
      public void should_have_a_bsa_parameter_set()
      {
         _simulation.Model.Root.EntityAt<IParameter>(Constants.ORGANISM, CoreConstants.Parameter.BSA).ShouldNotBeNull();
      }
   }

   public class When_creating_an_individual_simulation_with_the_standard_building_block_and_iv_bolus : concern_for_IndividualSimulation
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         _protocol = DomainFactoryForSpecs.CreateStandardIVProtocol();
         _simulation = DomainFactoryForSpecs.CreateSimulationWith(_individual, _compound, _protocol) as IndividualSimulation;
      }

      [Observation]
      public async Task should_be_able_to_simulate_the_simulation()
      {
         var simulationEngine = IoC.Resolve<ISimulationEngine<IndividualSimulation>>();
         var simSettingsRetriever = IoC.Resolve<ISimulationSettingsRetriever>();
         simSettingsRetriever.CreatePKSimDefaults(_simulation);
         await simulationEngine.RunAsync(_simulation, _simulationRunOptions);
         _simulation.HasResults.ShouldBeTrue();
      }

      [Observation]
      public void should_have_set_all_the_container_having_the_name_of_the_compound_to_the_type_molecule()
      {
         var compound = _simulation.BuildingBlock<Compound>().Name;
         var pathResolver = new EntityPathResolverForSpecs();
         var errorList = new List<string>();
         foreach (var container in _simulation.Model.Root.GetAllChildren<IContainer>(x => x.IsNamed(compound)))
         {
            if (container.ContainerType != ContainerType.Molecule)
               errorList.Add($"{container.ContainerType} {pathResolver.PathFor(container)}");
         }

         Assert.AreEqual(errorList.Count, 0, errorList.ToString("\n"));
      }
   }

   public class When_creating_a_population_simulation_with_the_standard_building_block_and_iv_bolus_for_all_populations : concern_for_PopulationSimulation
   {
      private IList<SpeciesPopulation> _allPopulations;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _protocol = DomainFactoryForSpecs.CreateStandardIVProtocol();

         var populationsRepo = IoC.Resolve<IPopulationRepository>();
         _allPopulations = populationsRepo.All().ToList();
      }

      [Observation]
      public async Task should_be_able_to_simulate_the_simulation()
      {
         var errors = new List<string>();
         foreach (var pop in _allPopulations)
         {
            //currently simulations with pregnant pop cannot be created
            if (pop.Name.Equals(CoreConstants.Population.Pregnant))
               continue; 

            await runPopulationSimulationFor(pop.Name, errors);
         }

         errors.Count.ShouldBeEqualTo(0, errors.ToString("\n"));
      }

      private async Task runPopulationSimulationFor(string populationName, List<string> errors)
      {
         try
         {
            var individual = DomainFactoryForSpecs.CreateStandardIndividual(populationName);
            var population = DomainFactoryForSpecs.CreateDefaultPopulation(individual);
            var simulation = DomainFactoryForSpecs.CreateSimulationWith(population, _compound, _protocol) as PopulationSimulation;

            var simulationEngine = IoC.Resolve<ISimulationEngine<PopulationSimulation>>();
            var simSettingsRetriever = IoC.Resolve<ISimulationSettingsRetriever>();
            simSettingsRetriever.CreatePKSimDefaults(simulation);
            await simulationEngine.RunAsync(simulation,_simulationRunOptions);
            simulation.HasResults.ShouldBeTrue();
         }
         catch(Exception ex)
         {
            var errorMsg = $"Population simulation for the population '{populationName}' failed: {ex.ToString()}";
            errors.Add(errorMsg);
         }
      }
   }

   public class When_creating_an_aging_population_simulation_with_the_standard_building_block_and_iv_bolus : concern_for_PopulationSimulation
   {
      private Population _population;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _protocol = DomainFactoryForSpecs.CreateStandardIVProtocol();
         _population = DomainFactoryForSpecs.CreateDefaultPopulation(_individual);
         _simulation = DomainFactoryForSpecs.CreateSimulationWith(_population, _compound, _protocol, allowAging: true) as PopulationSimulation;
      }

      [Observation]
      public async Task should_be_able_to_simulate_the_simulation()
      {
         var simulationEngine = IoC.Resolve<ISimulationEngine<PopulationSimulation>>();
         var simSettingsRetriever = IoC.Resolve<ISimulationSettingsRetriever>();
         simSettingsRetriever.CreatePKSimDefaults(_simulation);
         await simulationEngine.RunAsync(_simulation, _simulationRunOptions);
         _simulation.HasResults.ShouldBeTrue();
      }
   }

   public class When_setting_the_value_of_the_hematocrit_parameter_in_the_individual : concern_for_IndividualSimulation
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         _individual.Organism.Parameter(CoreConstants.Parameter.HCT).Value = 0.6;
         _simulation = DomainFactoryForSpecs.CreateSimulationWith(_individual, _compound, _protocol) as IndividualSimulation;
      }

      [Observation]
      public void should_transfer_the_value_in_the_simulation_parameter()
      {
         var hct = _simulation.All<IParameter>().FindByName(CoreConstants.Parameter.HCT);
         hct.Value.ShouldBeEqualTo(0.6);
      }
   }

   public class When_creating_an_individual_simulation_2pores_with_the_standard_building_block : concern_for_IndividualSimulation
   {
      private IBuildConfiguration _buildConfiguration;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _compound.Parameter(CoreConstants.Parameter.IS_SMALL_MOLECULE).Value = 0;
         _simulation = DomainFactoryForSpecs.CreateSimulationWith(_individual, _compound, _protocol, CoreConstants.Model.TwoPores) as IndividualSimulation;
         var buildConfigurationTask = IoC.Resolve<IBuildConfigurationTask>();
         _buildConfiguration = buildConfigurationTask.CreateFor(_simulation, shouldValidate: true, createAgingDataInSimulation: false);
      }

      [Observation]
      public void should_set_negative_values_allowed_true_to_predefined_compartments_and_molecules()
      {
         var msv = _buildConfiguration.MoleculeStartValues;
         var moleculesWithAllowedNegativeValues = (from molecule in msv
            where molecule.NegativeValuesAllowed
            select molecule).ToList();
         moleculesWithAllowedNegativeValues.Count.ShouldBeEqualTo(2);
      }

      [Observation]
      public void should_be_able_to_create_the_simulation()
      {
         _simulation.ShouldNotBeNull();
      }

      [Observation]
      public async Task should_be_able_to_simulate_the_simulation()
      {
         var simulationEngine = IoC.Resolve<ISimulationEngine<IndividualSimulation>>();
         await simulationEngine.RunAsync(_simulation, _simulationRunOptions);
         _simulation.HasResults.ShouldBeTrue();
      }

      [Observation]
      public void all_parmaeters_defined_in_a_template_building_blocks_should_have_an_origin_parameter_id_set()
      {
         var allParameters = _simulation.ParametersOfType(PKSimBuildingBlockType.Template)
            .Where(x => string.IsNullOrEmpty(x.Origin.ParameterId));
         allParameters.Count().ShouldBeEqualTo(0);
      }
   }

   public class When_creating_a_simulation_for_a_subject_allowing_aging : concern_for_IndividualSimulation
   {
      private PathCache<IDistributedParameter> _allDistributedParameter;
      private PathCache<IParameter> _allAgeDependentParameters;
      private const string _enzymeName = "CYP3A4";

      public override void GlobalContext()
      {
         base.GlobalContext();

         var enzymeFactory = IoC.Resolve<IIndividualEnzymeFactory>();
         var individualProtein = enzymeFactory.CreateFor(_individual).WithName(_enzymeName);
         individualProtein.Ontogeny = new UserDefinedOntogeny() {Table = createOntogenyTable()};
         _individual.AddMolecule(individualProtein.DowncastTo<IndividualEnzyme>().WithName(_enzymeName));

         var containerTask = IoC.Resolve<IContainerTask>();
         _individual.OriginData.Age = 2;
         _allDistributedParameter = containerTask.CacheAllChildren<IDistributedParameter>(_individual);
         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(_individual, _compound, _protocol) as IndividualSimulation;
         _simulation.AllowAging = true;
         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
         _allAgeDependentParameters = containerTask.CacheAllChildren<IParameter>(_simulation.Model.Root);
      }

      private DistributedTableFormula createOntogenyTable()
      {
         const double yearInMinutes = 365.25 * 24 * 60;
         var formulaFactory = IoC.Resolve<IFormulaFactory>();
         var tableFormula = formulaFactory.CreateDistributedTableFormula();

         tableFormula.AddPoint(0, 0.5, new DistributionMetaData());
         tableFormula.AddPoint(yearInMinutes, 0.8, new DistributionMetaData());
         tableFormula.AddPoint(100 * yearInMinutes, 1.0, new DistributionMetaData());

         return tableFormula;
      }

      [Observation]
      public void should_have_replaced_all_age_dependent_parameters_with_a_table_containing_the_age_variation_for_the_defined_GA()
      {
         var errorList = new List<string>();
         foreach (var parameter in _allDistributedParameter.KeyValues)
         {
            //these parameters are not converted
            if (parameter.Value.NameIsOneOf(CoreConstants.Parameter.MEAN_HEIGHT, CoreConstants.Parameter.MEAN_WEIGHT))
               continue;

            if (parameter.Value.Formula.DistributionType() == DistributionTypes.Discrete)
               continue;

            //only one point for this parameters
            if (ConverterConstants.Parameter.DistributedParametersWithOnlyOneSupportingPoint.Contains(parameter.Value.Name))
               continue;

            var simParameter = _allAgeDependentParameters[parameter.Key];

            checkDistributedParameterIsDefinedAsTable(simParameter, errorList, parameter.Key);
         }

         //check for ontogeny parameter
         var simParameterOnto = _simulation.Model.Root.EntityAt<IParameter>(_enzymeName, (CoreConstants.Parameter.ONTOGENY_FACTOR));
         checkOntogenyFactorIsDefinedAsTableFormula(simParameterOnto, errorList, CoreConstants.Parameter.ONTOGENY_FACTOR);

         Assert.IsTrue(errorList.Count == 0, errorList.ToString("\n"));
      }

      private void checkOntogenyFactorIsDefinedAsTableFormula(IParameter simParameter, List<string> errorList, string parameterKey)
      {
         var formula = simParameter.Formula;
         //make sure table formula is exported in Non-Derivated mode!
         var tableFormula = formula.DowncastTo<TableFormula>();
         if (tableFormula.UseDerivedValues)
            errorList.Add($"Parameter '{parameterKey}' was replaced with table formula in 'useDerivedValues' mode");
      }

      //check thet simulation parameter is table in nonderivedvalues-mode
      private void checkDistributedParameterIsDefinedAsTable(IParameter simParameter, List<string> errorList, string parameterKey)
      {
         var formula = simParameter.Formula;
         if (formula.IsAnImplementationOf<DistributedTableFormula>()) return;
         errorList.Add($"Parameter '{parameterKey}' was not replaced with table formula (formula type is '{simParameter.Formula.GetType().Name})");
      }
   }

   public class When_creating_an_individual_simulation_with_rat_4comp : concern_for_IndividualSimulation
   {
      private Individual _rat;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _rat = DomainFactoryForSpecs.CreateStandardIndividual("Rat");
         _simulation = DomainFactoryForSpecs.CreateSimulationWith(_rat, _compound, _protocol) as IndividualSimulation;
      }

      [Observation]
      public void should_be_able_to_create_the_simulation()
      {
         _simulation.ShouldNotBeNull();
      }

      [Observation]
      public async Task should_be_able_to_simulate_the_simulation()
      {
         var simulationEngine = IoC.Resolve<ISimulationEngine<IndividualSimulation>>();
         await simulationEngine.RunAsync(_simulation, _simulationRunOptions);
         _simulation.HasResults.ShouldBeTrue();
      }
   }

   public class When_creating_an_individual_simulation_with_rat_2pores : concern_for_IndividualSimulation
   {
      private Individual _rat;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _rat = DomainFactoryForSpecs.CreateStandardIndividual("Rat");
         _simulation = DomainFactoryForSpecs.CreateSimulationWith(_rat, _compound, _protocol, CoreConstants.Model.TwoPores) as IndividualSimulation;
      }

      [Observation]
      public void should_be_able_to_create_the_simulation()
      {
         _simulation.ShouldNotBeNull();
      }

      [Observation]
      public async Task should_be_able_to_simulate_the_simulation()
      {
         var simulationEngine = IoC.Resolve<ISimulationEngine<IndividualSimulation>>();
         await simulationEngine.RunAsync(_simulation, _simulationRunOptions);
         _simulation.HasResults.ShouldBeTrue();
      }
   }

   public class When_creating_an_individual_simulation_2pores_with_2_compounds : concern_for_IndividualSimulation
   {
      private Compound _compound2;
      private Protocol _protocol2;
      private string _compound1Name, _compound2Name;

      public override void GlobalContext()
      {
         base.GlobalContext();

         _compound2 = DomainFactoryForSpecs.CreateStandardCompound().WithName("C2");
         _protocol2 = DomainFactoryForSpecs.CreateStandardIVBolusProtocol().WithName("IV2");
         var modelProps = DomainFactoryForSpecs.CreateModelPropertiesFor(_individual, CoreConstants.Model.TwoPores);

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(
            _individual, new[] {_compound, _compound2}, new[] {_protocol, _protocol2}, modelProps) as IndividualSimulation;
         DomainFactoryForSpecs.AddModelToSimulation(_simulation);

         _compound1Name = _compound.Name;
         _compound2Name = _compound2.Name;
      }

      [Observation]
      public void should_be_able_to_create_the_simulation()
      {
         _simulation.ShouldNotBeNull();
      }

      [Observation]
      public void should_create_required_molecules_in_subcompartments_of_tissue_organ()
      {
         string comp1FcRnComplexName = CoreConstants.Molecule.DrugFcRnComplexName(_compound1Name);
         string comp2FcRnComplexName = CoreConstants.Molecule.DrugFcRnComplexName(_compound2Name);

         var bone = organ(CoreConstants.Organ.Bone);

         // plasma must contain molecules and their FcRn complex
         var plasma = bone.GetSingleChildByName<IContainer>(CoreConstants.Compartment.Plasma);
         var bonePlasmaMoleculeNames = moleculeNamesIn(plasma);

         bonePlasmaMoleculeNames.ShouldOnlyContain(_compound1Name, _compound2Name, comp1FcRnComplexName, comp2FcRnComplexName);

         // BC must contain compounds
         var bloodCells = bone.GetSingleChildByName<IContainer>(CoreConstants.Compartment.BloodCells);
         var boneBloodCellsMoleculeNames = moleculeNamesIn(bloodCells);

         boneBloodCellsMoleculeNames.ShouldOnlyContain(_compound1Name, _compound2Name);

         // interstitial must contain compounds and their FcRn complex
         var interstitial = bone.GetSingleChildByName<IContainer>(CoreConstants.Compartment.Interstitial);
         var boneInterstitialMoleculeNames = moleculeNamesIn(interstitial);

         boneInterstitialMoleculeNames.ShouldOnlyContain(_compound1Name, _compound2Name, comp1FcRnComplexName, comp2FcRnComplexName);

         // cell must contain compounds
         var intracellular = bone.GetSingleChildByName<IContainer>(CoreConstants.Compartment.Intracellular);
         var boneIntracellularMoleculeNames = moleculeNamesIn(intracellular);

         boneIntracellularMoleculeNames.ShouldOnlyContain(_compound1Name, _compound2Name);

         // endosome must contain compounds and their FcRn complex
         var endosome = bone.GetSingleChildByName<IContainer>(CoreConstants.Compartment.Endosome);
         var boneEndosomeMoleculeNames = moleculeNamesIn(endosome);

         boneEndosomeMoleculeNames.ShouldOnlyContain(_compound1Name, _compound2Name, comp1FcRnComplexName, comp2FcRnComplexName);
      }

      [Observation]
      public void should_create_required_molecules_in_subcompartments_of_endogenous_igg_organ()
      {
         string fcRn = CoreConstants.Molecule.FcRn;
         const string fcRnKineticsEndosome = "FcRn kinetics endosome";
         const string fcRnKineticsInterstitial = "FcRn kinetics interstitial";
         const string fcRnKineticsPlasma = "FcRn kinetics plasma";
         string ligandEndo = CoreConstants.Molecule.LigandEndo;
         string ligandEndoComplex = CoreConstants.Molecule.LigandEndoComplex;

         var endoIgg = organ(CoreConstants.Organ.EndogenousIgG);

         // plasma 
         var plasma = endoIgg.GetSingleChildByName<IContainer>(CoreConstants.Compartment.Plasma);
         var endoIggPlasmaMoleculeNames = moleculeNamesIn(plasma);

         endoIggPlasmaMoleculeNames.ShouldOnlyContain(fcRn, fcRnKineticsPlasma, ligandEndo, ligandEndoComplex);

         // interstitial 
         var interstitial = endoIgg.GetSingleChildByName<IContainer>(CoreConstants.Compartment.Interstitial);
         var endoIggInterstitialMoleculeNames = moleculeNamesIn(interstitial);

         endoIggInterstitialMoleculeNames.ShouldOnlyContain(fcRn, fcRnKineticsInterstitial, ligandEndo, ligandEndoComplex);

         // endosome
         var endosome = endoIgg.GetSingleChildByName<IContainer>(CoreConstants.Compartment.Endosome);
         var endoIggEndosomeMoleculeNames = moleculeNamesIn(endosome);

         endoIggEndosomeMoleculeNames.ShouldOnlyContain(fcRn, fcRnKineticsEndosome, ligandEndo, ligandEndoComplex);

         // IgG_Source
         var iggSrc = endoIgg.GetSingleChildByName<IContainer>("IgG_Source");
         var endoIggSrcMoleculeNames = moleculeNamesIn(iggSrc);

         endoIggSrcMoleculeNames.ShouldOnlyContain(ligandEndo);

         // plasma must contain molecules and their FcRn complex
         var endoCL = endoIgg.GetSingleChildByName<IContainer>("EndosomalClearance");
         var endoIggCLMoleculeNames = moleculeNamesIn(endoCL);

         endoIggCLMoleculeNames.ShouldOnlyContain(ligandEndo);
      }

      private IContainer organ(string organName)
      {
         var organism = _simulation.Model.Root.GetSingleChildByName<IContainer>("Organism");
         return organism.GetSingleChildByName<IContainer>(organName);
      }

      private IEnumerable<string> moleculeNamesIn(IContainer container)
      {
         return (from molecule in container.GetAllChildren<IMoleculeAmount>()
            select molecule.Name).ToList();
      }

      private IEnumerable<string> reactionNamesIn(IContainer container)
      {
         return (from molecule in container.GetAllChildren<IReaction>()
            select molecule.Name).ToList();
      }

      [Observation]
      public void should_create_required_reactions_in_subcompartments_of_tissue_organ()
      {
         string fcRnBindingTissueComp1 = CoreConstants.Reaction.FcRnBindingTissueNameFrom(_compound1Name);
         string fcRnBindingTissueComp2 = CoreConstants.Reaction.FcRnBindingTissueNameFrom(_compound2Name);

         var bone = organ(CoreConstants.Organ.Bone);

         // plasma must FcRn binding tissue for all compounds
         var plasma = bone.GetSingleChildByName<IContainer>(CoreConstants.Compartment.Plasma);
         var bonePlasmaReactionNames = reactionNamesIn(plasma);

         bonePlasmaReactionNames.ShouldOnlyContain(fcRnBindingTissueComp1, fcRnBindingTissueComp2);

         // BC has no reactions
         var bloodCells = bone.GetSingleChildByName<IContainer>(CoreConstants.Compartment.BloodCells);
         var boneBloodCellsReactionNames = reactionNamesIn(bloodCells);

         boneBloodCellsReactionNames.Count().ShouldBeEqualTo(0);

         // interstitial must FcRn binding tissue for all compounds
         var interstitial = bone.GetSingleChildByName<IContainer>(CoreConstants.Compartment.Interstitial);
         var boneInterstitialReactionNames = reactionNamesIn(interstitial);

         boneInterstitialReactionNames.ShouldOnlyContain(fcRnBindingTissueComp1, fcRnBindingTissueComp2);

         // Cells has no reactions
         var intracellular = bone.GetSingleChildByName<IContainer>(CoreConstants.Compartment.Intracellular);
         var boneIntracellularReactionNames = reactionNamesIn(intracellular);

         boneIntracellularReactionNames.Count().ShouldBeEqualTo(0);

         // endosome must FcRn binding tissue for all compounds
         var endosome = bone.GetSingleChildByName<IContainer>(CoreConstants.Compartment.Endosome);
         var boneEndosomeReactionNames = reactionNamesIn(endosome);

         boneEndosomeReactionNames.ShouldOnlyContain(fcRnBindingTissueComp1, fcRnBindingTissueComp2);
      }

      [Observation]
      public void should_create_required_reactions_in_subcompartments_of_endogenous_igg_organ()
      {
         string fcRnBindingDrugComp1Endo = CoreConstants.Reaction.FcRnBindingDrugEndosomeNameFrom(_compound1Name);
         string fcRnBindingDrugComp2Endo = CoreConstants.Reaction.FcRnBindingDrugEndosomeNameFrom(_compound2Name);

         string fcRnBindingDrugComp1Int = CoreConstants.Reaction.FcRnBindingDrugInterstitialNameFrom(_compound1Name);
         string fcRnBindingDrugComp2Int = CoreConstants.Reaction.FcRnBindingDrugInterstitialNameFrom(_compound2Name);

         string fcRnBindingDrugComp1Pls = CoreConstants.Reaction.FcRnBindingDrugPlasmaNameFrom(_compound1Name);
         string fcRnBindingDrugComp2Pls = CoreConstants.Reaction.FcRnBindingDrugPlasmaNameFrom(_compound2Name);

         const string fcRnBindingEndogenousIgg = "FcRn binding endogenous Igg";

         var endoIgg = organ(CoreConstants.Organ.EndogenousIgG);

         // plasma 
         var plasma = endoIgg.GetSingleChildByName<IContainer>(CoreConstants.Compartment.Plasma);
         var endoIggPlasmaReactionNames = reactionNamesIn(plasma);

         endoIggPlasmaReactionNames.ShouldOnlyContain(fcRnBindingDrugComp1Pls, fcRnBindingDrugComp2Pls, fcRnBindingEndogenousIgg);

         // interstitial 
         var interstitial = endoIgg.GetSingleChildByName<IContainer>(CoreConstants.Compartment.Interstitial);
         var endoIggInterstitialReactionNames = reactionNamesIn(interstitial);

         endoIggInterstitialReactionNames.ShouldOnlyContain(fcRnBindingDrugComp1Int, fcRnBindingDrugComp2Int, fcRnBindingEndogenousIgg);

         // endosome
         var endosome = endoIgg.GetSingleChildByName<IContainer>(CoreConstants.Compartment.Endosome);
         var endoIggEndosomeReactionNames = reactionNamesIn(endosome);

         endoIggEndosomeReactionNames.ShouldOnlyContain(fcRnBindingDrugComp1Endo, fcRnBindingDrugComp2Endo, fcRnBindingEndogenousIgg);

         // IgG_Source
         var iggSrc = endoIgg.GetSingleChildByName<IContainer>("IgG_Source");
         var endoIggSrcReactionNames = reactionNamesIn(iggSrc);

         endoIggSrcReactionNames.Count().ShouldBeEqualTo(0);

         // endosomal clearance
         var endoCL = endoIgg.GetSingleChildByName<IContainer>("EndosomalClearance");
         var endoIggCLReactionNames = reactionNamesIn(endoCL);

         endoIggCLReactionNames.Count().ShouldBeEqualTo(0);
      }

      [Observation]
      public async Task should_be_able_to_simulate_the_simulation()
      {
         var simulationEngine = IoC.Resolve<ISimulationEngine<IndividualSimulation>>();
         await simulationEngine.RunAsync(_simulation, _simulationRunOptions);
         _simulation.HasResults.ShouldBeTrue();
      }
   }

   [NightlyOnly]
   public abstract class When_creating_an_individual_simulation_with_drug_and_inhibitor : concern_for_IndividualSimulation
   {
      protected ICompoundProcessRepository _compoundProcessRepo;
      private IInteractionTask _interactionTask;

      protected abstract IEnumerable<PartialProcess> PartialProcesses { get; }
      protected string InhibitionProcessName { get; }

      protected When_creating_an_individual_simulation_with_drug_and_inhibitor(string inhibitorProcessName)
      {
         InhibitionProcessName = inhibitorProcessName;
      }

      public override void GlobalContext()
      {
         base.GlobalContext();

         _compoundProcessRepo = IoC.Resolve<ICompoundProcessRepository>();
         _interactionTask = IoC.Resolve<IInteractionTask>();
         var cloner = IoC.Resolve<ICloneManager>();
         var enzymeFactory = IoC.Resolve<IIndividualEnzymeFactory>();
         var transporterFactory = IoC.Resolve<IIndividualTransporterFactory>();

         var inhibitor = DomainFactoryForSpecs.CreateStandardCompound().WithName("Inhibitor");
         var protocol2 = DomainFactoryForSpecs.CreateStandardIVBolusProtocol().WithName("IV2");

         var allPartialProcesses = PartialProcesses.ToList();

         foreach (var metaTemplate in allPartialProcesses)
         {
            var moleculeName = "Molecule_" + metaTemplate.Name;

            if (metaTemplate as EnzymaticProcess != null)
            {
               var individualProtein = enzymeFactory.CreateFor(_individual).WithName(moleculeName);
               _individual.AddMolecule(individualProtein.DowncastTo<IndividualEnzyme>().WithName(moleculeName));
            }
            else
            {
               var individualProtein = transporterFactory.CreateFor(_individual).WithName(moleculeName);
               _individual.AddMolecule(individualProtein.DowncastTo<IndividualTransporter>().WithName(moleculeName));
            }

            var process = cloner.Clone(metaTemplate).DowncastTo<PartialProcess>();
            process.Name = "Process " + moleculeName;
            process.MoleculeName = moleculeName;
            _compound.AddProcess(process);

            var inhibitionTemplate = _compoundProcessRepo.ProcessByName<InteractionProcess>(InhibitionProcessName);
            var inhibitionProcess = cloner.Clone(inhibitionTemplate);
            inhibitionProcess.Name = "InhibitionProcess " + moleculeName;
            inhibitionProcess.MoleculeName = moleculeName;
            inhibitor.AddProcess(inhibitionProcess);
         }

         _simulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(
            _individual, new[] {_compound, inhibitor}, new[] {_protocol, protocol2}).DowncastTo<IndividualSimulation>();

         foreach (var inhibitionProcess in inhibitor.AllProcesses<InteractionProcess>())
         {
            var interactionSelection = new InteractionSelection {CompoundName = inhibitor.Name, MoleculeName = inhibitionProcess.MoleculeName, ProcessName = inhibitionProcess.Name};
            _simulation.InteractionProperties.AddInteraction(interactionSelection);
         }

         DomainFactoryForSpecs.AddModelToSimulation(_simulation);
      }

      protected void CreateSimulationTest()
      {
         _simulation.ShouldNotBeNull();
      }

      protected async Task RunSimulationTest()
      {
         var simulationEngine = IoC.Resolve<ISimulationRunner>();
         await simulationEngine.RunSimulation(_simulation, _simulationRunOptions);
         _simulation.HasResults.ShouldBeTrue();
      }

      protected void ValidateInteractionContainersTest()
      {
         _interactionTask.AllInteractionContainers(_simulation).Count().ShouldBeEqualTo(PartialProcesses.Count());
      }

      protected void CalculateDDIRatioForDrug()
      {
         _globalPKAnalysisTask.CalculateDDIRatioFor(_simulation, _compound.Name);
      }

      [Observation]
      public void should_be_able_to_create_the_simulation()
      {
         CreateSimulationTest();
      }

      [Observation]
      public async Task should_be_able_to_simulate_the_simulation()
      {
         await RunSimulationTest();
      }

      [Observation]
      public void should_be_able_to_retrieve_the_interaction_containers_for_the_given_interaction()
      {
         ValidateInteractionContainersTest();
      }

      [Observation]
      public void should_be_able_to_calculate_ddi_ratio()
      {
         CalculateDDIRatioForDrug();
      }
   }

   public abstract class When_creating_an_individual_simulation_with_drug_and_inhibitor_for_all_metabolic_processes : When_creating_an_individual_simulation_with_drug_and_inhibitor
   {
      protected When_creating_an_individual_simulation_with_drug_and_inhibitor_for_all_metabolic_processes(string processName) : base(processName)
      {
      }

      protected override IEnumerable<PartialProcess> PartialProcesses => _compoundProcessRepo.All<EnzymaticProcess>();
   }

   public abstract class When_creating_an_individual_simulation_with_drug_and_inhibitor_for_all_active_transport_processes : When_creating_an_individual_simulation_with_drug_and_inhibitor
   {
      protected When_creating_an_individual_simulation_with_drug_and_inhibitor_for_all_active_transport_processes(string processName) : base(processName)
      {
      }

      protected override IEnumerable<PartialProcess> PartialProcesses => _compoundProcessRepo.All<TransportPartialProcess>();
   }

   public class When_creating_an_individual_simulation_with_drug_and_competitiv_inhibitor_for_all_metabolic_processes : When_creating_an_individual_simulation_with_drug_and_inhibitor_for_all_metabolic_processes
   {
      public When_creating_an_individual_simulation_with_drug_and_competitiv_inhibitor_for_all_metabolic_processes()
         : base(CoreConstantsForSpecs.Process.COMPETITIVE_INHIBITION)
      {
      }
   }

   public class When_creating_an_individual_simulation_with_drug_and_competitiv_inhibitor_for_all_active_transport_processes : When_creating_an_individual_simulation_with_drug_and_inhibitor_for_all_active_transport_processes
   {
      public When_creating_an_individual_simulation_with_drug_and_competitiv_inhibitor_for_all_active_transport_processes()
         : base(CoreConstantsForSpecs.Process.COMPETITIVE_INHIBITION)
      {
      }
   }

   public class When_creating_an_individual_simulation_with_drug_and_uncompetitiv_inhibitor_for_all_metabolic_processes : When_creating_an_individual_simulation_with_drug_and_inhibitor_for_all_metabolic_processes
   {
      public When_creating_an_individual_simulation_with_drug_and_uncompetitiv_inhibitor_for_all_metabolic_processes()
         : base(CoreConstantsForSpecs.Process.UNCOMPETITIVE_INHIBITION)
      {
      }
   }

   public class When_creating_an_individual_simulation_with_drug_and_uncompetitiv_inhibitor_for_all_active_transport_processes : When_creating_an_individual_simulation_with_drug_and_inhibitor_for_all_active_transport_processes
   {
      public When_creating_an_individual_simulation_with_drug_and_uncompetitiv_inhibitor_for_all_active_transport_processes()
         : base(CoreConstantsForSpecs.Process.UNCOMPETITIVE_INHIBITION)
      {
      }
   }

   public class When_creating_an_individual_simulation_with_drug_and_noncompetitiv_inhibitor_for_all_metabolic_processes : When_creating_an_individual_simulation_with_drug_and_inhibitor_for_all_metabolic_processes
   {
      public When_creating_an_individual_simulation_with_drug_and_noncompetitiv_inhibitor_for_all_metabolic_processes()
         : base(CoreConstantsForSpecs.Process.NONCOMPETITIVE_INHIBITION)
      {
      }
   }

   public class When_creating_an_individual_simulation_with_drug_and_noncompetitiv_inhibitor_for_all_active_transport_processes : When_creating_an_individual_simulation_with_drug_and_inhibitor_for_all_active_transport_processes
   {
      public When_creating_an_individual_simulation_with_drug_and_noncompetitiv_inhibitor_for_all_active_transport_processes()
         : base(CoreConstantsForSpecs.Process.NONCOMPETITIVE_INHIBITION)
      {
      }
   }

   public class When_creating_an_individual_simulation_with_drug_and_mixed_inhibitor_for_all_metabolic_processes : When_creating_an_individual_simulation_with_drug_and_inhibitor_for_all_metabolic_processes
   {
      public When_creating_an_individual_simulation_with_drug_and_mixed_inhibitor_for_all_metabolic_processes()
         : base(CoreConstantsForSpecs.Process.MIXED_INHIBITION)
      {
      }
   }

   public class When_creating_an_individual_simulation_with_drug_and_mixed_inhibitor_for_all_active_transport_processes : When_creating_an_individual_simulation_with_drug_and_inhibitor_for_all_active_transport_processes
   {
      public When_creating_an_individual_simulation_with_drug_and_mixed_inhibitor_for_all_active_transport_processes()
         : base(CoreConstantsForSpecs.Process.MIXED_INHIBITION)
      {
      }
   }

   public class When_creating_an_individual_simulation_with_drug_and_irreversible_inhibitor_for_all_metabolic_processes : When_creating_an_individual_simulation_with_drug_and_inhibitor_for_all_metabolic_processes
   {
      public When_creating_an_individual_simulation_with_drug_and_irreversible_inhibitor_for_all_metabolic_processes()
         : base(CoreConstantsForSpecs.Process.IRREVERSIBLE_INHIBITION)
      {
      }
   }

   public class When_creating_an_individual_simulation_with_drug_and_irreversible_inhibitor_for_all_active_transport_processes : When_creating_an_individual_simulation_with_drug_and_inhibitor_for_all_active_transport_processes
   {
      public When_creating_an_individual_simulation_with_drug_and_irreversible_inhibitor_for_all_active_transport_processes()
         : base(CoreConstantsForSpecs.Process.IRREVERSIBLE_INHIBITION)
      {
      }
   }

   public class When_creating_an_individual_simulation_with_drug_and_inductor_for_all_metabolic_processes : When_creating_an_individual_simulation_with_drug_and_inhibitor_for_all_metabolic_processes
   {
      public When_creating_an_individual_simulation_with_drug_and_inductor_for_all_metabolic_processes()
         : base(CoreConstantsForSpecs.Process.INDUCTION)
      {
      }
   }

   public class When_creating_an_individual_simulation_with_drug_and_inductor_for_all_active_transport_processes : When_creating_an_individual_simulation_with_drug_and_inhibitor_for_all_active_transport_processes
   {
      public When_creating_an_individual_simulation_with_drug_and_inductor_for_all_active_transport_processes()
         : base(CoreConstantsForSpecs.Process.INDUCTION)
      {
      }
   }
}