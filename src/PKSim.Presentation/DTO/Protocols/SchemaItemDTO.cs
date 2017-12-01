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
         get { return SchemaItem.ApplicationType; }
         set { /*nothing to do*/}
      }

      public string FormulationKey
      {
         get { return SchemaItem.FormulationKey; }
         set { /*nothing to do*/}
      }

      public double StartTime
      {
         get { return StartTimeParameter.Value; }
         set { /*nothing to do*/}
      }

      public double Dose
      {
         get { return DoseParameter.Value; }
         set { /*nothing to do*/}
      }
   }
}