using PKSim.R.Domain;

namespace PKSim.R.Services
{
   public class CreateIndividualResults
   {
      public ParameterValueWithUnit[] DistributedParameters { get;  }
      public ParameterValueWithUnit[] DerivedParameters { get;  }

      public CreateIndividualResults(ParameterValueWithUnit[] distributedParameters, ParameterValueWithUnit[] derivedParameters)
      {
         DistributedParameters = distributedParameters;
         DerivedParameters = derivedParameters;
      }
   }
}