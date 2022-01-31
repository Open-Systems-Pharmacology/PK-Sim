using static OSPSuite.UI.UIConstants.Size;

namespace PKSim.UI
{
   public static class UIConstants
   {
      public static class Size
      {
         public static readonly int BUILDING_BLOCK_SELECTION_SIZE = 32;
         public static readonly int PARAMETER_WIDTH = 110;
         public static readonly int DATA_SOURCE_WIDTH = 180;

         public static readonly int SIMULATION_SETTINGS_WIDTH = ScaleForScreenDPI(750);
         public static readonly int SIMULATION_SETTINGS_HEIGHT = ScaleForScreenDPI(650);

         public static readonly int INDIVIDUAL_VIEW_WIDTH = ScaleForScreenDPI(880);
         public static readonly int INDIVIDUAL_VIEW_HEIGHT = ScaleForScreenDPI(780);

         public static readonly int CONFIGURE_SIMULATION_VIEW_HEIGHT = ScaleForScreenDPI(708);
         public static readonly int CREATE_SIMULATION_VIEW_HEIGHT = ScaleForScreenDPI(750);
         public static readonly int SIMULATION_VIEW_WIDTH = ScaleForScreenDPI(700);

         public static readonly int EXPRESSION_QUERY_VIEW_HEIGHT = ScaleForScreenDPI(900);
         public static readonly int EXPRESSION_QUERY_VIEW_WIDTH = ScaleForScreenDPI(1100);

         public static readonly int FORMULATION_VIEW_WIDTH = ScaleForScreenDPI(650);
         public static readonly int FORMULATION_VIEW_HEIGHT = ScaleForScreenDPI(760);

         public static readonly int OBSERVER_VIEW_HEIGHT = ScaleForScreenDPI(760);
         public static readonly int OBSERVER_VIEW_WIDTH = ScaleForScreenDPI(650);

         public static readonly int PROTOCOL_VIEW_WIDTH = ScaleForScreenDPI(700);
         public static readonly int PROTOCOL_VIEW_HEIGHT = ScaleForScreenDPI(760);

         public static readonly int EXPRESSION_PROFILE_VIEW_HEIGHT = ScaleForScreenDPI(900);
         public static readonly int EXPRESSION_PROFILE_VIEW_WIDTH = ScaleForScreenDPI(760);

         public static readonly int COMPOUND_VIEW_HEIGHT = ScaleForScreenDPI(900);
         public static readonly int COMPOUND_VIEW_WIDTH = ScaleForScreenDPI(760);
      }
   }
}