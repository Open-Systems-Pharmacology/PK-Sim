using System.Threading.Tasks;
using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using OSPSuite.Utility.Exceptions;
using PKSim.Core;
using PKSim.Core.Services;
using PKSim.Infrastructure.Services;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_VersionChecker : ContextSpecification<IVersionChecker>
   {
      protected override void Context()
      {
         sut = new VersionChecker();
      }
   }

   public class When_checking_if_a_new_version_is_available_when_a_newer_version_is_available : concern_for_VersionChecker
   {
      private bool _result;

      protected override void Context()
      {
         base.Context();
         sut.ProductName = CoreConstants.PRODUCT_NAME;
         sut.VersionFileUrl = DomainHelperForSpecs.DataFilePathFor("Version1.1.1.xml");
         sut.CurrentVersion = "1.0.0";
      }

      protected override void Because()
      {
         _result = sut.NewVersionIsAvailable();
      }

      [Observation]
      public void should_returns_that_the_software_has_indeed_a_newer_version()
      {
         _result.ShouldBeTrue();
      }

      [Observation]
      public void should_returns_the_newer_version()
      {
         sut.LatestVersion.Version.ShouldBeEqualTo("1.1.1");
      }
   }

   public class When_checking_if_a_new_version_is_available_when_no_newer_version_is_available : concern_for_VersionChecker
   {
      private bool _result;

      protected override void Context()
      {
         base.Context();
         sut.ProductName = CoreConstants.PRODUCT_NAME;
         sut.VersionFileUrl = DomainHelperForSpecs.DataFilePathFor("Version1.1.1.xml");
         sut.CurrentVersion = "1.1.1";
      }

      protected override void Because()
      {
         _result = sut.NewVersionIsAvailable();
      }

      [Observation]
      public void should_returns_that_the_software_has_indeed_no_newer_version()
      {
         _result.ShouldBeFalse();
      }
   }

   public class When_checking_if_a_new_version_is_available_from_a_corrupt_file : concern_for_VersionChecker
   {
      protected override void Context()
      {
         base.Context();
         sut.ProductName = CoreConstants.PRODUCT_NAME;
         sut.VersionFileUrl = DomainHelperForSpecs.DataFilePathFor("Version2.1.1corrupt.xml");
         sut.CurrentVersion = "1.1.1";
      }

      [Observation]
      public void should_returns_that_the_software_has_indeed_a_no_newer_version()
      {
         The.Action(() => sut.NewVersionIsAvailable()).ShouldThrowAn<OSPSuiteException>();
      }
   }

   public class When_checking_if_a_new_version_is_available_when_a_newer_version_is_available_asynchronously : concern_for_VersionChecker
   {
      
      protected override void Context()
      {
         base.Context();
         sut.ProductName = CoreConstants.PRODUCT_NAME;
         sut.VersionFileUrl = DomainHelperForSpecs.DataFilePathFor("Version2.1.1.xml");
         sut.CurrentVersion = "1.1.1";
      }

      [Observation]
      public async Task should_notify_a_new_version_event_containing_the_version()
      {
         await sut.NewVersionIsAvailableAsync();
         sut.LatestVersion.Version.ShouldBeEqualTo("2.1.1");
      }
   }

// This test is only used to debug connection to version file on Github
//   public class When_checking_the_real_version_for_debug_purpose : concern_for_VersionChecker
//   {
//      protected override void Context()
//      {
//         base.Context();
//         sut.ProductName = CoreConstants.ProductName;
//         sut.VersionFileUrl = CoreConstants.VersionFileUrl;
//         sut.CurrentVersion = "1.1.1";
//      }
//
//      [Observation]
//      public void should_returns_that_the_software_has_indeed_no_newer_version()
//      {
//         var result = sut.NewVersionIsAvailable();
//         result.ShouldBeFalse();
//      }
//   }
}