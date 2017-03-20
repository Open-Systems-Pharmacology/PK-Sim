using OSPSuite.UI.Services;
using DevExpress.LookAndFeel;
using PKSim.Core.Model;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.UI.Views.Compounds
{
   public partial class CompoundProcessView : BaseCompoundProcessView<CompoundProcess, CompoundProcessDTO>, ICompoundProcessView
   {
      public CompoundProcessView(IImageListRetriever imageListRetriever, UserLookAndFeel lookAndFeel)
         : base(imageListRetriever, lookAndFeel)
      {
         InitializeComponent();
      }

      public void AttachPresenter(ICompoundProcessPresenter presenter)
      {
         base.AttachPresenter(presenter);
      }
   }
}