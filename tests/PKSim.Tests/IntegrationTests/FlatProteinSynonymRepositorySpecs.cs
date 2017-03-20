using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_FlatProteinSynonymRepository : ContextForIntegration<IFlatProteinSynonymRepository>
   {
   }

   public class When_retrieving_the_synonym_for_a_given_protein : concern_for_FlatProteinSynonymRepository
   {
      private IReadOnlyList<string> _results;

      protected override void Because()
      {
         _results = sut.AllSynonymsFor("SLC47A1");
      }

      [Observation]
      public void should_returns_the_synonyms_defined_for_the_given_protein()
      {
         _results.ShouldOnlyContain("MATE1");
      }
   }
}