using OSPSuite.Assets;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Views;
using PKSim.Presentation.Presenters.ExpressionProfiles;
using PKSim.Presentation.Views.ExpressionProfiles;

namespace PKSim.UI.Views.ExpressionProfiles
{
   public partial class EditExpressionProfileView : BaseMdiChildView, IEditExpressionProfileView
   {
      public EditExpressionProfileView(Shell shell) : base(shell)
      {
         InitializeComponent();
      }

      public override void AddSubItemView(ISubPresenterItem subPresenterItem, IView viewToAdd)
      {
         panelControl.FillWith(viewToAdd);
      }

      public void AttachPresenter(IEditExpressionProfilePresenter presenter)
      {
         _presenter = presenter;
      }

      public override ApplicationIcon ApplicationIcon => ApplicationIcons.ExpressionProfile;
   }
}