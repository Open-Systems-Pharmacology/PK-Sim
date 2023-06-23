using System.Collections.Generic;
using OSPSuite.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using PKSim.Assets;
using PKSim.Core.Model;
using static PKSim.Core.CoreConstants.CalculationMethod;
using IFormulaFactory = PKSim.Core.Model.IFormulaFactory;
using ILazyLoadTask = OSPSuite.Core.Domain.Services.ILazyLoadTask;

namespace PKSim.Core.Mappers
{
   public interface IExpressionProfileToExpressionProfileBuildingBlockMapper : IPathAndValueBuildingBlockMapper<ExpressionProfile, ExpressionProfileBuildingBlock, ExpressionParameter>
   {
   }

   public class ExpressionProfileToExpressionProfileBuildingBlockMapper : PathAndValueBuildingBlockMapper<ExpressionProfile, ExpressionProfileBuildingBlock, ExpressionParameter>, IExpressionProfileToExpressionProfileBuildingBlockMapper
   {
      public ExpressionProfileToExpressionProfileBuildingBlockMapper(IObjectBaseFactory objectBaseFactory, IEntityPathResolver entityPathResolver, IApplicationConfiguration applicationConfiguration, ILazyLoadTask lazyLoadTask, IFormulaFactory formulaFactory) :
         base(objectBaseFactory, entityPathResolver, applicationConfiguration, lazyLoadTask, formulaFactory)
      {
      }

      protected override IFormula TemplateFormulaFor(IParameter parameter, IFormulaCache formulaCache, ExpressionProfile expressionProfile)
      {
         var formula = parameter.Formula;
         //for expression profile, all formula are in the calculation method EXPRESSION_PARAMETERS unless they are distributed table
         if (formula is TableFormula tableFormula)
            return tableFormula;

         return _formulaFactory.RateFor(new RateKey(EXPRESSION_PARAMETERS, parameter.Formula.Name), formulaCache);
      }

      protected override IReadOnlyList<IParameter> AllParametersFor(ExpressionProfile expressionProfile)
      {
         return expressionProfile.GetAllChildren<IParameter>();
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