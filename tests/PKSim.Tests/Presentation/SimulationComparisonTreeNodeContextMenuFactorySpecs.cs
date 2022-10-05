using OSPSuite.BDDHelper;
using OSPSuite.BDDHelper.Extensions;
using FakeItEasy;
using PKSim.Core.Model;
using PKSim.Presentation.Nodes;
using PKSim.Presentation.Presenters.ContextMenus;
using OSPSuite.Presentation.Nodes;
using OSPSuite.Presentation.Presenters;

namespace PKSim.Presentation
{
   public abstract class concern_for_SimulationComparisonTreeNodeContextMenuFactory : ContextSpecification<SimulationComparisonTreeNodeContextMenuFactory>
   {
      protected override void Context()
      {
         sut = new SimulationComparisonTreeNodeContextMenuFactory();
      }
   }

   public class When_checking_if_the_simulation_comparison_tree_node_context_menu_factory_can_create_a_context_menu_for_a_given_node : concern_for_SimulationComparisonTreeNodeContextMenuFactory
   {
      [Observation]
      public void should_return_true_if_the_node_is_a_classifiable_comparision_node()
      {
         var presenter = A.Fake<IPresenterWithContextMenu<ITreeNode>>();
         sut.IsSatisfiedBy(new ComparisonNode(new ClassifiableComparison {Subject = A.Fake<ISimulationComparison>()}), presenter).ShouldBeTrue();
      }
   }
}