using OSPSuite.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility;
using PKSim.Assets;
using PKSim.Core.Model;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;

namespace PKSim.Core.Mappers
{
   public interface IExpressionProfileToExpressionProfileBuildingBlockMapper : IMapper<ExpressionProfile, ExpressionProfileBuildingBlock>
   {
   }

   public class ExpressionProfileToExpressionProfileBuildingBlockMapper : PathAndValueBuildingBlockMapper<ExpressionProfile, ExpressionProfileBuildingBlock, ExpressionParameter>,  IExpressionProfileToExpressionProfileBuildingBlockMapper
   {
      public ExpressionProfileToExpressionProfileBuildingBlockMapper(IObjectBaseFactory objectBaseFactory, IEntityPathResolver entityPathResolver, IFormulaFactory formulaFactory, IApplicationConfiguration applicationConfiguration) :
         base(objectBaseFactory, entityPathResolver, formulaFactory, applicationConfiguration)
      {
      }

      public override ExpressionProfileBuildingBlock MapFrom(ExpressionProfile expressionProfile)
      {
         var expressionProfileBuildingBlock = base.MapFrom(expressionProfile);

         expressionProfileBuildingBlock.Type = mapExpressionType(expressionProfile.Molecule.MoleculeType);
         return expressionProfileBuildingBlock;
      }

      private ExpressionType mapExpressionType(QuantityType moleculeType)
      {
         switch (moleculeType)
         {
            case QuantityType.Enzyme:
               return ExpressionTypes.MetabolizingEnzyme;
            case QuantityType.Transporter:
               return ExpressionTypes.TransportProtein;
            case QuantityType.OtherProtein:
               return ExpressionTypes.ProteinBindingPartner;
         }

         throw new PKSimException(PKSimConstants.Error.CouldNotFindMoleculeType(moleculeType.ToString()));
      }
   }
}