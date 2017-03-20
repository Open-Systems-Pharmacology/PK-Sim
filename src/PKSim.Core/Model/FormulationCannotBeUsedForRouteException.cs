using PKSim.Assets;

namespace PKSim.Core.Model
{
   public class FormulationCannotBeUsedForRouteException : PKSimException
   {
      public FormulationCannotBeUsedForRouteException(Formulation formulation, string applicationRoute) : base(PKSimConstants.Error.FormulationCannotBeUsedWithRoute(formulation.Name, applicationRoute))
      {
      }
   }
}