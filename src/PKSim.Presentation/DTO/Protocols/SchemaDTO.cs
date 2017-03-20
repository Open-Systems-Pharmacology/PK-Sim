using System.ComponentModel;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;
using OSPSuite.Core.Domain.UnitSystem;
using OSPSuite.Presentation.DTO;

namespace PKSim.Presentation.DTO.Protocols
{
   public class SchemaDTO : ValidatableDTO<Schema>
   {
      private readonly IDimension _timeDimension;
      public virtual Schema Schema { get; }
      public virtual IParameterDTO NumberOfRepetitionsParameter { get; set; }
      public virtual IParameterDTO StartTimeParameter { get; set; }
      public virtual IParameterDTO TimeBetweenRepetitionsParameter { get; set; }

      public SchemaDTO(Schema schema, IDimension timeDimension) : base(schema)
      {
         _timeDimension = timeDimension;
         Schema = schema;
      }

      public virtual void AddSchemaItem(SchemaItemDTO schemaItemDTO)
      {
         SchemaItems.Add(schemaItemDTO);
         schemaItemDTO.ParentSchema = this;
      }

      public virtual void RemoveSchemaItem(SchemaItemDTO schemaItemDTOToDelete)
      {
         SchemaItems.Remove(schemaItemDTOToDelete);
      }

      public virtual BindingList<SchemaItemDTO> SchemaItems { get; } = new BindingList<SchemaItemDTO>();

      public virtual double StartTime
      {
         get { return StartTimeParameter.Value; }
         set
         {
/*nothing to do*/
         }
      }

      public virtual double TimeBetweenRepetitions
      {
         get { return TimeBetweenRepetitionsParameter.Value; }
         set
         {
/*nothing to do*/
         }
      }

      public virtual uint NumberOfRepetitions
      {
         get { return NumberOfRepetitionsParameter.Value.ConvertedTo<uint>(); }
         set
         {
/*nothing to do*/
         }
      }

      public virtual double EndTime
      {
         get
         {
            //value in kernel unit
            return _timeDimension.BaseUnitValueToUnitValue(TimeBetweenRepetitionsParameter.DisplayUnit, Schema.EndTime);
         }
      }
   }
}