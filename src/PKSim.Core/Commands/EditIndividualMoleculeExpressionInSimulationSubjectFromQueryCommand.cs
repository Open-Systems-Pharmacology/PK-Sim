using System.Linq;
using OSPSuite.Core.Commands.Core;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core.Commands
{
   public class EditIndividualMoleculeExpressionInSimulationSubjectFromQueryCommand : PKSimMacroCommand
   {
      private ISimulationSubject _simulationSubject;
      private IndividualMolecule _molecule;
      private QueryExpressionResults _queryExpressionResults;

      public EditIndividualMoleculeExpressionInSimulationSubjectFromQueryCommand(IndividualMolecule molecule, QueryExpressionResults queryExpressionResults, ISimulationSubject simulationSubject)
      {
         _molecule = molecule;
         _queryExpressionResults = queryExpressionResults;
         _simulationSubject = simulationSubject;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         var containerName = string.IsNullOrEmpty(simulationSubject.Name) ? CoreConstants.ContainerName.NameTemplate : simulationSubject.Name;
         Description = PKSimConstants.Command.AddEntityToContainer(ObjectType, molecule.Name, PKSimConstants.ObjectTypes.Individual, containerName);
         ExtendedDescription = queryExpressionResults.Description;
      }

      public override void Execute(IExecutionContext context)
      {
         ObjectType = context.TypeFor(_molecule);
         var containerName = string.IsNullOrEmpty(_simulationSubject.Name) ? CoreConstants.ContainerName.NameTemplate : _simulationSubject.Name;
         Description = PKSimConstants.Command.AddEntityToContainer(ObjectType, _molecule.Name, context.TypeFor(_simulationSubject), containerName);


         var allExpressionParameters = _simulationSubject.AllExpressionParametersFor(_molecule);
         //Then update only the expression values that have changed
         foreach (var expressionResult in _queryExpressionResults.ExpressionResults)
         {
            var expressionParameter = allExpressionParameters[expressionResult.ContainerName];
            if (expressionParameter.Value == expressionResult.RelativeExpression)
               continue;

            Add(new SetExpressionProfileValueCommand(expressionParameter, expressionResult.RelativeExpression, updateSimulationSubjects: false));
         }

         Add(new NormalizeRelativeExpressionCommand(_molecule, _simulationSubject, context));


         //update properties from first command
         this.UpdatePropertiesFrom(All().FirstOrDefault());

         //Execute the command first to update all relative expressions
         base.Execute(context);

         //special treatment for protein where we have to update localization after the fact
         if (_molecule is IndividualProtein protein)
            updateLocalizationForProtein(protein, context);

         // update depending object
         var updateTask = context.Resolve<IExpressionProfileUpdater>();
         updateTask.SynchronizeAllSimulationSubjectsWithExpressionProfile(_simulationSubject);

         //clear references
         _molecule = null;
         _queryExpressionResults = null;
         _simulationSubject = null;
      }

      private void updateLocalizationForProtein(IndividualProtein protein, IExecutionContext context)
      {
         //special case for blood cells and vascular endothelium. if the value is set > 0. we need to turn on some locations programatically
         var bloodCells = _queryExpressionResults.ExpressionResultFor(CoreConstants.Compartment.BLOOD_CELLS);
         if (bloodCells?.RelativeExpression > 0 && !protein.IsBloodCell())
            Add(new SetExpressionLocalizationCommand(protein, Localization.BloodCellsIntracellular, _simulationSubject, context).Run(context));

         var vascularEndothelium = _queryExpressionResults.ExpressionResultFor(CoreConstants.Compartment.VASCULAR_ENDOTHELIUM);
         if (vascularEndothelium?.RelativeExpression > 0 && !protein.IsVascularEndothelium())
            Add(new SetExpressionLocalizationCommand(protein, Localization.VascEndosome, _simulationSubject, context).Run(context));
      }
   }
}