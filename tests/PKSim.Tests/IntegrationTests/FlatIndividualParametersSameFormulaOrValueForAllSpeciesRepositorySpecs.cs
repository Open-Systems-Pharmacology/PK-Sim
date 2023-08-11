using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_FlatIndividualParametersSameFormulaOrValueForAllSpeciesRepository : ContextForIntegration<IFlatIndividualParametersSameFormulaOrValueForAllSpeciesRepository>
   {
   }

   public class When_resolving_all_parameters_with_the_same_formula_or_value_for_all_species_as_a_flat_table : concern_for_FlatIndividualParametersSameFormulaOrValueForAllSpeciesRepository
   {
      private IEnumerable<FlatIndividualParametersSameFormulaOrValueForAllSpecies> _result;

      protected override void Because()
      {
         _result = sut.All();
      }

      [Observation]
      public void should_retrieve_some_object_from_the_underlying_database()
      {
         _result.Count().ShouldBeGreaterThan(0);
      }
   }
}
