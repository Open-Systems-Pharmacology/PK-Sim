using OSPSuite.Core.Commands.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class UpdateParameterTableFormulaCommand : EditParameterCommand
   {
      private byte[] _serializationStream;
      private TableFormula _tableFormula;

      public UpdateParameterTableFormulaCommand(IParameter tableParameter, TableFormula tableFormula)
         : base(tableParameter)
      {
         _tableFormula = tableFormula;
      }

      protected override void ClearReferences()
      {
         base.ClearReferences();
         _tableFormula = null;
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new UpdateParameterTableFormulaCommand(_parameter, _tableFormula).AsInverseFor(this);
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _tableFormula = context.Deserialize<TableFormula>(_serializationStream);
      }

      protected override void ExecuteUpdateParameter(IExecutionContext context)
      {
         _serializationStream = context.Serialize(_parameter.Formula);
         UpdateParameter(context);
         Description = ParameterMessages.UpdateTableParameterFormula(context.DisplayNameFor(_parameter));
      }

      protected override void UpdateParameter(IParameter parameter, IExecutionContext context)
      {
         if (parameter == null) return;
         //Same instance of the formula? We should not upgrade as the UpdatePropertiesFrom erase the points 
         //before adding them to the table which would result in a data loss
         if (ReferenceEquals(parameter.Formula, _tableFormula)) return;
         parameter.Formula.UpdatePropertiesFrom(_tableFormula, context.CloneManager);
      }
   }
}