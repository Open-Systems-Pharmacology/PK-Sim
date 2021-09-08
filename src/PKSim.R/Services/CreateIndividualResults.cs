using PKSim.R.Domain;

namespace PKSim.R.Services
{
   public class CreateIndividualResults
   {
      public ParameterValueWithUnit[] DistributedParameters { get;  }
      public ParameterValueWithUnit[] DerivedParameters { get;  }
      public int Seed { get;  }

      public CreateIndividualResults(ParameterValueWithUnit[] distributedParameters, ParameterValueWithUnit[] derivedParameters, int seed)
      {
         DistributedParameters = distributedParameters;
         DerivedParameters = derivedParameters;
         Seed = seed;
      }
   }
}