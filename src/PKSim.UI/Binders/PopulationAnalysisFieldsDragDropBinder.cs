using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using OSPSuite.DataBinding.DevExpress.XtraGrid;
using OSPSuite.Utility.Extensions;
using DevExpress.Utils;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using PKSim.Core.Model.PopulationAnalyses;
using PKSim.Presentation.DTO.PopulationAnalyses;
using PKSim.Presentation.Presenters.PopulationAnalyses;

namespace PKSim.UI.Binders
{
   public class PopulationAnalysisFieldsDragDropBinder : IPopulationAnalysisFieldsDragDropBinder
   {
      private readonly GridView _gridView;
      private readonly GridViewBinder<PopulationAnalysisFieldDTO> _gridViewBinder;
      private readonly GridControl _gridControl;
      public event EventHandler<FieldsMovedEventArgs> FieldsDropped = delegate { };
      public PivotArea Area { get; set; }
      public int? MaximNumberOfAllowedFields { get; set; }

      private GridHitInfo _downHitInfo;

      public PopulationAnalysisFieldsDragDropBinder(GridViewBinder<PopulationAnalysisFieldDTO> gridViewBinder)
      {
         _gridView = gridViewBinder.GridView;
         _gridViewBinder = gridViewBinder;
         _gridControl = _gridView.GridControl;
         _gridControl.DragOver += (o, e) => OnEvent(gridDragOver, e);
         _gridControl.DragDrop += (o, e) => OnEvent(gridDragDrop, e);
         _gridView.MouseDown += (o, e) => OnEvent(gridViewMouseDown, e);
         _gridView.MouseMove += (o, e) => OnEvent(gridViewMouseMove, e);
         _gridControl.AllowDrop = true;
      }

      private void gridDragOver(DragEventArgs e)
      {
         if (canAcceptData(fieldsFrom(e)))
            e.Effect = DragDropEffects.Move;
         else
            e.Effect = DragDropEffects.None;
      }

      private bool canAcceptData(List<PopulationAnalysisFieldDTO> data)
      {
         if (data == null || !data.Any())
            return false;

         if (!MaximNumberOfAllowedFields.HasValue)
            return true;

         return _gridView.RowCount + data.Count <= MaximNumberOfAllowedFields;
      }

      private void gridDragDrop(DragEventArgs e)
      {
         var data = fieldsFrom(e);
         if (!canAcceptData(data))
            return;

         // Required because mouse position using this event is relative to screen, not to control
         var targetPoint = _gridControl.PointToClient(new Point(e.X, e.Y));
         var hitInfo = _gridView.CalcHitInfo(targetPoint);

         var droppedFields = data.Select(x => x.Field).ToList();
         var targetFieldDTO = _gridViewBinder.ElementAt(hitInfo.RowHandle);
         var targetField = targetFieldDTO != null ? targetFieldDTO.Field : null;
         FieldsDropped(this, new FieldsMovedEventArgs(droppedFields, targetField, Area));
      }

      private static List<PopulationAnalysisFieldDTO> fieldsFrom(DragEventArgs e)
      {
         return e.Data.GetData(typeof (List<PopulationAnalysisFieldDTO>)) as List<PopulationAnalysisFieldDTO>;
      }

      private void gridViewMouseDown(MouseEventArgs e)
      {
         _downHitInfo = null;
         if (Control.ModifierKeys != Keys.None) return;
         if (e.Button != MouseButtons.Left) return;
         var hitInfo = _gridView.CalcHitInfo(new Point(e.X, e.Y));
         if (hitInfo.IsValid && hitInfo.RowHandle >= 0)
            _downHitInfo = hitInfo;
      }

      private void gridViewMouseMove(MouseEventArgs e)
      {
         if (_downHitInfo == null || e.Button != MouseButtons.Left) return;

         var dragRect = createDragReactForCurrentHitInfo();

         if (dragRect.Contains(new Point(e.X, e.Y)))
            return;

         _gridControl.DoDragDrop(selectedFields, DragDropEffects.All);
         _downHitInfo = null;
         DXMouseEventArgs.GetMouseArgs(e).Handled = true;
      }

      private Rectangle createDragReactForCurrentHitInfo()
      {
         var dragSize = SystemInformation.DragSize;
         return new Rectangle(new Point(_downHitInfo.HitPoint.X - dragSize.Width / 2,
            _downHitInfo.HitPoint.Y - dragSize.Height / 2), dragSize);
      }

      private IEnumerable<PopulationAnalysisFieldDTO> selectedFields
      {
         get
         {
            return _gridView.GetSelectedRows()
               .Select(rowHandle => _gridViewBinder.ElementAt(rowHandle)).ToList();
         }
      }

      protected void OnEvent<TEventArgs>(Action<TEventArgs> action, TEventArgs e)
      {
         this.DoWithinExceptionHandler(() => action(e));
      }
   }
}