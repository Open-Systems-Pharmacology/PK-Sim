using System.Linq;
using System.Threading;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure;

namespace PKSim.IntegrationTests
{
   public class When_importing_a_population_file_containing_an_advanced_parameters : ContextForIntegration<IImportPopulationFactory>
   {
      private Individual _individual;
      private ImportPopulation _population;

      public override void GlobalContext()
      {
         base.GlobalContext();
         _individual = DomainFactoryForSpecs.CreateStandardIndividual();
         sut = IoC.Resolve<IImportPopulationFactory>();
         _population = sut.CreateFor(new[] {DomainHelperForSpecs.PopulationFilePathFor("pop_3_advanced_parameter")}, _individual, new CancellationToken()).Result;
      }

      protected override void Context()
      {
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
}