using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Registrator;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.Handler;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.Drawing;
using DevExpress.XtraGrid.Views.Grid.Handler;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

// Developer Express Code Central Example:
// How to create a GridView descendant, which will allow using a specific repository item for displaying and editing data in a row preview section
// 
// This example shows how to create a GridView
// (ms-help://MS.VSCC.v90/MS.VSIPCC.v90/DevExpress.NETv9.2/DevExpress.XtraGrid/clsDevExpressXtraGridViewsGridGridViewtopic.htm)
// descendant, which will allow using a specific repository item for displaying and
// editing data in a row preview section.
// 
// 
// See Also:
// <kblink id = "K18341"/>
// 
// You can find sample updates and versions for different programming languages here:
// http://www.devexpress.com/example=E2002

namespace PKSim.UI.Views.Core
{
   public class CustomGridView : OSPSuite.UI.Controls.UxGridView
   {
      protected RepositoryItem fRowPreviewEdit;

      public CustomGridView()
         : base()
      {
         fRowPreviewEdit = null;
      }

      protected virtual int PreviewFieldHandle
      {
         get
         {
            int previewFieldHandle = DataController.Columns.GetColumnIndex(PreviewFieldName);
            if (previewFieldHandle == -1) previewFieldHandle = -2;
            return previewFieldHandle;
         }
      }

      protected internal virtual void SetGridControlAccessMetod(GridControl newControl)
      {
         SetGridControl(newControl);
      }

      protected override string ViewName
      {
         get { return "CustomGridView"; }
      }

      [Category("Appearance"), Description("Gets or sets the repository item specifying the editor used to show row preview."), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
       TypeConverter("DevExpress.XtraGrid.TypeConverters.ColumnEditConverter, " + AssemblyInfo.SRAssemblyGridDesign),
       Editor("DevExpress.XtraGrid.Design.ColumnEditEditor, " + AssemblyInfo.SRAssemblyGridDesign, typeof (UITypeEditor))]
      public RepositoryItem PreviewRowEdit
      {
         get { return fRowPreviewEdit; }
         set
         {
            if (PreviewRowEdit != value)
            {
               RepositoryItem old = fRowPreviewEdit;
               fRowPreviewEdit = value;
               CustomGridViewInfo vi = ViewInfo as CustomGridViewInfo;
               if (vi != null) vi.UpdateRowPreviewEdit(fRowPreviewEdit);
               if (old != null) old.Disconnect(this);
               if (PreviewRowEdit != null)
               {
                  PreviewRowEdit.Connect(this);
               }
            }
         }
      }

      protected override void Dispose(bool disposing)
      {
         if (PreviewRowEdit != null)
         {
            PreviewRowEdit.Disconnect(this);
            this.fRowPreviewEdit = null;
         }
         base.Dispose(disposing);
      }

      public virtual object GetRowPreviewValue(int rowHandle)
      {
         object result = null;
         if (PreviewFieldName.Length != 0 && DataController.IsReady)
            result = DataController.GetRowValue(rowHandle, PreviewFieldHandle);
         if (result is string) return GetRowPreviewDisplayText(rowHandle);
         return result;
      }
   }

   public class CustomGridControl : GridControl
   {
      protected override void RegisterAvailableViewsCore(InfoCollection collection)
      {
         base.RegisterAvailableViewsCore(collection);
         collection.Add(new CustomGridInfoRegistrator());
      }

      protected override BaseView CreateDefaultView()
      {
         return CreateView("CustomGridView");
      }
   }

   public class CustomGridPainter : GridPainter
   {
      public CustomGridPainter(GridView view) : base(view)
      {
      }

      public new virtual CustomGridView View
      {
         get { return (CustomGridView) base.View; }
      }

      protected override void DrawRowPreview(GridViewDrawArgs e, GridDataRowInfo ri)
      {  RepositoryItem item = ((CustomGridView) e.ViewInfo.View).PreviewRowEdit;
         if (item == null)
            base.DrawRowPreview(e, ri);
         else
            DrawRowPreviewEditor(e, ri, item);
      }

