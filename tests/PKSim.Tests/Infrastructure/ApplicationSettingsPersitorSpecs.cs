using PKSim.Core;
using PKSim.Core.Services;
using PKSim.Infrastructure.Services;
using OSPSuite.BDDHelper;
using FakeItEasy;

namespace PKSim.Infrastructure
{
   public abstract class concern_for_ApplicationSettingsPersitor : ContextSpecification<IApplicationSettingsPersitor>
   {
      private IStringSerializer _serializationManager;
      private IApplicationSettings _defaultApplicationSettings;
      private IPKSimConfiguration _pkSimConfiguration;

      protected override void Context()
      {
         _serializationManager =A.Fake<IStringSerializer>();
         _defaultApplicationSettings = A.Fake<IApplicationSettings>();
         _pkSimConfiguration = A.Fake<IPKSimConfiguration>();
         sut = new ApplicationSettingsPersitor(_serializationManager,_defaultApplicationSettings, _pkSimConfiguration);
      }
   }
}	