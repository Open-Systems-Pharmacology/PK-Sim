using System.Linq;
using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   /// <summary>
   ///    In charge of normalizing all expressions value so that the maximal value is 1, and all
   ///    other values are scaled accordingly
   ///    For Instance if we have the following expressions
   ///    Liver : 10
   ///    Kidney :10
   ///    Stomach:20
   ///    The normlized result would be
   ///    Liver : 0.5
   ///    Kidney :0.5
   ///    Stomach:1
   /// </summary>
   public class NormalizeRelativeExpressionCommand : BuildingBlockChangeCommand
   {
      private IndividualMolecule _molecule;
      private readonly string _moleculeId;

      public NormalizeRelativeExpressionCommand(IndividualMolecule molecule, IExecutionContext context)
      {
         _molecule = molecule;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         ObjectType = context.TypeFor(molecule);
         _moleculeId = molecule.Id;
         //This command is necessary to insure consitency but does not need to be seen
         Visible = false;
      }


      protected override void ClearReferences() 
      {
         _molecule = null;
      }

      protected override IReversibleCommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new NormalizeRelativeExpressionCommand(_molecule, context).AsInverseFor(this);
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _molecule = context.Get<IndividualMolecule>(_moleculeId);
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         //Retrieve building block id in execute only since molecule might have been added in a macro command and bbid was not available in constructor
         BuildingBlockId = context.BuildingBlockIdContaining(_molecule);
         var allRelExpressionContainer = _molecule.AllExpressionsContainers();
         if (!allRelExpressionContainer.Any()) return;
         var max = allRelExpressionContainer.Select(x => x.RelativeExpression).Max();
         allRelExpressionContainer.Each(relExp => relExp.RelativeExpressionNorm = max == 0 ? 0 : relExp.RelativeExpression / max);
      }
   }
}