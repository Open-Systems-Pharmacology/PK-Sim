using PKSim.Presentation.Presenters.Compounds;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Compounds
{
    public interface ICreateCompoundView : IWizardView, IModalView<ICreateCompoundPresenter>
    {
       void BindToProperties(ObjectBaseDTO compoundPropertiesDTO);
    }
}