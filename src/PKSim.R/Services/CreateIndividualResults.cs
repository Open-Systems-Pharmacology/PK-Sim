
using OSPSuite.Core.Domain.Populations;

namespace PKSim.R.Services
{
   public class CreateIndividualResults
   {
      public ParameterValue[] DistributedParameters { get;  }
      public ParameterValue[] DerivedParameters { get;  }

      public CreateIndividualResults(ParameterValue[] distributedParameters, ParameterValue[] derivedParameters)
      {
         DistributedParameters = distributedParameters;
         DerivedParameters = derivedParameters;
      }
   }
}