      private void DrawRowPreviewEditor(GridViewDrawArgs e, GridDataRowInfo ri, RepositoryItem item)
      {
         GridCellInfo info = new GridCellInfo(null, ri, ri.PreviewBounds);
         info.Editor = item;
         DrawCellEdit(e, ((CustomGridViewInfo) e.ViewInfo).GetRowPreviewViewInfo(e, ri), info, ri.AppearancePreview, false);
      }
   }

   public class CustomGridViewInfo : GridViewInfo
   {
      private BaseEditViewInfo fRowPreviewViewInfo;

      public CustomGridViewInfo(GridView gridView)
         : base(gridView)
      {
         UpdateRowPreviewEdit(View.PreviewRowEdit);
      }

      public new virtual CustomGridView View
      {
         get { return base.View as CustomGridView; }
      }

      public virtual void UpdateRowPreviewEdit(RepositoryItem item)
      {
         if (item != null)
            fRowPreviewViewInfo = CreateRowPreviewViewInfo(item);
         else
            fRowPreviewViewInfo = null;
      }

      protected virtual BaseEditViewInfo CreateRowPreviewViewInfo(RepositoryItem item)
      {
         BaseEditViewInfo info = item.CreateViewInfo();
         UpdateEditViewInfo(info);
         Graphics g = GInfo.AddGraphics(null);
         try
         {
            info.CalcViewInfo(g);
         }
         finally
         {
            GInfo.ReleaseGraphics();
         }
         return info;
      }

      public virtual BaseEditViewInfo GetRowPreviewViewInfo(GridViewDrawArgs e, GridDataRowInfo ri)
      {
         fRowPreviewViewInfo.Bounds =  GetRowPreviewEditBounds(ri);
         fRowPreviewViewInfo.EditValue = View.GetRowPreviewValue(ri.RowHandle);
         fRowPreviewViewInfo.Focused = true;
         fRowPreviewViewInfo.CalcViewInfo(e.Graphics);
         return fRowPreviewViewInfo;
      }

      public virtual Rectangle GetRowPreviewEditBounds(GridDataRowInfo ri)
      {
         Rectangle r = new Rectangle(new Point(0, 0), ri.PreviewBounds.Size);
         r.Inflate(-GridRowPreviewPainter.PreviewTextIndent, -GridRowPreviewPainter.PreviewTextVIndent);
         r.X += ri.PreviewIndent;
         r.Width -= ri.PreviewIndent;
         return r;
      }
   }

   public class CustomGridHandler : GridHandler
   {
      public CustomGridHandler(GridView gridView) : base(gridView)
      {
      }

      protected override GridRowNavigator CreateRowNavigator()
      {
         return new CustomGridRegularRowNavigator(this);
      }
   }

   public class CustomGridRegularRowNavigator : GridRegularRowNavigator
   {
      public CustomGridRegularRowNavigator(GridHandler handler) : base(handler)
      {
      }

      protected new CustomGridView View
      {
         get { return base.View as CustomGridView; }
      }
   }

   public class CustomGridInfoRegistrator : GridInfoRegistrator
   {
      public override BaseViewPainter CreatePainter(BaseView view)
      {
         return new CustomGridPainter(view as GridView);
      }

      public override BaseViewInfo CreateViewInfo(BaseView view)
      {
         return new CustomGridViewInfo(view as GridView);
      }

      public override BaseViewHandler CreateHandler(BaseView view)
      {
         return new CustomGridHandler(view as GridView);
      }

      public override string ViewName
      {
         get { return "CustomGridView"; }
      }

      public override BaseView CreateView(GridControl grid)
      {
         CustomGridView view = new CustomGridView();
         view.SetGridControlAccessMetod(grid);
         return view;
      }
   }
}