using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views
{
   public interface IResizableWithDefaultHeightView : IResizableView
   {
      int DefaultHeight { get; }
   }
}