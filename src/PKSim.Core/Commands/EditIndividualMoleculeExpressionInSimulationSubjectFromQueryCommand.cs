﻿using System.Linq;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;

namespace PKSim.Core.Commands
{
   public class EditIndividualMoleculeExpressionInSimulationSubjectFromQueryCommand : PKSimMacroCommand, IExpressionProfileCommand
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

            Add(new SetExpressionProfileValueCommand(expressionParameter, expressionResult.RelativeExpression, updateSimulationSubjects:false));
         }

         Add(new NormalizeRelativeExpressionCommand(_molecule, _simulationSubject, context));


         //update properties from first command
         this.UpdatePropertiesFrom(All().FirstOrDefault());


         base.Execute(context);

         // update depending object
         var updateTask = context.Resolve<IExpressionProfileUpdater>();
         updateTask.SynchronizeExpressionProfileInAllSimulationSubjects(_simulationSubject);

         //clear references
         _molecule = null;
         _queryExpressionResults = null;
         _simulationSubject = null;
      }
   }
}