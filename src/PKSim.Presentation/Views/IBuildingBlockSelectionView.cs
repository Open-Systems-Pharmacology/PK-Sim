using OSPSuite.Presentation.Views;
using PKSim.Presentation.DTO;
using PKSim.Presentation.Presenters;

namespace PKSim.Presentation.Views
{
    public interface IBuildingBlockSelectionView : IView<IBuildingBlockSelectionPresenter>
    {
       void BindTo(BuildingBlockSelectionDTO buildingBlockSelectionDTO);
       bool DisplayIcons { set; }
       void RefreshBuildingBlockList();
    }
}