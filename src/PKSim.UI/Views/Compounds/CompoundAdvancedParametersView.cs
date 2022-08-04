using OSPSuite.Assets;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;
using PKSim.Assets;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;

namespace PKSim.UI.Views.Compounds
{
   public partial class CompoundAdvancedParametersView : BaseContainerUserControl, ICompoundAdvancedParametersView
   {
      public CompoundAdvancedParametersView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(ICompoundAdvancedParametersPresenter presenter)
      {
         /*nothing to do*/
      }

      public override ApplicationIcon ApplicationIcon => ApplicationIcons.Parameters;

      public void AddViewForGroup(ISubPresenterItem subPresenterItem, IView view) => AddViewTo(layoutMainGroup,layoutControl,  view);

      public void AddEmptyPlaceHolder() => AddEmptyPlaceHolder(layoutMainGroup);

      public override void InitializeResources()
      {
         base.InitializeResources();
         Caption = PKSimConstants.UI.AdvancedParameterTabCaption;
      }
   }
}