using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Extensions;
using OSPSuite.Utility.Extensions;
using PKSim.Assets;
using PKSim.Core.Extensions;
using PKSim.Core.Model;

namespace PKSim.Core.Commands
{
   public class RenameMoleculeReferenceInSimulationSubjectCommand : BuildingBlockIrreversibleStructureChangeCommand
   {
      private ISimulationSubject _simulationSubject;
      private readonly string _oldMoleculeName;
      private readonly string _newMoleculeName;

      public RenameMoleculeReferenceInSimulationSubjectCommand(ISimulationSubject simulationSubject, string oldMoleculeName, string newMoleculeName, IExecutionContext context)
      {
         _simulationSubject = simulationSubject;
         _oldMoleculeName = oldMoleculeName;
         _newMoleculeName = newMoleculeName;
         BuildingBlockId = context.BuildingBlockIdContaining(simulationSubject);
         ObjectType = context.TypeFor(simulationSubject);
         CommandType = PKSimConstants.Command.CommandTypeEdit;
         Description = PKSimConstants.Command.RenameEntityCommandDescripiton(ObjectType, _oldMoleculeName, _newMoleculeName);

         //Command is hidden as it only deals with internals 
         Visible = false;
      }

      protected override void PerformExecuteWith(IExecutionContext context)
      {
         var individual = _simulationSubject.Individual;

         //This needs to be retrieved before performing base operation to ensure that we capture all containers before they are renamed
         var allMoleculeContainers = individual.GetAllChildren<IContainer>(x => x.IsNamed(_oldMoleculeName));

         //Now rename all other containers
         allMoleculeContainers.Each(x => x.Name = _newMoleculeName);

         //We also need to update all object path
         allMoleculeContainers.SelectMany(x => x.GetAllChildren<IUsingFormula>()).Each(x => x.Formula.ReplaceKeywordInObjectPaths(_oldMoleculeName, _newMoleculeName));

         renameMoleculeInParameterPathsOf(_simulationSubject as Population);
      }

      private void renameMoleculeInParameterPathsOf(Population population)
      {
         if (population == null)
            return;

         var individualValues = population.IndividualValuesCache;
         foreach (var oldPath in individualValues.AllParameterPaths())
         {
            var newPath = renameMoleculeIn(oldPath);
            if (!string.Equals(newPath, oldPath))
               individualValues.RenamePath(oldPath, newPath);
         }

         population.AdvancedParameters.Each(x => x.ParameterPath = renameMoleculeIn(x.ParameterPath));
      }

      private string renameMoleculeIn(string parameterPath) =>
         parameterPath.ToPathArray()
            .Select(x => string.Equals(x, _oldMoleculeName) ? _newMoleculeName : x)
            .ToPathString();

      protected override void ClearReferences()
      {
         _simulationSubject = null;
      }
   }
}