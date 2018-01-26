using System.Collections.Generic;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Compounds
{
   public interface IMolWeightGroupView : ICompoundParameterGroupView
   {
      void BindTo(IEnumerable<IParameterDTO> parameters);
      void SetHalogensView(IView view);
      void AddValueOriginView(IView view);
      void RefreshData();
   }
}