using System.Linq;
using System.Threading;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure;

namespace PKSim.IntegrationTests
{
   public class concern_for_ImportPopulationFactory : ContextForIntegration<IImportPopulationFactory>
   {
      protected Individual _individual;
      protected ImportPopulation _population;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _individual = DomainFactoryForSpecs.CreateStandardIndividual();
         sut = IoC.Resolve<IImportPopulationFactory>();
      }

      protected override void Context()
      {
      }
   }

   public class When_importing_a_population_file_containing_an_advanced_parameters : concern_for_ImportPopulationFactory
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         _population = sut.CreateFor(new[] {DomainHelperForSpecs.PopulationFilePathFor("pop_3_advanced_parameter")}, _individual, new CancellationToken()).Result;
      }

      [Observation]
      public void should_have_created_one_advanced_parameter()
      {
         _population.AdvancedParameters.Count().ShouldBeEqualTo(1);
      }

      [Observation]
      public void should_have_imported_3_individuals()
      {
         _population.NumberOfItems.ShouldBeEqualTo(3);
      }
   }

   public class When_importing_a_population_file_containing_two_column_with_the_same_entity_path : concern_for_ImportPopulationFactory
   {
      public override void GlobalContext()
      {
         base.GlobalContext();
         _population = sut.CreateFor(new[] { DomainHelperForSpecs.PopulationFilePathFor("pop_3_duplicated_column") }, _individual, new CancellationToken()).Result;
      }

      [Observation]
      public void should_show_an_error_to_the_user_explaning_that_a_column_duplicate_and_specify_which_one_is_duplicated()
      {
         var populationfIle = _population.Settings.AllFiles.ElementAt(0);
         populationfIle.Status.ShouldBeEqualTo(NotificationType.Error);
         populationfIle.Log.ElementAt(0).Contains("Organism|ArterialBlood|Volume [l]").ShouldBeTrue();
      }
   }
}