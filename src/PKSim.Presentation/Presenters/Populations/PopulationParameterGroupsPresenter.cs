using System;
using System.Collections.Generic;
using System.Linq;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Utility.Extensions;
using PKSim.Core;
using PKSim.Core.Model;
using PKSim.Core.Services;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Services;
using PKSim.Presentation.Views.Populations;
using OSPSuite.Core.Domain;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation.Presenters.Populations
{
   public interface IPopulationParameterGroupsPresenter : IPresenter<IPopulationParameterGroupsView>
   {
      /// <summary>
      ///    This function is called whenever the user selects a new node in the parameter group view
      /// </summary>
      /// <param name="node">selected node</param>
      void NodeSelected(ITreeNode node);

      /// <summary>
      ///    This function is called whenever the user double clicks a node in the parameter group view
      /// </summary>
      /// <param name="node">selected node</param>
      void NodeDoubleClicked(ITreeNode node);

      /// <summary>
      ///    Define the parameters that will be used to create the group structure
      /// </summary>
      /// <param name="allParameters">Enumeration of all parameters that will be displayed in groups</param>
      /// <param name="displayParameterUsingGroupStructure">
      ///    Should the parameters be displayed using the container hierarchy
      ///    (MoBi) or group structure (PKSim)
      /// </param>
      void AddParameters(IEnumerable<IParameter> allParameters, bool displayParameterUsingGroupStructure);

      /// <summary>
      ///    Define the parameters that will be used to create the group structure and add the covariates defined in the
      ///    population given as parameter
      /// </summary>
      /// <param name="allParameters">Enumeration of all parameters that will be displayed in groups</param>
      /// <param name="allCovariateNames">Covariates to add under the individual charateristics node</param>
      /// <param name="displayParameterUsingGroupStructure">
      ///    Should the parameters be displayed using the container hierarchy
      ///    (MoBi) or group structure (PKSim)
      /// </param>
      void AddParamtersAndCovariates(IEnumerable<IParameter> allParameters, IReadOnlyList<string> allCovariateNames , bool displayParameterUsingGroupStructure);

      /// <summary>
      ///    Returns the selected node (null if no node is being selected)
      /// </summary>
      ITreeNode SelectedNode { get; }

      /// <summary>
      ///    Returns the selected parameter or null if no parameter is selected.
      /// </summary>
      IParameter SelectedParameter { get; }

      /// <summary>
      ///    Returns the selected covariate or null if no covariate is selected
      /// </summary>
      string SelectedCovariate { get; }

      /// <summary>
      ///    Event is triggered whenever a group node is selected (i.e. a folder node)
      /// </summary>
      event EventHandler<NodeSelectedEventArgs> GroupNodeSelected;

      /// <summary>
      ///    Event is triggered whenever a node representing a covariate is selected
      /// </summary>
      event EventHandler<CovariateNodeSelectedEventArgs> CovariateNodeSelected;

      /// <summary>
      ///    Event is triggered whenevener a parameter node is selected
      /// </summary>
      event EventHandler<ParameterNodeSelectedEventArgs> ParameterNodeSelected;

      /// <summary>
      ///    Event is triggered whenevener a parameter node was double clicked
      /// </summary>
      event EventHandler<ParameterNodeSelectedEventArgs> ParameterNodeDoubleClicked;

      /// <summary>
      ///    Event is triggered whenever a node representing a covariate is selected
      /// </summary>
      event EventHandler<CovariateNodeSelectedEventArgs> CovariateNodeDoubleClicked;

      /// <summary>
      ///    Remove parameter from the view
      /// </summary>
      void RemoveParameter(IParameter parameter);

      /// <summary>
      ///    Add parameter to the view
      /// </summary>
      void AddParameter(IParameter parameter);

      /// <summary>
      ///    Select the node for the given parameter (if the node is defined)
      /// </summary>
      void SelectParameter(IParameter parameter);

      /// <summary>
      ///    Returns if a node was defined for the given parameter
      /// </summary>
      bool HasNodeFor(IParameter parameter);

      /// <summary>
      ///    Returns the node defined for the given parameter. Null otherwise
      /// </summary>
      ITreeNode NodeFor(IParameter parameter);

      /// <summary>
      ///    prunes the given node and returns the deleted nodes
      /// </summary>
      IEnumerable<ITreeNode> PruneNode(ITreeNode rootNode);

      /// <summary>
      ///    True if filter is enabled otherwise false
      /// </summary>
      bool EnableFilter { set; }
   }

   public class PopulationParameterGroupsPresenter : AbstractPresenter<IPopulationParameterGroupsView, IPopulationParameterGroupsPresenter>, IPopulationParameterGroupsPresenter
   {
      private readonly IParameterGroupTask _parameterGroupTask;
      private readonly IPopulationGroupNodeCreator _groupNodeCreator;
      private readonly IPopulationHierarchyNodeCreator _hierarchyNodeCreator;
      private bool _displayParameterUsingGroupStructure;
      public event EventHandler<NodeSelectedEventArgs> GroupNodeSelected = delegate { };
      public event EventHandler<CovariateNodeSelectedEventArgs> CovariateNodeSelected = delegate { };
      public event EventHandler<ParameterNodeSelectedEventArgs> ParameterNodeSelected = delegate { };
      public event EventHandler<ParameterNodeSelectedEventArgs> ParameterNodeDoubleClicked = delegate { };
      public event EventHandler<CovariateNodeSelectedEventArgs> CovariateNodeDoubleClicked = delegate { };

      public PopulationParameterGroupsPresenter(IPopulationParameterGroupsView view, IParameterGroupTask parameterGroupTask,
         IPopulationGroupNodeCreator groupNodeCreator, IPopulationHierarchyNodeCreator hierarchyNodeCreator) : base(view)
      {
         _parameterGroupTask = parameterGroupTask;
         _groupNodeCreator = groupNodeCreator;
         _hierarchyNodeCreator = hierarchyNodeCreator;
      }

      public IParameter SelectedParameter
      {
         get
         {
            var paramNode = SelectedNode as ITreeNode<IParameter>;
            return paramNode == null ? null : paramNode.Tag;
         }
      }

      public string SelectedCovariate
      {
         get
         {
            var covariteNode = SelectedNode as CovariateNode;
            return covariteNode == null ? null : covariteNode.Id;
         }
      }

      public ITreeNode SelectedNode
      {
         get { return _view.SelectedNode; }
      }

      public void RemoveParameter(IParameter parameter)
      {
         var parameterNode = NodeFor(parameter);
         if (parameterNode == null) return;

         //remove the parameter from its parent and then prune the root node
         var rootNode = parameterNode.RootNode;
         parameterNode.Delete();
         var allNodesToDelete = PruneNode(rootNode).ToList();
         allNodesToDelete.Add(parameterNode);
         //finally remove all nodes from the view at once
         _view.RemoveNodes(allNodesToDelete);
      }

      public ITreeNode NodeFor(IParameter parameter)
      {
         return _view.NodeFor(parameter);
      }

      public void AddParameter(IParameter parameter)
      {
         addParametersToView(new[] {parameter});
      }

      public void SelectParameter(IParameter parameter)
      {
         var node = NodeFor(parameter);
         if (node == null) return;
         _view.SelectNode(node);
      }

      public bool HasNodeFor(IParameter parameter)
      {
         return NodeFor(parameter) != null;
      }

      public IEnumerable<ITreeNode> PruneNode(ITreeNode rootNode)
      {
         var allDeletedNodes = new List<ITreeNode>();
         pruneNode(rootNode, allDeletedNodes);
         return allDeletedNodes;
      }

      public bool EnableFilter
      {
         set { _view.EnableFilter = value; }
      }

      private void pruneNode(ITreeNode node, IList<ITreeNode> deletedNodes)
      {
         var allLeafNodes = node.AllLeafNodes.Where(notAParameterNode).ToList();
         if (allLeafNodes.Count == 0)
            return;

         foreach (var allLeafNode in allLeafNodes)
         {
            allLeafNode.Delete();
            deletedNodes.Add(allLeafNode);
         }

         //only the root node was deleted.
         if (allLeafNodes[0] == node)
            return;

         pruneNode(node, deletedNodes);
      }

      private bool notAParameterNode(ITreeNode node)
      {
         if (node.TagAsObject == null) return true;
         return !node.TagAsObject.IsAnImplementationOf<IParameter>();
      }

      public void NodeSelected(ITreeNode node)
      {
         if (node == null)
            return;

         //parameter node?
         var parameterNode = node as ITreeNode<IParameter>;
         if (parameterNode != null)
         {
            ParameterNodeSelected(this, new ParameterNodeSelectedEventArgs(parameterNode));
            return;
         }

         //covariate node?
         var covariateNode = node as CovariateNode;
         if (covariateNode != null)
         {
            CovariateNodeSelected(this, new CovariateNodeSelectedEventArgs(covariateNode));
            return;
         }

         //This is a group node
         GroupNodeSelected(this, new NodeSelectedEventArgs(node));
      }

      public void NodeDoubleClicked(ITreeNode node)
      {
         var parameterNode = node as ITreeNode<IParameter>;
         if (parameterNode != null)
         {
            ParameterNodeDoubleClicked(this, new ParameterNodeSelectedEventArgs(parameterNode));
            return;
         }

         //covariate node?
         var covariateNode = node as CovariateNode;
         if (covariateNode != null)
         {
            CovariateNodeDoubleClicked(this, new CovariateNodeSelectedEventArgs(covariateNode));
            return;
         }
      }

      public void AddParameters(IEnumerable<IParameter> allParameters, bool displayParameterUsingGroupStructure)
      {
         //Initialize groups
         _view.Clear();
         var parameterList = allParameters.ToList();
         _displayParameterUsingGroupStructure = displayParameterUsingGroupStructure;
         addParametersToView(parameterList);
      }

      private void addParametersToView(IReadOnlyList<IParameter> allParameters)
      {
         var rootNodes = _displayParameterUsingGroupStructure
            ? createGroupStructure(allParameters)
            : createHierarchyStructure(allParameters);

         rootNodes.Each(node => PruneNode(node));
         _view.AddNodes(rootNodes);
      }

      private IReadOnlyCollection<ITreeNode> createHierarchyStructure(IReadOnlyList<IParameter> allParameters)
      {
         return _hierarchyNodeCreator.CreateHierarchyNodeFor(allParameters);
      }

      private IReadOnlyCollection<ITreeNode> createGroupStructure(IReadOnlyCollection<IParameter> allParameters)
      {
         //Initialize groups
         return _parameterGroupTask.TopGroupsUsedBy(allParameters)
            .OrderBy(group => group.Sequence)
            .ThenBy(group => group.DisplayName)
            .Select(group => _groupNodeCreator.CreateGroupNodeFor(group, allParameters))
            .ToList();
      }

      public void AddParamtersAndCovariates(IEnumerable<IParameter> allParameters, IReadOnlyList<string> allCovariateNames, bool displayParameterUsingGroupStructure)
      {
         AddParameters(allParameters, displayParameterUsingGroupStructure);
         addCovariates(allCovariateNames);
      }

      private void addCovariates(IReadOnlyList<string> allCovariateNames)
      {
         var individualCharacteristicNode = _view.NodeById(CoreConstants.Groups.INDIVIDUAL_CHARACTERISTICS);
         if (individualCharacteristicNode == null) return;

         var covariateNodes = new List<ITreeNode>();
         foreach (var covariateName in allCovariateNames)
         {
            var node = _groupNodeCreator.CreateCovariateNodeFor(covariateName);
            individualCharacteristicNode.AddChild(node);
            covariateNodes.Add(node);
         }

         _view.AddNodes(covariateNodes);
      }
   }
}