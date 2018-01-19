using System.Collections.Generic;
using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Infrastructure.ORM.FlatObjects;
using PKSim.Infrastructure.ORM.Repositories;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_FlatProteinPropertyRepository : ContextForIntegration<IFlatMoleculeParameterRepository>
   {
   }

   public class When_resolving_all_protein_properties_definied_as_a_flat_table : concern_for_FlatProteinPropertyRepository
   {
      private IEnumerable<FlatMoleculeParameter> _result;

      protected override void Because()
      {
         _result = sut.All();
      }

      [Observation]
      public void should_retrieve_some_object_from_the_underlying_database()
      {
         _result.Count().ShouldBeGreaterThan(0);
         var firstElement = _result.ElementAt(0);
         firstElement.Deviation.ShouldNotBeNull();
         firstElement.ParameterName.ShouldNotBeNull();
         firstElement.MoleculeName.ShouldNotBeNull();
         firstElement.MoleculeName.ShouldNotBeNull();
         firstElement.DistributionType.ShouldNotBeNull();
      }
   }
}