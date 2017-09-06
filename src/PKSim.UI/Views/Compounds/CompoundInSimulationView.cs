using DevExpress.LookAndFeel;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using OSPSuite.Presentation.Views;
using OSPSuite.Utility.Collections;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;
using PKSim.UI.Views.Core;

namespace PKSim.UI.Views.Compounds
{
   public partial class CompoundInSimulationView : BaseDynamicContainerUserControl, ICompoundInSimulationView
   {
      private readonly ICache<IView, LayoutControlItem> _layoutItemCache;

      public CompoundInSimulationView(UserLookAndFeel defaultLookAndFeel) : base(defaultLookAndFeel)
      {
         InitializeComponent();
         _layoutItemCache = new Cache<IView, LayoutControlItem>(onMissingKey: view => null);
      }

      public void AttachPresenter(ICompoundInSimulationPresenter presenter)
      {
         //nothing to do
      }

      public void HideCachedView(IView view)
      {
         var cmLayoutItem = _layoutItemCache[view];
         if (cmLayoutItem == null) return;

         cmLayoutItem.Visibility = LayoutVisibilityConvertor.FromBoolean(false);
      }

      public void AddCachedView(IView baseView)
      {
         _layoutItemCache.Add(baseView, AddViewToLayout(baseView));
      }

      public void AddView(IView view) => AddViewToLayout(view);

      public override void InitializeResources()
      {
         base.InitializeResources();
         mainLayoutGroup.Padding = new Padding(0);
      }
   }
}