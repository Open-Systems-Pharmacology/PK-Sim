using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Core.Domain;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;

namespace PKSim.IntegrationTests
{
   public abstract class concern_for_OrganTypeRepository : ContextForIntegration<IOrganTypeRepository>
   {
   }

   public class When_retrieving_the_organ_type_for_a_container_by_name : concern_for_OrganTypeRepository
   {
      [Observation]
      public void should_return_the_expected_organ_type()
      {
         sut.OrganTypeFor(CoreConstants.Organ.BONE).ShouldBeEqualTo(OrganType.TissueOrgansNotInGiTract);
      }
   }

   public class When_retrieving_the_organ_type_for_a_container : concern_for_OrganTypeRepository
   {
      [Observation]
      public void should_return_the_expected_organ_type()
      {
         sut.OrganTypeFor(new Container {Name = CoreConstants.Organ.BONE}).ShouldBeEqualTo(OrganType.TissueOrgansNotInGiTract);
      }
   }

   public class When_retrieving_the_organ_type_for_an_unknown_organ : concern_for_OrganTypeRepository
   {
      [Observation]
      public void should_return_the_expected_organ_type()
      {
         sut.OrganTypeFor("BLA BLA").ShouldBeEqualTo(OrganType.Unknown);
      }
   }
}