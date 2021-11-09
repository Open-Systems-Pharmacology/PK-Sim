using OSPSuite.Core.Commands.Core;
using PKSim.Core.Model;

namespace PKSim.Core.Services
{
   public interface IExpressionProfileTask : IBuildingBlockTask<ExpressionProfile>
   {
      ExpressionProfile AddForMoleculeToProject<TMolecule>() where TMolecule : IndividualMolecule;
      ICommand UpdateMoleculeName(ExpressionProfile expressionProfile);

   }
}