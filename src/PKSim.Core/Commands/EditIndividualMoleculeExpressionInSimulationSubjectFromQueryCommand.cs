using System.Linq;
using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{

   public abstract class EditIndividualMoleculeExpressionInSimulationSubjectFromQueryCommand<TSimulationSubject> : PKSimMacroCommand where TSimulationSubject: class, ISimulationSubject
   {
      private IndividualMolecule _editedMolecule;
      private TSimulationSubject _simulationSubject;
      private IndividualMolecule _originalMolecule;
      private QueryExpressionResults _queryExpressionResults;

      protected EditIndividualMoleculeExpressionInSimulationSubjectFromQueryCommand(IndividualMolecule originalMolecule, IndividualMolecule editedMolecule,
         QueryExpressionResults queryExpressionResults, TSimulationSubject simulationSubject)
      {
         _originalMolecule = originalMolecule;
         _editedMolecule = editedMolecule;
         _queryExpressionResults = queryExpressionResults;
         _simulationSubject = simulationSubject;
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         var containerName = string.IsNullOrEmpty(simulationSubject.Name) ? CoreConstants.ContainerName.NameTemplate : simulationSubject.Name;
         Description = PKSimConstants.Command.AddEntityToContainer(ObjectType, _editedMolecule.Name, PKSimConstants.ObjectTypes.Individual, containerName);
         ExtendedDescription = queryExpressionResults.Description;
      }

      public override void Execute(IExecutionContext context)
      {
         ObjectType = context.TypeFor(_originalMolecule);
         var containerName = string.IsNullOrEmpty(_simulationSubject.Name) ? CoreConstants.ContainerName.NameTemplate : _simulationSubject.Name;
         Description = PKSimConstants.Command.AddEntityToContainer(ObjectType, _originalMolecule.Name, context.TypeFor(_simulationSubject), containerName);


         //First remove the original molecule
         Add(RemoveMoleculeFromSimulationSubjectCommand(_originalMolecule, _simulationSubject,  context));

         //Then add the new protein expression to the individual so tha undo will be available
         Add(AddMoleculeToSimulationSubjectCommand(_editedMolecule, _simulationSubject, context));

         var allExpressionParameters = _simulationSubject.AllExpressionParametersFor(_editedMolecule);
         //Then update only the expression values that have changed
         foreach (var expressionResult in _queryExpressionResults.ExpressionResults)
         {
            var expressionParameter = allExpressionParameters[expressionResult.ContainerName];
            if (expressionParameter.Value == expressionResult.RelativeExpression)
               continue;

            Add(new SetRelativeExpressionCommand(expressionParameter, expressionResult.RelativeExpression));
         }

         Add(new NormalizeRelativeExpressionCommand(_editedMolecule, _simulationSubject,  context));

         //update properties from first command
         this.UpdatePropertiesFrom(All().FirstOrDefault());


         base.Execute(context);

         //clear references
         _originalMolecule = null;
         _editedMolecule = null;
         _queryExpressionResults = null;
         _simulationSubject = null;
      }

      protected abstract ICommand RemoveMoleculeFromSimulationSubjectCommand(IndividualMolecule molecule, TSimulationSubject simulationSubject, IExecutionContext context);
      protected abstract ICommand AddMoleculeToSimulationSubjectCommand(IndividualMolecule molecule, TSimulationSubject simulationSubject, IExecutionContext context);

   }

}