using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Core.Services
{
   public class ParameterAnalysableParameterSelector : AbstractParameterAnalysableParameterSelector
   {
      public override bool CanUseParameter(IParameter parameter)
      {
         return parameter.CanBeVaried
                && !parameter.Info.ReadOnly
                && parameter.Visible
                && !ParameterIsTable(parameter)
                && !ParameterIsCategorial(parameter);
      }

      public override ParameterGroupingMode DefaultParameterSelectionMode => ParameterGroupingModes.Simple;
   }
}