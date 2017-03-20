namespace PKSim.Core.Model
{
   public class FormulationMapping
   {
      /// <summary>
      ///    Key used in the protocol for which a mapping is required
      /// </summary>
      public string FormulationKey { get; set; }

      /// <summary>
      ///    Id of template formulation in project used for mapping
      /// </summary>
      public string TemplateFormulationId { get; set; }

      /// <summary>
      /// Actual formulation reference used in the mapping (this does not be to be serialized. It's only used temporarily to create the simulation).
      /// The Formulation referenced here is either the template building block or the simulation building block changed in the simulation
      /// </summary>
      public Formulation Formulation { get; set; }
   }
}