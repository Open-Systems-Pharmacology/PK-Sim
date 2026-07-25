using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Mappers;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
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

      /// <summary>
      ///    Every parameter the new absorption model added to the lumen of an individual. They are looked up by name
      ///    rather than by path so the assertion does not depend on which segments a given species defines.
      /// </summary>
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

      /// <summary>
      ///    The lumen pH of the lower intestine is a constant up to v12 and a distribution from v13 on. It is the clearest
      ///    evidence that the definitions were taken over from the database rather than only the missing parameters.
      /// </summary>
      protected void ShouldHaveADistributedLumenPh(Individual individual)
      {
         var lumen = individual.Organism.GetSingleChildByName<IContainer>(CoreConstants.Organ.LUMEN);
         lumen.ShouldNotBeNull();

         var lowerJejunumPh = lumen.GetSingleChildByName<IContainer>("LowerJejunum")?.Parameter(ConverterConstants.Parameters.PH);
         (lowerJejunumPh != null).ShouldBeTrue($"individual '{individual.Name}' has no lumen pH in the lower jejunum");
         (lowerJejunumPh is IDistributedParameter).ShouldBeTrue($"the lumen pH of '{individual.Name}' is not distributed");

         //The default individuals in the fixture never edited the pH, so it must follow its new distribution rather than
         //stay pinned at the value it had as a constant before the conversion
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

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("SimplePop_73");

         LoadAll<Individual>();
         LoadAll<Population>();

         _allIndividuals = All<Individual>();
         _allPopulations = All<Population>();
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

   /// <summary>
   ///    End to end check of the v12 → v13 conversion against a real project provided for the new oral absorption model.
   ///    The project carries several individuals and populations (healthy, diseased and animal) and a set of oral
   ///    particle-dissolution simulations with all meals plus the gallbladder and urinary bladder emptying events, and one
   ///    large-molecule IV simulation. The acceptance criteria come from the model owner.
   /// </summary>
   public abstract class concern_for_Converter12To13_with_the_test_project : ContextWithLoadedProject<Converter12To13>
   {
      protected IEntityPathResolver _entityPathResolver;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _entityPathResolver = OSPSuite.Utility.Container.IoC.Resolve<IEntityPathResolver>();
         LoadProject("V12_TestProject");
      }

      //The lumen pH parameters are the only user facing parameters redefined by the new model, so they are the ones the
      //conversion has to carry over unchanged. Every segment uses "pH" except the stomach, which uses "pH in fasted state".
      protected IParameter LumenPhParameterIn(ISimulationSubject simulationSubject, string segment)
      {
         var parameterName = segment == CoreConstants.Organ.STOMACH
            ? ConverterConstants.Parameters.PH_IN_FASTED_STATE
            : ConverterConstants.Parameters.PH;

         return simulationSubject.Individual.EntityAt<IParameter>(Constants.ORGANISM, CoreConstants.Organ.LUMEN, segment, parameterName);
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

   public class When_running_the_converted_simulations_of_the_test_project : concern_for_Converter12To13_with_the_test_project
   {
      private ISimulationRunner _simulationRunner;
      private ISimulationToModelCoreSimulationMapper _modelCoreSimulationMapper;
      private List<Simulation> _allSimulations;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _simulationRunner = OSPSuite.Utility.Container.IoC.Resolve<ISimulationRunner>();
         _modelCoreSimulationMapper = OSPSuite.Utility.Container.IoC.Resolve<ISimulationToModelCoreSimulationMapper>();
         _allSimulations = All<Simulation>();
         _allSimulations.Each(Load);
      }

      [Observation]
      public void should_run_every_converted_simulation_without_error()
      {
         _allSimulations.Any().ShouldBeTrue();

         var errors = new List<string>();
         foreach (var simulation in _allSimulations)
         {
            var error = runErrorFor(simulation);
            if (error != null)
               errors.Add($"{simulation.Name}: {error}");
         }

         Assert.IsTrue(errors.Count == 0, errors.ToString("\n"));
      }

      //Returns the error a run reported, or null when it succeeded. An individual simulation is run through the SimModel
      //manager so the returned SimulationRunResults can be inspected directly: a solver failure sets Success to false and
      //carries the error, which the higher level engine swallows. A population run raises an exception on failure, so it
      //is run through the runner and the exception is captured.
      private string runErrorFor(Simulation simulation)
      {
         switch (simulation)
         {
            case IndividualSimulation individualSimulation:
               var modelCoreSimulation = _modelCoreSimulationMapper.MapFrom(individualSimulation, shouldCloneModel: false);
               var runResults = OSPSuite.Utility.Container.IoC.Resolve<ISimModelManager>().RunSimulation(modelCoreSimulation);
               return runResults.Success ? null : errorFrom(runResults);

            case PopulationSimulation populationSimulation:
               try
               {
                  _simulationRunner.RunSimulation(populationSimulation).Wait();
                  return populationSimulation.HasResults ? null : "produced no results";
               }
               catch (System.Exception e)
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
   }
}
