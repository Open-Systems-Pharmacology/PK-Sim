using System.Collections.Generic;
using OSPSuite.Presentation.Views;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.Presenters.Compounds;

namespace PKSim.Presentation.Views.Compounds;

public interface IOverwriteParameterSetsView : IView<IOverwriteParameterSetsPresenter>
{
   void BindTo(IReadOnlyList<OverwriteParameterSetDTO> overwriteParameterSets);
}