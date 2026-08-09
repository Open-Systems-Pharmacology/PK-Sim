using System;
using DevExpress.XtraLayout.Utils;
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
      private const int HINT_HEIGHT = 60;
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
         //without this the layout splits the available height between the note and the parameters instead of giving the
         //parameter grid the height it asks for, and the grid ends up scrolling
         view.HeightChanged += (o, e) => OnEvent(() => layoutItemParameters.AdjustControlHeight(e.Height, layoutControl1));
      }

      public string Hint
      {
         set
         {
            panelNote.NoteText = value;
            layoutItemNote.Visibility = LayoutVisibilityConvertor.FromBoolean(!string.IsNullOrEmpty(value));
         }
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
         var noteHeight = layoutItemNote.Visibility == LayoutVisibility.Never ? 0 : layoutItemNote.Height;
         return _parameterView.OptimalHeight + noteHeight + layoutItemParameters.Padding.Height;
      }

      public void Repaint()
      {
         _parameterView.Repaint();
      }

      public int OptimalHeight => calculateHeight();
   }
}