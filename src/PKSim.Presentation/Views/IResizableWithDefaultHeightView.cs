using OSPSuite.Presentation.Views;

namespace PKSim.Presentation.Views
{
   public interface IResizableWithDefaultHeightView : IResizableView
   {
      /// <summary>
      /// Default height placeholder that will be used to initialize the container of this view
      /// This is required to avoid a flickering effect. 
      /// </summary>
      int DefaultHeight { get; }
   }
}