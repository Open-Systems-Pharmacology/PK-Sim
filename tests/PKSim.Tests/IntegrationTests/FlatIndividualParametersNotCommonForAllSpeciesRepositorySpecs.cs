using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_FlatIndividualParametersNotCommonForAllSpeciesRepository : ContextForIntegration<IFlatIndividualParametersNotCommonForAllSpeciesRepository>
   {
   }

   public class
      When_resolving_all_parameters_not_common_for_all_species_as_a_flat_table :
      concern_for_FlatIndividualParametersNotCommonForAllSpeciesRepository
   {
      private IEnumerable<FlatIndividualParametersNotCommonForAllSpecies> _result;
      private IEnumerable<FlatIndividualParametersNotCommonForAllSpecies> _organismParameters;

      protected override void Because()
      {
         _result = sut.All();
         _organismParameters = _result.Where(p => p.ContainerName.Equals("Organism"));
      }

      [Observation]
      public void should_retrieve_some_object_from_the_underlying_database()
      {
         _result.Count().ShouldBeGreaterThan(0);
      }

      [Observation]
      [Ignore("TODO s. https://github.com/Open-Systems-Pharmacology/MoBi/issues/1401")]
      public void should_contain_mean_height()
      {
         _organismParameters.Count(p => p.ParameterName.Equals(CoreConstants.Parameters.MEAN_HEIGHT))
            .ShouldBeEqualTo(1);
      }
   }
}