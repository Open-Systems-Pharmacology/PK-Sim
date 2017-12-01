using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OSPSuite.Core.Diagram;

namespace PKSim.CLI.Core.MinimalImplementations
{
   public class CLIDiagramModel : IDiagramModel
   {
      public PointF Location { get; set; }
      public PointF Center { get; set; }
      public SizeF Size { get; set; }
      public RectangleF Bounds { get; set; }

      public IEnumerable<T> GetDirectChildren<T>() where T : class
      {
         return Enumerable.Empty<T>();
      }

      public IEnumerable<T> GetAllChildren<T>() where T : class
      {
         return Enumerable.Empty<T>();
      }

      public void AddChildNode(IBaseNode node)
      {
      }

      public void RemoveChildNode(IBaseNode node)
      {
      }

      public bool ContainsChildNode(IBaseNode node, bool recursive)
      {
         return false;
      }

      public RectangleF CalculateBounds()
      {
         return new RectangleF(0, 0, 10, 10);
      }

      public void SetHiddenRecursive(bool hidden)
      {
      }

      public void PostLayoutStep()
      {
      }

      public void Collapse(int level)
      {
      }

      public void Expand(int level)
      {
      }

      public IBaseNode GetNode(string id)
      {
         return null;
      }

      public T GetNode<T>(string id) where T : class, IBaseNode
      {
         return null;
      }

      public T CreateNode<T>(string id, PointF location, IContainerBase parentContainerBase) where T : class, IBaseNode, new()
      {
         return new T();
      }

      public void RemoveNode(string id)
      {
      }

      public void RenameNode(string id, string name)
      {
      }

      public IDiagramModel CreateCopy(string containerId = null)
      {
         return new CLIDiagramModel();
      }

      public void ReplaceNodeIds(IDictionary<string, string> changedIds)
      {
      }

      public bool IsEmpty()
      {
         return true;
      }

      public void Clear()
      {
      }

      public void SetDefaultExpansion()
      {
      }

      public void RefreshSize()
      {
      }

      public void Undo()
      {
      }

      public void ClearUndoStack()
      {
      }

      public void BeginUpdate()
      {
      }

      public void EndUpdate()
      {
      }

      public bool StartTransaction()
      {
         return true;
      }

      public bool FinishTransaction(string layoutrecursivedone)
      {
         return true;
      }

      public void ShowDefaultExpansion()
      {
      }

      public IBaseNode FindByName(string name)
      {
         return null;
      }

      public void AddNodeId(IBaseNode baseNode)
      {
      }

      public bool IsLayouted { get; set; }
      public IDiagramOptions DiagramOptions { get; set; }

      public IDiagramModel Create()
      {
         return new CLIDiagramModel();
      }
   }
}