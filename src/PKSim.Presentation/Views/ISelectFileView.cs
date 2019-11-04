using OSPSuite.Presentation.Views;
using PKSim.Core.Services;
using PKSim.Presentation.Presenters;

namespace PKSim.Presentation.Views
{
   public interface ISelectFileView : IModalView<ISelectFilePresenter>
   {
      void BindTo(FileSelection fileSelection);
      void SetFileSelectionCaption(string caption);
   }
}