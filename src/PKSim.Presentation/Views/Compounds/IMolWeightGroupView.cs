using System.Collections.Generic;

using PKSim.Presentation.DTO.Parameters;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Compounds
{
   public interface IMolWeightGroupView : ICompoundParameterGroupView
   {
      void BindTo(IEnumerable<IParameterDTO> parameters);
      void SetHalogensView(IView view);
      void RefreshData();
   }
}