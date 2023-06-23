﻿using System.Collections.Generic;
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
using IMoleculeBuilderFactory = PKSim.Core.Model.IMoleculeBuilderFactory;

namespace PKSim.Core.Mappers
{
   public interface IExpressionProfileToExpressionProfileBuildingBlockMapper : IPathAndValueBuildingBlockMapper<ExpressionProfile, ExpressionProfileBuildingBlock, ExpressionParameter>
   {
   }

   public class ExpressionProfileToExpressionProfileBuildingBlockMapper : PathAndValueBuildingBlockMapper<ExpressionProfile, ExpressionProfileBuildingBlock, ExpressionParameter>, IExpressionProfileToExpressionProfileBuildingBlockMapper
   {
      private readonly IInitialConditionsCreator _initialConditionsCreator;
      private readonly IMoleculeBuilderFactory _moleculeBuilderFactory;

      public ExpressionProfileToExpressionProfileBuildingBlockMapper(
         IObjectBaseFactory objectBaseFactory, 
         IEntityPathResolver entityPathResolver, 
         IApplicationConfiguration applicationConfiguration, 
         ILazyLoadTask lazyLoadTask, 
         IFormulaFactory formulaFactory,
         IInitialConditionsCreator initialConditionsCreator,
         IMoleculeBuilderFactory moleculeBuilderFactory) :
         base(objectBaseFactory, entityPathResolver, applicationConfiguration, lazyLoadTask, formulaFactory)
      {
         _initialConditionsCreator = initialConditionsCreator;
         _moleculeBuilderFactory = moleculeBuilderFactory;
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
         addInitialConditions(expressionProfile, expressionProfileBuildingBlock);
         expressionProfileBuildingBlock.Type = mapExpressionType(expressionProfile.Molecule.MoleculeType);
         return expressionProfileBuildingBlock;
      }

      private void addInitialConditions(ExpressionProfile expressionProfile, ExpressionProfileBuildingBlock expressionProfileBuildingBlock)
      {
         var builder = _moleculeBuilderFactory.Create(expressionProfile.Molecule.MoleculeType, expressionProfileBuildingBlock.FormulaCache)
            .WithName(expressionProfile.Molecule.Name);

         _initialConditionsCreator.AddToExpressionProfile(expressionProfileBuildingBlock, expressionProfile.Individual.AllPhysicalContainersWithMoleculeFor(expressionProfile.Molecule), builder);
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