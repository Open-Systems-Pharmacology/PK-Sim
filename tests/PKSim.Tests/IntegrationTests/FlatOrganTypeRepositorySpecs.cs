using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Repositories;
using static PKSim.Core.CoreConstants.Organ;

namespace PKSim.IntegrationTests
{
    public abstract class concern_for_FlatOrganTypeRepository : ContextForIntegration<IOrganTypeRepository>
    {
       
    }

    public class When_retrieving_the_organ_type_for_all_known_organs : concern_for_FlatOrganTypeRepository
    {
        [Observation]
        public void should_return_the_expected_organ_type()
        {
            sut.OrganTypeFor(ARTERIAL_BLOOD).ShouldBeEqualTo(OrganType.VascularSystem);
            sut.OrganTypeFor(BONE).ShouldBeEqualTo(OrganType.TissueOrgansNotInGiTract);
            sut.OrganTypeFor(BRAIN).ShouldBeEqualTo(OrganType.TissueOrgansNotInGiTract);
            sut.OrganTypeFor(Dummy).ShouldBeEqualTo(OrganType.Unknown);
            sut.OrganTypeFor(ENDOGENOUS_IGG).ShouldBeEqualTo(OrganType.Unknown);
            sut.OrganTypeFor(FAT).ShouldBeEqualTo(OrganType.TissueOrgansNotInGiTract);
            sut.OrganTypeFor(GALLBLADDER).ShouldBeEqualTo(OrganType.Unknown);
            sut.OrganTypeFor(GONADS).ShouldBeEqualTo(OrganType.TissueOrgansNotInGiTract);
            sut.OrganTypeFor(HEART).ShouldBeEqualTo(OrganType.TissueOrgansNotInGiTract);
            sut.OrganTypeFor(KIDNEY).ShouldBeEqualTo(OrganType.TissueOrgansNotInGiTract);
            sut.OrganTypeFor(LARGE_INTESTINE).ShouldBeEqualTo(OrganType.GiTractOrgans);
            sut.OrganTypeFor(LUMEN).ShouldBeEqualTo(OrganType.Lumen);
            sut.OrganTypeFor(LUNG).ShouldBeEqualTo(OrganType.TissueOrgansNotInGiTract);
            sut.OrganTypeFor(LIVER).ShouldBeEqualTo(OrganType.TissueOrgansNotInGiTract);
            sut.OrganTypeFor(MUSCLE).ShouldBeEqualTo(OrganType.TissueOrgansNotInGiTract);
            sut.OrganTypeFor(PANCREAS).ShouldBeEqualTo(OrganType.TissueOrgansNotInGiTract);
            sut.OrganTypeFor(PORTAL_VEIN).ShouldBeEqualTo(OrganType.VascularSystem);
            sut.OrganTypeFor(SALIVA).ShouldBeEqualTo(OrganType.Unknown);
            sut.OrganTypeFor(SKIN).ShouldBeEqualTo(OrganType.TissueOrgansNotInGiTract);
            sut.OrganTypeFor(SMALL_INTESTINE).ShouldBeEqualTo(OrganType.GiTractOrgans);
            sut.OrganTypeFor(SPLEEN).ShouldBeEqualTo(OrganType.TissueOrgansNotInGiTract);
            sut.OrganTypeFor(STOMACH).ShouldBeEqualTo(OrganType.GiTractOrgans);
            sut.OrganTypeFor(VENOUS_BLOOD).ShouldBeEqualTo(OrganType.VascularSystem);
            sut.OrganTypeFor(PERIPHERAL_VENOUS_BLOOD).ShouldBeEqualTo(OrganType.Unknown);
        }
    }
}	