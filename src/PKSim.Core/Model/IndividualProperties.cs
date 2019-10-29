using System.Collections.Generic;
using OSPSuite.Core.Domain.Populations;
using OSPSuite.Utility.Collections;

namespace PKSim.Core.Model
{
   public class IndividualProperties
   {
      private readonly Cache<string, ParameterValue> _parameterValues = new Cache<string, ParameterValue>(x => x.ParameterPath);
      public IndividualCovariates Covariates { get; set; }

      public IndividualProperties()
      {
         Covariates = new IndividualCovariates();
      }

      public virtual IEnumerable<ParameterValue> ParameterValues => _parameterValues;

      public virtual void AddParameterValue(ParameterValue parameterValue)
      {
         _parameterValues.Add(parameterValue);
      }

      public virtual ParameterValue ParameterValue(string parameterPath)
      {
         return _parameterValues[parameterPath];
      }
   }
}