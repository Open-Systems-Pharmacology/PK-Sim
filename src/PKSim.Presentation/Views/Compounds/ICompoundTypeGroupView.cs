using System.Collections.Generic;

using PKSim.Presentation.DTO.Compounds;

namespace PKSim.Presentation.Views.Compounds
{
   public interface ICompoundTypeGroupView : ICompoundParameterGroupView
   {
      void BindTo (IEnumerable<TypePKaDTO> allTypePKas);
      bool ShowFavorites {  set; }
   }
}