using System.Linq;
using OSPSuite.Core.Domain;
using static PKSim.Core.CoreConstants.Parameters;

namespace PKSim.Core.Extensions;

public static class WithNameExtensions
{
   public static bool HasGlobalExpressionName(this IWithName withName)
   {
      return withName != null && AllGlobalRelExpParameters.Contains(withName.Name);
   }

   public static bool HasExpressionName(this IWithName withName)
   {
      return withName != null && (HasGlobalExpressionName(withName) || withName.IsNamed(Constants.Parameters.REL_EXP));
   }
}