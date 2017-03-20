using System.Collections.Generic;
using OSPSuite.Presentation.Nodes;
using PKSim.Presentation.Presenters.Parameters;
using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views.Parameters
{
   public interface IParameterGroupsView : IView<IParameterGroupsPresenter>
   {
      void AddNodes(IEnumerable<ITreeNode> nodesToAdds);
      string GroupCaption { get; set; }
      void ActivateView(IView parametersView);
      void SelectNodeById(string nodeId);
      void BindToGroupingMode(IParameterGroupsPresenter parameterGroupsPresenter);
   }
}