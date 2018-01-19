using System.Collections.Generic;
using OSPSuite.Presentation.Views;
using PKSim.Presentation.DTO.Compounds;

namespace PKSim.Presentation.Views.Compounds
{
   public interface ICompoundTypeGroupView : ICompoundParameterGroupView
   {
      void BindTo (IEnumerable<TypePKaDTO> allTypePKas);
      bool ShowFavorites {  set; }
      void AddValueOriginView(IView view);
   }
}