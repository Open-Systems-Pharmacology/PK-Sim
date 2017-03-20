using System;
using PKSim.Presentation.Presenters.Compounds;
using PKSim.Presentation.Views.Compounds;
using PKSim.Presentation.Views.Parameters;
using OSPSuite.Presentation.Views;
using OSPSuite.UI.Extensions;
using OSPSuite.UI.Controls;

namespace PKSim.UI.Views.Compounds
{
   public partial class CompoundAdvancedParameterGroupView : BaseUserControl, ICompoundAdvancedParameterGroupView
   {
      private IMultiParameterEditView _parameterView;
      public event EventHandler<ViewResizedEventArgs> HeightChanged = delegate { };

      public CompoundAdvancedParameterGroupView()
      {
         InitializeComponent();
      }

      public void AttachPresenter(ICompoundAdvancedParameterGroupPresenter presenter)
      {
      }

      public void SetParameterView(IMultiParameterEditView view)
      {
         _parameterView = view;
         panelParameters.FillWith(view);
      }

      public string Hint
      {
         set { panelNote.NoteText = value; }
      }

      public void AdjustHeight()
      {
         HeightChanged(this, new ViewResizedEventArgs(calculateHeight()));
      }

      private int calculateHeight()
      {
         return _parameterView.OptimalHeight + layoutItemNote.Height + layoutItemParameters.Padding.Height;
      }

      public void Repaint()
      {
         _parameterView.Repaint();
      }

      public int OptimalHeight => calculateHeight();
   }
}