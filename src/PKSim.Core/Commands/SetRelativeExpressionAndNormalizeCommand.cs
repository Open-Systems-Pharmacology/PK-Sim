using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Commands
{
   public class SetRelativeExpressionAndNormalizeCommand : PKSimMacroCommand
   {
      private readonly double _newValue;
      private IndividualMolecule _molecule;
      private IParameter _relExpParameter;

      public SetRelativeExpressionAndNormalizeCommand(IndividualMolecule molecule, string proteinContainerName, double value)
      {
         _molecule = molecule;
         _relExpParameter = molecule.GetRelativeExpressionParameterFor(proteinContainerName);
         _newValue = value;
      }

      public override void Execute(IExecutionContext context)
      {
         //First scale the value. This step in only necessary for the rollback command
         Add(new NormalizeRelativeExpressionCommand(_molecule, context));

         //Then set the new relative expression command
         Add(new SetRelativeExpressionCommand(_relExpParameter, _newValue));

         //last scale wthe relative expression according to the new value
         Add(new NormalizeRelativeExpressionCommand(_molecule, context));


         base.Execute(context);

         //clear references
         _molecule = null;
         _relExpParameter = null;
      }
   }
}