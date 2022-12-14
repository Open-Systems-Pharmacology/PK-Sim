using System.Collections.Generic;
using OSPSuite.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Services;
using PKSim.Assets;
using PKSim.Core.Model;

namespace PKSim.Core.Mappers
{
   public interface IExpressionProfileToExpressionProfileBuildingBlockMapper : IPathAndValueBuildingBlockMapper<ExpressionProfile, ExpressionProfileBuildingBlock>
   {
   }

   public class ExpressionProfileToExpressionProfileBuildingBlockMapper : PathAndValueBuildingBlockMapper<ExpressionProfile, ExpressionProfileBuildingBlock, ExpressionParameter>, IExpressionProfileToExpressionProfileBuildingBlockMapper
   {
      public ExpressionProfileToExpressionProfileBuildingBlockMapper(IObjectBaseFactory objectBaseFactory, IEntityPathResolver entityPathResolver, IApplicationConfiguration applicationConfiguration) :
         base(objectBaseFactory, entityPathResolver, applicationConfiguration)
      {
      }

      protected override IReadOnlyList<IParameter> AllParametersFor(ExpressionProfile sourcePKSimBuildingBlock)
      {
         return sourcePKSimBuildingBlock.GetAllChildren<IParameter>();
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