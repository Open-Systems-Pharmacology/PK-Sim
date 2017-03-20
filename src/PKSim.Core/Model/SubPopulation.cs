using System.Collections.Generic;
using OSPSuite.Utility.Collections;

namespace PKSim.Core.Model
{
   public class SubPopulation
   {
      private readonly ICache<string, ParameterValueVersion> _allParameterValueVersions;

      public SubPopulation()
      {
         _allParameterValueVersions = new Cache<string, ParameterValueVersion>(pv => pv.Name);
      }

      public IEnumerable<ParameterValueVersion> ParameterValueVersions
      {
         get { return _allParameterValueVersions; }
      }

      public bool Contains(string parameterValueVersion)
      {
         return _allParameterValueVersions.Contains(parameterValueVersion);
      }

      public void AddParameterValueVersion(ParameterValueVersion parameterValueVersion)
      {
         if (parameterValueVersion == null) return;
         _allParameterValueVersions.Add(parameterValueVersion);
      }
   }
}