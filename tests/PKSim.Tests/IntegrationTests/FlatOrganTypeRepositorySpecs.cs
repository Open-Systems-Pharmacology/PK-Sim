using FakeItEasy;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Repositories;
using PKSim.Infrastructure.ORM.Repositories;

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
            sut.OrganTypeFor(CoreConstants.Organ.ArterialBlood).ShouldBeEqualTo(OrganType.VascularSystem);
            sut.OrganTypeFor(CoreConstants.Organ.Bone).ShouldBeEqualTo(OrganType.TissueOrgansNotInGiTract);
            sut.OrganTypeFor(CoreConstants.Organ.Brain).ShouldBeEqualTo(OrganType.TissueOrgansNotInGiTract);
            sut.OrganTypeFor(CoreConstants.Organ.Dummy).ShouldBeEqualTo(OrganType.Unknown);
            sut.OrganTypeFor(CoreConstants.Organ.EndogenousIgG).ShouldBeEqualTo(OrganType.Unknown);
            sut.OrganTypeFor(CoreConstants.Organ.Fat).ShouldBeEqualTo(OrganType.TissueOrgansNotInGiTract);
            sut.OrganTypeFor(CoreConstants.Organ.Gallbladder).ShouldBeEqualTo(OrganType.Unknown);
            sut.OrganTypeFor(CoreConstants.Organ.Gonads).ShouldBeEqualTo(OrganType.TissueOrgansNotInGiTract);
            sut.OrganTypeFor(CoreConstants.Organ.Heart).ShouldBeEqualTo(OrganType.TissueOrgansNotInGiTract);
            sut.OrganTypeFor(CoreConstants.Organ.Kidney).ShouldBeEqualTo(OrganType.TissueOrgansNotInGiTract);
            sut.OrganTypeFor(CoreConstants.Organ.LargeIntestine).ShouldBeEqualTo(OrganType.GiTractOrgans);
            sut.OrganTypeFor(CoreConstants.Organ.Lumen).ShouldBeEqualTo(OrganType.Lumen);
            sut.OrganTypeFor(CoreConstants.Organ.Lung).ShouldBeEqualTo(OrganType.TissueOrgansNotInGiTract);
            sut.OrganTypeFor(CoreConstants.Organ.Liver).ShouldBeEqualTo(OrganType.TissueOrgansNotInGiTract);
            sut.OrganTypeFor(CoreConstants.Organ.Muscle).ShouldBeEqualTo(OrganType.TissueOrgansNotInGiTract);
            sut.OrganTypeFor(CoreConstants.Organ.Pancreas).ShouldBeEqualTo(OrganType.TissueOrgansNotInGiTract);
            sut.OrganTypeFor(CoreConstants.Organ.PortalVein).ShouldBeEqualTo(OrganType.VascularSystem);
            sut.OrganTypeFor(CoreConstants.Organ.Saliva).ShouldBeEqualTo(OrganType.Unknown);
            sut.OrganTypeFor(CoreConstants.Organ.Skin).ShouldBeEqualTo(OrganType.TissueOrgansNotInGiTract);
            sut.OrganTypeFor(CoreConstants.Organ.SmallIntestine).ShouldBeEqualTo(OrganType.GiTractOrgans);
            sut.OrganTypeFor(CoreConstants.Organ.Spleen).ShouldBeEqualTo(OrganType.TissueOrgansNotInGiTract);
            sut.OrganTypeFor(CoreConstants.Organ.Stomach).ShouldBeEqualTo(OrganType.GiTractOrgans);
            sut.OrganTypeFor(CoreConstants.Organ.VenousBlood).ShouldBeEqualTo(OrganType.VascularSystem);
            sut.OrganTypeFor(CoreConstants.Organ.PeripheralVenousBlood).ShouldBeEqualTo(OrganType.Unknown);
        }
    }
}	