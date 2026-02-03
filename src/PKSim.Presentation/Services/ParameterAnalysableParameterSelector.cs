using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Services;

namespace PKSim.Presentation.Services
{
   public class ParameterAnalysableParameterSelector : AbstractParameterAnalysableParameterSelector
   {
      private readonly IUserSettings _userSettings;

      public ParameterAnalysableParameterSelector(IUserSettings userSettings)
      {
         _userSettings = userSettings;
      }

      public override bool CanUseParameter(IParameter parameter)
      {
         return parameter.CanBeVaried
                && !parameter.Info.ReadOnly
                && parameter.Visible
                && !ParameterIsTable(parameter)
                && !ParameterIsCategorial(parameter);
      }

      public override ParameterGroupingModeForParameterAnalyzable DefaultParameterSelectionMode => ParameterGroupingModesForParameterAnalyzable.ById(_userSettings.DefaultParameterGroupingModeForPIAndSA);
   }
}