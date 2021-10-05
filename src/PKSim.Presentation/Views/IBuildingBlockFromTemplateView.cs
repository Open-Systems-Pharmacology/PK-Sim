using System.Collections.Generic;
using OSPSuite.Assets;
using OSPSuite.Presentation.Views;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters;

namespace PKSim.Presentation.Views
{
   public interface IBuildingBlockFromTemplateView : IModalView<ITemplatePresenter>
   {
      void SetIcon(ApplicationIcon icon);
      void SelectTemplate(Template template);
      void BindTo(IReadOnlyList<Template> availableTemplates);
   }
}