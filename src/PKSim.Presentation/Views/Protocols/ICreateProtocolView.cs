using PKSim.Presentation.Presenters.Protocols;
using OSPSuite.Presentation.DTO;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Protocols
{
   public interface ICreateProtocolView : IModalView<ICreateProtocolPresenter>, IContainerView
   {
      void UpdateEditControl(IView view);
      void UpdateChartControl(IView view);
      void BindToProperties(ObjectBaseDTO protocolPropertiesDTO);
      bool SimpleProtocolEnabled { set; }
   }
}