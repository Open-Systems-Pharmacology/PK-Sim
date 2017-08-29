using OSPSuite.Presentation.Views;

namespace PKSim.BatchTool.Views
{
   public interface IBatchView<TStartOptions>
   {
      bool CalculateEnabled { set; }
      void Display();
      void AddLogView(IView view);
      void BindTo(TStartOptions startOptions);
   }
}