using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Commands
{
   public class SetCalculationMethodCommand<TBuildingBlockWithCalculationMethods> : BuildingBlockChangeCommand<TBuildingBlockWithCalculationMethods>
      where TBuildingBlockWithCalculationMethods : class, IWithCalculationMethods, IPKSimBuildingBlock
   {
      private readonly CalculationMethod _oldCalculationMethod;
      private readonly CalculationMethod _newCalculationMethod;
      private readonly string _category;

      public SetCalculationMethodCommand(
         TBuildingBlockWithCalculationMethods buildingBlock, 
         string category, 
         CalculationMethod newCalculationMethod, 
         CalculationMethod oldCalculationMethod) : 
         base(buildingBlock)
      {
         _newCalculationMethod = newCalculationMethod;
         _oldCalculationMethod = oldCalculationMethod;
         _category = category;

         CommandType = PKSimConstants.Command.CommandTypeEdit;
         
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         base.PerformExecuteWith(context);
         Description = PKSimConstants.Command.SetCalculationMethodFor(_buildingBlock.Name, _oldCalculationMethod.DisplayName, _newCalculationMethod.DisplayName);

         _buildingBlock.RemoveCalculationMethodFor(_category);
         _buildingBlock.AddCalculationMethod(_newCalculationMethod);
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SetCalculationMethodCommand<TBuildingBlockWithCalculationMethods>(_buildingBlock, _category, _oldCalculationMethod, _newCalculationMethod).AsInverseFor(this);
      }
   }
}