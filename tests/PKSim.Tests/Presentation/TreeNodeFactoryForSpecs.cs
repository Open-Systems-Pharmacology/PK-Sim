using FakeItEasy;
using PKSim.Presentation.Nodes;
using OSPSuite.Presentation.Services;

namespace PKSim.Presentation
{
   public class TreeNodeFactoryForSpecs : TreeNodeFactory
   {
      public TreeNodeFactoryForSpecs() : base(null, null, A.Fake<IToolTipPartCreator>())
      {
      }
   }
}