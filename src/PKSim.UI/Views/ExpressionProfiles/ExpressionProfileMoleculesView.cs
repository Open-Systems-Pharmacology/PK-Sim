using OSPSuite.UI.Controls;
using PKSim.Presentation.Presenters.ExpressionProfiles;
using PKSim.Presentation.Views.ExpressionProfiles;

namespace PKSim.UI.Views.ExpressionProfiles
{
   public partial class ExpressionProfileMoleculesView : BaseUserControl, IExpressionProfileMoleculesView
   {
      public ExpressionProfileMoleculesView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(IExpressionProfileMoleculesPresenter presenter)
      {
         
      }
   }
}
