using PKSim.Presentation.Presenters.Individuals;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Individuals
{
    public interface IIndividualParametersView : IView<IIndividualParametersPresenter>
    {
        void AddParametersView(IView parameterView);
    }
}