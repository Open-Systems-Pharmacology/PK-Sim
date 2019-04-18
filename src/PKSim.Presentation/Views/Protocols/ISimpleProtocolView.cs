
using PKSim.Presentation.DTO.Protocols;
using PKSim.Presentation.Presenters.Protocols;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Protocols
{
   public interface ISimpleProtocolView : IView<ISimpleProtocolPresenter>, IProtocolView
   {
      void BindTo(SimpleProtocolDTO simpleProtocolDTO);
      bool EndTimeVisible { set; }
      bool DynamicParameterVisible { get; set; }
      bool TargetDefinitionVisible { get; set; }
      void AddDynamicParameterView(IView view);
      void RefreshCompartmentList();
   }
}