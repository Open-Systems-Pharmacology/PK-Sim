using System.Collections.Generic;
using OSPSuite.Assets;
using OSPSuite.Presentation.Views;
using PKSim.Core.Model;
using PKSim.Presentation.Presenters;

namespace PKSim.Presentation.Views
{
   public interface IBuildingBlockFromTemplateView : IModalView<ITemplatePresenter>, IExplorerView
   {
      void SetIcon(ApplicationIcon icon);
      string Description { get; set; }
      void SelectTemplate(Template template);
   }
}