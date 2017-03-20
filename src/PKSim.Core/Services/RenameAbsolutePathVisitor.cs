using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Extensions;
using OSPSuite.Utility.Visitor;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Extensions;

namespace PKSim.Core.Services
{
   public interface IRenameAbsolutePathVisitor
   {
      void RenameAllAbsolutePathIn(Simulation simulation, string oldName);
   }

   public class RenameAbsolutePathVisitor : IRenameAbsolutePathVisitor,
                                            IVisitor<IUsingFormula>,
                                            IVisitor<IEventAssignment>,
                                            IVisitor<IParameter>,
                                            IVisitor<PopulationSimulation>
   {
      private string _oldName;
      private string _newName;

      public void RenameAllAbsolutePathIn(Simulation simulation, string oldName)
      {
         _oldName = oldName;
         _newName = simulation.Name;
         this.Visit(simulation);
         simulation.Model.Root.AcceptVisitor(this);
      }

      private bool isAbsolutePath(IEnumerable<string> path)
      {
         var pathList = path.ToList();
         if (!pathList.Any()) return false;
         return string.Equals(pathList.First(), _oldName);
      }

      private bool isAbsolutePath(string pathAsString)
      {
         return isAbsolutePath(pathAsString.ToPathArray());
      }

      private void renameAbsolutePathIn(IFormula formula)
      {
         if (formula == null || !formula.IsExplicit()) return;
         formula.ObjectPaths.Where(isAbsolutePath).Each(renameObjectPath);
      }

      private void renameObjectPath(IObjectPath objectPath)
      {
         objectPath.Replace(_oldName, _newName);
      }

      public void Visit(IUsingFormula entityUsingFormula)
      {
         renameAbsolutePathIn(entityUsingFormula.Formula);
      }

      public void Visit(IEventAssignment eventAssignment)
      {
         Visit((IUsingFormula) eventAssignment);
         renameObjectPath(eventAssignment.ObjectPath);
      }

      public void Visit(IParameter parameter)
      {
         Visit((IUsingFormula) parameter);
         renameAbsolutePathIn(parameter.RHSFormula);
      }

      public void Visit(PopulationSimulation populationSimulation)
      {
         foreach (var parameterPath in populationSimulation.ParameterValuesCache.AllParameterPaths().Where(isAbsolutePath).ToList())
         {
            var newPath = parameterPath.Replace(_oldName, _newName);
            populationSimulation.ParameterValuesCache.RenamePath(parameterPath, newPath);
         }

         foreach (var advancedParameter in populationSimulation.AdvancedParameters.Where(x=>isAbsolutePath(x.ParameterPath)))
         {
            advancedParameter.ParameterPath = advancedParameter.ParameterPath.Replace(_oldName, _newName);
         }
      }
   }
}