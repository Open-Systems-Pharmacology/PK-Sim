using System.Collections.Generic;
using System.Linq;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public abstract class BasePKAnalysis : Container
   {
      public IReadOnlyList<IParameter> PKParameters(string pkParameterName)
      {
         return AllPKParameters.Where(x => x.IsNamed(pkParameterName)).ToList();
      }

      public IReadOnlyList<string> AllPKParameterNames
      {
         get { return AllPKParameters.Select(x => x.Name).Distinct().ToList(); }
      }

      public IReadOnlyList<IParameter> AllPKParameters => GetAllChildren<IParameter>();
   }
}