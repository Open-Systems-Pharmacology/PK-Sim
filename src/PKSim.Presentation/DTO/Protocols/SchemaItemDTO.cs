using PKSim.Core;
using PKSim.Core.Model;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Protocols
{
   public class SchemaItemDTO : DxValidatableDTO<ISchemaItem>
   {
      public SchemaItem SchemaItem { get; private set; }
      public SchemaDTO ParentSchema { get; set; }
      public IParameterDTO StartTimeParameter { get; set; }
      public IParameterDTO DoseParameter { get; set; }

      public SchemaItemDTO(SchemaItem schemaItem) : base(schemaItem)
      {
         SchemaItem = schemaItem;
      }

      public string IconName => ApplicationType.IconName;

      public bool NeedsFormulation => ApplicationType.NeedsFormulation;

      public bool IsUserDefined => SchemaItem.ApplicationType == ApplicationTypes.UserDefined;

      public string DisplayName
      {
         get
         {
            if (NeedsFormulation)
               return CoreConstants.CompositeNameFor(ApplicationType.DisplayName, FormulationKey);

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