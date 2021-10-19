using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain.Formulas;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Commands
{
   public class SetParameterFormulaCommand : EditParameterCommand
   {
      private IFormula _formulaToSet;
      private byte[] _serializationStream;

      public SetParameterFormulaCommand(IParameter parameter, IFormula formulaToSet)
         : base(parameter)
      {
         _formulaToSet = formulaToSet;
      }

      protected override void ClearReferences()
      {
         base.ClearReferences();
         _formulaToSet = null;
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _formulaToSet = context.Deserialize<TableFormula>(_serializationStream);
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new SetParameterFormulaCommand(_parameter, _formulaToSet).AsInverseFor(this);
      }

      protected override void ExecuteUpdateParameter(IExecutionContext context)
      {
         _serializationStream = context.Serialize(_parameter.Formula);
         UpdateParameter(context);
         Description = ParameterMessages.SetParameterFormula(context.DisplayNameFor(_parameter));
      }

      protected override void UpdateParameter(IParameter parameter, IExecutionContext context)
      {
         if (parameter == null) return;
         parameter.Formula = _formulaToSet;
      }
   }
}