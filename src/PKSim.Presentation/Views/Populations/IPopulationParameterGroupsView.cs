using System.Collections.Generic;
using PKSim.Presentation.Presenters.Populations;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Populations
{
   public interface IPopulationParameterGroupsView : IView<IPopulationParameterGroupsPresenter>
   {
      /// <summary>
      ///    Returns the seleced node
      /// </summary>
      ITreeNode SelectedNode { get; }

      bool EnableFilter { set; }

      /// <summary>
      ///    Add the nodes to the tree view
      /// </summary>
      void AddNodes(IEnumerable<ITreeNode> nodesToAdd);

      /// <summary>
      ///    Returns the node representing the parameter in the hiearchy. Null is returned if no node has been defined for the
      ///    parameter
      /// </summary>
      ITreeNode NodeFor(IParameter parameter);

      /// <summary>
      ///    Remove the given node from the hiearchy
      /// </summary>
      void RemoveNode(ITreeNode node);

      /// <summary>
      ///    Remove all nodes
      /// </summary>
      void Clear();

      /// <summary>
      ///    Select the given <paramref name="node" />
      /// </summary>
      void SelectNode(ITreeNode node);

      /// <summary>
      ///    Remove the nodes from the hierarchy
      /// </summary>
      /// <param name="allDeletedNodes"></param>
      void RemoveNodes(IEnumerable<ITreeNode> allDeletedNodes);

      /// <summary>
      ///    Returns the node with the id <paramref name="id" /> or null if this node does not exist
      /// </summary>
      ITreeNode NodeById(string id);
   }
}