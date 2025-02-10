using System.Collections.Generic;
using OSPSuite.Core;
using OSPSuite.Core.Domain;
using OSPSuite.Core.Domain.Builder;
using OSPSuite.Core.Domain.Formulas;
using OSPSuite.Core.Domain.Services;
using PKSim.Assets;
using PKSim.Core.Model;
using PKSim.Core.Services;
using static PKSim.Core.CoreConstants.CalculationMethod;
using static PKSim.Core.CoreConstants.Parameters;
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

      public ExpressionProfileToExpressionProfileBuildingBlockMapper(IObjectBaseFactory objectBaseFactory,
         IEntityPathResolver entityPathResolver,
         IApplicationConfiguration applicationConfiguration,
         ILazyLoadTask lazyLoadTask,
         IFormulaFactory formulaFactory,
         IInitialConditionsCreator initialConditionsCreator,
         IMoleculeBuilderFactory moleculeBuilderFactory,
         ICloner cloner) :
         base(objectBaseFactory, entityPathResolver, applicationConfiguration, lazyLoadTask, formulaFactory, cloner)
      {
         _initialConditionsCreator = initialConditionsCreator;
         _moleculeBuilderFactory = moleculeBuilderFactory;
      }

      protected override IFormula TemplateFormulaFor(IParameter parameter, IFormulaCache formulaCache, ExpressionProfile expressionProfile)
      {
         //Ontogeny factor parameter as formula will be a table formula defined in the ONTOGENY_FACTORS calculation method
         var calculationMethod = parameter.NameIsOneOf(OntogenyFactors) ? ONTOGENY_FACTORS : EXPRESSION_PARAMETERS;
         return _formulaFactory.RateFor(new RateKey(calculationMethod, parameter.Formula.Name), formulaCache);
      }

      protected override IReadOnlyList<IParameter> AllParametersFor(ExpressionProfile expressionProfile) => expressionProfile.GetAllChildren<IParameter>();

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