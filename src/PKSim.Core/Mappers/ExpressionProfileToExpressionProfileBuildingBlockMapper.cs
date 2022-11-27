using OSPSuite.Utility;
using PKSim.Core.Model;
using OSPSuite.Core.Domain.Builder;
using PKSim.Core.Services;

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
            
         };

         //expressionProfileBuildingBlock.FormulaCache = expressionProfile.Molecule.

         return expressionProfileBuildingBlock;
      }
   }
}
