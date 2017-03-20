using PKSim.Core.Model;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Protocols
{
   public interface IProtocolPresenter : IContainerPresenter
   {
      ProtocolMode ProtocolMode { get; set; }

      /// <summary>
      ///    Returns true if the switch between protocol mode is allowed or has been approved by the user
      /// </summary>
      bool SwitchModeConfirm(ProtocolMode protocolMode);
   }
}