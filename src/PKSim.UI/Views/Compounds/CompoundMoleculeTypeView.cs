using System;
using OSPSuite.DataBinding;
using OSPSuite.DataBinding.DevExpress;
using OSPSuite.Utility.Extensions;
using PKSim.Presentation.DTO.Compounds;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.Compounds
{
   public partial class CompoundMoleculeTypeView : BaseUserControl, ICompoundMoleculeTypeView
   {
      private readonly IToolTipCreator _toolTipCreator;
      private ICompoundParameterGroupPresenter _presenter;
      private readonly ScreenBinder<IsSmallMoleculeDTO> _screenBinder;
      public event EventHandler<ViewResizedEventArgs> HeightChanged = delegate { };

      public CompoundMoleculeTypeView(IToolTipCreator toolTipCreator)
      {
         _toolTipCreator = toolTipCreator;
         InitializeComponent();
         _screenBinder = new ScreenBinder<IsSmallMoleculeDTO>();
      }

      public void AttachPresenter(ICompoundParameterGroupPresenter presenter)
      {
         _presenter = presenter;
      }

      public override void InitializeBinding()
      {
         _screenBinder.Bind(x => x.Value)
            .To(chkIsSmallMolecule)
            .OnValueUpdating += (o, e) => OnEvent(() => moleculeTypePresenter.SetMoleculeType(e.NewValue));
      }

      private ICompoundMoleculeTypePresenter moleculeTypePresenter
      {
         get { return _presenter.DowncastTo<ICompoundMoleculeTypePresenter>(); }
      }

      public void AdjustHeight()
      {
         HeightChanged(this, new ViewResizedEventArgs(calculateHeight()));
      }

      private int calculateHeight()
      {
         return chkIsSmallMolecule.Height;
      }

      public void Repaint()
      {
         /*nothing to do here*/
      }

      public int OptimalHeight { get { return calculateHeight(); } }

      public void BindTo(IsSmallMoleculeDTO isSmallMolecule)
      {
         _screenBinder.BindToSource(isSmallMolecule);
         chkIsSmallMolecule.Text = isSmallMolecule.Display;
         chkIsSmallMolecule.SuperTip = _toolTipCreator.CreateToolTip(isSmallMolecule.Description, isSmallMolecule.Display);
         AdjustHeight();
      }
   }
}