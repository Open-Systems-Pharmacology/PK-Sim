using OSPSuite.Presentation.MenuAndBars;

namespace PKSim.Presentation.UICommands
{
    public interface IExitCommand : IUICommand
    {
        bool Canceled { get; }
    }
}