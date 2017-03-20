using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Parameters
{
   public class ParameterGroupsPresenterSettings : DefaultPresentationSettings
   {
      private const string PARAMETER_GROUPING_MODE_ID_SETTING = "ParameterGroupingModeId";
      private const string SELECTED_NODE_ID = "SelectedNodeId";

      public ParameterGroupingModeId DefaultParameterGroupingModeId { get; set; }

      public virtual ParameterGroupingMode ParameterGroupingMode
      {
         get { return groupingModeFrom(GetSetting(PARAMETER_GROUPING_MODE_ID_SETTING, DefaultParameterGroupingModeId)); }
         set { SetSetting(PARAMETER_GROUPING_MODE_ID_SETTING, value.Id); }
      }

      public virtual string SelectedNodeId
      {
         get { return GetSetting<string>(SELECTED_NODE_ID); }
         set { SetSetting(SELECTED_NODE_ID, value); }
      }

      private ParameterGroupingMode groupingModeFrom(ParameterGroupingModeId parameterGroupingModeId)
      {
         return ParameterGroupingModes.ById(parameterGroupingModeId);
      }
   }
}