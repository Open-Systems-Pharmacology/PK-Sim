using System.Collections.Generic;
using System.Linq;
using OSPSuite.Utility.Visitor;
using PKSim.Core.Model;
using OSPSuite.Core.Domain;

namespace PKSim.Core.Services
{
   public interface IIndividualPathWithRootExpander
   {
      void AddRootToPathIn(Individual individual);
   }

   public class IndividualPathWithRootExpander : IIndividualPathWithRootExpander, IVisitor<IUsingFormula>
   {
      public void AddRootToPathIn(Individual individual)
      {
         individual.AcceptVisitor(this);
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
         return string.Equals(objectPath[0], Constants.ORGANISM) ||
                string.Equals(objectPath[0], Constants.NEIGHBORHOODS);
      }
   }
}