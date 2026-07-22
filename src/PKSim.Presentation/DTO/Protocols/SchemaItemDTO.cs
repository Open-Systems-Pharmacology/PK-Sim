using OSPSuite.Presentation.DTO;
using PKSim.Core.Model;
using static OSPSuite.Core.Domain.Constants;

namespace PKSim.Presentation.DTO.Protocols
{
   public class SchemaItemDTO : DxValidatableDTO<ISchemaItem>
   {
      public SchemaItem SchemaItem { get; private set; }
      public SchemaDTO ParentSchema { get; set; }
      public IParameterDTO StartTimeParameter { get; set; }
      public IParameterDTO DoseParameter { get; set; }

      /// <summary>
      ///    Infusion duration of the administration. <c>null</c> for application types that are not an infusion.
      /// </summary>
      public IParameterDTO InfusionTimeParameter { get; set; }

      public SchemaItemDTO(SchemaItem schemaItem) : base(schemaItem)
      {
         SchemaItem = schemaItem;
      }

      public string IconName => ApplicationType.IconName;

      public bool NeedsFormulation => ApplicationType.NeedsFormulation;

      public bool IsUserDefined => SchemaItem.ApplicationType == ApplicationTypes.UserDefined;

      public bool IsEvent => SchemaItem.IsEvent;

      /// <summary>
      ///    Returns true when the placeholder column should be editable
      ///    (either for formulation keys or event keys).
      /// </summary>
      public bool NeedsPlaceholder => NeedsFormulation || IsEvent;

      public string DisplayName
      {
         get
         {
            if (NeedsFormulation)
               return CompositeNameFor(ApplicationType.DisplayName, FormulationKey);

            if (IsEvent)
               return EventKey;

            return ApplicationType.DisplayName;
         }
      }

      public ApplicationType ApplicationType
      {
         get => SchemaItem.ApplicationType;
         set { /*nothing to do*/}
      }

      public string FormulationKey
      {
         get => SchemaItem.FormulationKey;
         set { /*nothing to do*/}
      }

      public string EventKey
      {
         get => SchemaItem.EventKey;
         set { /*nothing to do*/}
      }

      /// <summary>
      ///    Unified placeholder key that returns <see cref="FormulationKey"/> for administration types
      ///    requiring a formulation, <see cref="EventKey"/> for event entries, or empty string otherwise.
      /// </summary>
      public string PlaceholderKey
      {
         get
         {
            if (IsEvent) return EventKey;
            if (NeedsFormulation) return FormulationKey;
            return string.Empty;
         }
         set { /*nothing to do - value changes handled by presenter*/ }
      }

      public double StartTime
      {
         get => StartTimeParameter.Value;
         set { /*nothing to do*/}
      }

      public double Dose
      {
         get => DoseParameter.Value;
         set { /*nothing to do*/}
      }

      public string TargetOrgan
      {
         get => SchemaItem.TargetOrgan;
         set { /*nothing to do*/}
      }

      public string TargetCompartment
      {
         get => SchemaItem.TargetCompartment;
         set { /*nothing to do*/}
      }
   }
}