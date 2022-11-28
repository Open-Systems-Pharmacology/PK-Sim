using System.Linq;
using OSPSuite.Core.Domain;
using OSPSuite.Utility;
using PKSim.Core.Model;
using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Services;
using OSPSuite.Core.Domain.Formulas;

namespace PKSim.Core.Mappers
{
   public interface IExpressionProfileToExpressionProfileBuildingBlockMapper : IMapper<ExpressionProfile, ExpressionProfileBuildingBlock>
   {
   }

   public class ExpressionProfileToExpressionProfileBuildingBlockMapper : IExpressionProfileToExpressionProfileBuildingBlockMapper
   {
      private readonly IPKSimProjectRetriever _projectRetriever;

      public ExpressionProfileToExpressionProfileBuildingBlockMapper(IPKSimProjectRetriever projectRetriever)
      {
         _projectRetriever = projectRetriever;
      }
      public ExpressionProfileBuildingBlock MapFrom(ExpressionProfile expressionProfile)
      {
         var expressionProfileBuildingBlock = new ExpressionProfileBuildingBlock()
         {
            Name = expressionProfile.Name,
            PKSimVersion = ProjectVersions.Current,
            Type = ExpressionTypes.MetabolizingEnzyme, //here we have to actually serialize the correct one
            MoleculeBuildingBlockId = expressionProfile.Molecule.Id
         };

         var allParameters = expressionProfile.Molecule.AllParameters().Union(expressionProfile.Individual.GetAllChildren<IParameter>());

         foreach (var parameter in expressionProfile.Individual.GetAllChildren<IParameter>())
         {
            if (parameter.Formula != null)
               expressionProfileBuildingBlock.FormulaCache.Add(parameter.EntityPath() + parameter.Formula.Name, parameter.Formula);
            else
               expressionProfileBuildingBlock.Add(new ExpressionParameter(parameter));
         }
         return expressionProfileBuildingBlock;
      }
   }
}
