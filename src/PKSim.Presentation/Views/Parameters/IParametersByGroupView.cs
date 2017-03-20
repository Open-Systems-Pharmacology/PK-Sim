using System.Collections.Generic;
using PKSim.Presentation.DTO.Parameters;
using PKSim.Presentation.Presenters.Parameters;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Parameters
{
   public interface IParametersByGroupView : IView<IParametersByGroupPresenter>, IBatchUpdatable
   {
      int OptimalHeight { get; }
      void BindTo(IEnumerable<ParameterDTO> allParameters);
      void RefreshData();
      bool GroupingVisible { set; }
      bool FavoritesVisible { set; }
      bool HeaderVisible { set; }
   }
}