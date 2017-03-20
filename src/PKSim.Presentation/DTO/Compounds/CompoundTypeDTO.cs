using System.Collections.Generic;

namespace PKSim.Presentation.DTO.Compounds
{
   public class CompoundTypeDTO 
   {
      private readonly IList<TypePKaDTO> _allTypePKas;

      public CompoundTypeDTO()
      {
         _allTypePKas = new List<TypePKaDTO>();
      }

      public IEnumerable<TypePKaDTO> AllTypePKas
      {
         get { return _allTypePKas; }
      }

      public void AddTypePKa(TypePKaDTO typePkaDTO)
      {
         _allTypePKas.Add(typePkaDTO);
      }
   }
}