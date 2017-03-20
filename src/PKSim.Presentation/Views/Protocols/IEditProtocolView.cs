using PKSim.Core.Model;
using PKSim.Presentation.Presenters.Protocols;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Protocols
{
   public interface IEditProtocolView : IView<IEditProtocolPresenter>, IMdiChildView
   {
      void UpdateChartControl(IView view);
      void UpdateEditControl(IView view);
      void SetProtocolMode(ProtocolMode protocolMode);
   }
}