using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;

namespace PKSim.Presentation.Presenters.Parameters
{
   public class ParameterGroupsPresenterSettings : DefaultPresentationSettings
   {
      private const string PARAMETER_GROUPING_MODE_ID_SETTING = "ParameterGroupingModeId";
      private const string SELECTED_NODE_ID = "SelectedNodeId";

      public virtual string SelectedNodeId
      {
         get => GetSetting<string>(SELECTED_NODE_ID);
         set => SetSetting(SELECTED_NODE_ID, value);
      }

      private ParameterGroupingMode groupingModeFrom(ParameterGroupingModeId parameterGroupingModeId)
      {
         return ParameterGroupingModes.ById(parameterGroupingModeId);
      }
   }
}