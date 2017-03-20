using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Core;
using PKSim.Infrastructure.ORM.Mappers;
using PKSim.Infrastructure.ORM.Repositories;
using PKSim.Infrastructure.ProjectConverter;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Extensions;
using OSPSuite.Utility.Extensions;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_DatabaseUpdate : ContextForIntegration<IModelDatabase>
   {
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
         inhibitionProcess.Parameter(CoreConstants.Parameter.KI).ShouldNotBeNull();
         inhibitionProcess.Parameter(CoreConstantsForSpecs.Parameter.KINACT).ShouldNotBeNull();
      }

      [Observation]
      public void should_set_halflife_intestine_to_23_h_for_all_predefined_molecules()
      {
         var moleculeParams = IoC.Resolve<IMoleculeParameterRepository>();
         foreach (var moleculeParameter in moleculeParams.All())
         {
            var param = moleculeParameter.Parameter;
            if (!param.Name.Equals(CoreConstants.Parameter.HALF_LIFE_INTESTINE))
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

            var halfLifeParam = someMolecule.Parameter(CoreConstants.Parameter.HALF_LIFE_INTESTINE);
            halfLifeParam.Value.ShouldBeEqualTo(23 * 60, 1e-5);
         }
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
            where moleculeParameter.Parameter.Name.Equals(CoreConstants.Parameter.HALF_LIFE_LIVER)
            select moleculeParameter.Parameter as DistributedParameter).FirstOrDefault();
      }

      [Observation]
      public void standard_deviation_for_all_reference_concentration_parametes_should_be_zero()
      {
         foreach (var moleculeParameter in _allMoleculeParameters)
         {
            var param = moleculeParameter.Parameter;
            if (!param.Name.Equals(CoreConstants.Parameter.REFERENCE_CONCENTRATION))
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

   public class When_checking_the_changes_in_the_database_for_version_6_3 : concern_for_DatabaseUpdate
   {
      [Observation]
      public void should_set_plasma_protein_scale_factor_variable_in_population()
      {
         var paramValueRepo = IoC.Resolve<IParameterValueRepository>();
         var plasmaProteinScaleFactor = paramValueRepo.All().First(p => p.ParameterName.Equals(CoreConstants.Parameter.PLASMA_PROTEIN_SCALE_FACTOR));

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

         protein.Parameter(CoreConstants.Parameter.ONTOGENY_FACTOR_GI).CanBeVaried.ShouldBeTrue();
         protein.Parameter(CoreConstants.Parameter.ONTOGENY_FACTOR).CanBeVaried.ShouldBeTrue();
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

   public class When_checking_the_changes_in_the_database_for_version_6_4_2 : concern_for_DatabaseUpdate
   {
      [Observation]
      public void should_have_set_the_building_block_type_of_ontogeny_factor_and_onteogeny_factor_gi_in_protein_to_individual()
      {
         var paramValueRepo = IoC.Resolve<IParameterRateRepository>();

         var ontogenyFactory = paramValueRepo.All().First(p => p.ParameterName.Equals(CoreConstants.Parameter.ONTOGENY_FACTOR) && p.ContainerName == "PROTEIN");
         ontogenyFactory.BuildingBlockType.ShouldBeEqualTo(PKSimBuildingBlockType.Individual);

         var ontogenyFactoryGI = paramValueRepo.All().First(p => p.ParameterName.Equals(CoreConstants.Parameter.ONTOGENY_FACTOR_GI) && p.ContainerName == "PROTEIN");
         ontogenyFactoryGI.BuildingBlockType.ShouldBeEqualTo(PKSimBuildingBlockType.Individual);
      }
   }

   public class When_checking_the_changes_in_the_database_for_version_7_1_0 : concern_for_DatabaseUpdate
   {
      [Observation]
      public void should_remove_species_cattle_and_cat()
      {
         var speciesRepo = IoC.Resolve<ISpeciesRepository>();
         var speciesNames = from species in speciesRepo.All() select species.Name;

         speciesNames.ShouldNotContain("Cat", "Cattle");
      }

      [Observation]
      public void should_remove_intestinal_permeability_MPTC()
      {
         var calcMethodsRepo = IoC.Resolve<ICalculationMethodRepository>();
         var calcMethodNames = from cm in calcMethodsRepo.All() select cm.Name;

         calcMethodNames.ShouldNotContain("IntestinalPermeability_CACO_MPTC");
      }

      [Observation]
      public void should_use_updated_rates_for_lymph_and_fluid_recirculation_flow()
      {
         var parameterRateRepo    = IoC.Resolve<IParameterRateRepository>();
         var rateFormulaRepo = IoC.Resolve<IRateFormulaRepository>();

         var lymphFlowRates = from pr in parameterRateRepo.All().Where(isChangedLymphFlowParameter)
                              select rateFormulaRepo.FormulaFor(new RateKey(pr.CalculationMethod, pr.Rate));
         lymphFlowRates.Each(r => r.Contains("f_lymph").ShouldBeTrue());

         var fluidRecircFlowRates = from pr in parameterRateRepo.All().Where(isChangedFluidRecircFlowParameter)
                                    select rateFormulaRepo.FormulaFor(new RateKey(pr.CalculationMethod, pr.Rate));
         fluidRecircFlowRates.Each(r => r.Contains("f_Jiso").ShouldBeTrue());

      }

      private bool isChangedFluidRecircFlowParameter(ParameterRateMetaData parameterRateMetaData)
      {
         return isChangedFlowParameter(parameterRateMetaData,
                                       CoreConstants.Parameter.RECIRCULATION_FLOW,
                                       CoreConstants.Parameter.RECIRCULATION_FLOW_INCL_MUCOSA);
      }
      private bool isChangedLymphFlowParameter(ParameterRateMetaData parameterRateMetaData)
      {
         return isChangedFlowParameter(parameterRateMetaData, 
                                       CoreConstants.Parameter.LYMPH_FLOW, 
                                       CoreConstants.Parameter.LYMPH_FLOW_INCL_MUCOSA);
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
         var rateObjectPathsRepo = IoC.Resolve<IRateObjectPathsRepository>();

         checkRate(rateObjectPathsRepo, "FcRn binding drug in endosomal space");
         checkRate(rateObjectPathsRepo, "FcRn binding drug in interstitial");
         checkRate(rateObjectPathsRepo, "FcRn binding drug in plasma");
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
         var rateFormulaRepo = IoC.Resolve<IRateFormulaRepository>();
         var rates = rateFormulaRepo.All().Where(r => r.Rate.IsOneOf("PARAM_Meal_alpha", "PARAM_Meal_beta"));

         rates.Each(r=>r.Formula.Contains("Variability_Factor").ShouldBeTrue());
      } 

   }
}