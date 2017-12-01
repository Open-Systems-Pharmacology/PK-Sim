using OSPSuite.Assets;
using DevExpress.LookAndFeel;
using PKSim.Assets;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;
using PKSim.UI.Views.Core;
using OSPSuite.Presentation.Core;
using OSPSuite.Presentation.Extensions;
using OSPSuite.Presentation.Views;

namespace PKSim.UI.Views.Compounds
{
   public partial class CompoundParametersView : BaseDynamicContainerUserControl, ICompoundParametersView
   {
      public CompoundParametersView(UserLookAndFeel defaultLookAndFeel)
         : base(defaultLookAndFeel)
      {
         InitializeComponent();
      }

      public void AttachPresenter(ICompoundParametersPresenter presenter)
      {
         //nothing to do
      }

      public override string Caption => PKSimConstants.UI.BasicPharmacochemistry;

      public override ApplicationIcon ApplicationIcon => ApplicationIcons.BasicPharmacochemistry;

      public void AddViewForGroup(ISubPresenterItem subPresenterItem, IView view)
      {
         var layoutControlItem = AddViewToLayout(view);
         layoutControlItem.Text =  view.Caption.FormatForLabel(checkCase:false);
      }
   }
}