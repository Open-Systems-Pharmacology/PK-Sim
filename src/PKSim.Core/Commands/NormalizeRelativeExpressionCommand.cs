using System.Linq;
using OSPSuite.Core.Commands.Core;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
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
   ///    The normalized result would be
   ///    Liver : 0.5
   ///    Kidney :0.5
   ///    Stomach:1
   /// </summary>
   public class NormalizeRelativeExpressionCommand : BuildingBlockChangeCommand
   {
      private IndividualMolecule _molecule;
      private ISimulationSubject _simulationSubject;
      private readonly string _moleculeId;

      public NormalizeRelativeExpressionCommand(IndividualMolecule molecule, ISimulationSubject simulationSubject, IExecutionContext context)
      {
         _molecule = molecule;
         _simulationSubject = simulationSubject;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         ObjectType = context.TypeFor(molecule);
         _moleculeId = molecule.Id;
         //This command is necessary to insure consistency but does not need to be seen
         Visible = false;
      }


      protected override void ClearReferences() 
      {
         _molecule = null;
         _simulationSubject = null;
      }

      protected override ICommand<IExecutionContext> GetInverseCommand(IExecutionContext context)
      {
         return new NormalizeRelativeExpressionCommand(_molecule, _simulationSubject, context).AsInverseFor(this);
      }

      public override void RestoreExecutionData(IExecutionContext context)
      {
         base.RestoreExecutionData(context);
         _molecule = context.Get<IndividualMolecule>(_moleculeId);
         _simulationSubject = context.Get<ISimulationSubject>(BuildingBlockId);
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         //Retrieve building block id in execute only since molecule might have been added in a macro command and bbid was not available in constructor
         BuildingBlockId = context.BuildingBlockIdContaining(_molecule);
         var allExpressionParameters = _simulationSubject.AllExpressionParametersFor(_molecule);
         if (!allExpressionParameters.Any()) 
            return;

         var max = allExpressionParameters.Select(x => x.Value).Max();

         allExpressionParameters.Each(relExp => relExp.Value = (max == 0 ? 0 : relExp.Value / max));
       }
   }
}