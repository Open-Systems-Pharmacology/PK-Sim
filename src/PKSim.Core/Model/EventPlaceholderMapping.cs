namespace PKSim.Core.Model
{
   public class EventPlaceholderMapping
   {
      /// <summary>
      ///    Key used in the protocol for which a mapping is required (e.g. "Event")
      /// </summary>
      public string EventKey { get; set; }

      /// <summary>
      ///    Id of template event in project used for mapping
      /// </summary>
      public string TemplateEventId { get; set; }

      /// <summary>
      ///    Actual event reference used in the mapping (not serialized, used temporarily to create the simulation).
      ///    The Event referenced here is either the template building block or the simulation building block changed in the simulation.
      /// </summary>
      public PKSimEvent Event { get; set; }
   }
}
