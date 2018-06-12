using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Extensions;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.Mappers;
using PKSim.Infrastructure.ORM.Repositories;
using PKSim.Infrastructure.ProjectConverter;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_DatabaseUpdate : ContextForIntegration<IModelDatabase>
   {
   }

   public class When_checking_the_changes_in_the_database_for_version_7_2_0 : concern_for_DatabaseUpdate
   {
      private IParameterDistributionRepository _parameterDistributionRepository;
      private IPopulationRepository _populationRepository;
      private IRepresentationInfoRepository _representationInfoRepository;
      private IParameterRateRepository _parameterRateRepository;
      private IFlatSpeciesRepository _flatSpeciesRepository;
      private IPopulationAgeRepository _populationAgeRepository;

      public override void GlobalContext()
      {
         base.GlobalContext();

         _representationInfoRepository = IoC.Resolve<IRepresentationInfoRepository>();
         _parameterRateRepository = IoC.Resolve<IParameterRateRepository>();
         _flatSpeciesRepository = IoC.Resolve<IFlatSpeciesRepository>();
         _parameterDistributionRepository = IoC.Resolve<IParameterDistributionRepository>();
         _populationRepository = IoC.Resolve<IPopulationRepository>();
         _populationAgeRepository = IoC.Resolve<IPopulationAgeRepository>();
      }

      [Observation]
      public void should_update_elderly_ICRP_organ_volumes()
      {
         var meanStdRatios = new[]
         {
            new {organ=CoreConstants.Organ.ArterialBlood, maleRatio=0.05, femaleRatio=0.05 },
            new {organ=CoreConstants.Organ.Bone, maleRatio=0.011059, femaleRatio=0.01 },
            new {organ=CoreConstants.Organ.Gonads, maleRatio=0.05, femaleRatio=0.05 },
            new {organ=CoreConstants.Organ.Kidney, maleRatio=0.24628, femaleRatio=0.25 },
            new {organ=CoreConstants.Organ.LargeIntestine, maleRatio=0.14, femaleRatio=0.14 },
            new {organ=CoreConstants.Organ.Liver, maleRatio=0.25, femaleRatio=0.25 },
            new {organ=CoreConstants.Organ.Pancreas, maleRatio=0.2689, femaleRatio=0.28 },
            new {organ=CoreConstants.Organ.Skin, maleRatio=0.05, femaleRatio=0.05 },
            new {organ=CoreConstants.Organ.VenousBlood, maleRatio=0.05, femaleRatio=0.05 }
         };

         foreach (var meanStdRatio in meanStdRatios)
         {
            var volumeParameters = _parameterDistributionRepository.All().Where(pd => isElderlyICRPVolumeParameterIn(pd, meanStdRatio.organ));

            foreach (var volumeParameter in volumeParameters)
            {
               if (meanStdRatio.organ.Equals(CoreConstants.Organ.Liver) && volumeParameter.Gender.Equals(CoreConstants.Gender.Male))
                  continue; //no update for this combination

               var ratio = volumeParameter.Deviation / volumeParameter.Mean;
               var expectedRatio = volumeParameter.Gender.Equals(CoreConstants.Gender.Male) ?
                  meanStdRatio.maleRatio : meanStdRatio.femaleRatio;

               ratio.ShouldBeEqualTo(expectedRatio, 1e-3, $"{meanStdRatio.organ}.{volumeParameter.Gender}.{volumeParameter.Age} years");
            }
         }
      }

      bool isElderlyICRPVolumeParameterIn(ParameterDistributionMetaData pd, string organ)
      {
         return pd.Population.Equals(CoreConstants.Population.ICRP) &&
                pd.Age >= 40 &&
                pd.ParameterName.Equals("Volume") &&
                pd.ContainerType.Equals(CoreConstants.ContainerType.Organ) &&
                pd.ContainerName.Equals(organ);
      }

      [Observation]
      public void should_set_min_max_age_population_dependent()
      {
         var ageDependentPops = _populationRepository.All().Where(pop => pop.IsAgeDependent);

         //check [Min..Max] range of the Age parameter for every {Population, Gender}-combination
         foreach (var population in ageDependentPops)
         {
            foreach (var gender in population.Genders)
            {
               checkPopulationAgeSettings(population.Name, gender.Name);
            }
         }
      }

      private void checkPopulationAgeSettings(string populationName, string genderName)
      {
         //get age settings for the given population (currently gender-independent, might change in the future)
         var populationAgeSettings = _populationAgeRepository.PopulationAgeSettingsFrom(populationName);
         (var minAge, var maxAge) = getAgeBoundsFor(populationName, genderName);

         //---- min/max of the age should be set to min/max age available for this population
         populationAgeSettings.MinAge.ShouldBeEqualTo(minAge, $"Min age for {populationName}|{genderName}");
         populationAgeSettings.MaxAge.ShouldBeEqualTo(maxAge, $"Max age for {populationName}|{genderName}");

         //default age of the population should be between min and max age
         populationAgeSettings.DefaultAge.ShouldBeGreaterThanOrEqualTo(minAge);
         populationAgeSettings.DefaultAge.ShouldBeSmallerThanOrEqualTo(maxAge);
      }

      private (double minAge, double maxAge) getAgeBoundsFor(string populationName, string genderName)
      {
         //get all age dependent parameters for the population except the age parameter itself
         var ageDependentParams = _parameterDistributionRepository.All().Where(
            pd => pd.Population.Equals(populationName) && pd.Gender.Equals(genderName)).ToList();

         var ageValues = (from p in ageDependentParams select p.Age).ToList();

         var maxAge = populationName.Equals(CoreConstants.Population.PREGNANT) ? 30.75 : ageValues.Max();

         return (ageValues.Min(), maxAge);
      }

      [Observation]
      public void should_have_updated_the_description_of_the_auc_ratio()
      {
         _representationInfoRepository.InfoFor(RepresentationObjectType.PARAMETER, CoreConstants.PKAnalysis.AUCRatio).Description.Contains("AUC_inf_tDLast").ShouldBeTrue();
      }

      [Observation]
      public void should_set_the_body_surface_area_parameter_of_an_individual_to_can_be_varied_true_and_can_be_varied_in_population_false()
      {
         var bsaParameter = _parameterRateRepository.All().First(p => string.Equals(p.ParameterName, CoreConstants.Parameters.BSA));

         bsaParameter.CanBeVaried.ShouldBeTrue();
         bsaParameter.CanBeVariedInPopulation.ShouldBeFalse();
      }

      [Observation]
      public void only_the_species_human_should_have_the_flag_is_human()
      {
         _flatSpeciesRepository.All().Each(species => { species.IsHuman.ShouldBeEqualTo(string.Equals(species.Id, CoreConstants.Species.HUMAN)); });
      }
   }

   public class When_checking_the_changes_in_the_database_for_version_7_1_0 : concern_for_DatabaseUpdate
   {
      private IParameterValueRepository _parameterValueRepository;
      private IParameterRateRepository _parameterRateRepository;
      private ISpeciesRepository _speciesRepository;
      private ICalculationMethodRepository _calculationMethodRepository;
      private IRateFormulaRepository _rateFormulaRepository;
      private IRateObjectPathsRepository _rateObjectPathsRepository;
      private IParameterDistributionRepository _parameterDistributionRepository;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _parameterValueRepository = IoC.Resolve<IParameterValueRepository>();
         _parameterRateRepository = IoC.Resolve<IParameterRateRepository>();
         _speciesRepository = IoC.Resolve<ISpeciesRepository>();
         _calculationMethodRepository = IoC.Resolve<ICalculationMethodRepository>();
         _rateFormulaRepository = IoC.Resolve<IRateFormulaRepository>();
         _rateObjectPathsRepository = IoC.Resolve<IRateObjectPathsRepository>();
         _parameterDistributionRepository = IoC.Resolve<IParameterDistributionRepository>();
      }

      [Observation]
      public void should_update_nhanes_values()
      {
         var nhanesParams = _parameterDistributionRepository.All().Where(pd => pd.Population.EndsWith("NHANES_1997")).ToList();

         var bwParams = nhanesParams.Where(p => p.ParameterName.Equals(CoreConstants.Parameters.MEAN_WEIGHT)).ToList();
         var heightParams = nhanesParams.Where(p => p.ParameterName.Equals(CoreConstants.Parameters.MEAN_HEIGHT)).ToList();
         var volumeParams = nhanesParams.Where(p => p.ParameterName.Equals(Constants.Parameters.VOLUME)).ToList();

         //check number of new bw/height/volume parameters
         (bwParams.Count + heightParams.Count + volumeParams.Count).ShouldBeEqualTo(1420);

         //check some heigth standard deviations
         var teenHeightParams = heightParams.Where(p => p.Age == 13 || p.Age == 15).ToList();
         teenHeightParams.Count.ShouldBeEqualTo(2 * 2 * 3); //2 age groups*2genders*3populations
         teenHeightParams.Each(p => p.Deviation.ShouldBeGreaterThan(0.6));
      }

      [Observation]
      public void should_remove_species_cattle_and_cat()
      {
         var speciesNames = from species in _speciesRepository.All() select species.Name;
         speciesNames.ShouldNotContain("Cat", "Cattle");
      }

      [Observation]
      public void should_remove_intestinal_permeability_MPTC()
      {
         var calcMethodNames = from cm in _calculationMethodRepository.All() select cm.Name;
         calcMethodNames.ShouldNotContain("IntestinalPermeability_CACO_MPTC");
      }

      [Observation]
      public void should_use_updated_rates_for_lymph_and_fluid_recirculation_flow()
      {
         var lymphFlowRates = from pr in _parameterRateRepository.All().Where(isChangedLymphFlowParameter)
            select _rateFormulaRepository.FormulaFor(new RateKey(pr.CalculationMethod, pr.Rate));
         lymphFlowRates.Each(r => r.Contains("f_lymph").ShouldBeTrue());

         var fluidRecircFlowRates = from pr in _parameterRateRepository.All().Where(isChangedFluidRecircFlowParameter)
            select _rateFormulaRepository.FormulaFor(new RateKey(pr.CalculationMethod, pr.Rate));
         fluidRecircFlowRates.Each(r => r.Contains("f_Jiso").ShouldBeTrue());
      }

      private bool isChangedFluidRecircFlowParameter(ParameterRateMetaData parameterRateMetaData)
      {
         return isChangedFlowParameter(parameterRateMetaData,
            CoreConstants.Parameters.RECIRCULATION_FLOW,
            CoreConstants.Parameters.RECIRCULATION_FLOW_INCL_MUCOSA);
      }

      private bool isChangedLymphFlowParameter(ParameterRateMetaData parameterRateMetaData)
      {
         return isChangedFlowParameter(parameterRateMetaData,
            CoreConstants.Parameters.LYMPH_FLOW,
            CoreConstants.Parameters.LYMPH_FLOW_INCL_MUCOSA);
      }

      private bool isChangedFlowParameter(ParameterRateMetaData parameterRateMetaData,
         string flowParameterName,
         string flowInclMucosaParameterName)
      {
         if (parameterRateMetaData.ContainerType != CoreConstants.ContainerType.Organ)
            return false;

         var container = parameterRateMetaData.ContainerName;
         if (container.Equals(CoreConstants.Organ.EndogenousIgG) ||
             container.Equals(CoreConstants.Organ.PortalVein))
            return false;

         var name = parameterRateMetaData.ParameterName;
         var isIntestine = container.IsOneOf(CoreConstants.Organ.LargeIntestine, CoreConstants.Organ.SmallIntestine);

         return (name.Equals(flowParameterName) && !isIntestine) ||
                (name.Equals(flowInclMucosaParameterName) && isIntestine);
      }

      [Observation]
      public void should_fix_FCRN_binding_reactions()
      {
         checkRate(_rateObjectPathsRepository, "FcRn binding drug in endosomal space");
         checkRate(_rateObjectPathsRepository, "FcRn binding drug in interstitial");
         checkRate(_rateObjectPathsRepository, "FcRn binding drug in plasma");
      }

      private void checkRate(IRateObjectPathsRepository rateObjectPathsRepo, string rate)
      {
         const string calcMethod = "EndosomalSpaceBindingFormulas";

         var objectPaths = rateObjectPathsRepo.ObjectPathsFor(new RateKey(calcMethod, rate));
         var kassObjectPath = objectPaths.First(p => p.Alias.Equals("kass_FcRn"));

         kassObjectPath.PathAsString.EndsWith("kass (FcRn)").ShouldBeTrue();
      }

      [Observation]
      public void should_add_variability_factor_to_meal_rates()
      {
         var rates = _rateFormulaRepository.All().Where(r => r.Rate.IsOneOf("PARAM_Meal_alpha", "PARAM_Meal_beta"));
         rates.Each(r => r.Formula.Contains("Variability_Factor").ShouldBeTrue());
      }

      [Observation]
      public void should_update_proteinmodel_values()
      {
         checkOrganParameter("Bone", "Flow fraction via large pores", 0.05);
         checkOrganParameter("Bone", "Radius (small pores)", 4.5e-8);
         checkOrganParameter("Bone", "Radius (large pores)", 2.5e-7);
         checkOrganParameter("Bone", "Hydraulic conductivity", 9e-12);
         checkOrganParameter("Bone", "Lymph flow proportionality factor", 6.62e-4);
         checkOrganParameter("Bone", "Fluid recirculation flow proportionality factor", 0.96);

         checkOrganParameter("Brain", "Lymph flow proportionality factor", 7.27e-5);
         checkOrganParameter("Brain", "Fluid recirculation flow proportionality factor", 0.404);

         checkOrganParameter("Fat", "Hydraulic conductivity", 9e-12);
         checkOrganParameter("Fat", "Lymph flow proportionality factor", 7.54e-3);
         checkOrganParameter("Fat", "Fluid recirculation flow proportionality factor", 0.357);

         checkOrganParameter("Gonads", "Hydraulic conductivity", 9e-12);
         checkOrganParameter("Gonads", "Lymph flow proportionality factor", 0.0111);
         checkOrganParameter("Gonads", "Fluid recirculation flow proportionality factor", 0.96);

         checkOrganParameter("Heart", "Lymph flow proportionality factor", 1.47e-3);
         checkOrganParameter("Heart", "Fluid recirculation flow proportionality factor", 0.96);

         checkOrganParameter("Kidney", "Hydraulic conductivity", 1.25e-10);
         checkOrganParameter("Kidney", "Lymph flow proportionality factor", 7.09e-4);
         checkOrganParameter("Kidney", "Fluid recirculation flow proportionality factor", 0.761);

         checkOrganParameter("LargeIntestine", "Lymph flow proportionality factor", 0.0144);
         checkOrganParameter("LargeIntestine", "Fluid recirculation flow proportionality factor", 0.179);

         checkOrganParameter("Liver", "Lymph flow proportionality factor", 0.0199);
         checkOrganParameter("Liver", "Fluid recirculation flow proportionality factor", 0.96);

         checkOrganParameter("Lung", "Lymph flow proportionality factor", 3.56E-5);
         checkOrganParameter("Lung", "Fluid recirculation flow proportionality factor", 0.01);

         checkOrganParameter("Muscle", "Hydraulic conductivity", 9E-12);
         checkOrganParameter("Muscle", "Lymph flow proportionality factor", 2.01E-3);
         checkOrganParameter("Muscle", "Fluid recirculation flow proportionality factor", 0.292);

         checkOrganParameter("Pancreas", "Lymph flow proportionality factor", 0.0303);
         checkOrganParameter("Pancreas", "Fluid recirculation flow proportionality factor", 0.01);

         checkOrganParameter("Skin", "Hydraulic conductivity", 1.94722E-11);
         checkOrganParameter("Skin", "Lymph flow proportionality factor", 3.52E-3);
         checkOrganParameter("Skin", "Fluid recirculation flow proportionality factor", 0.617);

         checkOrganParameter("SmallIntestine", "Lymph flow proportionality factor", 1.95E-3);
         checkOrganParameter("SmallIntestine", "Fluid recirculation flow proportionality factor", 0.179);

         checkOrganParameter("Spleen", "Lymph flow proportionality factor", 0.0199);
         checkOrganParameter("Spleen", "Fluid recirculation flow proportionality factor", 0.01);

         checkOrganParameter("Stomach", "Lymph flow proportionality factor", 2.04E-3);
         checkOrganParameter("Stomach", "Fluid recirculation flow proportionality factor", 0.96);

         checkOrganParameter("EndogenousIgG", "Radius (small pores)", 4.5E-8);
         checkOrganParameter("EndogenousIgG", "Radius (large pores)", 2.5E-7);
         checkOrganParameter("EndogenousIgG", "Hydraulic conductivity", 1.84722E-11);

         checkParameter("Organism|EndogenousIgG|Plasma", "Start concentration of free endogenous IgG (plasma)", 70, "Human");
         checkParameter("Organism|EndogenousIgG|Plasma", "Start concentration of free endogenous IgG (plasma)", 75, "Monkey");
         checkParameter("Organism|EndogenousIgG|Plasma", "Start concentration of free endogenous IgG (plasma)", 70, "Beagle");
         checkParameter("Organism|EndogenousIgG|Plasma", "Start concentration of free endogenous IgG (plasma)", 70, "Dog");
         checkParameter("Organism|EndogenousIgG|Plasma", "Start concentration of free endogenous IgG (plasma)", 70, "Minipig");
         checkParameter("Organism|EndogenousIgG|Plasma", "Start concentration of free endogenous IgG (plasma)", 70, "Rat");
         checkParameter("Organism|EndogenousIgG|Plasma", "Start concentration of free endogenous IgG (plasma)", 18, "Mouse");
         checkParameter("Organism|EndogenousIgG|Plasma", "Start concentration of free endogenous IgG (plasma)", 70, "Rabbit");

         checkParameter("Organism|EndogenousIgG|Endosome", "Start concentration of free FcRn (endosome)", 80.8, "Human");
         checkParameter("Organism|EndogenousIgG|Endosome", "Start concentration of free FcRn (endosome)", 21, "Monkey");
         checkParameter("Organism|EndogenousIgG|Endosome", "Start concentration of free FcRn (endosome)", 80.8, "Beagle");
         checkParameter("Organism|EndogenousIgG|Endosome", "Start concentration of free FcRn (endosome)", 80.8, "Dog");
         checkParameter("Organism|EndogenousIgG|Endosome", "Start concentration of free FcRn (endosome)", 80.8, "Minipig");
         checkParameter("Organism|EndogenousIgG|Endosome", "Start concentration of free FcRn (endosome)", 80.8, "Rat");
         checkParameter("Organism|EndogenousIgG|Endosome", "Start concentration of free FcRn (endosome)", 38.7, "Mouse");
         checkParameter("Organism|EndogenousIgG|Endosome", "Start concentration of free FcRn (endosome)", 80.8, "Rabbit");

         checkOrganismParameter("Fraction of endosomal uptake from plasma (global)", 1);
         checkOrganismParameter("Fraction recycled to plasma (global)", 1);
         checkOrganismParameter("Rate constant for endosomal uptake (global)", 0.294);
         checkOrganismParameter("Rate constant for recycling from endosomal space (global)", 0.0888);

         _rateFormulaRepository.FormulaFor(new RateKey("CompoundCommon", "PARAM_Kass_FcRn")).ShouldBeEqualTo("0.87", "'kass (FcRn)' has wrong value");
         _rateFormulaRepository.FormulaFor(new RateKey("Individual_PKSim", "PARAM_Kass_FcRn_LigandEndo")).ShouldBeEqualTo("0.87", "'kass (FcRn, endogenous IgG)' has wrong value");

         checkInitialFormula("krint", "STARTAMOUNT_FcRn_Interstitial", "STARTAMOUNT_LigandEndoComplex_Interstitial");
         checkInitialFormula("krpls", "STARTAMOUNT_FcRn_Plasma", "STARTAMOUNT_LigandEndoComplex_Plasma");

         _rateFormulaRepository.FormulaFor(new RateKey("TwoPoresTransportLinkPls2Int", "TwoPoresTransportLink")).StartsWith("fu").ShouldBeTrue();
      }

      private void checkInitialFormula(string alias, params string[] rates)
      {
         string newFormulaStart = $"{alias} <= 0 ? 0 :";

         foreach (var rate in rates)
         {
            _rateFormulaRepository.FormulaFor(new RateKey("EndosomalSpaceBindingFormulas", rate))
               .StartsWith(newFormulaStart).ShouldBeTrue();
         }
      }

      private void checkOrganismParameter(string parameterName, double expectedValue, string speciesName = "")
      {
         checkParameter("Organism", parameterName, expectedValue, speciesName);
      }

      private void checkOrganParameter(string organName, string parameterName, double expectedValue)
      {
         checkParameter("Organism|" + organName, parameterName, expectedValue);
      }

      private void checkParameter(string containerPath, string parameterName, double expectedValue, string speciesName = "")
      {
         var message = $"'{containerPath}|{parameterName}' has wrong value";

         var parameters = _parameterValueRepository.AllFor(containerPath).Where(p => p.ParameterName.Equals(parameterName)).ToList();
         if (!string.IsNullOrEmpty(speciesName))
            parameters = parameters.Where(p => p.Species.Equals(speciesName)).ToList();

         parameters.Count.ShouldBeGreaterThan(0);
         parameters.Each(p => p.DefaultValue.ShouldBeEqualTo(expectedValue, message));
      }
   }

   public class When_checking_the_changes_in_the_database_for_version_6_4_2 : concern_for_DatabaseUpdate
   {
      [Observation]
      public void should_have_set_the_building_block_type_of_ontogeny_factor_and_onteogeny_factor_gi_in_protein_to_individual()
      {
         var paramValueRepo = IoC.Resolve<IParameterRateRepository>();

         var ontogenyFactory = paramValueRepo.All().First(p => p.ParameterName.Equals(CoreConstants.Parameters.ONTOGENY_FACTOR) && p.ContainerName == "PROTEIN");
         ontogenyFactory.BuildingBlockType.ShouldBeEqualTo(PKSimBuildingBlockType.Individual);

         var ontogenyFactoryGI = paramValueRepo.All().First(p => p.ParameterName.Equals(CoreConstants.Parameters.ONTOGENY_FACTOR_GI) && p.ContainerName == "PROTEIN");
         ontogenyFactoryGI.BuildingBlockType.ShouldBeEqualTo(PKSimBuildingBlockType.Individual);
      }
   }

   public class When_checking_the_changes_in_the_database_for_version_6_4 : concern_for_DatabaseUpdate
   {
      [Observation]
      public void should_have_updated_the_description_for_residual_fraction()
      {
         var represenationInfoRepository = IoC.Resolve<IRepresentationInfoRepository>();
         represenationInfoRepository.InfoFor(RepresentationObjectType.PARAMETER, ConverterConstants.Parameter.RESIDUAL_FRACTION)
            .Description.ShouldBeEqualTo("Residual fraction after measuring time");
      }
   }

   public class When_checking_the_changes_in_the_database_for_version_6_3 : concern_for_DatabaseUpdate
   {
      [Observation]
      public void should_set_plasma_protein_scale_factor_variable_in_population()
      {
         var paramValueRepo = IoC.Resolve<IParameterValueRepository>();
         var plasmaProteinScaleFactor = paramValueRepo.All().First(p => p.ParameterName.Equals(CoreConstants.Parameters.PLASMA_PROTEIN_SCALE_FACTOR));

         plasmaProteinScaleFactor.CanBeVariedInPopulation.ShouldBeTrue();
      }

      [Observation]
      public void should_add_concentration_in_feces_observer_for_all_models()
      {
         var observersRepo = IoC.Resolve<IFlatModelObserverRepository>();

         var observerName = CoreConstants.Observer.CONCENTRATION_IN_FECES;
         observersRepo.Contains(CoreConstants.Model.FourComp, observerName).ShouldBeTrue();
         observersRepo.Contains(CoreConstants.Model.TwoPores, observerName).ShouldBeTrue();
      }

      [Observation]
      public void should_extend_specific_binding_reaction_with_free_concentration()
      {
         var processRepo = IoC.Resolve<ISimulationActiveProcessRepository>();
         var specBinding = processRepo.ProcessFor("SpecificBinding");

         var aliases = from path in specBinding.Formula.ObjectPaths select path.Alias;
         aliases.ShouldContain("K_water");
      }

      [Observation]
      public void should_set_protein_ontogeny_factors_to_variable()
      {
         var flatMoleculesRepo = IoC.Resolve<IFlatMoleculeRepository>();
         var flatMoleculeMapper = IoC.Resolve<IFlatMoleculeToMoleculeBuilderMapper>();

         var protein = flatMoleculeMapper.MapFrom(flatMoleculesRepo.FindBy(QuantityType.Protein), new FormulaCache());

         protein.Parameter(CoreConstants.Parameters.ONTOGENY_FACTOR_GI).CanBeVaried.ShouldBeTrue();
         protein.Parameter(CoreConstants.Parameters.ONTOGENY_FACTOR).CanBeVaried.ShouldBeTrue();
      }
   }

   public class When_checking_the_changed_in_the_database_for_version_6_1 : concern_for_DatabaseUpdate
   {
      [Observation]
      public void should_update_the_default_value_of_n_cells_per_incubuation_to_1000()
      {
         var simulationActiveProcessRepository = IoC.Resolve<ISimulationActiveProcessRepository>();
         var process = simulationActiveProcessRepository.ProcessFor("HepatocytesRes");
         process.Parameter(CoreConstantsForSpecs.Parameter.NUMBER_OF_CELLS_PER_INCUBATION).Value.ShouldBeEqualTo(1000);
      }
   }

   public class When_checking_the_changed_in_the_database_for_version_6_0 : concern_for_DatabaseUpdate
   {
      private IEnumerable<MoleculeParameter> _allMoleculeParameters;

      public override void GlobalContext()
      {
         base.GlobalContext();
         var moleculeParams = IoC.Resolve<IMoleculeParameterRepository>();
         _allMoleculeParameters = moleculeParams.All();
      }

      private IDistributedParameter halfLifeLiverParameterFor(string moleculeName)
      {
         return (from moleculeParameter in _allMoleculeParameters
            where moleculeParameter.MoleculeName.Equals(moleculeName)
            where moleculeParameter.Parameter.Name.Equals(CoreConstants.Parameters.HALF_LIFE_LIVER)
            select moleculeParameter.Parameter as DistributedParameter).FirstOrDefault();
      }

      [Observation]
      public void standard_deviation_for_all_reference_concentration_parametes_should_be_zero()
      {
         foreach (var moleculeParameter in _allMoleculeParameters)
         {
            var param = moleculeParameter.Parameter;
            if (!param.Name.Equals(CoreConstants.Parameters.REFERENCE_CONCENTRATION))
               continue;

            var refConcParam = param as DistributedParameter;
            refConcParam.ShouldNotBeNull();

            refConcParam.DeviationParameter.Value.ShouldBeEqualTo(0);
         }
      }

      [Observation]
      public void standard_deviation_for_CYP1A2_should_match_predefined_value()
      {
         halfLifeLiverParameterFor("CYP1A2").DeviationParameter.Value.ShouldBeEqualTo(24 * 60);
      }

      [Observation]
      public void standard_deviation_for_CYP2C8_should_match_predefined_value()
      {
         halfLifeLiverParameterFor("CYP2C8").DeviationParameter.Value.ShouldBeEqualTo(19 * 60);
      }

      [Observation]
      public void standard_deviation_for_CYP2E1_should_match_predefined_value()
      {
         halfLifeLiverParameterFor("CYP2E1").DeviationParameter.Value.ShouldBeEqualTo(19 * 60);
      }
   }

   public class When_checking_the_changed_in_the_database_for_version_5_6 : concern_for_DatabaseUpdate
   {
      [Observation]
      public void should_have_set_the_saliva_icon_and_gall_bladder_icon()
      {
         var represenationInfoRepository = IoC.Resolve<IRepresentationInfoRepository>();
         represenationInfoRepository.InfoFor(RepresentationObjectType.CONTAINER, CoreConstants.Organ.Gallbladder)
            .IconName.ShouldBeEqualTo(CoreConstants.Organ.Gallbladder);

         represenationInfoRepository.InfoFor(RepresentationObjectType.CONTAINER, CoreConstants.Organ.Saliva)
            .IconName.ShouldBeEqualTo(CoreConstants.Organ.Saliva);
      }

      [Observation]
      public void should_have_set_the_display_name_and_description_for_auc_ratio_and_c_max_ratio()
      {
         var represenationInfoRepository = IoC.Resolve<IRepresentationInfoRepository>();
         represenationInfoRepository.InfoFor(RepresentationObjectType.PARAMETER, CoreConstants.PKAnalysis.AUCRatio)
            .Description.ShouldNotBeNull();

         represenationInfoRepository.InfoFor(RepresentationObjectType.PARAMETER, CoreConstants.PKAnalysis.C_maxRatio)
            .Description.ShouldNotBeNull();
      }

      [Observation]
      public void should_have_created_the_irreversible_inhibiton_process_with_two_parameters()
      {
         var compoundProcessRepository = IoC.Resolve<ICompoundProcessRepository>();
         var inhibitionProcess = compoundProcessRepository.All<InhibitionProcess>()
            .First(x => x.InteractionType == InteractionType.IrreversibleInhibition);

         inhibitionProcess.ShouldNotBeNull();
         inhibitionProcess.Parameter(CoreConstants.Parameters.KI).ShouldNotBeNull();
         inhibitionProcess.Parameter(CoreConstantsForSpecs.Parameter.KINACT).ShouldNotBeNull();
      }

      [Observation]
      public void should_set_halflife_intestine_to_23_h_for_all_predefined_molecules()
      {
         var moleculeParams = IoC.Resolve<IMoleculeParameterRepository>();
         foreach (var moleculeParameter in moleculeParams.All())
         {
            var param = moleculeParameter.Parameter;
            if (!param.Name.Equals(CoreConstants.Parameters.HALF_LIFE_INTESTINE))
               continue;

            param.Value.ShouldBeEqualTo(23 * 60, 1e-5);
         }
      }

      [Observation]
      public void should_set_halflife_intestine_to_23_h_for_non_predefined_molecules()
      {
         var moleculeBuilderFactory = IoC.Resolve<IMoleculeBuilderFactory>();

         var moleculeTypes = new[] {QuantityType.Enzyme, QuantityType.Transporter, QuantityType.OtherProtein};

         foreach (var moleculeType in moleculeTypes)
         {
            var someMolecule = moleculeBuilderFactory.Create(moleculeType, new FormulaCache());

            var halfLifeParam = someMolecule.Parameter(CoreConstants.Parameters.HALF_LIFE_INTESTINE);
            halfLifeParam.Value.ShouldBeEqualTo(23 * 60, 1e-5);
         }
      }
   }

   public class When_checking_the_changes_in_the_database_for_version_7_2_2 : concern_for_DatabaseUpdate
   {
      [Observation]
      public void should_set_characteristics_of_individual_group_mode_to_simple()
      {
         var groupsRepo = IoC.Resolve<IGroupRepository>();
         var group = groupsRepo.GroupByName(CoreConstants.Groups.INDIVIDUAL_CHARACTERISTICS);

         group.IsAdvanced.ShouldBeFalse();
      }

      [Observation]
      public void should_fix_reference_concentration_parameter_name_in_tab_molecule_parameters()
      {
         var moleculeParameterRepo = IoC.Resolve<IMoleculeParameterRepository>();
         var refConcParameters = moleculeParameterRepo.All().Where(mp => mp.Parameter.Name.StartsWith("Reference")).ToList();

         refConcParameters.Count.ShouldBeGreaterThan(0);
         refConcParameters.Each(mp=>mp.Parameter.Name.ShouldBeEqualTo(CoreConstants.Parameters.REFERENCE_CONCENTRATION));
      }
   }

   public class When_checking_the_changes_in_the_database_for_version_7_3 : concern_for_DatabaseUpdate
   {
      private IValueOriginRepository _valueOriginsRepository;
      private IParameterDistributionRepository _parameterDistributionRepository;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _valueOriginsRepository = IoC.Resolve<IValueOriginRepository>();
         _parameterDistributionRepository = IoC.Resolve<IParameterDistributionRepository>();
      }

      [Observation]
      public void value_origins_with_nonempty_description_should_not_be_undefined()
      {
         foreach (var valueOrigin in _valueOriginsRepository.All().Where(vo=>!string.IsNullOrEmpty(vo.Description)))
         {
            (valueOrigin.Source.Id==ValueOriginSourceId.Undefined).ShouldBeFalse(valueOrigin.Description);
         }
      }

      [Observation]
      public void should_properly_set_some_value_origins()
      {
         var agesWithFilledValueOrigins = new double[] {0, 1, 5, 10, 15, 30, 40, 50, 60, 70, 80, 90, 100};
         var icrpParams = _parameterDistributionRepository.All()
            .Where(p => p.Population.Equals(CoreConstants.Population.ICRP))
            .Where(p=>p.ParameterName.IsOneOf(
               CoreConstantsForSpecs.Parameter.VOLUME,
               CoreConstants.Parameters.BLOOD_FLOW)).ToList();

         icrpParams.Count.ShouldBeGreaterThanOrEqualTo(agesWithFilledValueOrigins.Length*2*2); //2 Genders * 2 parameters per age

         foreach (var param in icrpParams)
         {
            if(param.Age.IsOneOf(agesWithFilledValueOrigins))
               param.ValueOrigin.IsUndefined.ShouldBeFalse();
            else
               param.ValueOrigin.IsUndefined.ShouldBeTrue();
         } 
      }

      [Observation]
      public void should_retrieve_new_agp_ontogeny()
      {
         var ontogenyRepo = IoC.Resolve<IOntogenyRepository>();
         var ontogeny = new DatabaseOntogeny { Name = CoreConstants.Molecule.AGP, SpeciesName = CoreConstants.Species.HUMAN};

         var agpOntogenies = ontogenyRepo.AllValuesFor(ontogeny).OrderBy(onto=>onto.PostmenstrualAge).ToArray();
         agpOntogenies.Length.ShouldBeEqualTo(19);

         agpOntogenies[0].PostmenstrualAge.ShouldBeEqualTo(0.76, 1e-2);
         agpOntogenies[agpOntogenies.Length-1].PostmenstrualAge.ShouldBeEqualTo(90.45, 1e-2);

         foreach (var agpOntogeny in agpOntogenies)
         {
            agpOntogeny.Deviation.ShouldBeGreaterThan(1.44);
            agpOntogeny.Deviation.ShouldBeSmallerThan(1.89);
         }
      }

      [Observation]
      public void first_human_population_returned_from_the_database_should_be_ICRP()
      {
         var populationsRepo = IoC.Resolve<IPopulationRepository>();
         var populationsOrderedBySequence = populationsRepo.All().OrderBy(pop => pop.Sequence);
         var firstPopulation = populationsOrderedBySequence.First(pop=>pop.Species.Equals(CoreConstants.Species.HUMAN));

         firstPopulation.Name.ShouldBeEqualTo(CoreConstants.Population.ICRP);
      }

      [Observation]
      public void first_population_gender_returned_from_the_database_should_be_ICRP_male()
      {
         var populationGenderRepo = IoC.Resolve<IFlatPopulationGenderRepository>();
         var firstPopulationGender = populationGenderRepo.All().First();

         firstPopulationGender.Population.ShouldBeEqualTo(CoreConstants.Population.ICRP);
         firstPopulationGender.GenderName.ShouldBeEqualTo(CoreConstants.Gender.Male);
      }
   }
}