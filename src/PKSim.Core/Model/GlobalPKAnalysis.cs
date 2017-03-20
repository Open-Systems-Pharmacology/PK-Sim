using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Model
{
   public class GlobalPKAnalysis : BasePKAnalysis
   {
      public IReadOnlyList<string> CompoundNames
      {
         get { return GetChildren<IContainer>().Select(x => x.Name).ToList(); }
      }

      public IParameter PKParameter(string compoundName, string pkParameterName)
      {
         return this.EntityAt<IParameter>(compoundName, pkParameterName);
      }

      public bool HasParameter(string compoundName, string pkParameterName)
      {
         var parameter = PKParameter(compoundName, pkParameterName);
         return !(parameter == null || parameter.IsAnImplementationOf<NullParameter>());
      }
   }
}