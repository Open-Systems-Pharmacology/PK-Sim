using OSPSuite.Utility.Container;
using OSPSuite.Core.Domain.Services;

namespace PKSim.IntegrationTests
{
   public class When_cloning_a_parameter_identification : duplicated_parameter_identification_comparison_test
   {
      protected ICloneManagerForModel _cloneManagerForModel;
      protected override void GlobalBecause()
      {
         _cloneManagerForModel = IoC.Resolve<ICloneManagerForModel>();
         _duplicatedParameterIdentification = _cloneManagerForModel.Clone(_parameterIdentification);
      }
   }
}