using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using System;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Mappers;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Core.Services;
using PKSim.Infrastructure;
using PKSim.Infrastructure.ProjectConverter;
using PKSim.Infrastructure.ProjectConverter.v13;
using PKSim.IntegrationTests;

namespace PKSim.ProjectConverter.v13
{
   public abstract class concern_for_Converter12To13 : ContextWithLoadedProject<Converter12To13>
   {
      protected IEntityPathResolver _entityPathResolver;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _entityPathResolver = OSPSuite.Utility.Container.IoC.Resolve<IEntityPathResolver>();
      }

      protected void LoadAll<TBuildingBlock>() where TBuildingBlock : class, IPKSimBuildingBlock =>
         All<TBuildingBlock>().Each(Load);

      //Looked up by name so the assertion does not depend on which segments a species defines
      protected void ShouldHaveTheNewLumenParameters(Individual individual)
      {
         var newLumenParameterNames = new[]
         {
            ConverterConstants.Parameters.BILE_SALT_CONCENTRATION,
            "Average fluid velocity",
            "Fluid kinematic viscosity",
            "Micellar diffusion coefficient in fasted state"
         };

         newLumenParameterNames.Each(parameterName =>
         {
            var parameters = individual.GetAllChildren<IParameter>(x => x.IsNamed(parameterName));
            parameters.Any().ShouldBeTrue($"'{parameterName}' was not added to individual '{individual.Name}'");
         });
      }

      //The lumen pH turned from constant into distribution in v13, proving the definitions came from the database
      protected void ShouldHaveADistributedLumenPh(Individual individual)
      {
         var lumen = individual.Organism.GetSingleChildByName<IContainer>(CoreConstants.Organ.LUMEN);
         lumen.ShouldNotBeNull();

         var lowerJejunumPh = lumen.GetSingleChildByName<IContainer>("LowerJejunum")?.Parameter(ConverterConstants.Parameters.PH);
         (lowerJejunumPh != null).ShouldBeTrue($"individual '{individual.Name}' has no lumen pH in the lower jejunum");
         (lowerJejunumPh is IDistributedParameter).ShouldBeTrue($"the lumen pH of '{individual.Name}' is not distributed");

         //Unedited pH must follow the new distribution, not stay pinned at its old constant value
         lowerJejunumPh.IsFixedValue.ShouldBeFalse($"the unedited lumen pH of '{individual.Name}' was pinned to a value");
      }

