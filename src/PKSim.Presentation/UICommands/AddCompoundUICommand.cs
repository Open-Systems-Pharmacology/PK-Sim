using PKSim.Core.Model;
using PKSim.Presentation.Services;

namespace PKSim.Presentation.UICommands
{
    public class AddCompoundUICommand : AddBuildingBlockUICommand<Compound, ICompoundTask>
    {
        public AddCompoundUICommand(ICompoundTask compoundTask) : base(compoundTask)
        {
        }
    }
}