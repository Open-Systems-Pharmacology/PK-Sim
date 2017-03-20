using System.Linq;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Infrastructure.ProjectConverter.v6_0;
using PKSim.IntegrationTests;

namespace PKSim.ProjectConverter.V6_0
{
   public class When_converting_the_Individual_CYP3A4_Query_553S_project : ContextWithLoadedProject<Converter601To602>
   {
      private Individual _individual;

      public override void GlobalContext()
      {
         base.GlobalContext();
         LoadProject("Individual_CYP3A4_Query_553");
         _individual = First<Individual>();
      }

      [Observation]
      public void should_have_replaced_the_reference_to_liver_with_pericentral_and_periportal_in_the_database_query()
      {
         var enzyme = _individual.AllMolecules().First();
         enzyme.QueryConfiguration.Contains(CoreConstants.Compartment.Pericentral).ShouldBeTrue();
         enzyme.QueryConfiguration.Contains(CoreConstants.Compartment.Periportal).ShouldBeTrue();
      }
   }
}