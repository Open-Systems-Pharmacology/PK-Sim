using System.Collections.Generic;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Protocols;
using PKSim.Presentation.Presenters.Protocols;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Protocols
{
   public interface IAdvancedProtocolView : IView<IAdvancedProtocolPresenter>, IProtocolView
   {
      void BindToSchemas(IEnumerable<SchemaDTO> allSchemas);
      void UpdateEndTime();
      void BindToProperties(AdvancedProtocol advancedProtocol);
      void Rebind();
   }
}