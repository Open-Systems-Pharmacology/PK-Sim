using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Utils;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Repositories;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_ContainerParametersNotCommonForAllSpeciesRepository : ContextForIntegration<IContainerParametersNotCommonForAllSpeciesRepository>
   {
   }

   public class When_retrieving_parameters_not_common_for_all_species_from_the_repository : concern_for_ContainerParametersNotCommonForAllSpeciesRepository
   {
      private IEnumerable<(string ContainerPath, string ParameterName, int SpeciesCount)> _result;

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

   public class When_testing_if_a_parameter_is_common_for_all_species : concern_for_ContainerParametersNotCommonForAllSpeciesRepository
   {
      [Observation]
      public void age_parameter_should_be_defined_not_for_all_species()
      {
         sut.UsedForAllSpecies("Organism","Age").ShouldBeFalse();
      }

      [Observation]
      public void weight_parameter_should_be_defined_for_all_species()
      {
         sut.UsedForAllSpecies("Organism", "Weight").ShouldBeTrue();
      }
   }
}