using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
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
}
