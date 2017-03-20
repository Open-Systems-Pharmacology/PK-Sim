using System.Linq;
using PKSim.Assets;
using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public abstract class AddMoleculeExpressionsFromQueryToSimulationSubjectCommand<TSimulationSubject> : PKSimMacroCommand where TSimulationSubject : class, ISimulationSubject
   {
      private TSimulationSubject _simulationSubject;
      private IndividualMolecule _molecule;
      private QueryExpressionResults _queryExpressionResults;

      protected AddMoleculeExpressionsFromQueryToSimulationSubjectCommand(IndividualMolecule molecule, QueryExpressionResults queryExpressionResults, TSimulationSubject simulationSubject)
      {
         _molecule = molecule;
         _queryExpressionResults = queryExpressionResults;
         _simulationSubject = simulationSubject;
         CommandType = PKSimConstants.Command.CommandTypeAdd;
         ExtendedDescription = queryExpressionResults.Description;
      }

      public override void Execute(IExecutionContext context)
      {
         ObjectType = context.TypeFor(_molecule);
         var containerName = string.IsNullOrEmpty(_simulationSubject.Name) ? CoreConstants.ContainerName.NameTemplate : _simulationSubject.Name;
         Description = PKSimConstants.Command.AddEntityToContainer(ObjectType, _molecule.Name, context.TypeFor(_simulationSubject), containerName);

         //First add the protein expression to the individual so that undo will be available
         Add(AddMoleculeToSimulationSubjectCommand(_molecule, _simulationSubject, context));

         //Then update the new expression values
         foreach (var expressionResult in _queryExpressionResults.ExpressionResults)
         {
            Add(new SetRelativeExpressionCommand(_molecule.GetRelativeExpressionParameterFor(expressionResult.ContainerName), expressionResult.RelativeExpression));
         }

         Add(new NormalizeRelativeExpressionCommand(_molecule, context));

         //update properties from first command
         this.UpdatePropertiesFrom(All().FirstOrDefault());

         base.Execute(context);

         //clear references
         _molecule = null;
         _queryExpressionResults = null;
         _simulationSubject = null;
      }

      protected abstract ICommand AddMoleculeToSimulationSubjectCommand(IndividualMolecule molecule, TSimulationSubject simulationSubject, IExecutionContext context);
   }
}