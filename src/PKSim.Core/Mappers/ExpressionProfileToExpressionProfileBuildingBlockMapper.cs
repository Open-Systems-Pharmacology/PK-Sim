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

   public class ExpressionProfileToExpressionProfileBuildingBlockMapper : IExpressionProfileToExpressionProfileBuildingBlockMapper
   {
      private readonly IObjectBaseFactory _objectBaseFactory;
      private readonly IEntityPathResolver _entityPathResolver;
      private readonly IFormulaFactory _formulaFactory;
      private readonly IApplicationConfiguration _applicationConfiguration;

      public ExpressionProfileToExpressionProfileBuildingBlockMapper(IObjectBaseFactory objectBaseFactory, IEntityPathResolver entityPathResolver, IFormulaFactory formulaFactory, IApplicationConfiguration applicationConfiguration)
      {
         _objectBaseFactory = objectBaseFactory;
         _entityPathResolver = entityPathResolver;
         _formulaFactory = formulaFactory;
         _applicationConfiguration = applicationConfiguration;
      }

      public ExpressionProfileBuildingBlock MapFrom(ExpressionProfile expressionProfile)
      {
         var expressionProfileBuildingBlock = _objectBaseFactory.Create<ExpressionProfileBuildingBlock>();

         expressionProfileBuildingBlock.Name = expressionProfile.Name;
         expressionProfileBuildingBlock.PKSimVersion = _applicationConfiguration.Version;
         expressionProfileBuildingBlock.Description = expressionProfile.Description;

         expressionProfileBuildingBlock.Type = mapExpressionType(expressionProfile.Molecule.MoleculeType);

         var allParameters = expressionProfile.GetAllChildren<IParameter>();

         foreach (var parameter in allParameters)
         {
            var expressionParameter = mapExpressionParameterFromExpressionProfile(parameter, expressionProfileBuildingBlock);
            expressionProfileBuildingBlock.Add(expressionParameter);
         }

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

      private ExpressionParameter mapExpressionParameterFromExpressionProfile(IParameter parameter,
         ExpressionProfileBuildingBlock expressionProfileBuildingBlock)
      {
         var expressionParameter = _objectBaseFactory.Create<ExpressionParameter>();

         if (parameter.Formula != null && parameter.Formula.IsCachable())
         {
            var formula = _formulaFactory.RateFor(CoreConstants.CalculationMethod.EXPRESSION_PARAMETERS, parameter.Formula.Name,
               expressionProfileBuildingBlock.FormulaCache);
            expressionParameter.Formula = formula;
         }
         else
         {
            (expressionParameter.Value, _) = parameter.TryGetValue();
         }

         expressionParameter.Name = parameter.Name;

         expressionParameter.Path = _entityPathResolver.ObjectPathFor(parameter);
         expressionParameter.Dimension = parameter.Dimension;
         expressionParameter.DisplayUnit = parameter.DisplayUnit;
         return expressionParameter;
      }
   }
}