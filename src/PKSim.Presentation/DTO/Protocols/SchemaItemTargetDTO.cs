using System;
using PKSim.Assets;

namespace PKSim.Presentation.DTO.Protocols
{
   public class SchemaItemTargetDTO
   {
      private readonly Func<SchemaItemDTO, string> _targetFunc;
      public string Name { get; }
      public SchemaItemDTO SchemaItemDTO { get; }
      public bool IsOrgan { get; }

      public SchemaItemTargetDTO(string name, SchemaItemDTO schemaItemDTO, Func<SchemaItemDTO, string> targetFunc)
      {
         _targetFunc = targetFunc;
         Name = name;
         SchemaItemDTO = schemaItemDTO;
         IsOrgan = string.Equals(Name, PKSimConstants.UI.TargetOrgan);
      }

      public string Target
      {
         get => _targetFunc(SchemaItemDTO);
         set
         {
            /* nothing to do */
         }
      }
   }
}