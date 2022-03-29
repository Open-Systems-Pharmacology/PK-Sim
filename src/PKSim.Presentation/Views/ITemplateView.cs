using System.Collections.Generic;
using OSPSuite.Assets;
using OSPSuite.Presentation.Views;
using PKSim.Presentation.DTO;
using PKSim.Presentation.Presenters;

namespace PKSim.Presentation.Views
{
   public interface ITemplateView : IModalView<ITemplatePresenter>
   {
      void SelectTemplate(TemplateDTO template);
      void BindTo(IReadOnlyList<TemplateDTO> availableTemplates);
      string Description { get; set; }
   }
}