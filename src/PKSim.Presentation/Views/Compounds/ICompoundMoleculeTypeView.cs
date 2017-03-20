
using PKSim.Presentation.DTO.Compounds;

namespace PKSim.Presentation.Views.Compounds
{
   public interface ICompoundMoleculeTypeView : ICompoundParameterGroupView
   {
      void BindTo(IsSmallMoleculeDTO isSmallMolecule);
   }
}