using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Visitor;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Services
{
   public interface IIndividualPathWithRootExpander
   {
      void AddRootToPathIn(ISimulationSubject simulationSubject);
      void AddRootToPathIn(ISimulationSubject simulationSubject, string moleculeName);
   }

   public class IndividualPathWithRootExpander : IIndividualPathWithRootExpander, IVisitor<IUsingFormula>
   {
      private readonly IReadOnlyList<string> _defaultTopContainer = new[] {Constants.ORGANISM, Constants.NEIGHBORHOODS};

      private List<string> _topContainers = new List<string>();

      public void AddRootToPathIn(ISimulationSubject simulationSubject)
      {
         _topContainers = new List<string>(_defaultTopContainer);
         simulationSubject.AcceptVisitor(this);
      }

      public void AddRootToPathIn(ISimulationSubject simulationSubject, string moleculeName)
      {
         _topContainers = new List<string>(_defaultTopContainer){moleculeName};
         simulationSubject.AcceptVisitor(this);
      }

      public void Visit(IUsingFormula usingFormula)
      {
         var formula = usingFormula.Formula;
         if (formula == null)
            return;

         foreach (var objectPath in formula.ObjectPaths.Where(path => path.Any()))
         {
            if (shouldAddRootToPath(objectPath.ToList()))
               objectPath.AddAtFront(Constants.ROOT);
         }
      }

      private bool shouldAddRootToPath(IList<string> objectPath)
      {
         return _topContainers.Contains(objectPath[0]);
      }
   }
}