using System.Collections.Generic;
using System.Linq;
using FluentNHibernate.Utils;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_IndividualParametersNotCommonForAllSpeciesRepository : ContextForIntegration<IIndividualParametersNotCommonForAllSpeciesRepository>
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

      [Observation]
      public void age_parameter_should_be_defined_not_for_all_species_when_queried_by_full_path()
      {
         sut.UsedForAllSpecies("Organism|Age").ShouldBeFalse();
      }

      [Observation]
      public void weight_parameter_should_be_defined_for_all_species_when_queried_by_full_path()
      {
         sut.UsedForAllSpecies("Organism|Weight").ShouldBeTrue();
      }
   }
}