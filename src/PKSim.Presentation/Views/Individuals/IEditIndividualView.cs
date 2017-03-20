using OSPSuite.Assets;
using PKSim.Presentation.Presenters.Individuals;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Individuals
{
    public interface IEditIndividualView : IMdiChildView<IEditIndividualPresenter>
    {
       void UpdateIcon(ApplicationIcon speciesIcon);
    }
}