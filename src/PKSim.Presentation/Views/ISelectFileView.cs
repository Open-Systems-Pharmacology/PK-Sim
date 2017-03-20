using PKSim.Presentation.Core;
using PKSim.Presentation.Presenters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views
{
   public interface ISelectFileView : IModalView<ISelectFilePresenter>
   {
      void BindTo(FileSelection fileSelection);
      void SetFileSelectionCaption(string caption);
   }
}