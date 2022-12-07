using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using OSPSuite.Utility;
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
      private IFormulaFactory _formulaFactory;

      public ExpressionProfileToExpressionProfileBuildingBlockMapper(IObjectBaseFactory objectBaseFactory, IEntityPathResolver entityPathResolver, IFormulaFactory formulaFactory)
      {
         _objectBaseFactory = objectBaseFactory;
         _entityPathResolver = entityPathResolver;
         _formulaFactory = formulaFactory;
      }

      public ExpressionProfileBuildingBlock MapFrom(ExpressionProfile expressionProfile)
      {
         var expressionProfileBuildingBlock = _objectBaseFactory.Create<ExpressionProfileBuildingBlock>();

         expressionProfileBuildingBlock.Name = expressionProfile.Name;
         expressionProfileBuildingBlock.PKSimVersion = ProjectVersions.Current;

         var moleculeType = expressionProfile.Molecule.MoleculeType;
         switch (moleculeType)
         {
            case QuantityType.Enzyme:
               expressionProfileBuildingBlock.Type = ExpressionTypes.MetabolizingEnzyme;
               break;
            case QuantityType.Transporter:
               expressionProfileBuildingBlock.Type = ExpressionTypes.TransportProtein;
               break;
            default:
               expressionProfileBuildingBlock.Type = ExpressionTypes.ProteinBindingPartner;
               break;
         }

         var allParameters = expressionProfile.GetAllChildren<IParameter>();

         foreach (var parameter in allParameters)
         {
            var expressionParameter = _objectBaseFactory.Create<ExpressionParameter>();

            if (parameter.Formula != null && parameter.Formula.IsCachable())
            {
               var formula = _formulaFactory.RateFor(CoreConstants.CalculationMethod.EXPRESSION_PARAMETERS, parameter.Formula.Name, expressionProfileBuildingBlock.FormulaCache);
               expressionParameter.Formula = formula;
            }
            else
            {
               (expressionParameter.StartValue, _) = parameter.TryGetValue();
            }

            expressionParameter.Name = parameter.Name;

            expressionParameter.Path = _entityPathResolver.ObjectPathFor(parameter);
            expressionParameter.Dimension = parameter.Dimension;
            expressionParameter.DisplayUnit = parameter.DisplayUnit;
            expressionProfileBuildingBlock.Add(expressionParameter);
         }

         return expressionProfileBuildingBlock;
      }
   }
}