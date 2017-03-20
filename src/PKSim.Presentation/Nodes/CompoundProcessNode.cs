using PKSim.Core.Model;
using OSPSuite.Presentation.Presenters.Nodes;

namespace PKSim.Presentation.Nodes
{
   public class CompoundProcessNode : ObjectWithIdAndNameNode<CompoundProcess>
   {
      public CompoundProcessNode(CompoundProcess compoundProcess)
         : base(compoundProcess)
      {
      }

      protected override void UpdateText()
      {
         Text = Tag.DataSource;
      }
   }
}