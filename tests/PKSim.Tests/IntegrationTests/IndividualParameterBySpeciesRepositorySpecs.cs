using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Container;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_IndividualParametersNotCommonForAllSpeciesRepository : ContextForIntegration<IIndividualParameterBySpeciesRepository>
   {
   }

   public class When_retrieving_parameters_not_common_for_all_species_from_the_repository : concern_for_IndividualParametersNotCommonForAllSpeciesRepository
   {
      private IEnumerable<IndividualParameterBySpecies> _result;

      protected override void Because()
      {
         _result = sut.All();
      }

      [Observation]
      public void should_return_at_least_one_parameter()
      {
         _result.Count().ShouldBeGreaterThan(0);
      }

      [Observation]
      public void all_parameter_paths_should_start_with_organism_or_neighborhoods()
      {
         //because we deal here with individual parameters only: only parameters with path "Organism|..." or "Neighborhoods|..." may appear in the list
         _result.Each(p=>
         {
            var containerPath = p.ContainerPath;
            (containerPath.StartsWith("Organism")|| containerPath.StartsWith("Neighborhoods")).ShouldBeTrue($"{p.ContainerPath}|{p.ParameterName}");
         });
      }
   }

   public class When_testing_if_a_parameter_is_common_for_all_species : concern_for_IndividualParametersNotCommonForAllSpeciesRepository
   {
      private IFlatContainerRepository _flatContainerRepository;

      protected override void Context()
      {
         base.Context();
         _flatContainerRepository = IoC.Resolve<IFlatContainerRepository>();
      }

      private ParameterMetaData parameterMetaDataFor(string containerPath, string parameterName)
      {
         var containerId = _flatContainerRepository.ContainerFrom(containerPath).Id;

         return new ParameterMetaData()
         {
            ContainerId = containerId,
            ParameterName = parameterName
         };
      }

      protected static IEnumerable<object[]> TestData_UsageForAllSpecies()
      {
         yield return new object[]
         {
            "Organism", "Age", false //available only for human
         };

         yield return new object[]
         {
            "Organism", "Weight", true
         };

         yield return new object[]
         {
            "Organism|Bone", "Volume", false //distributed parameter for human, value parameter for others
         };

         yield return new object[]
         {
            "Organism|Lumen|Duodenum", "Length", true //TODO I think it's wrong and should be false (human: formula, others: value)
         };

         yield return new object[]
         {
            "Organism|Bone", "Specific blood flow rate", false //distributed parameter for human, value parameter for others
         };
      }

      [TestCaseSource(nameof(TestData_UsageForAllSpecies))]
      [Observation]
      public void should_return_correct_usage_for_all_species(string containerPath, string parameterName, bool expectedUsageForAllSpecies)
      {
         sut.UsedForAllSpecies(containerPath,parameterName).ShouldBeEqualTo(expectedUsageForAllSpecies);
         sut.UsedForAllSpecies($"{containerPath}|{parameterName}").ShouldBeEqualTo(expectedUsageForAllSpecies);
         sut.UsedForAllSpecies(parameterMetaDataFor(containerPath,parameterName)).ShouldBeEqualTo(expectedUsageForAllSpecies);
      }
   }
}