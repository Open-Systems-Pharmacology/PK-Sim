using PKSim.Assets;
using OSPSuite.Assets;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;
using PKSim.UI.Views.Core;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Views;

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

      public void AddViewForGroup(ISubPresenterItem subPresenterItem, IView view)
      {
         AddViewTo(layoutMainGroup, view);
      }

      public void AddEmptyPlaceHolder()
      {
         AddEmptyPlaceHolder(layoutMainGroup);
      }

      public override void InitializeResources()
      {
         base.InitializeResources();
         Caption = PKSimConstants.UI.AdvancedParameterTabCaption;
      }
   }
}