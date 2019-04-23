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
      private const int NUMBER_OF_LINES_HINT = 3;
      private const int NUMBER_OF_LINES_LARGE_HINT = 5;
      private const int HINT_HEIGHT = 80;
      private const int LARGE_HINT_HEIGHT = 80;

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
         set => panelNote.NoteText = value;
      }

      public bool IsLargeHint
      {
         set
         {
            var height = value ? LARGE_HINT_HEIGHT : HINT_HEIGHT;
            var lines = value ? NUMBER_OF_LINES_LARGE_HINT : NUMBER_OF_LINES_HINT;
            panelNote.MaxLines = lines;
            panelNote.MinimumSize = new System.Drawing.Size(panelNote.MinimumSize.Width, height);
         }
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