      protected void ShouldReferenceTheLumenSegmentVolumeCalculationMethod(Individual individual) =>
         individual.OriginData.CalculationMethodCache
            .Contains(ConverterConstants.CalculationMethod.LumenSegmentVolume)
            .ShouldBeTrue($"individual '{individual.Name}' does not reference the new calculation method");
   }

   public class When_converting_a_project_saved_before_the_new_oral_absorption_model : concern_for_Converter12To13
   {
      private List<Individual> _allIndividuals;
      private List<Population> _allPopulations;
      private List<PKSimEvent> _allEvents;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("SimplePop_73");

         LoadAll<Individual>();
         LoadAll<Population>();
         LoadAll<PKSimEvent>();

         _allIndividuals = All<Individual>();
         _allPopulations = All<Population>();
         _allEvents = All<PKSimEvent>();
      }

      [Observation]
      public void should_have_added_the_new_lumen_parameters_to_all_individuals()
      {
         _allIndividuals.Any().ShouldBeTrue();
         _allIndividuals.Each(ShouldHaveTheNewLumenParameters);
      }

      [Observation]
      public void should_have_added_the_new_calculation_method_to_all_individuals()
      {
         _allIndividuals.Each(ShouldReferenceTheLumenSegmentVolumeCalculationMethod);
      }

      [Observation]
      public void should_have_taken_over_the_new_definition_of_the_lumen_ph()
      {
         _allIndividuals.Each(ShouldHaveADistributedLumenPh);
      }

      [Observation]
      public void should_have_added_the_new_lumen_parameters_to_the_individual_of_every_population()
      {
         _allPopulations.Each(x => ShouldHaveTheNewLumenParameters(x.FirstIndividual));
      }

      [Observation]
      public void should_have_given_every_population_one_value_per_individual_for_the_new_varying_parameters()
      {
         _allPopulations.Each(population =>
         {
            var newVaryingParameters = population.FirstIndividual
               .GetAllChildren<IParameter>(x => x.IsChangedByCreateIndividual && x.IsNamed(ConverterConstants.Parameters.BILE_SALT_CONCENTRATION));

            newVaryingParameters.Any().ShouldBeTrue($"population '{population.Name}' has no new varying parameter to check");

            newVaryingParameters.Each(parameter =>
            {
               var path = _entityPathResolver.PathFor(parameter);
               population.IndividualValuesCache.Has(path).ShouldBeTrue($"'{path}' has no values in population '{population.Name}'");
               population.AllValuesFor(path).Count.ShouldBeEqualTo(population.NumberOfItems);
            });
         });
      }

      [Observation]
      public void should_no_longer_contain_the_obsolete_meal_stop_event_container()
      {
         _allEvents.Each(x =>
            x.GetAllChildren<IContainer>(c => c.IsNamed(ConverterConstants.Containers.MEAL_STOP_EVENT))
               .ShouldBeEmpty());
      }

      [Observation]
      public void should_not_have_created_any_duplicated_parameter()
      {
         _allIndividuals.Each(individual =>
         {
            var duplicates = individual.GetAllChildren<IContainer>()
               .Select(container => container.GetChildren<IParameter>().GroupBy(x => x.Name).FirstOrDefault(g => g.Count() > 1))
               .Where(x => x != null)
               .ToList();

            duplicates.ShouldBeEmpty();
         });
      }
   }

   public class When_converting_a_project_through_the_full_chain_of_conversions : concern_for_Converter12To13
   {
      private List<Individual> _allIndividuals;

      public override void GlobalContext()
      {
         base.GlobalContext();
         //A v11 project runs through every converter up to v13, so it also proves the chain arrives in a valid state
         LoadProject("v11_expression_profile");
         LoadAll<Individual>();
         _allIndividuals = All<Individual>();
      }

      [Observation]
      public void should_have_added_the_new_lumen_parameters_to_all_individuals()
      {
         _allIndividuals.Any().ShouldBeTrue();
         _allIndividuals.Each(ShouldHaveTheNewLumenParameters);
      }

      [Observation]
      public void should_have_added_the_new_calculation_method_to_all_individuals()
      {
         _allIndividuals.Each(ShouldReferenceTheLumenSegmentVolumeCalculationMethod);
      }
   }

   //A second BSA method in the same category crashes the mapping to a building block
   public class When_converting_a_human_individual_that_uses_the_du_bois_body_surface_area_method : ContextForIntegration<Converter12To13>
   {
      private Individual _individual;
      private const string _bsaCategory = "BSA";

      public override void GlobalContext()
      {
         base.GlobalContext();
         var calculationMethodRepository = OSPSuite.Utility.Container.IoC.Resolve<ICalculationMethodRepository>();
         _individual = OSPSuite.Utility.Container.IoC.Resolve<ICloner>()
            .Clone(OSPSuite.Utility.Container.IoC.Resolve<IDefaultIndividualRetriever>().DefaultHuman());

         var calculationMethodCache = _individual.OriginData.CalculationMethodCache;
         var currentBodySurfaceArea = calculationMethodCache.CalculationMethodFor(_bsaCategory);
         if (currentBodySurfaceArea != null)
            calculationMethodCache.RemoveCalculationMethod(currentBodySurfaceArea);
         calculationMethodCache.AddCalculationMethod(calculationMethodRepository.FindBy("Body surface area - Du Bois"));
      }

      protected override void Because()
      {
         sut.Convert(_individual, ProjectVersions.V12);
      }

      [Observation]
      public void should_not_have_added_a_second_body_surface_area_method()
      {
         _individual.OriginData.AllCalculationMethods().Count(x => x.Category == _bsaCategory).ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_still_be_mappable_to_an_individual_building_block()
      {
         var mapper = OSPSuite.Utility.Container.IoC.Resolve<IIndividualToIndividualBuildingBlockMapper>();
         mapper.MapFrom(_individual).ShouldNotBeNull();
      }
   }

   //No project fixture carries a meal event, so one is rebuilt from the database template and degraded to pre-v13 state
   public abstract class concern_for_Converter12To13_events : ContextForIntegration<Converter12To13>
   {
      protected PKSimEvent _oldEvent;
      protected string _resetEventName;
      protected ICloner _cloner;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _cloner = OSPSuite.Utility.Container.IoC.Resolve<ICloner>();
         var eventGroupRepository = OSPSuite.Utility.Container.IoC.Resolve<IEventGroupRepository>();

         //Any meal that gained the new reset events works
         var mealTemplate = eventGroupRepository.All()
            .First(x => x.GetAllChildren<IContainer>(isResetEvent).Any());

         _resetEventName = mealTemplate.GetAllChildren<IContainer>(isResetEvent).First().Name;
         _oldEvent = oldEventFrom(mealTemplate);
      }

      protected override void Because()
      {
         sut.Convert(_oldEvent, ProjectVersions.V12);
      }

      //Strips one reset event and adds back the obsolete stop event, like a meal saved before v13
      private PKSimEvent oldEventFrom(EventGroupBuilder template)
      {
         var oldEvent = new PKSimEvent {TemplateName = template.Name}.WithName(template.Name);
         template.Children.Each(x => oldEvent.Add(_cloner.Clone(x)));

         var resetEventToDrop = oldEvent.GetAllChildren<IContainer>(isResetEvent).First(x => x.IsNamed(_resetEventName));
         resetEventToDrop.ParentContainer.RemoveChild(resetEventToDrop);

         var subContainer = oldEvent.GetAllChildren<IContainer>()
            .First(x => x.IsNamed(CoreConstants.ContainerName.EventGroupMainSubContainer));
         subContainer.Add(new Container().WithName(ConverterConstants.Containers.MEAL_STOP_EVENT));

         return oldEvent;
      }

      private static bool isResetEvent(IContainer container) => container.Name.StartsWith("Reset ");
   }

   public class When_converting_a_meal_event_saved_before_the_new_oral_absorption_model : concern_for_Converter12To13_events
   {
      [Observation]
      public void should_have_added_back_the_reset_event_that_was_missing()
      {
         _oldEvent.GetAllChildren<IContainer>(x => x.IsNamed(_resetEventName)).Any().ShouldBeTrue();
      }

      [Observation]
      public void should_have_removed_the_obsolete_meal_stop_event()
      {
         _oldEvent.GetAllChildren<IContainer>(x => x.IsNamed(ConverterConstants.Containers.MEAL_STOP_EVENT)).ShouldBeEmpty();
      }
   }

   //End to end check against the real v12 project provided by the model owner
   public abstract class concern_for_Converter12To13_with_the_test_project : ContextWithLoadedProject<Converter12To13>
   {
      protected IEntityPathResolver _entityPathResolver;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _entityPathResolver = OSPSuite.Utility.Container.IoC.Resolve<IEntityPathResolver>();
         LoadProject("V12_TestProject");
      }

      //Every segment uses "pH" except the stomach, which uses "pH in fasted state"
      protected IParameter LumenPhParameterIn(ISimulationSubject simulationSubject, string segment)
      {
         var parameterName = segment == CoreConstants.Organ.STOMACH
            ? ConverterConstants.Parameters.PH_IN_FASTED_STATE
            : ConverterConstants.Parameters.PH;

         return simulationSubject.Individual.EntityAt<IParameter>(Constants.ORGANISM, CoreConstants.Organ.LUMEN, segment, parameterName);
      }

      //An individual run goes through the SimModel manager because the higher level engine swallows solver errors
      protected string RunErrorFor(Simulation simulation)
      {
         switch (simulation)
         {
            case IndividualSimulation individualSimulation:
               var modelCoreSimulation = OSPSuite.Utility.Container.IoC.Resolve<ISimulationToModelCoreSimulationMapper>().MapFrom(individualSimulation, shouldCloneModel: false);
               var runResults = OSPSuite.Utility.Container.IoC.Resolve<ISimModelManager>().RunSimulation(modelCoreSimulation);
               return runResults.Success ? null : errorFrom(runResults);

            case PopulationSimulation populationSimulation:
               try
               {
                  OSPSuite.Utility.Container.IoC.Resolve<ISimulationRunner>().RunSimulation(populationSimulation).Wait();
                  return populationSimulation.HasResults ? null : "produced no results";
               }
               catch (Exception e)
               {
                  return (e.InnerException ?? e).Message;
               }

            default:
               return null;
         }
      }

      private static string errorFrom(SimulationRunResults runResults) =>
         string.IsNullOrEmpty(runResults.Error)
            ? $"solver failed with {runResults.Warnings.Count()} warning(s)"
            : runResults.Error;

      //The run uses as many individuals as there are ids, so trimming the ids is enough to keep it fast
      protected static void ReduceToTwoIndividuals(PopulationSimulation simulation)
      {
         var individualIds = simulation.Population.IndividualValuesCache.IndividualIds;
         individualIds.Skip(2).ToList().Each(x => individualIds.Remove(x));
      }
   }

   public class When_converting_the_modified_individual_of_the_test_project : concern_for_Converter12To13_with_the_test_project
   {
      private Individual _modifiedIndividual;
      private readonly IDictionary<string, double> _expectedPhBySegment = new Dictionary<string, double>
      {
         ["Stomach"] = 2.2,
         ["Duodenum"] = 6.2,
         ["UpperJejunum"] = 6.4,
         ["LowerJejunum"] = 6.5,
         ["UpperIleum"] = 7.0,
         ["LowerIleum"] = 7.2,
         ["Caecum"] = 5.2,
         ["ColonAscendens"] = 5.4,
         ["ColonTransversum"] = 5.5,
         ["ColonDescendens"] = 6.2,
         ["ColonSigmoid"] = 6.3,
         ["Rectum"] = 6.4
      };

      public override void GlobalContext()
      {
         base.GlobalContext();
         _modifiedIndividual = FindByName<Individual>("11_Human_Modified_Healthy");
      }

      [Observation]
      public void should_have_kept_the_user_defined_lumen_ph_values()
      {
         var errors = new List<string>();
         foreach (var expected in _expectedPhBySegment)
         {
            var parameter = LumenPhParameterIn(_modifiedIndividual, expected.Key);
            if (parameter == null)
            {
               errors.Add($"{expected.Key}: parameter not found");
               continue;
            }

            if (!ValueComparer.AreValuesEqual(parameter.Value, expected.Value, 1e-6))
               errors.Add($"{expected.Key}: expected {expected.Value} but was {parameter.Value}");
         }

         Assert.IsTrue(errors.Count == 0, errors.ToString("\n"));
      }
   }

   public class When_converting_the_population_with_distributed_lumen_ph_of_the_test_project : concern_for_Converter12To13_with_the_test_project
   {
      private Population _distributedPopulation;

      //segment -> (mean, deviation) of the Normal distribution the user defined on the lumen pH
      private readonly IDictionary<string, (double mean, double deviation)> _expectedDistributionBySegment =
         new Dictionary<string, (double, double)>
         {
            ["Stomach"] = (2.2, 0.2),
            ["Duodenum"] = (6.5, 0.2),
            ["UpperJejunum"] = (6.5, 0.2),
            ["LowerJejunum"] = (6.5, 0.2),
            ["UpperIleum"] = (7.0, 0.5),
            ["LowerIleum"] = (7.0, 0.5),
            ["Caecum"] = (6.0, 0.5),
            ["ColonAscendens"] = (6.0, 0.5),
            ["ColonTransversum"] = (6.0, 0.5),
            ["ColonDescendens"] = (6.0, 0.5),
            ["ColonSigmoid"] = (6.0, 0.5),
            ["Rectum"] = (6.0, 0.5)
         };

      public override void GlobalContext()
      {
         base.GlobalContext();
         _distributedPopulation = FindByName<Population>("01_02_Human_Default_Healthy_distributed_pH");
      }

      [Observation]
      public void should_have_kept_the_user_defined_lumen_ph_distributions()
      {
         var errors = new List<string>();
         foreach (var expected in _expectedDistributionBySegment)
         {
            var parameter = LumenPhParameterIn(_distributedPopulation, expected.Key);
            var advancedParameter = parameter == null ? null : _distributedPopulation.AdvancedParameterFor(_entityPathResolver, parameter);
            if (advancedParameter == null)
            {
               errors.Add($"{expected.Key}: no advanced parameter");
               continue;
            }

            if (advancedParameter.DistributionType != DistributionType.Normal)
               errors.Add($"{expected.Key}: distribution is {advancedParameter.DistributionType}, expected Normal");

            var distributedParameter = advancedParameter.DistributedParameter;
            if (!ValueComparer.AreValuesEqual(distributedParameter.MeanParameter.Value, expected.Value.mean, 1e-6))
               errors.Add($"{expected.Key}: mean {distributedParameter.MeanParameter.Value}, expected {expected.Value.mean}");

            if (!ValueComparer.AreValuesEqual(distributedParameter.DeviationParameter.Value, expected.Value.deviation, 1e-6))
               errors.Add($"{expected.Key}: deviation {distributedParameter.DeviationParameter.Value}, expected {expected.Value.deviation}");
         }

         Assert.IsTrue(errors.Count == 0, errors.ToString("\n"));
      }
   }

   public class When_converting_the_compounds_of_the_test_project : concern_for_Converter12To13_with_the_test_project
   {
      private List<Compound> _allCompounds;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _allCompounds = All<Compound>();
         _allCompounds.Each(Load);
      }

      [Observation]
      public void should_have_added_the_new_compound_parameters()
      {
         //New in v13, needed so a value edited in a simulation can be committed back to the compound
         var newCompoundParameterNames = new[] {"Surface integration factor", "Diffusion layer thickness exponent"};

         _allCompounds.Any().ShouldBeTrue();
         _allCompounds.Each(compound =>
            newCompoundParameterNames.Each(name =>
               (compound.Parameter(name) != null).ShouldBeTrue($"'{name}' was not added to compound '{compound.Name}'")));
      }
   }

   //An incomplete conversion fails the model rebuild, a wrong value the run (issue 3640)
   public class When_reconfiguring_and_running_the_converted_simulations_of_the_test_project : concern_for_Converter12To13_with_the_test_project
   {
      private ISimulationModelCreator _simulationModelCreator;
      private List<Simulation> _allSimulations;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _simulationModelCreator = OSPSuite.Utility.Container.IoC.Resolve<ISimulationModelCreator>();
         _allSimulations = All<Simulation>();
         _allSimulations.Each(Load);
      }

      [Observation]
      public void should_rebuild_the_model_of_every_converted_simulation_and_run_it()
      {
         _allSimulations.Any().ShouldBeTrue();

         var errors = new List<string>();
         foreach (var simulation in _allSimulations)
         {
            try
            {
               if (simulation is PopulationSimulation populationSimulation)
                  ReduceToTwoIndividuals(populationSimulation);

               _simulationModelCreator.CreateModelFor(simulation);
               if (simulation.Model?.Root == null)
               {
                  errors.Add($"{simulation.Name}: no model was built");
                  continue;
               }

               var error = RunErrorFor(simulation);
               if (error != null)
                  errors.Add($"{simulation.Name}: {error}");
            }
            catch (Exception e)
            {
               errors.Add($"{simulation.Name}: {(e.InnerException ?? e).Message}");
            }
         }

         Assert.IsTrue(errors.Count == 0, errors.ToString("\n"));
      }
   }

   //Uses the converted standalone building blocks, not the copies stored inside the simulations
   public class When_creating_and_running_new_simulations_from_the_converted_building_blocks_of_the_test_project : concern_for_Converter12To13_with_the_test_project
   {
      private IEventMappingFactory _eventMappingFactory;
      private List<Simulation> _allSimulations;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _eventMappingFactory = OSPSuite.Utility.Container.IoC.Resolve<IEventMappingFactory>();
         _allSimulations = All<Simulation>();
         _allSimulations.Each(Load);
      }

      [Observation]
      public void should_create_and_run_a_new_simulation_for_the_configuration_of_every_simulation_of_the_project()
      {
         _allSimulations.Any().ShouldBeTrue();

         var errors = new List<string>();
         foreach (var storedSimulation in _allSimulations)
         {
            try
            {
               var newSimulation = createSimulationWithTheConfigurationOf(storedSimulation);
               if (newSimulation.Model?.Root == null)
               {
                  errors.Add($"{storedSimulation.Name}: no model was built");
                  continue;
               }

               var error = RunErrorFor(newSimulation);
               if (error != null)
                  errors.Add($"{storedSimulation.Name}: {error}");
            }
            catch (Exception e)
            {
               errors.Add($"{storedSimulation.Name}: {(e.InnerException ?? e).Message}");
            }
         }

         Assert.IsTrue(errors.Count == 0, errors.ToString("\n"));
      }

      private Simulation createSimulationWithTheConfigurationOf(Simulation storedSimulation)
      {
         var subject = templateSubjectOf(storedSimulation);
         var compounds = storedSimulation.CompoundPropertiesList.Select(x => FindByName<Compound>(x.Compound.Name)).ToList();
         var protocols = storedSimulation.CompoundPropertiesList.Select(templateProtocolOf).ToList();
         var formulation = templateFormulationOf(storedSimulation);
         var modelProperties = DomainFactoryForSpecs.CreateModelPropertiesFor(subject, storedSimulation.ModelConfiguration.ModelName);

         var newSimulation = DomainFactoryForSpecs.CreateModelLessSimulationWith(subject, compounds, protocols, modelProperties, storedSimulation.AllowAging, formulation);
         addTemplateEventsOf(storedSimulation, newSimulation);

         if (newSimulation is PopulationSimulation populationSimulation)
            ReduceToTwoIndividuals(populationSimulation);

         DomainFactoryForSpecs.AddModelToSimulation(newSimulation);
         return newSimulation;
      }

      private ISimulationSubject templateSubjectOf(Simulation storedSimulation)
      {
         var usedSubject = storedSimulation.UsedBuildingBlocksInSimulation<Population>().FirstOrDefault()
                           ?? storedSimulation.UsedBuildingBlocksInSimulation<Individual>().First();

         var subject = _project.BuildingBlockById<ISimulationSubject>(usedSubject.TemplateId);
         Load(subject);
         return subject;
      }

      private Protocol templateProtocolOf(CompoundProperties compoundProperties)
      {
         var protocol = compoundProperties.ProtocolProperties.Protocol;
         return protocol == null ? null : FindByName<Protocol>(protocol.Name);
      }

      private Formulation templateFormulationOf(Simulation storedSimulation)
      {
         var formulationMapping = storedSimulation.CompoundPropertiesList
            .SelectMany(x => x.ProtocolProperties.FormulationMappings)
            .FirstOrDefault();

         if (formulationMapping == null)
            return null;

         var formulation = _project.BuildingBlockById<Formulation>(formulationMapping.TemplateFormulationId);
         Load(formulation);
         return formulation;
      }

      //The event resolves through a used building block keyed by the template id
      private void addTemplateEventsOf(Simulation storedSimulation, Simulation newSimulation)
      {
         foreach (var eventMapping in storedSimulation.EventProperties.EventMappings)
         {
            var templateEvent = _project.BuildingBlockById<PKSimEvent>(eventMapping.TemplateEventId);
            Load(templateEvent);

            var newEventMapping = _eventMappingFactory.Create(templateEvent);
            newEventMapping.StartTime.Value = eventMapping.StartTime.Value;

            newSimulation.AddUsedBuildingBlock(new UsedBuildingBlock(templateEvent.Id, PKSimBuildingBlockType.Event) {BuildingBlock = templateEvent});
            newSimulation.EventProperties.AddEventMapping(newEventMapping);
         }
      }
   }

   public class When_creating_a_simulation_using_the_building_blocks_of_the_simple_project_730_project : ContextWithLoadedProject<Converter12To13>
   {
      private Individual _individual;
      private Compound _compound;
      private Protocol _protocol;
      private Simulation _simulation;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("SimpleProject_730");
         _individual = FindByName<Individual>("Ind");
         _compound = FindByName<Compound>("Caffeine");
         _protocol = FindByName<Protocol>("IV");
      }

      protected override void Context()
      {
         base.Context();
         _simulation = DomainFactoryForSpecs.CreateSimulationWith(_individual, _compound, _protocol);
      }

      [Observation]
      public void should_be_able_to_create_a_simulation_with_the_converted_building_blocks()
      {
         _simulation.Model.ShouldNotBeNull();
      }
   }
}
